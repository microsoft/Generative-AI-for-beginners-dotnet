# Retrieval-Augmented Generation (RAG)

In this lesson, you'll learn how to ground AI responses in your own data, making them accurate, current, and relevant to your domain.

---

[![Retrieval-Augmented Generation](./images/LIM_GAN_07_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson3-rag)

_Click the image to watch the video_

---

## The Knowledge Problem

Language models are trained on data from a specific point in time. They don't know about:

- Your company's internal documents
- Events that happened after training
- Private data they were never trained on
- Domain-specific knowledge unique to your organization

**RAG solves this** by giving the AI relevant information at query time.

---

## Part 1: How RAG Works

RAG has two phases:

### Phase 1: Retrieval
Find relevant documents from your knowledge base using semantic search.

### Phase 2: Generation
Pass those documents to the AI along with the user's question.

![How RAG Works](./images/how-rag-works.png)

### The RAG Flow

```
User Question: "What's our refund policy for software?"
         ↓
┌─────────────────────────────────────────────────┐
│ 1. RETRIEVE: Search knowledge base              │
│    - Embed the question                         │
│    - Find similar documents                     │
│    - Return top matches                         │
└─────────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────────┐
│ 2. AUGMENT: Build the prompt                    │
│    - System: "Answer using only this context"   │
│    - Context: [Retrieved documents]             │
│    - User: Original question                    │
└─────────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────────┐
│ 3. GENERATE: Get AI response                    │
│    - AI reads context + question                │
│    - Generates grounded answer                  │
│    - Response cites actual policy               │
└─────────────────────────────────────────────────┘
         ↓
Answer: "Our software refund policy allows returns 
within 30 days of purchase with proof of receipt..."
```

> **Learn more:** [Retrieval-Augmented Generation (RAG)](https://learn.microsoft.com/dotnet/ai/conceptual/rag) on Microsoft Learn explains when and why to use this pattern.

---

## Part 2: Building a RAG System

Let's build a complete RAG pipeline step by step.

### Step 1: Define Your Data Model

```csharp
using Microsoft.Extensions.VectorData;

public class Document
{
    [VectorStoreKey]
    public string Id { get; set; } = string.Empty;

    [VectorStoreData]
    public string Title { get; set; } = string.Empty;

    [VectorStoreData]
    public string Content { get; set; } = string.Empty;

    [VectorStoreData]
    public string Category { get; set; } = string.Empty;

    [VectorStoreVector(1536, DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}
```

### Step 2: Set Up Your Services

```csharp
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Azure;
using Azure.AI.Inference;

// Chat client for generation
IChatClient chatClient = new ChatCompletionsClient(
    endpoint: new Uri("https://models.github.ai/inference"),
    new AzureKeyCredential(githubToken))
    .AsIChatClient("gpt-4o-mini");

// Embedding generator for retrieval
IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator =
    new EmbeddingsClient(
        endpoint: new Uri("https://models.github.ai/inference"),
        new AzureKeyCredential(githubToken))
    .AsIEmbeddingGenerator("text-embedding-3-small");

// Vector store
var vectorStore = new InMemoryVectorStore();
var collection = vectorStore.GetCollection<string, Document>("docs");
await collection.EnsureCollectionExistsAsync();
```

### Step 3: Populate Your Knowledge Base

```csharp
var documents = new[]
{
    new { Id = "policy-1", Title = "Refund Policy", 
          Category = "Policies",
          Content = "Software purchases can be refunded within 30 days of purchase with proof of receipt. Digital downloads are non-refundable once activated." },
    
    new { Id = "policy-2", Title = "Hardware Warranty", 
          Category = "Policies",
          Content = "All hardware products come with a 2-year warranty covering manufacturing defects. Extended warranties are available for purchase." },
    
    new { Id = "faq-1", Title = "Account Recovery", 
          Category = "FAQ",
          Content = "To recover your account, click 'Forgot Password' on the login page. A reset link will be sent to your registered email within 5 minutes." },
    
    // Add more documents...
};

foreach (var doc in documents)
{
    var embedding = await embeddingGenerator.GenerateVectorAsync(doc.Content);
    await collection.UpsertAsync(new Document
    {
        Id = doc.Id,
        Title = doc.Title,
        Category = doc.Category,
        Content = doc.Content,
        Embedding = embedding
    });
}

Console.WriteLine($"Indexed {documents.Length} documents");
```

### Step 4: Implement Retrieval

```csharp
async Task<List<Document>> RetrieveAsync(string query, int topK = 3)
{
    var queryEmbedding = await embeddingGenerator.GenerateVectorAsync(query);
    var results = collection.SearchAsync(queryEmbedding, top: topK);
    
    var documents = new List<Document>();
    await foreach (var result in results)
    {
        documents.Add(result.Record);
    }
    
    return documents;
}
```

### Step 5: Build the Augmented Prompt

```csharp
string BuildPrompt(string question, List<Document> context)
{
    var contextText = string.Join("\n\n", context.Select(d => 
        $"### {d.Title}\n{d.Content}"));
    
    return $"""
        You are a helpful assistant. Answer the user's question using ONLY the 
        information provided in the context below. If the answer is not in the 
        context, say "I don't have information about that."
        
        ## Context
        {contextText}
        
        ## Question
        {question}
        """;
}
```

### Step 6: Generate the Response

```csharp
async Task<string> AskAsync(string question)
{
    // 1. Retrieve relevant documents
    var relevantDocs = await RetrieveAsync(question);
    
    Console.WriteLine($"Found {relevantDocs.Count} relevant documents:");
    foreach (var doc in relevantDocs)
    {
        Console.WriteLine($"  - {doc.Title}");
    }
    
    // 2. Build augmented prompt
    var prompt = BuildPrompt(question, relevantDocs);
    
    // 3. Generate response
    var response = await chatClient.GetResponseAsync(prompt);
    
    return response.Text;
}
```

### Putting It All Together

```csharp
// Ask questions against your knowledge base
var questions = new[]
{
    "Can I get a refund on my software purchase?",
    "How long is the hardware warranty?",
    "I forgot my password, what should I do?"
};

foreach (var question in questions)
{
    Console.WriteLine($"\nQ: {question}");
    var answer = await AskAsync(question);
    Console.WriteLine($"A: {answer}");
    Console.WriteLine(new string('-', 50));
}
```

**Learn more:** [Build a RAG Solution](https://learn.microsoft.com/dotnet/ai/tutorials/tutorial-ai-vector-search) walks through building a complete RAG application step by step.

---

## Part 3: RAG Best Practices

### Chunking Strategies

Large documents need to be split into smaller chunks for effective retrieval.

**Learn more:** [Chunking strategies](https://learn.microsoft.com/azure/search/vector-search-how-to-chunk-documents) covers best practices for splitting documents effectively.

```csharp
IEnumerable<string> ChunkText(string text, int chunkSize = 500, int overlap = 50)
{
    var words = text.Split(' ');
    
    for (int i = 0; i < words.Length; i += chunkSize - overlap)
    {
        var chunk = string.Join(" ", words.Skip(i).Take(chunkSize));
        if (!string.IsNullOrWhiteSpace(chunk))
            yield return chunk;
    }
}

// Use it when indexing
var content = File.ReadAllText("long-document.txt");
var chunks = ChunkText(content).ToList();

for (int i = 0; i < chunks.Count; i++)
{
    var embedding = await embeddingGenerator.GenerateVectorAsync(chunks[i]);
    await collection.UpsertAsync(new Document
    {
        Id = $"doc-chunk-{i}",
        Title = $"Document Part {i + 1}",
        Content = chunks[i],
        Embedding = embedding
    });
}
```

### Retrieval Quality

| Technique | Purpose |
|-----------|---------|
| **Increase top-K** | Get more context (but may add noise) |
| **Reranking** | Score retrieved docs by relevance |
| **Hybrid search** | Combine vector + keyword search |
| **Metadata filtering** | Filter by category, date, etc. |

### Prompt Engineering for RAG

```csharp
// Be explicit about using only the context
var systemPrompt = """
    You are a customer support assistant. Your job is to help users 
    by answering their questions based on our company documentation.
    
    RULES:
    1. Only use information from the provided context
    2. If the answer isn't in the context, say so clearly
    3. Quote relevant parts of the documentation when possible
    4. Be concise but complete
    """;
```

---

## Part 4: RAG with Ollama (Local)

Run RAG entirely locally with Ollama:

```csharp
using OllamaSharp;

// Local Ollama client
var ollamaClient = new OllamaApiClient("http://localhost:11434");

// Use as chat client
IChatClient chatClient = ollamaClient.AsIChatClient("phi4-mini");

// Use as embedding generator
IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = 
    ollamaClient.AsIEmbeddingGenerator("all-minilm");

// Rest of the RAG code stays the same!
```

**Local Models for RAG:**

| Task | Recommended Models |
|------|-------------------|
| Chat/Generation | phi4-mini, llama3.2, mistral |
| Embeddings | all-minilm, nomic-embed-text |

---

## Let's Review: What You Learned

| Concept | Key Takeaway |
|---------|-------------|
| **RAG** | Retrieve relevant docs, then generate response |
| **Why RAG** | Ground AI in your current, private data |
| **Chunking** | Split large docs for better retrieval |
| **Prompt Engineering** | Tell the AI to use only the context |
| **Provider Flexibility** | Same code works with cloud or local models |

### Quick Self-Check

1. What are the two phases of RAG?
2. Why do we need to chunk large documents?
3. What happens if the user asks about something not in your knowledge base?

---

## Sample Code Reference

| Sample | Description |
|--------|-------------|
| [RAGSimple-02MEAIVectorsMemory](../samples/CoreSamples/RAGSimple-02MEAIVectorsMemory/) | RAG with in-memory vectors |
| [RAGSimple-03MEAIVectorsAISearch](../samples/CoreSamples/RAGSimple-03MEAIVectorsAISearch/) | RAG with Azure AI Search |
| [RAGSimple-04MEAIVectorsQdrant](../samples/CoreSamples/RAGSimple-04MEAIVectorsQdrant/) | RAG with Qdrant |
| [RAGSimple-15Ollama-DeepSeekR1](../samples/CoreSamples/RAGSimple-15Ollama-DeepSeekR1/) | RAG with DeepSeek reasoning |

---

## Additional Resources

- [Build a RAG Solution](https://learn.microsoft.com/dotnet/ai/tutorials/tutorial-ai-vector-search): Complete tutorial for building RAG in .NET
- [RAG Pattern Deep Dive](https://learn.microsoft.com/dotnet/ai/conceptual/rag): Conceptual overview of retrieval-augmented generation
- [Azure AI Search Vectors](https://learn.microsoft.com/azure/search/vector-search-overview): Enterprise vector search with Azure
- [Chunking strategies](https://learn.microsoft.com/azure/search/vector-search-how-to-chunk-documents): Best practices for splitting documents

---

## Up Next

RAG helps AI understand your documents, but what about understanding images and visual content?

[Continue to Part 3: Vision and Document Understanding →](./03-vision-document-understanding.md)
