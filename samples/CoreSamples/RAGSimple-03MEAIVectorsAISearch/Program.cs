// This sample demonstrates RAG using GitHub Models for embeddings and Azure AI Search for vector storage.
// To use Ollama for embeddings instead, replace the GitHub Models code with:
// new OllamaEmbeddingGenerator(new Uri("http://localhost:11434/"), "all-minilm")

using Microsoft.Extensions.AI;
using Azure;
using Azure.AI.Inference;
using Azure.Search.Documents.Indexes;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;
using Microsoft.Extensions.Configuration;
using Azure.Identity;

// get the search index client using Azure Default Credentials or Azure Key Credential with the service secret
var client = GetSearchIndexClient();
var vectorStore = new AzureAISearchVectorStore(searchIndexClient: client);

// get movie list
var movies = vectorStore.GetCollection<string, MovieVector<string>>("movies");
await movies.EnsureCollectionExistsAsync();
var movieData = MovieFactory<string>.GetMovieVectorList();

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

// creates a list of questions
var questions = new List<(string Question, int ResultCount)>
{
    ("A family friendly movie that includes ogres and dragons", 1),
    ("Movie released in year 1999 and 2003", 3),
    ("Una pelicula de ciencia ficcion", 1)
};

foreach (var question in questions)
{
    await SearchMovieAsync(question.Question, question.ResultCount);
}
async Task SearchMovieAsync(string question, int resultCount) 
{
    Console.WriteLine($"====================================================");
    Console.WriteLine($"Searching for: {question}");
    Console.WriteLine();

    // perform the search
    var queryEmbedding = await generator.GenerateVectorAsync(question);


    await foreach (var resultItem in movies.SearchAsync(queryEmbedding, top: 2))
    {
        Console.WriteLine($">> Title: {resultItem.Record.Title}");
        Console.WriteLine($">> Year: {resultItem.Record.Year}");
        Console.WriteLine($">> Description: {resultItem.Record.Description}");
        Console.WriteLine($">> Score: {resultItem.Score}");
        Console.WriteLine();
    }
    Console.WriteLine($"====================================================");
    Console.WriteLine();
}
SearchIndexClient GetSearchIndexClient()
{
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    var azureAISearchUri = config["AZURE_AISEARCH_URI"];

    var credential = new DefaultAzureCredential();
    var client = new SearchIndexClient(new Uri(azureAISearchUri), credential);
    var secret = config["AZURE_AISEARCH_SECRET"];

    if (!string.IsNullOrEmpty(secret))
    {
        client = new SearchIndexClient(new Uri(azureAISearchUri), new AzureKeyCredential(secret));
    }
    
    return client;
}
