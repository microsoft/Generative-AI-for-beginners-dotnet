// This sample demonstrates semantic search using the OFFICIAL .NET VectorData abstraction
// (VectorStoreCollection + VectorData attributes) with an ElBruno sqlite-vec backed store
// instead of Semantic Kernel connectors or a hand-rolled cosine-similarity loop. It is the
// *same* abstraction the chat app uses — swap the backing store with no changes to the
// search code.
//
// Keyless: uses your `az login` credentials (Microsoft Entra ID). Reuses the standard secrets:
//      dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-endpoint>.services.ai.azure.com/"
//      dotnet user-secrets set "AzureOpenAI:EmbeddingDeployment" "text-embedding-3-small"
// Then sign in with: az login

using Azure.AI.OpenAI;
using Azure.Identity;
using ElBruno.Connectors.SqliteVec;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.VectorData;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["AzureOpenAI:Endpoint"]
    ?? throw new InvalidOperationException("Set AzureOpenAI:Endpoint in User Secrets.");
var embeddingDeployment = config["AzureOpenAI:EmbeddingDeployment"] ?? "text-embedding-3-small";

// Keyless Azure OpenAI embeddings (Microsoft Entra ID via `az login`) — no API key in config.
IEmbeddingGenerator<string, Embedding<float>> generator =
    new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetEmbeddingClient(embeddingDeployment)
    .AsIEmbeddingGenerator();

// --- The official VectorData abstraction: a typed collection over a supported local store ---
var vectorStorePath = Path.Combine(AppContext.BaseDirectory, "movie-vectors.db");
var vectorStoreConnectionString = $"Data Source={vectorStorePath}";
var movies = new SqliteVecVectorStoreCollection<int, MovieVectorRecord>(
    "movies",
    vectorStoreConnectionString,
    generator);

await movies.EnsureCollectionDeletedAsync();
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

// Search the collection — embeddings + similarity handled by the VectorData building block.
// The same query path works for any natural-language question: embed it, then search.
string[] queries =
[
    "A family friendly movie that includes ogres and dragons",
    "A movie about space adventures and aliens"
];

foreach (var query in queries)
{
    var queryEmbedding = await generator.GenerateVectorAsync(query);

    Console.WriteLine($"Query: {query}\n");
    await foreach (var result in movies.SearchAsync(queryEmbedding, top: 2))
    {
        Console.WriteLine($"Score: {result.Score:F3}");
        Console.WriteLine($"  {result.Record.Title}");
        Console.WriteLine($"  {result.Record.Description}\n");
    }

    Console.WriteLine(new string('-', 60) + "\n");
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