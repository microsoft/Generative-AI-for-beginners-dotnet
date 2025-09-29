using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Connectors.InMemory;

var vectorStore = new InMemoryVectorStore();

// get movie list
var movies = vectorStore.GetCollection<int, MovieVector<int>>("movies");
await movies.EnsureCollectionExistsAsync();
var movieData = MovieFactory<int>.GetMovieVectorList();

// get embeddings generator and generate embeddings for movies
IEmbeddingGenerator<string, Embedding<float>> generator =
    new OllamaEmbeddingGenerator(new Uri("http://localhost:11434/"), "all-minilm");
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