// This sample demonstrates RAG using Azure OpenAI for embeddings and Azure AI Search for vector storage.
// Uses the native Azure.Search.Documents SDK directly for vector search operations.
// To use Ollama for embeddings instead, replace the Azure OpenAI code with:
// new OllamaEmbeddingGenerator(new Uri("http://localhost:11434/"), "all-minilm")

using Microsoft.Extensions.AI;
using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
using System.ClientModel;

const string indexName = "movies";
const int vectorDimensions = 384;

// All samples in this repo share the user-secrets store "genai-beginners-dotnet", so scoped keys
// are read first ("AzureAISearch:*", the repo-wide "AzureOpenAI:*") before the legacy generic ones.
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["AzureOpenAI:Endpoint"] ?? config["endpoint"];
var apiKey = config["AzureOpenAI:ApiKey"] ?? config["apikey"];
var embeddingModelName = config["AzureOpenAI:EmbeddingDeployment"] ?? config["embeddingModelName"] ?? "text-embedding-3-small";
var azureAISearchUri = config["AzureAISearch:Uri"] ?? config["AZURE_AISEARCH_URI"];
var azureAISearchSecret = config["AzureAISearch:Secret"] ?? config["AZURE_AISEARCH_SECRET"];

var missing = new List<string>();
if (string.IsNullOrWhiteSpace(azureAISearchUri)) missing.Add("AzureAISearch:Uri");
if (string.IsNullOrWhiteSpace(azureAISearchSecret)) missing.Add("AzureAISearch:Secret");
if (string.IsNullOrWhiteSpace(endpoint)) missing.Add("AzureOpenAI:Endpoint");

if (missing.Count > 0)
{
    Console.WriteLine($"""
        This RAG sample is not configured. Missing: {string.Join(", ", missing)}

        It needs TWO different Azure services:

        1) Azure AI Search - a separate resource from Azure OpenAI. Create one in the Azure portal
           (Search services > Create). Then open the resource:
             - Overview blade > "Url" -> that is AzureAISearch:Uri (https://<name>.search.windows.net)
             - Settings > Keys blade > "Primary admin key" -> that is AzureAISearch:Secret

        2) Azure OpenAI with a text embedding deployment (default "text-embedding-3-small").

        Set the missing values (all samples share the user-secrets id "genai-beginners-dotnet"):

        dotnet user-secrets set --id genai-beginners-dotnet "AzureAISearch:Uri" "https://<your-search-service>.search.windows.net"
        dotnet user-secrets set --id genai-beginners-dotnet "AzureAISearch:Secret" "<your-search-admin-key>"
        dotnet user-secrets set --id genai-beginners-dotnet "AzureOpenAI:Endpoint" "https://<your-resource>.services.ai.azure.com/"
        dotnet user-secrets set --id genai-beginners-dotnet "AzureOpenAI:EmbeddingDeployment" "text-embedding-3-small"

        AzureOpenAI:EmbeddingDeployment is optional and defaults to "text-embedding-3-small".
        Embeddings are keyless by default and use your `az login` credentials; set
        "AzureOpenAI:ApiKey" only if you prefer key based authentication.
        The legacy keys "endpoint", "apikey", "embeddingModelName", "AZURE_AISEARCH_URI" and
        "AZURE_AISEARCH_SECRET" still work, but "endpoint"/"apikey" are shared with other samples
        in this store, so the scoped keys are recommended.
        """);
    return 1;
}

// get the search index client using the Azure AI Search admin key
var indexClient = new SearchIndexClient(new Uri(azureAISearchUri!), new AzureKeyCredential(azureAISearchSecret!));

// create or update the search index with vector field
await CreateOrUpdateIndexAsync(indexClient);

var searchClient = indexClient.GetSearchClient(indexName);

// get movie list
var movieData = MovieFactory<string>.GetMovieVectorList();

// keyless by default (Microsoft Entra ID via `az login`), or key based when AzureOpenAI:ApiKey is set
IEmbeddingGenerator<string, Embedding<float>> generator =
    (string.IsNullOrWhiteSpace(apiKey)
        ? new AzureOpenAIClient(new Uri(endpoint!), new AzureCliCredential())
        : new AzureOpenAIClient(new Uri(endpoint!), new ApiKeyCredential(apiKey)))
    .GetEmbeddingClient(embeddingModelName)
    .AsIEmbeddingGenerator();

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

return 0;

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
