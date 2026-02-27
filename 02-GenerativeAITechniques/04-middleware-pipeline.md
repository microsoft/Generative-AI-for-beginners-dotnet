# Middleware Pipelines

In this lesson, you'll learn how to build production-ready AI applications using middleware pipelines. This is a powerful pattern from Microsoft.Extensions.AI that lets you add caching, telemetry, rate limiting, and custom behaviors to your AI client.

---

[![Middleware Pipelines](./images/LIM_GAN_03_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson2-middleware)

_Click the image to watch the video_

---

## Why Middleware?

Production AI applications need more than just calling models. They need:

- **Caching**: Avoid redundant API calls and reduce costs
- **Telemetry**: Track usage, monitor performance, debug issues
- **Rate Limiting**: Prevent overwhelming the AI service
- **Logging**: Record requests and responses for analysis
- **Retry Logic**: Handle transient failures gracefully

Microsoft.Extensions.AI uses a **middleware pipeline** pattern, similar to ASP.NET Core middleware. Each piece of middleware wraps the next, adding its functionality.

---

## Part 1: The ChatClientBuilder Pattern

The `ChatClientBuilder` creates a pipeline of middleware around your base chat client:

```csharp
IChatClient client = new AzureOpenAIClient(
        new Uri(config["endpoint"]),
        new ApiKeyCredential(config["apikey"]))
        .GetChatClient("gpt-4o-mini")
        .AsIChatClient()
    .AsBuilder()                    // Start building the pipeline
    .UseFunctionInvocation()        // Add function calling
    .UseDistributedCache(cache)     // Add caching
    .UseOpenTelemetry(sourceName)   // Add telemetry
    .Build();                       // Create the final client
```

Each `Use*` method adds a layer to the pipeline. Order matters - middleware executes in the order you add it.

**Learn more:** [IChatClient Functionality Pipelines](https://learn.microsoft.com/dotnet/ai/ichatclient#functionality-pipelines) explains how middleware chains work in Microsoft.Extensions.AI.

---

## Part 2: Caching Responses

Caching saves responses for identical requests, reducing API calls and costs.

### Basic In-Memory Caching

```csharp
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

// Create the cache
var cache = new MemoryDistributedCache(
    Options.Create(new MemoryDistributedCacheOptions()));

// Build the client with caching
IChatClient client = new AzureOpenAIClient(
        new Uri(config["endpoint"]),
        new ApiKeyCredential(config["apikey"]))
        .GetChatClient("gpt-4o-mini")
        .AsIChatClient()
    .AsBuilder()
    .UseDistributedCache(cache)
    .Build();

// These calls demonstrate caching
string[] prompts = ["What is AI?", "What is .NET?", "What is AI?"];

foreach (var prompt in prompts)
{
    Console.WriteLine($"Prompt: {prompt}");
    var response = await client.GetResponseAsync(prompt);
    Console.WriteLine($"Response: {response.Text}\n");
}
```

The third prompt "What is AI?" returns immediately from cache instead of calling the API.

### Using Redis for Distributed Caching

For production applications, use Redis or another distributed cache:

```csharp
using Microsoft.Extensions.Caching.StackExchangeRedis;

var cache = new RedisCache(new RedisCacheOptions
{
    Configuration = "localhost:6379"
});

IChatClient client = baseClient
    .AsBuilder()
    .UseDistributedCache(cache)
    .Build();
```

**Learn more:** [Caching in .NET](https://learn.microsoft.com/dotnet/core/extensions/caching) covers distributed caching patterns for production applications.

---

## Part 3: Adding Telemetry

Telemetry helps you monitor AI usage, track costs, and debug issues.

### OpenTelemetry Integration

```csharp
using Microsoft.Extensions.AI;
using OpenTelemetry.Trace;

// Configure OpenTelemetry
string sourceName = "MyChatApp";
var tracerProvider = OpenTelemetry.Sdk.CreateTracerProviderBuilder()
    .AddSource(sourceName)
    .AddConsoleExporter()  // Or AddOtlpExporter for production
    .Build();

// Build the client with telemetry
IChatClient client = new AzureOpenAIClient(
        new Uri(config["endpoint"]),
        new ApiKeyCredential(config["apikey"]))
        .GetChatClient("gpt-4o-mini")
        .AsIChatClient()
    .AsBuilder()
    .UseOpenTelemetry(
        sourceName: sourceName,
        configure: c => c.EnableSensitiveData = true)  // Include prompt/response in traces
    .Build();

// Now all requests are automatically traced
var response = await client.GetResponseAsync("What is machine learning?");
```

The telemetry includes:

- Request timing
- Token counts (input/output)
- Model information
- Errors and exceptions
- Optionally: full prompts and responses

### Simple Logging

For simpler logging needs, use the logging middleware:

```csharp
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder => 
    builder.AddConsole().SetMinimumLevel(LogLevel.Debug));

IChatClient client = baseClient
    .AsBuilder()
    .UseLogging(loggerFactory)
    .Build();
```

**Learn more:** [OpenTelemetry in .NET](https://learn.microsoft.com/dotnet/core/diagnostics/observability-with-otel) provides comprehensive guidance on observability and tracing.

---

## Part 4: Combining Multiple Middleware

The real power comes from combining multiple middleware:

```csharp
IChatClient client = new AzureOpenAIClient(
        new Uri(config["endpoint"]),
        new ApiKeyCredential(config["apikey"]))
        .GetChatClient("gpt-4o-mini")
        .AsIChatClient()
    .AsBuilder()
    .UseDistributedCache(cache)           // Check cache first
    .UseFunctionInvocation()              // Enable function calling
    .UseOpenTelemetry(sourceName: "App")  // Track everything
    .Build();
```

### Understanding Execution Order

Middleware executes in the order added:

```
Request → Cache Check → Function Invocation → Telemetry → AI Model
Response ← Cache Store ← Function Results ← Telemetry ← AI Model
```

If the cache has a hit, the request never reaches the AI model.

---

## Part 5: Configuring Default Options

Set default options for all requests:

```csharp
IChatClient client = new AzureOpenAIClient(new Uri(config["endpoint"]), new ApiKeyCredential(config["apikey"]))
    .GetChatClient("gpt-4o-mini")
    .AsIChatClient()
    .AsBuilder()
    .ConfigureOptions(options => 
    {
        options.Temperature = 0.7f;
        options.MaxOutputTokens = 1000;
    })
    .Build();

// All requests use these defaults unless overridden
var response = await client.GetResponseAsync("Tell me a story");

// Override for specific request
var response2 = await client.GetResponseAsync(
    "Tell me a short story", 
    new ChatOptions { MaxOutputTokens = 200 });
```

---

## Part 6: Custom Middleware

You can create custom middleware for specialized needs.

### Inline Middleware

For simple cases, use the `Use` method with a delegate:

```csharp
IChatClient client = baseClient
    .AsBuilder()
    .Use(async (messages, options, next, cancellation) =>
    {
        Console.WriteLine($"Request at: {DateTime.Now}");
        Console.WriteLine($"Message count: {messages.Count()}");
        
        // Call the next middleware in the pipeline
        var response = await next(messages, options, cancellation);
        
        Console.WriteLine($"Response received at: {DateTime.Now}");
        return response;
    })
    .Build();
```

### Custom Middleware Class

For more complex scenarios, create a `DelegatingChatClient`:

```csharp
using Microsoft.Extensions.AI;
using System.Threading.RateLimiting;

public sealed class RateLimitingChatClient : DelegatingChatClient
{
    private readonly RateLimiter _rateLimiter;

    public RateLimitingChatClient(IChatClient innerClient, RateLimiter rateLimiter)
        : base(innerClient)
    {
        _rateLimiter = rateLimiter;
    }

    public override async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        using var lease = await _rateLimiter.AcquireAsync(
            permitCount: 1, 
            cancellationToken);
            
        if (!lease.IsAcquired)
            throw new InvalidOperationException("Rate limit exceeded");

        return await base.GetResponseAsync(messages, options, cancellationToken);
    }

    public override async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var lease = await _rateLimiter.AcquireAsync(
            permitCount: 1, 
            cancellationToken);
            
        if (!lease.IsAcquired)
            throw new InvalidOperationException("Rate limit exceeded");

        await foreach (var update in base.GetStreamingResponseAsync(
            messages, options, cancellationToken))
        {
            yield return update;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _rateLimiter.Dispose();
        base.Dispose(disposing);
    }
}
```

### Creating an Extension Method

Make your middleware easy to use:

```csharp
public static class RateLimitingExtensions
{
    public static ChatClientBuilder UseRateLimiting(
        this ChatClientBuilder builder,
        RateLimiter rateLimiter)
    {
        return builder.Use(innerClient => 
            new RateLimitingChatClient(innerClient, rateLimiter));
    }
}

// Usage
var limiter = new ConcurrencyLimiter(new ConcurrencyLimiterOptions
{
    PermitLimit = 1,
    QueueLimit = 10
});

IChatClient client = baseClient
    .AsBuilder()
    .UseRateLimiting(limiter)
    .Build();
```

**Learn more:** [IChatClient Custom Middleware](https://learn.microsoft.com/dotnet/ai/ichatclient#custom-ichatclient-middleware) shows how to build your own middleware components.

---

## Part 7: Dependency Injection

For ASP.NET Core and other DI-enabled applications:

```csharp
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();

// Register the distributed cache
builder.Services.AddDistributedMemoryCache();

// Register the chat client with middleware
builder.Services.AddChatClient(services =>
{
    var cache = services.GetRequiredService<IDistributedCache>();
    
    return new AzureOpenAIClient(
        new Uri(config["endpoint"]),
        new ApiKeyCredential(config["apikey"]))
        .GetChatClient("gpt-4o-mini")
        .AsIChatClient()
        .AsBuilder()
        .UseDistributedCache(cache)
        .UseFunctionInvocation()
        .UseOpenTelemetry(sourceName: "MyApp")
        .Build(services);
});

var host = builder.Build();

// Use the client anywhere via DI
var chatClient = host.Services.GetRequiredService<IChatClient>();
var response = await chatClient.GetResponseAsync("Hello!");
```

**Learn more:** [Dependency Injection in .NET](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection) covers the DI patterns used in ASP.NET Core and hosted services.

---

## Part 8: Production-Ready Example

Here's a complete example combining all these concepts:

```csharp
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;

var builder = Host.CreateApplicationBuilder();

// Configure services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddLogging(b => b.AddConsole());

// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("MyChatApp")
        .AddConsoleExporter());

// Register the AI client with full middleware pipeline
builder.Services.AddChatClient(services =>
{
    var cache = services.GetRequiredService<IDistributedCache>();
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    
    return new AzureOpenAIClient(
        new Uri(config["endpoint"]),
        new ApiKeyCredential(config["apikey"]))
        .GetChatClient("gpt-4o-mini")
        .AsIChatClient()
        .AsBuilder()
        .ConfigureOptions(opt => opt.Temperature = 0.7f)
        .UseDistributedCache(cache)
        .UseFunctionInvocation()
        .UseLogging(loggerFactory)
        .UseOpenTelemetry(sourceName: "MyChatApp")
        .Build(services);
});

var host = builder.Build();
var chatClient = host.Services.GetRequiredService<IChatClient>();

// Use the production-ready client
Console.WriteLine("Production AI Client Ready!");
var response = await chatClient.GetResponseAsync("What is .NET?");
Console.WriteLine(response.Text);
```

---

## Let's Review: What You Learned

| Concept | Key Takeaway |
|---------|-------------|
| **ChatClientBuilder** | Creates middleware pipelines around chat clients |
| **Caching** | Reduces API calls for identical requests |
| **Telemetry** | Tracks usage, performance, and errors |
| **Custom Middleware** | Add any behavior using DelegatingChatClient |
| **Dependency Injection** | Integrate with ASP.NET Core and hosted services |

### Quick Self-Check

1. Why does middleware order matter in the pipeline?
2. What are the benefits of caching AI responses?
3. How would you add retry logic to handle transient failures?

---

## Sample Code Reference

| Sample | Description |
|--------|-------------|
| [MEAIFunctions](../samples/CoreSamples/MEAIFunctions/) | Function calling with ChatClientBuilder pipeline |
| [MEAIFunctionsOllama](../samples/CoreSamples/MEAIFunctionsOllama/) | Middleware pipeline with local Ollama models |
| [MAF-AIWebChatApp-Middleware](../samples/MAF/MAF-AIWebChatApp-Middleware/) | Custom middleware in a web application |
| [MAF-BackgroundResponses-02-Tools](../samples/MAF/MAF-BackgroundResponses-02-Tools/) | OpenTelemetry integration example |

---

## Additional Resources

- [IChatClient Functionality Pipelines](https://learn.microsoft.com/dotnet/ai/ichatclient#functionality-pipelines): How middleware chains work in MEAI
- [IChatClient Custom Middleware](https://learn.microsoft.com/dotnet/ai/ichatclient#custom-ichatclient-middleware): Building your own middleware components
- [Caching in .NET](https://learn.microsoft.com/dotnet/core/extensions/caching): Deep dive into distributed caching patterns
- [OpenTelemetry in .NET](https://learn.microsoft.com/dotnet/core/diagnostics/observability-with-otel): Observability and tracing for production apps

---

## Lesson Summary

Congratulations! You've completed Lesson 2 on Generative AI Techniques!

You learned:

1. **Text Completions and Chat**: Single responses and conversational AI with memory
2. **Streaming and Structured Output**: Real-time responses and typed results
3. **Function Calling**: Extending AI with your own .NET code
4. **Middleware Pipelines**: Production-ready caching, telemetry, and custom behaviors

These are the foundational patterns you'll use in every AI application.

---

## Next Steps

In Lesson 3, we'll build on these foundations with advanced patterns:

- **Retrieval-Augmented Generation (RAG)**: Ground AI in your own documents
- **Semantic Search**: Search by meaning, not just keywords
- **Vision and Audio**: Work with images and sound

[Continue to Lesson 3: AI Patterns and Applications →](../03-AIPatternsAndApplications/readme.md)
