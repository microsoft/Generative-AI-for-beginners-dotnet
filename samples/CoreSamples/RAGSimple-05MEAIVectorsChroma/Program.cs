// The same movie search as RAGSimple-02, backed by Chroma running in Docker.
using Azure.AI.OpenAI;
using Azure.Identity;
using ChromaDB.Client;
using CommunityToolkit.VectorData.Chroma;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.VectorData;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["AzureOpenAI:Endpoint"]
    ?? throw new InvalidOperationException("Set AzureOpenAI:Endpoint in User Secrets.");
var embeddingDeployment = config["AzureOpenAI:EmbeddingDeployment"] ?? "text-embedding-3-small";

using IEmbeddingGenerator<string, Embedding<float>> generator =
    new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetEmbeddingClient(embeddingDeployment)
    .AsIEmbeddingGenerator();

using VectorStoreCollection<string, MovieVectorRecord> movies =
    new ChromaCollection<string, MovieVectorRecord>(
        new ChromaClient("http://localhost:8000"), "movies", ownsClient: true);

// Reset only this demo collection, since MovieFactory generates new keys on every run.
await movies.EnsureCollectionDeletedAsync();
await movies.EnsureCollectionExistsAsync();

var movieList = MovieFactory<string>.GetMovieList();
Console.WriteLine("Movies in the vector store:");
foreach (var movie in movieList)
{
    Console.WriteLine($"  - {movie.Title}");
}
Console.WriteLine();

foreach (var movie in movieList)
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

// Embedding and retrieval are unchanged from the sqlite-vec sample.
string[] queries =
[
    "A family friendly movie that includes ogres and dragons",
    "A movie about a hacker who discovers reality is a simulation"
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
