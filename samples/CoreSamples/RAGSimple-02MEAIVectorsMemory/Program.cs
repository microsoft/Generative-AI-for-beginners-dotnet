// This sample demonstrates RAG using Azure OpenAI for embeddings with simple in-memory cosine similarity search.
// To use Ollama instead, replace the Azure OpenAI code with:
// new OllamaEmbeddingGenerator(new Uri("http://localhost:11434/"), "all-minilm")
// Or see RAGSimple-01SK or RAGSimple-10SKOllama samples for complete Ollama examples.

using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ClientModel;
using System.Numerics.Tensors;

// get movie list and prepare in-memory storage
var movieData = MovieFactory<int>.GetMovieVectorList();

// get embeddings generator and generate embeddings for movies
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["endpoint"];
var apiKey = new ApiKeyCredential(config["apikey"]);
var embeddingModelName = config["embeddingModelName"] ?? "text-embedding-3-small";

IEmbeddingGenerator<string, Embedding<float>> generator =
    new AzureOpenAIClient(new Uri(endpoint), apiKey)
    .GetEmbeddingClient(embeddingModelName)
    .AsIEmbeddingGenerator();

// generate embeddings for all movies and store them in memory
foreach (var movie in movieData)
{
    movie.Vector = await generator.GenerateVectorAsync(movie.Description);
}

// perform the search using cosine similarity
var query = "A family friendly movie that includes ogres and dragons";
var queryEmbedding = await generator.GenerateVectorAsync(query);

var results = movieData
    .Select(movie => (Movie: movie, Score: CosineSimilarity(queryEmbedding.Span, movie.Vector.Span)))
    .OrderByDescending(x => x.Score)
    .Take(2);

foreach (var (movie, score) in results)
{
    Console.WriteLine($"Title: {movie.Title}");
    Console.WriteLine($"Description: {movie.Description}");
    Console.WriteLine($"Score: {score}");
    Console.WriteLine();
}

static float CosineSimilarity(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
{
    return TensorPrimitives.CosineSimilarity(a, b);
}