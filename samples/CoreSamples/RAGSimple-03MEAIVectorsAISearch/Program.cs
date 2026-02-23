// This sample demonstrates RAG using GitHub Models for embeddings and Azure AI Search for vector storage.
// Uses the native Azure.Search.Documents SDK directly for vector search operations.
// To use Ollama for embeddings instead, replace the GitHub Models code with:
// new OllamaEmbeddingGenerator(new Uri("http://localhost:11434/"), "all-minilm")

using Microsoft.Extensions.AI;
using Azure;
using Azure.AI.Inference;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;
using Azure.Identity;

const string indexName = "movies";
const int vectorDimensions = 384;

// get the search index client using Azure Default Credentials or Azure Key Credential
var indexClient = GetSearchIndexClient();

// create or update the search index with vector field
await CreateOrUpdateIndexAsync(indexClient);

var searchClient = indexClient.GetSearchClient(indexName);

// get movie list
var movieData = MovieFactory<string>.GetMovieVectorList();

// get embeddings generator and generate embeddings for movies
var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN")
    ?? throw new InvalidOperationException("Missing GITHUB_TOKEN environment variable. Set it to use GitHub Models.");

IEmbeddingGenerator<string, Embedding<float>> generator =
    new EmbeddingsClient(
        endpoint: new Uri("https://models.github.ai/inference"),
        new AzureKeyCredential(githubToken))
    .AsIEmbeddingGenerator("text-embedding-3-small");

// generate embeddings and upload documents to Azure AI Search
var documents = new List<SearchDocument>();
foreach (var movie in movieData)
{
    movie.Vector = await generator.GenerateVectorAsync(movie.Description);
    var doc = new SearchDocument
    {
        ["Key"] = movie.Key,
        ["Title"] = movie.Title,
        ["Year"] = movie.Year,
        ["Category"] = movie.Category,
        ["Description"] = movie.Description,
        ["Vector"] = movie.Vector.ToArray()
    };
    documents.Add(doc);
}
await searchClient.IndexDocumentsAsync(IndexDocumentsBatch.Upload(documents));

// wait briefly for indexing to complete
await Task.Delay(2000);

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

    // generate query embedding and perform vector search
    var queryEmbedding = await generator.GenerateVectorAsync(question);

    var searchOptions = new SearchOptions
    {
        VectorSearch = new VectorSearchOptions
        {
            Queries =
            {
                new VectorizedQuery(queryEmbedding.ToArray())
                {
                    KNearestNeighborsCount = resultCount,
                    Fields = { "Vector" }
                }
            }
        },
        Size = resultCount
    };

    var response = await searchClient.SearchAsync<SearchDocument>(null, searchOptions);

    await foreach (var result in response.Value.GetResultsAsync())
    {
        Console.WriteLine($">> Title: {result.Document["Title"]}");
        Console.WriteLine($">> Year: {result.Document["Year"]}");
        Console.WriteLine($">> Description: {result.Document["Description"]}");
        Console.WriteLine($">> Score: {result.Score}");
        Console.WriteLine();
    }
    Console.WriteLine($"====================================================");
    Console.WriteLine();
}

async Task CreateOrUpdateIndexAsync(SearchIndexClient client)
{
    var vectorSearch = new VectorSearch();
    vectorSearch.Algorithms.Add(new HnswAlgorithmConfiguration("hnsw-config"));
    vectorSearch.Profiles.Add(new VectorSearchProfile("vector-profile", "hnsw-config"));

    var index = new SearchIndex(indexName)
    {
        VectorSearch = vectorSearch,
        Fields =
        {
            new SimpleField("Key", SearchFieldDataType.String) { IsKey = true, IsFilterable = true },
            new SearchableField("Title") { IsFilterable = true },
            new SimpleField("Year", SearchFieldDataType.Int32) { IsFilterable = true, IsSortable = true },
            new SearchableField("Category") { IsFilterable = true },
            new SearchableField("Description"),
            new SearchField("Vector", SearchFieldDataType.Collection(SearchFieldDataType.Single))
            {
                IsSearchable = true,
                VectorSearchDimensions = vectorDimensions,
                VectorSearchProfileName = "vector-profile"
            }
        }
    };

    await client.CreateOrUpdateIndexAsync(index);
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