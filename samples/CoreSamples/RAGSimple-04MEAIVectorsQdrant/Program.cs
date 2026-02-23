// This sample demonstrates RAG using GitHub Models for embeddings and Qdrant for vector storage.
// To use Ollama for embeddings instead, replace the GitHub Models code with:
// new OllamaEmbeddingGenerator(new Uri("http://localhost:11434/"), "all-minilm")

using Microsoft.Extensions.AI;
using Azure;
using Azure.AI.Inference;
// TODO: Replace with MEAI-native Qdrant vector store package when available
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;

var vectorStore = new QdrantVectorStore(new QdrantClient("localhost"), true);

// get movie list
var movies = vectorStore.GetCollection<ulong, MovieVector<ulong>>("movies");
await movies.EnsureCollectionExistsAsync();
var movieData = MovieFactory<ulong>.GetMovieVectorList();

// get embeddings generator and generate embeddings for movies
var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN") 
    ?? throw new InvalidOperationException("Missing GITHUB_TOKEN environment variable. Set it to use GitHub Models.");

IEmbeddingGenerator<string, Embedding<float>> generator =
    new EmbeddingsClient(
        endpoint: new Uri("https://models.github.ai/inference"),
        new AzureKeyCredential(githubToken))
    .AsIEmbeddingGenerator("text-embedding-3-small");
foreach (var movie in movieData)
{
    movie.Vector = await generator.GenerateVectorAsync(movie.Description);
    await movies.UpsertAsync(movie);
}

// perform the search
var query = "A family friendly movie that includes ogres and dragons";
var queryEmbedding = await generator.GenerateVectorAsync(query);

await foreach (var resultItem in movies.SearchAsync(queryEmbedding, top: 2))
{
    Console.WriteLine($"Title: {resultItem.Record.Title}");
    Console.WriteLine($"Description: {resultItem.Record.Description}");
    Console.WriteLine($"Score: {resultItem.Score}");
    Console.WriteLine();
}