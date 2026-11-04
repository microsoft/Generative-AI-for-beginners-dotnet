using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using OllamaSharp;
using System.Text.Json;

string mode = args.FirstOrDefault() ?? "stream";
if (args.Length > 1 || mode is not ("stream" or "structured" or "cache"))
{
    throw new ArgumentException("Usage: dotnet run -- [stream|structured|cache]");
}
using var httpClient = new HttpClient
{
    BaseAddress = new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434"),
    Timeout = TimeSpan.FromMinutes(2)
};
using IChatClient client = new OllamaApiClient(
    httpClient, Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "phi4-mini");
using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(2));
var options = new ChatOptions { Temperature = 0 };

switch (mode)
{
    case "stream":
        int chunks = 0;
        await foreach (var update in client.GetStreamingResponseAsync(
            "Explain local AI in two short sentences.", options, timeout.Token))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                chunks++;
                Console.Write(update.Text);
            }
        }
        Console.WriteLine($"\nStreaming text chunks: {chunks}");
        if (chunks == 0)
        {
            throw new InvalidOperationException("The model returned no streaming text.");
        }
        break;

    case "structured":
        var result = await client.GetResponseAsync<Review>(
            "Classify this review: 'The product is excellent.' Use sentiment Positive and rating 5.",
            options, cancellationToken: timeout.Token);
        Review review = result.Result;
        if (review.Sentiment != "Positive" || review.Rating != 5)
        {
            throw new InvalidOperationException("The structured review did not match the input. Try a more capable model.");
        }
        Console.WriteLine(JsonSerializer.Serialize(review));
        break;

    case "cache":
        {
            var cache = new MemoryDistributedCache(
                Microsoft.Extensions.Options.Options.Create(new MemoryDistributedCacheOptions()));
            var counter = new CountingChatClient(client);
            using var cachedClient = counter.AsBuilder()
                .UseDistributedCache(cache)
                .Build();
            const string prompt = "What is 2 + 2? Reply with only the number.";
            var first = await cachedClient.GetResponseAsync(prompt, options, timeout.Token);
            var second = await cachedClient.GetResponseAsync(prompt, options, timeout.Token);
            Console.WriteLine($"First: {first.Text}\nCached: {second.Text}\nModel calls: {counter.Calls}");
            if (counter.Calls != 1 || string.IsNullOrWhiteSpace(first.Text) || first.Text != second.Text)
            {
                throw new InvalidOperationException("Expected the second response to come from the cache.");
            }
            break;
        }
}

internal sealed record Review(string Sentiment, int Rating);

internal sealed class CountingChatClient(IChatClient innerClient) : DelegatingChatClient(innerClient)
{
    public int Calls { get; private set; }

    public override Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        Calls++;
        return base.GetResponseAsync(messages, options, cancellationToken);
    }
}
