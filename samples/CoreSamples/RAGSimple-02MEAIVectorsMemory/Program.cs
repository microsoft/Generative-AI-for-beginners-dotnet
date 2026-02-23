// This sample demonstrates RAG using GitHub Models for embeddings with simple in-memory cosine similarity search.
// To use Ollama instead, replace the GitHub Models code with:
// new OllamaEmbeddingGenerator(new Uri("http://localhost:11434/"), "all-minilm")
// Or see RAGSimple-01SK or RAGSimple-10SKOllama samples for complete Ollama examples.

using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using System.Numerics.Tensors;

// get movie list and prepare in-memory storage
var movieData = MovieFactory<int>.GetMovieVectorList();

// get embeddings generator and generate embeddings for movies
var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN")
    ?? throw new InvalidOperationException("Missing GITHUB_TOKEN environment variable. Set it to use GitHub Models.");

IEmbeddingGenerator<string, Embedding<float>> generator =
    new EmbeddingsClient(
        endpoint: new Uri("https://models.github.ai/inference"),
        new AzureKeyCredential(githubToken))
    .AsIEmbeddingGenerator("text-embedding-3-small");

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