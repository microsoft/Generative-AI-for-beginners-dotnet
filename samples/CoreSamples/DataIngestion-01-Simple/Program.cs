// This sample shows the full RAG data-ingestion process using the official .NET
// building blocks:
//
//   Microsoft.Extensions.DataIngestion  ->  read -> chunk -> enrich -> embed -> write
//   Microsoft.Extensions.VectorData     ->  store the chunks + embeddings and search them
//
// The pipeline reads Markdown files from the ./data folder, splits them into semantic
// chunks, enriches each chunk with an AI-generated summary, generates embeddings, and
// writes everything into a local sqlite-vec vector store via the ElBruno VectorData
// connector (no Semantic Kernel). You can then ask questions and the app runs a semantic
// search over the ingested content.
//
// To run the sample (Azure OpenAI, keyless via Azure CLI), set these user secrets:
//      dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-endpoint>.openai.azure.com/"
//      dotnet user-secrets set "AzureOpenAI:ChatDeployment" "gpt-5-mini"
//      dotnet user-secrets set "AzureOpenAI:EmbeddingDeployment" "text-embedding-3-small"
// Then sign in with: az login

using Azure.AI.OpenAI;
using Azure.Identity;
using ElBruno.Connectors.SqliteVec;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DataIngestion;
using Microsoft.Extensions.DataIngestion.Chunkers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;
using Microsoft.ML.Tokenizers;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["AzureOpenAI:Endpoint"]
    ?? throw new InvalidOperationException("Set AzureOpenAI:Endpoint in User Secrets.");
var chatModel = config["AzureOpenAI:ChatDeployment"] ?? "gpt-5-mini";
var embeddingModel = config["AzureOpenAI:EmbeddingDeployment"] ?? "text-embedding-3-small";

using ILoggerFactory loggerFactory =
    LoggerFactory.Create(builder => builder.AddSimpleConsole());

// Create the Azure OpenAI clients (keyless: uses your `az login` credentials).
var azureClient = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential());
IChatClient chatClient = azureClient.GetChatClient(chatModel).AsIChatClient();
IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator =
    azureClient.GetEmbeddingClient(embeddingModel).AsIEmbeddingGenerator();

// --- READ ------------------------------------------------------------------
// Read Markdown documents into a unified, LLM-friendly representation.
IngestionDocumentReader reader = new MarkdownReader();

// --- CHUNK -----------------------------------------------------------------
// Split documents into semantic chunks so related content stays together.
IngestionChunkerOptions chunkerOptions = new(TiktokenTokenizer.CreateForModel(chatModel))
{
    MaxTokensPerChunk = 2000,
    OverlapTokens = 0
};
IngestionChunker<string> chunker = new SemanticSimilarityChunker(embeddingGenerator, chunkerOptions);

// --- ENRICH ----------------------------------------------------------------
// Generate a short AI summary for each chunk to improve retrieval quality.
EnricherOptions enricherOptions = new(chatClient) { LoggerFactory = loggerFactory };
IngestionChunkProcessor<string> summaryEnricher = new SummaryEnricher(enricherOptions);

// --- EMBED + WRITE ---------------------------------------------------------
// Store the chunks (and their embeddings) in a local sqlite-vec vector store using the
// official VectorData abstraction backed by the ElBruno connector. A small custom
// IngestionChunkWriter embeds each chunk with the Azure OpenAI generator and upserts it.
var vectorStorePath = Path.Combine(AppContext.BaseDirectory, "buildingblocks-vectors.db");
var vectorStoreConnectionString = $"Data Source={vectorStorePath}";
var collection = new SqliteVecVectorStoreCollection<string, ChunkRecord>(
    "buildingblocks",
    vectorStoreConnectionString,
    embeddingGenerator);

await collection.EnsureCollectionDeletedAsync();
await collection.EnsureCollectionExistsAsync();

using IngestionChunkWriter<string> writer = new SqliteVecChunkWriter(collection, embeddingGenerator);

// --- PIPELINE --------------------------------------------------------------
// Chain reader -> chunker -> (enrich) -> writer into one workflow.
using IngestionPipeline<string> pipeline =
    new(reader, chunker, writer, loggerFactory: loggerFactory)
    {
        ChunkProcessors = { summaryEnricher }
    };

Console.WriteLine("Ingesting documents from ./data ...\n");
await foreach (IngestionResult result in pipeline.ProcessAsync(
    new DirectoryInfo("./data"), searchPattern: "*.md"))
{
    Console.WriteLine($"Processed '{result.DocumentId}' - Succeeded: {result.Succeeded}");
}
Console.WriteLine();

// --- SEARCH ----------------------------------------------------------------
// Query the same VectorData collection with natural-language questions. We embed the
// question ourselves and run the similarity search over the stored chunk vectors.
while (true)
{
    Console.Write("Enter your question (or 'exit' to quit): ");
    string? query = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(query) || query.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    Console.WriteLine("Searching...\n");
    var queryEmbedding = await embeddingGenerator.GenerateVectorAsync(query);
    await foreach (VectorSearchResult<ChunkRecord> result in
        collection.SearchAsync(queryEmbedding, top: 3))
    {
        Console.WriteLine($"Score: {result.Score:F3}");
        Console.WriteLine($"  {result.Record.Content}\n");
    }
}

// The chunk record stored in the vector store. text-embedding-3-small produces
// 1536-dimension vectors compared with cosine similarity. The key is a string because the
// sqlite-vec connector stores the key metadata column as TEXT.
internal sealed class ChunkRecord
{
    [VectorStoreKey]
    public string Key { get; set; } = "";

    [VectorStoreData]
    public string Content { get; set; } = "";

    [VectorStoreVector(1536, DistanceFunction = DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}

// Minimal IngestionChunkWriter that embeds each chunk and writes it to the sqlite-vec
// collection — the SK-free counterpart of the built-in VectorStoreWriter.
internal sealed class SqliteVecChunkWriter : IngestionChunkWriter<string>
{
    private readonly SqliteVecVectorStoreCollection<string, ChunkRecord> _collection;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;

    public SqliteVecChunkWriter(
        SqliteVecVectorStoreCollection<string, ChunkRecord> collection,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    {
        _collection = collection;
        _embeddingGenerator = embeddingGenerator;
    }

    public override async Task WriteAsync(
        IAsyncEnumerable<IngestionChunk<string>> chunks,
        CancellationToken cancellationToken)
    {
        await foreach (var chunk in chunks.WithCancellation(cancellationToken))
        {
            var embedding = await _embeddingGenerator.GenerateVectorAsync(
                chunk.Content, cancellationToken: cancellationToken);

            await _collection.UpsertAsync(new ChunkRecord
            {
                Key = Guid.NewGuid().ToString(),
                Content = chunk.Content,
                Embedding = embedding
            }, cancellationToken);
        }
    }
}
