using FoundryLocal.Course;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Text.Json;

string mode = FoundryCourseChecks.Mode(args, "hello", "prepare", "hello", "history", "stream", "structured", "cache", "tools");
await using var runtime = await FoundryCourseRuntime.StartAsync(
    args.Contains("--prepare"), tools: mode == "tools");
if (mode == "prepare")
{
    Console.WriteLine("Chat model prepared.");
    return;
}
using IChatClient client = runtime.CreateChatClient();
using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(3));
var options = FoundryCourseChecks.TextOptions;
switch (mode)
{
    case "hello":
        var hello = await client.GetResponseAsync("What is 2 + 2? Reply with only the number.", options, timeout.Token);
        Console.WriteLine($"Answer: {FoundryCourseChecks.RequireText(hello.Text)}");
        if (hello.Text.Trim() != "4")
        {
            throw new InvalidOperationException("Expected 4. The selected model failed the Hello World quality check.");
        }
        break;
    case "history":
        List<ChatMessage> messages =
        [
            new(ChatRole.System, "Remember facts from this conversation. Reply briefly."),
            new(ChatRole.User, "My secret demo word is ORCHID. Acknowledge it.")
        ];
        var first = await client.GetResponseAsync(messages, options, timeout.Token);
        FoundryCourseChecks.RequireText(first.Text);
        messages.AddRange(first.Messages);
        messages.Add(new(ChatRole.User, "What is my secret demo word? Reply with only that word."));
        var second = await client.GetResponseAsync(messages, options, timeout.Token);
        Console.WriteLine($"Remembered: {FoundryCourseChecks.RequireText(second.Text)}");
        if (!second.Text.Contains("ORCHID", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("The model did not remember the previous turn.");
        }
        break;
    case "stream":
        int chunks = 0;
        await foreach (var update in client.GetStreamingResponseAsync(
            "Explain running language model inference on your own computer, instead of a cloud server, in two short sentences.",
            options, timeout.Token))
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
            throw new InvalidOperationException("No streaming text received.");
        }
        break;
    case "structured":
        // SDK 2.3.1 expects a different JSON-schema envelope than the OpenAI client.
        // Request JSON-object decoding and validate the typed contract explicitly.
        var result = await client.GetResponseAsync(
            """Classify "Excellent product!" Return exactly {"Sentiment":"Positive","Rating":5}, with no extra fields.""",
            new ChatOptions { Temperature = 0, MaxOutputTokens = 128, ResponseFormat = ChatResponseFormat.Json, ToolMode = ChatToolMode.None },
            timeout.Token);
        var review = FoundryCourseChecks.ParseReview(FoundryCourseChecks.RequireText(result.Text));
        Console.WriteLine(JsonSerializer.Serialize(review));
        break;
    case "cache":
        var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var counter = new CountingChatClient(client);
        using (var cached = counter.AsBuilder().UseDistributedCache(cache).Build())
        {
            var a = await cached.GetResponseAsync("What is 2 + 2? Reply with only the number.", options, timeout.Token);
            var b = await cached.GetResponseAsync("What is 2 + 2? Reply with only the number.", options, timeout.Token);
            FoundryCourseChecks.RequireText(a.Text);
            Console.WriteLine($"First: {a.Text}\nCached: {b.Text}\nModel calls: {counter.Calls}");
            if (counter.Calls != 1 || a.Text != b.Text)
            {
                throw new InvalidOperationException("The cache did not prevent the second inference.");
            }
        }
        break;
    case "tools":
        int calls = 0;
        [Description("Get the library's weekday opening hours from fixed demo data.")]
        string GetLibraryHours()
        {
            calls++;
            Console.WriteLine("GetLibraryHours invoked.");
            return "The library opens at 9 AM on weekdays.";
        }
        using (var tools = client.AsBuilder()
            .UseFunctionInvocation(configure: c => c.MaximumIterationsPerRequest = 3).Build())
        {
            var math = await tools.GetResponseAsync("What is 2 + 2? Reply with only the number.", options, timeout.Token);
            Console.WriteLine($"Math without tools: {FoundryCourseChecks.RequireText(math.Text)}; tool calls: {calls}");
            var answer = await tools.GetResponseAsync(
                "Call GetLibraryHours and tell me when the library opens on weekdays.",
                new ChatOptions
                {
                    Temperature = 0,
                    MaxOutputTokens = 256,
                    ToolMode = ChatToolMode.RequireAny,
                    Tools = [AIFunctionFactory.Create(GetLibraryHours, "GetLibraryHours")]
                }, timeout.Token);
            Console.WriteLine($"Answer: {FoundryCourseChecks.RequireText(answer.Text)}\nLibrary tool calls: {calls}");
            if (calls != 1 || !FoundryCourseChecks.ContainsOpeningTime(answer.Text))
            {
                throw new InvalidOperationException("Expected one real library function invocation.");
            }
        }
        break;
}

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
