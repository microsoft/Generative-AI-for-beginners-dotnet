// This sample demonstrates semantic search using the OFFICIAL .NET building block
// Microsoft.Extensions.VectorData (InMemoryVectorStore + VectorStoreCollection) instead of
// a hand-rolled cosine-similarity loop. It is the *same* abstraction the chat app uses —
// swap InMemoryVectorStore for SQLite / Qdrant / Azure AI Search in production with no
// changes to the search code.
//
// Keyless: uses your `az login` credentials (Microsoft Entra ID). Reuses the standard secrets:
//      dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-endpoint>.services.ai.azure.com/"
//      dotnet user-secrets set "AzureOpenAI:EmbeddingDeployment" "text-embedding-3-small"
// Then sign in with: az login

using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["AzureOpenAI:Endpoint"]
    ?? throw new InvalidOperationException("Set AzureOpenAI:Endpoint in User Secrets.");
var embeddingDeployment = config["AzureOpenAI:EmbeddingDeployment"] ?? "text-embedding-3-small";

// Keyless Azure OpenAI embeddings (Microsoft Entra ID via `az login`) — no API key in config.
IEmbeddingGenerator<string, Embedding<float>> generator =
    new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetEmbeddingClient(embeddingDeployment)
    .AsIEmbeddingGenerator();

// --- The official VectorData abstraction: a store and a typed collection ---
using var vectorStore = new InMemoryVectorStore();
var movies = vectorStore.GetCollection<int, MovieVectorRecord>("movies");
await movies.EnsureCollectionExistsAsync();

// Embed each movie description and upsert it into the collection.
foreach (var movie in MovieFactory<int>.GetMovieList())
{
    var embedding = await generator.GenerateVectorAsync(movie.Description!);
    await movies.UpsertAsync(new MovieVectorRecord
    {
        Key = movie.Key,
        Title = movie.Title!,
        Description = movie.Description!,
        Embedding = embedding
    });
}

// Search the collection — embeddings + similarity handled by the building block.
var query = "A family friendly movie that includes ogres and dragons";
var queryEmbedding = await generator.GenerateVectorAsync(query);

Console.WriteLine($"Query: {query}\n");
await foreach (var result in movies.SearchAsync(queryEmbedding, top: 2))
{
    Console.WriteLine($"Score: {result.Score:F3}");
    Console.WriteLine($"  {result.Record.Title}");
    Console.WriteLine($"  {result.Record.Description}\n");
}

// The data model annotated with the VectorData attributes. text-embedding-3-small
// produces 1536-dimension vectors compared with cosine similarity.
internal sealed class MovieVectorRecord
{
    [VectorStoreKey]
    public int Key { get; set; }

    [VectorStoreData]
    public string Title { get; set; } = "";

    [VectorStoreData]
    public string Description { get; set; } = "";

    [VectorStoreVector(1536, DistanceFunction = DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}