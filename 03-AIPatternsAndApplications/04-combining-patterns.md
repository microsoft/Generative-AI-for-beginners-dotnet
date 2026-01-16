# Combining Patterns

In this lesson, you'll learn how to combine the patterns you've learned to build sophisticated AI applications that solve complex real-world problems.

---

[![Combining AI Patterns](./images/LIM_GAN_07_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson3-patterns)

_Click the image to watch the video_

---

## Real Problems Need Multiple Patterns

Most real-world AI applications don't use just one pattern. They combine several:

| Application | Patterns Used |
|------------|---------------|
| **Customer support bot** | RAG + Function calling + Streaming |
| **Document search engine** | Vision + Embeddings + RAG |
| **Meeting assistant** | Audio transcription + Summarization + RAG |
| **Product recommendation** | Embeddings + Structured output + Chat |

This lesson shows you how to combine patterns effectively.

---

## Part 1: Pattern Selection Guide

### When to Use Each Pattern

```
┌─────────────────────────────────────────────────────────────────┐
│                     PATTERN SELECTION                           │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Need to find similar content?                                  │
│     └─► Embeddings + Semantic Search                           │
│                                                                 │
│  Need to ground responses in your data?                         │
│     └─► RAG (Retrieval-Augmented Generation)                   │
│                                                                 │
│  Need to understand images or documents?                        │
│     └─► Vision / Multimodal AI                                 │
│                                                                 │
│  Need the AI to take actions?                                   │
│     └─► Function Calling (from Lesson 2)                       │
│                                                                 │
│  Need structured, parseable responses?                          │
│     └─► Structured Output (from Lesson 2)                      │
│                                                                 │
│  Need real-time responses?                                      │
│     └─► Streaming (from Lesson 2)                              │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

**Learn more:** [AI Application Patterns](https://learn.microsoft.com/azure/architecture/ai-ml/) on Azure Architecture Center provides guidance on choosing and combining patterns.

---

## Part 2: Visual Document Search

**Problem:** Search through a library of documents (PDFs, images) using natural language.

**Patterns:** Vision + Embeddings + RAG

### Architecture

```
User Query: "Find documents about quarterly sales in Europe"
         ↓
┌─────────────────────────────────────────────────┐
│ 1. PREPROCESSING (done once)                    │
│    - Convert PDFs to images                     │
│    - Use vision to extract text summaries       │
│    - Generate embeddings for each document      │
│    - Store in vector database                   │
└─────────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────────┐
│ 2. SEARCH (at query time)                       │
│    - Embed the user query                       │
│    - Find similar document embeddings           │
│    - Return matching documents                  │
└─────────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────────┐
│ 3. ANSWER (optional)                            │
│    - Use vision to read the actual documents    │
│    - Generate answer using RAG                  │
└─────────────────────────────────────────────────┘
```

### Implementation

```csharp
public class VisualDocumentSearch
{
    private readonly IChatClient _chatClient;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embedder;
    private readonly IVectorStoreRecordCollection<string, DocumentRecord> _collection;

    public record DocumentRecord
    {
        [VectorStoreKey]
        public string Id { get; set; } = "";
        
        [VectorStoreData]
        public string FilePath { get; set; } = "";
        
        [VectorStoreData]
        public string Summary { get; set; } = "";
        
        [VectorStoreVector(1536, DistanceFunction.CosineSimilarity)]
        public ReadOnlyMemory<float> Embedding { get; set; }
    }

    // Index a document image
    public async Task IndexDocumentAsync(string imagePath)
    {
        var imageBytes = await File.ReadAllBytesAsync(imagePath);
        
        // 1. Use vision to create a searchable summary
        var summary = await GetDocumentSummaryAsync(imageBytes);
        
        // 2. Generate embedding from the summary
        var embedding = await _embedder.GenerateVectorAsync(summary);
        
        // 3. Store in vector database
        await _collection.UpsertAsync(new DocumentRecord
        {
            Id = Path.GetFileName(imagePath),
            FilePath = imagePath,
            Summary = summary,
            Embedding = embedding
        });
    }

    private async Task<string> GetDocumentSummaryAsync(byte[] imageBytes)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, """
                Create a detailed summary of this document for search indexing.
                Include: document type, main topics, key entities, dates, and numbers.
                """),
            new(ChatRole.User, new AIContent[]
            {
                new TextContent("Summarize this document for search:"),
                new DataContent(imageBytes, "image/png")
            })
        };
        
        var response = await _chatClient.GetResponseAsync(messages);
        return response.Text;
    }

    // Search for documents
    public async Task<List<SearchResult>> SearchAsync(string query, int topK = 5)
    {
        var queryEmbedding = await _embedder.GenerateVectorAsync(query);
        var results = new List<SearchResult>();
        
        await foreach (var match in _collection.SearchAsync(queryEmbedding, top: topK))
        {
            results.Add(new SearchResult
            {
                FilePath = match.Record.FilePath,
                Summary = match.Record.Summary,
                Score = match.Score ?? 0
            });
        }
        
        return results;
    }

    // Answer a question about specific documents
    public async Task<string> AnswerQuestionAsync(string question, List<string> documentPaths)
    {
        var contents = new List<AIContent> { new TextContent(question) };
        
        foreach (var path in documentPaths)
        {
            var imageBytes = await File.ReadAllBytesAsync(path);
            contents.Add(new DataContent(imageBytes, "image/png"));
        }
        
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, """
                Answer the user's question based on the provided documents.
                Reference specific documents in your answer.
                """),
            new(ChatRole.User, contents)
        };
        
        var response = await _chatClient.GetResponseAsync(messages);
        return response.Text;
    }
}
```

**Learn more:** [Build a Vector Search App](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-ai-chat-with-data) provides a quickstart for combining vision with vector search.

---

## Part 3: Customer Support Bot

**Problem:** Build a support bot that answers questions, can take actions, and streams responses.

**Patterns:** RAG + Function Calling + Streaming

### Architecture

```
Customer: "What's my order status and can you extend the return window?"
         ↓
┌─────────────────────────────────────────────────────────────────┐
│ 1. RAG: Find relevant policies                                  │
│    - Search: "order status" → Order tracking policy             │
│    - Search: "return window" → Return policy documentation      │
└─────────────────────────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────────────────────────┐
│ 2. FUNCTION CALLING: Take actions                               │
│    - Call: GetOrderStatus(customerId)                          │
│    - Call: ExtendReturnWindow(orderId, days)                   │
└─────────────────────────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────────────────────────┐
│ 3. STREAMING: Real-time response                                │
│    - Stream the response as it's generated                      │
│    - Show typing indicator                                      │
└─────────────────────────────────────────────────────────────────┘
```

### Implementation

```csharp
public class SupportBot
{
    private readonly IChatClient _chatClient;
    private readonly KnowledgeBase _kb;
    
    // Define available tools
    [Description("Get the current status of a customer's order")]
    async Task<string> GetOrderStatus(int orderId)
    {
        // Call your order system
        return $"Order {orderId}: Shipped on Dec 15, arriving Dec 18";
    }
    
    [Description("Extend the return window for an order")]
    async Task<string> ExtendReturnWindow(int orderId, int additionalDays)
    {
        // Call your order system
        return $"Return window for order {orderId} extended by {additionalDays} days";
    }
    
    public async IAsyncEnumerable<string> RespondAsync(string customerId, string question)
    {
        // 1. RAG: Get relevant knowledge
        var relevantDocs = await _kb.SearchAsync(question, topK: 3);
        var context = string.Join("\n\n", relevantDocs.Select(d => d.Content));
        
        // 2. Build messages with context and tools
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, $"""
                You are a customer support agent. Use the knowledge base to answer 
                questions accurately. Use the available tools to take actions.
                
                Customer ID: {customerId}
                
                Knowledge Base:
                {context}
                """),
            new(ChatRole.User, question)
        };
        
        var options = new ChatOptions
        {
            Tools = [
                AIFunctionFactory.Create(GetOrderStatus),
                AIFunctionFactory.Create(ExtendReturnWindow)
            ]
        };
        
        // 3. Stream the response
        await foreach (var update in _chatClient.GetStreamingResponseAsync(messages, options))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                yield return update.Text;
            }
        }
    }
}

// Usage in a web API
app.MapPost("/api/support", async (SupportRequest request, SupportBot bot) =>
{
    async IAsyncEnumerable<string> StreamResponse()
    {
        await foreach (var chunk in bot.RespondAsync(request.CustomerId, request.Question))
        {
            yield return chunk;
        }
    }
    
    return Results.Stream(StreamResponse(), "text/event-stream");
});
```

---

## Part 4: Intelligent Form Processor

**Problem:** Extract data from various form formats and validate it.

**Patterns:** Vision + Structured Output

### Implementation

```csharp
public class FormProcessor
{
    private readonly IChatClient _chatClient;
    
    public class ExtractedFormData
    {
        public string FormType { get; set; } = "";
        public Dictionary<string, string> Fields { get; set; } = new();
        public List<ValidationIssue> Issues { get; set; } = new();
        public float Confidence { get; set; }
    }
    
    public class ValidationIssue
    {
        public string Field { get; set; } = "";
        public string Issue { get; set; } = "";
        public string Suggestion { get; set; } = "";
    }
    
    public async Task<ExtractedFormData> ProcessFormAsync(byte[] formImage)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, """
                You are a form processing assistant. Extract all fields from forms
                and identify any potential issues or missing information.
                """),
            new(ChatRole.User, new AIContent[]
            {
                new TextContent("""
                    Extract all fields from this form. For each field:
                    1. Identify the field name/label
                    2. Extract the value (or note if empty)
                    3. Flag any issues (illegible, incomplete, inconsistent)
                    
                    Also determine the form type and your confidence level.
                    """),
                new DataContent(formImage, "image/png")
            })
        };
        
        // Use structured output
        var response = await _chatClient.GetResponseAsync<ExtractedFormData>(messages);
        return response;
    }
}

// Usage
var processor = new FormProcessor(chatClient);
var formImage = await File.ReadAllBytesAsync("application-form.png");
var data = await processor.ProcessFormAsync(formImage);

Console.WriteLine($"Form Type: {data.FormType} (Confidence: {data.Confidence:P0})");
Console.WriteLine("\nExtracted Fields:");
foreach (var (field, value) in data.Fields)
{
    Console.WriteLine($"  {field}: {value}");
}

if (data.Issues.Any())
{
    Console.WriteLine("\nIssues Found:");
    foreach (var issue in data.Issues)
    {
        Console.WriteLine($"  [{issue.Field}] {issue.Issue}");
        Console.WriteLine($"    Suggestion: {issue.Suggestion}");
    }
}
```

---

## Part 5: Multi-Source Knowledge Assistant

**Problem:** Answer questions using multiple data sources (documents, databases, APIs).

**Patterns:** RAG + Function Calling + Embeddings

### Implementation

```csharp
public class KnowledgeAssistant
{
    private readonly IChatClient _chatClient;
    private readonly DocumentStore _documents;  // RAG
    private readonly DatabaseService _database;  // SQL queries
    private readonly ApiService _apis;           // External APIs
    
    [Description("Search internal documentation")]
    async Task<string> SearchDocs(string query) 
        => await _documents.SearchAsync(query);
    
    [Description("Query the sales database")]
    async Task<string> QueryDatabase(string sqlDescription)
        => await _database.QueryAsync(sqlDescription);
    
    [Description("Get current stock price")]
    async Task<string> GetStockPrice(string symbol)
        => await _apis.GetStockPriceAsync(symbol);
    
    [Description("Get weather information")]
    async Task<string> GetWeather(string location)
        => await _apis.GetWeatherAsync(location);
    
    public async Task<string> AskAsync(string question)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, """
                You are a knowledge assistant with access to:
                1. Internal documentation (use SearchDocs)
                2. Sales database (use QueryDatabase)
                3. Stock prices (use GetStockPrice)
                4. Weather data (use GetWeather)
                
                Use the appropriate tools to answer questions completely.
                Combine information from multiple sources when needed.
                """),
            new(ChatRole.User, question)
        };
        
        var options = new ChatOptions
        {
            Tools = [
                AIFunctionFactory.Create(SearchDocs),
                AIFunctionFactory.Create(QueryDatabase),
                AIFunctionFactory.Create(GetStockPrice),
                AIFunctionFactory.Create(GetWeather)
            ]
        };
        
        // Let the AI choose which tools to use
        var response = await _chatClient.GetResponseAsync(messages, options);
        return response.Text;
    }
}

// Usage
var assistant = new KnowledgeAssistant(...);

// This might use SearchDocs + QueryDatabase
var answer1 = await assistant.AskAsync(
    "What's our return policy and how many returns did we have last month?");

// This might use GetStockPrice + SearchDocs
var answer2 = await assistant.AskAsync(
    "What's Microsoft's current stock price and what does our investment policy say about tech stocks?");
```

---

## Part 6: Pattern Composition Best Practices

### Keep Components Modular

```csharp
// Good: Each component has a single responsibility
public interface IDocumentSearcher
{
    Task<List<Document>> SearchAsync(string query, int topK);
}

public interface IDocumentReader
{
    Task<string> ExtractTextAsync(byte[] image);
}

public interface IAnswerGenerator
{
    Task<string> GenerateAsync(string question, List<Document> context);
}

// Compose them
public class DocumentQA
{
    private readonly IDocumentSearcher _searcher;
    private readonly IDocumentReader _reader;
    private readonly IAnswerGenerator _generator;
    
    public async Task<string> AskAsync(string question)
    {
        var docs = await _searcher.SearchAsync(question, topK: 3);
        // Can enhance with _reader if needed
        return await _generator.GenerateAsync(question, docs);
    }
}
```

### Handle Failures Gracefully

```csharp
public async Task<string> ResilientAskAsync(string question)
{
    // Try RAG first
    var docs = await _searcher.SearchAsync(question, topK: 3);
    
    if (docs.Count == 0)
    {
        // Fallback: direct answer with disclaimer
        return await _chatClient.GetResponseAsync($"""
            I couldn't find specific documentation about this. 
            Here's a general answer: {question}
            
            Note: This answer is not based on verified documentation.
            """);
    }
    
    return await GenerateWithContextAsync(question, docs);
}
```

### Optimize Token Usage

```csharp
// Summarize large contexts before sending
async Task<string> SummarizeIfNeeded(string content, int maxTokens = 1000)
{
    var estimatedTokens = content.Length / 4;  // Rough estimate
    
    if (estimatedTokens <= maxTokens)
        return content;
    
    var response = await _chatClient.GetResponseAsync(
        $"Summarize this in {maxTokens / 2} tokens, keeping key facts:\n\n{content}");
    
    return response.Text;
}
```

---

## Let's Review: What You Learned

| Concept | Key Takeaway |
|---------|-------------|
| **Pattern Selection** | Match patterns to problem requirements |
| **Combination** | Real apps use multiple patterns together |
| **Visual Search** | Vision + Embeddings + RAG for document search |
| **Smart Bots** | RAG + Functions + Streaming for support |
| **Modularity** | Keep components separate for flexibility |

### Quick Self-Check

1. When would you combine Vision with RAG?
2. Why use streaming with a support chatbot?
3. How do function calls work with RAG in the same conversation?

---

## Sample Code Reference

The patterns in this lesson build on samples from throughout the workshop:

| Pattern | Samples |
|---------|---------|
| **RAG** | [RAGSimple-*](../samples/CoreSamples/) series |
| **Vision** | [Vision-*](../samples/CoreSamples/) series |
| **Function Calling** | [FunctionCalling-*](../samples/CoreSamples/) series |
| **Structured Output** | [StructuredOutput-*](../samples/CoreSamples/) series |
| **Streaming** | [Streaming-*](../samples/CoreSamples/) series |

---

## Additional Resources

- [Build AI Apps with .NET](https://learn.microsoft.com/dotnet/ai/): The official .NET AI developer hub
- [AI Application Patterns](https://learn.microsoft.com/azure/architecture/ai-ml/): Azure architecture guidance for AI solutions
- [Reference Architectures for AI](https://learn.microsoft.com/azure/architecture/ai-ml/architecture/): Production-ready patterns and blueprints
- [End-to-end AI solutions](https://learn.microsoft.com/dotnet/ai/tutorials/tutorials-overview): Complete tutorials for real-world scenarios

---

## Lesson Complete

You now understand the core AI patterns for .NET development:

- **Embeddings & Semantic Search**: Understanding meaning as vectors
- **RAG**: Grounding AI in your data
- **Vision**: Understanding images and documents
- **Combining Patterns**: Building real applications

These patterns are the building blocks for AI agents, systems that can reason, plan, and take complex actions autonomously.

---

## What's Next?

You're ready to move beyond patterns into building **AI Agents**, systems that can:
- Plan multi-step solutions
- Use tools and APIs autonomously
- Maintain state across interactions
- Collaborate with other agents

[Continue to Lesson 4: AI Agents with Microsoft Agent Framework →](../04-AgentsWithMAF/readme.md)
