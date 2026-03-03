# Embeddings and Semantic Search

In this lesson, you'll learn how AI represents meaning as numbers and how to build search that understands intent, not just keywords.

---

[![Embeddings and Semantic Search](./images/LIM_GAN_07_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson3-embeddings)

_Click the image to watch the video_

---

## The Keyword Search Problem

Traditional search works by matching keywords. If you search for "running shoes," it finds documents containing those exact words.

But what if someone searches for:
- "sneakers for jogging"
- "athletic footwear"
- "shoes for my morning run"

These all mean the same thing, but keyword search would miss them.

**Semantic search** solves this by understanding *meaning*, not just words.

---

## Part 1: What Are Embeddings?

An **embedding** is a way to represent text (or images, or other data) as a list of numbers - a **vector**.

```
"I love pizza" → [0.12, -0.45, 0.89, 0.23, -0.67, ...]
```

These numbers capture the *meaning* of the text. Similar meanings produce similar numbers.

> **Learn more:** [Understand embeddings in .NET](https://learn.microsoft.com/dotnet/ai/conceptual/embeddings) explains how embedding models work and when to use them.

### Why Vectors?

Vectors let us do math with meaning:

```
"king" - "man" + "woman" ≈ "queen"
```

The vector for "king" minus "man" plus "woman" is close to the vector for "queen." The model has learned the relationship between these concepts.

### How Embeddings Work

1. You send text to an **embedding model**
2. The model returns a vector (array of floats)
3. Similar texts produce vectors that are close together in "vector space"
4. You compare vectors to find similar content

---

## Part 2: Creating Embeddings in .NET

Microsoft.Extensions.AI provides the `IEmbeddingGenerator` interface for creating embeddings.

### Basic Embedding Generation

```csharp
using Microsoft.Extensions.AI;
using Azure;
using Azure.AI.Inference;

// Create an embedding generator
IEmbeddingGenerator<string, Embedding<float>> generator =
    new AzureOpenAIClient(
        new Uri(config["endpoint"]),
        new ApiKeyCredential(config["apikey"]))
    .GetEmbeddingClient("text-embedding-3-small")
    .AsIEmbeddingGenerator();

// Generate an embedding for text
var embedding = await generator.GenerateAsync("I love pizza");

// The result is a vector of floats
Console.WriteLine($"Embedding dimensions: {embedding[0].Vector.Length}");
// Output: Embedding dimensions: 1536 (varies by model)
```

### Embedding Multiple Items

```csharp
string[] documents = [
    "The quick brown fox jumps over the lazy dog",
    "A fast auburn fox leaps above a sleepy canine",
    "The weather is nice today",
    "I enjoy programming in C#"
];

var embeddings = await generator.GenerateAsync(documents);

foreach (var (doc, emb) in documents.Zip(embeddings))
{
    Console.WriteLine($"'{doc.Substring(0, 20)}...' → {emb.Vector.Length} dimensions");
}
```

**Learn more:** [IEmbeddingGenerator interface](https://learn.microsoft.com/dotnet/ai/iembeddinggenerator) provides the full API reference for embedding generation in .NET.

---

## Part 3: Measuring Similarity

Once you have embeddings, you compare them using **cosine similarity**.

```csharp
static float CosineSimilarity(ReadOnlyMemory<float> a, ReadOnlyMemory<float> b)
{
    var spanA = a.Span;
    var spanB = b.Span;
    
    float dotProduct = 0, normA = 0, normB = 0;
    
    for (int i = 0; i < spanA.Length; i++)
    {
        dotProduct += spanA[i] * spanB[i];
        normA += spanA[i] * spanA[i];
        normB += spanB[i] * spanB[i];
    }
    
    return dotProduct / (MathF.Sqrt(normA) * MathF.Sqrt(normB));
}
```

Cosine similarity returns a value between -1 and 1:
- **1.0** = Identical meaning
- **0.0** = Unrelated
- **-1.0** = Opposite meaning

### Example: Finding Similar Sentences

```csharp
var query = "athletic footwear for running";
var queryEmbedding = await generator.GenerateVectorAsync(query);

string[] products = [
    "Running shoes for marathon training",
    "Comfortable sneakers for jogging",
    "Leather dress shoes for formal occasions",
    "Hiking boots for mountain trails",
    "Basketball shoes for indoor courts"
];

var productEmbeddings = await generator.GenerateAsync(products);

// Calculate similarity for each product
var results = products
    .Zip(productEmbeddings)
    .Select(p => new { 
        Product = p.First, 
        Similarity = CosineSimilarity(queryEmbedding, p.Second.Vector) 
    })
    .OrderByDescending(r => r.Similarity)
    .ToList();

Console.WriteLine($"Query: {query}\n");
foreach (var result in results)
{
    Console.WriteLine($"{result.Similarity:F3} - {result.Product}");
}
```

**Output:**

```
Query: athletic footwear for running

0.892 - Running shoes for marathon training
0.847 - Comfortable sneakers for jogging
0.634 - Basketball shoes for indoor courts
0.521 - Hiking boots for mountain trails
0.312 - Leather dress shoes for formal occasions
```

Notice: "sneakers for jogging" ranks high even though it shares no words with the query!

---

## Part 4: Vector Stores

For real applications, you don't want to:
1. Generate embeddings for your entire dataset on every search
2. Loop through all embeddings to find matches

**Vector stores** (or vector databases) solve this by:
- Storing embeddings persistently
- Using optimized algorithms for fast similarity search
- Scaling to millions of documents

### Using InMemoryVectorStore

Microsoft.Extensions.VectorData provides abstractions for vector stores:

```csharp
using Microsoft.Extensions.VectorData;

// Define your data model
public class Product
{
    [VectorStoreKey]
    public int Id { get; set; }

    [VectorStoreData]
    public string Name { get; set; }

    [VectorStoreData]
    public string Description { get; set; }

    [VectorStoreVector(1536, DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}
```

Key attributes:
- `[VectorStoreKey]` - Unique identifier
- `[VectorStoreData]` - Searchable/filterable data
- `[VectorStoreVector(dimensions, distanceFunction)]` - The embedding vector

### Populating the Vector Store

```csharp
// Create the vector store and collection
var vectorStore = new InMemoryVectorStore();
var products = vectorStore.GetCollection<int, Product>("products");
await products.EnsureCollectionExistsAsync();

// Your product data
var productData = new[]
{
    new { Id = 1, Name = "Trail Runner Pro", Description = "Lightweight running shoes for trail running" },
    new { Id = 2, Name = "Urban Jogger", Description = "Comfortable sneakers for city jogging" },
    new { Id = 3, Name = "Executive Oxford", Description = "Classic leather dress shoes for business" },
    // ... more products
};

// Generate embeddings and store
foreach (var p in productData)
{
    var embedding = await generator.GenerateVectorAsync(p.Description);
    await products.UpsertAsync(new Product
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Embedding = embedding
    });
}
```

### Searching the Vector Store

```csharp
// Search for similar products
var searchQuery = "shoes for my morning jog";
var queryEmbedding = await generator.GenerateVectorAsync(searchQuery);

var searchResults = products.SearchAsync(queryEmbedding, top: 3);

Console.WriteLine($"Results for: '{searchQuery}'\n");
await foreach (var result in searchResults)
{
    Console.WriteLine($"Score: {result.Score:F3}");
    Console.WriteLine($"  {result.Record.Name}");
    Console.WriteLine($"  {result.Record.Description}\n");
}
```

**Learn more:** [Work with Vector Databases](https://learn.microsoft.com/dotnet/ai/conceptual/vector-databases) explains how to choose and configure vector stores for your application.

---

## Part 5: Production Vector Stores

The `InMemoryVectorStore` is great for development, but for production you need persistent storage.

### Azure AI Search

```csharp
using Azure.Search.Documents.Indexes;
using Microsoft.Extensions.VectorData.AzureAISearch;

var searchClient = new SearchIndexClient(
    new Uri(azureSearchEndpoint),
    new AzureKeyCredential(azureSearchKey));

var vectorStore = new AzureAISearchVectorStore(searchClient);
var collection = vectorStore.GetCollection<string, Product>("products");
```

### Qdrant

```csharp
using Microsoft.Extensions.VectorData.Qdrant;
using Qdrant.Client;

var qdrantClient = new QdrantClient("localhost", 6333);
var vectorStore = new QdrantVectorStore(qdrantClient);
var collection = vectorStore.GetCollection<Guid, Product>("products");
```

### Other Supported Stores

- **Azure Cosmos DB** - For globally distributed applications
- **PostgreSQL with pgvector** - For SQL-based workflows
- **Redis** - For high-speed caching scenarios
- **Pinecone**, **Weaviate**, **Milvus** - Specialized vector databases

The code stays the same - just swap the vector store implementation!

---

## Let's Review: What You Learned

| Concept | Key Takeaway |
|---------|-------------|
| **Embeddings** | Represent meaning as vectors of numbers |
| **IEmbeddingGenerator** | .NET interface for creating embeddings |
| **Cosine Similarity** | Math formula to compare embeddings (0-1 scale) |
| **Vector Stores** | Databases optimized for similarity search |
| **Semantic Search** | Find by meaning, not just keywords |

### Quick Self-Check

1. Why does "sneakers for jogging" match "running shoes" in semantic search but not keyword search?
2. What do the numbers in an embedding represent?
3. When would you use a production vector store instead of InMemoryVectorStore?

---

## Sample Code Reference

| Sample | Description |
|--------|-------------|
| [RAGSimple-02MEAIVectorsMemory](../samples/CoreSamples/RAGSimple-02MEAIVectorsMemory/) | In-memory vector store with Azure OpenAI |
| [RAGSimple-03MEAIVectorsAISearch](../samples/CoreSamples/RAGSimple-03MEAIVectorsAISearch/) | Azure AI Search vector store |
| [RAGSimple-04MEAIVectorsQdrant](../samples/CoreSamples/RAGSimple-04MEAIVectorsQdrant/) | Qdrant vector store |

---

## Additional Resources

- [Understand Embeddings](https://learn.microsoft.com/dotnet/ai/conceptual/embeddings): Conceptual guide to how embeddings represent meaning
- [Work with Vector Databases](https://learn.microsoft.com/dotnet/ai/conceptual/vector-databases): Choosing and using vector stores in .NET
- [Build a Vector Search App](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-ai-chat-with-data): Complete quickstart tutorial
- [IEmbeddingGenerator interface](https://learn.microsoft.com/dotnet/ai/iembeddinggenerator): API reference for embedding generation

---

## Up Next

Now that you understand embeddings and semantic search, let's use them to build RAG, grounding AI responses in your own data:

[Continue to Part 2: Retrieval-Augmented Generation (RAG) →](./02-retrieval-augmented-generation.md)
