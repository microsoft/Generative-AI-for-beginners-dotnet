// This sample demonstrates RAG using Azure OpenAI for embeddings and Qdrant for vector storage.
// Uses the native Qdrant.Client SDK directly for vector operations.
// To use Ollama for embeddings instead, replace the Azure OpenAI code with:
// new OllamaEmbeddingGenerator(new Uri("http://localhost:11434/"), "all-minilm")

using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using System.ClientModel;
using Qdrant.Client;
using Qdrant.Client.Grpc;

const string collectionName = "movies";
const int vectorSize = 384;

var qdrantClient = new QdrantClient("localhost");

// create collection if it doesn't exist
try
{
    await qdrantClient.CreateCollectionAsync(collectionName, new VectorParams
    {
        Size = (ulong)vectorSize,
        Distance = Distance.Cosine
    });
}
catch
{
    // collection may already exist
}

// get movie list
var movieData = MovieFactory<ulong>.GetMovieVectorList();

// get embeddings generator and generate embeddings for movies
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["endpoint"];
var apiKey = new ApiKeyCredential(config["apikey"]);
var embeddingModelName = config["embeddingModelName"] ?? "text-embedding-3-small";

IEmbeddingGenerator<string, Embedding<float>> generator =
    new AzureOpenAIClient(new Uri(endpoint), apiKey)
    .GetEmbeddingClient(embeddingModelName)
    .AsIEmbeddingGenerator();

// generate embeddings and upsert points into Qdrant
var points = new List<PointStruct>();
foreach (var movie in movieData)
{
    movie.Vector = await generator.GenerateVectorAsync(movie.Description);
    var point = new PointStruct
    {
        Id = new PointId { Num = Convert.ToUInt64(movie.Key) },
        Vectors = movie.Vector.ToArray(),
        Payload =
        {
            ["Title"] = movie.Title ?? string.Empty,
            ["Year"] = movie.Year ?? 0,
            ["Category"] = movie.Category ?? string.Empty,
            ["Description"] = movie.Description ?? string.Empty
        }
    };
    points.Add(point);
}
await qdrantClient.UpsertAsync(collectionName, points);

// perform the search
var query = "A family friendly movie that includes ogres and dragons";
var queryEmbedding = await generator.GenerateVectorAsync(query);

var searchResults = await qdrantClient.SearchAsync(
    collectionName,
    queryEmbedding.ToArray(),
    limit: 2);

foreach (var result in searchResults)
{
    Console.WriteLine($"Title: {result.Payload["Title"].StringValue}");
    Console.WriteLine($"Description: {result.Payload["Description"].StringValue}");
    Console.WriteLine($"Score: {result.Score}");
    Console.WriteLine();
}