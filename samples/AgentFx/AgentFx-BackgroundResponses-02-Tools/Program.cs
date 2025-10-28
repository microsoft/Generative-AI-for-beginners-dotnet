using AgentFx_BackgroundResponses_01_Simple;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenTelemetry;
using OpenTelemetry.Trace;
using System.ComponentModel;
using System.Text;

await BackgroundResponsesWithToolsDemo.RunAsync();

internal static class BackgroundResponsesWithToolsDemo
{
    public static async Task RunAsync()
    {
        // get chat response client
        var responseClient = ResponseClientProvider.GetResponseClient();

        // Create a TracerProvider that exports to the console
        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource("agent-telemetry-source")
            .AddConsoleExporter()
            .Build();

        // Create agent with tools and OpenTelemetry
        var agent = responseClient.CreateAIAgent(
                name: "agent",
                instructions: "You are a helpful assistant",
                tools: new[] { AIFunctionFactory.Create(Tools.GetWeatherAsync) }
            )
            .AsBuilder()
            .UseOpenTelemetry(sourceName: "agent-telemetry-source")
            .Build();

        AgentRunOptions options = new()
        {
            AllowBackgroundResponses = true
        };

        AgentThread thread = agent.GetNewThread();
        AgentRunResponseUpdate? latestReceivedUpdate = null;

        var question = "How is the weather in Toronto";

        BackgroundConsoleHelper.PrintHeader("Background Responses (tools) — streaming with interruption and continuation");
        StreamConsoleHelper.PrintLabeled("Question", question);

        // STREAMING PHASE
        BackgroundConsoleHelper.PrintSection("Start streaming (background responses enabled)");

        await foreach (var update in agent.RunStreamingAsync(question, thread, options))
        {
            // print raw streaming chunks (helpful for debugging)
            var tokenString = update.ContinuationToken?.ToString();
            BackgroundConsoleHelper.PrintUpdate(update.Text, tokenString);

            latestReceivedUpdate = update;

            // Simulate interruption when we receive some text
            if (!string.IsNullOrWhiteSpace(update.Text))
            {
                BackgroundConsoleHelper.PrintInfo(">> interruption simulated — capturing continuation token and stopping streaming");
                break;
            }
        }

        BackgroundConsoleHelper.PrintLabeled("After interruption", latestReceivedUpdate?.Text ?? "<no content received>");
        Console.WriteLine();

        // CONTINUATION PHASE
        BackgroundConsoleHelper.PrintSection("Start continuation (resume using continuation token)");

        if (latestReceivedUpdate?.ContinuationToken is null)
        {
            BackgroundConsoleHelper.PrintError("No continuation token available to resume the response. Aborting continuation.");
        }
        else
        {
            options.ContinuationToken = latestReceivedUpdate!.ContinuationToken;

            // Prepare accumulator to join token fragments into readable sentences
            BackgroundConsoleHelper.StartAccumulatedStream();

            await foreach (var update in agent.RunStreamingAsync(thread, options))
            {
                var tokenString = update.ContinuationToken?.ToString();
                BackgroundConsoleHelper.AccumulateAndPrint(update.Text, tokenString);
            }

            BackgroundConsoleHelper.FlushAccumulated();
        }

        Console.WriteLine();
        BackgroundConsoleHelper.PrintFooter("End of demo — press any key to exit");
        Console.ReadKey();
    }
}

internal static class Tools
{
    [Description("Get the weather for a given location.")]
    public static async Task<string> GetWeatherAsync(
        [Description("The location to get the weather for.")] string location)
    {
        Console.WriteLine($" >> Function Start");
        Console.WriteLine($" >> Get weather for >> {location}");
        var response = $"The weather in {location} is cloudy with a high of 15°C.";
        Console.WriteLine($" >> {response}");
        Console.WriteLine($" >> Function End");
        await Task.Delay(10);
        return response;
    }
}

/// <summary>
/// Console helper specialized for streaming + continuation printing in this tools sample.
/// Accumulates token fragments during continuation to print human-friendly sentences instead of one token per line.
/// </summary>
internal static class BackgroundConsoleHelper
{
    private static readonly StringBuilder _accum = new();
    private static string? _firstTimestamp;

    public static void PrintHeader(string text)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string('=', 60));
        Console.WriteLine(text);
        Console.WriteLine(new string('=', 60));
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void PrintSection(string title)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"--- {title} ---");
        Console.ResetColor();
    }

    public static void PrintUpdate(string updateText, string? continuationToken)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"[{timestamp}] ");
        Console.ResetColor();

        if (string.IsNullOrEmpty(updateText))
        {
            Console.WriteLine("(empty update)");
        }
        else
        {
            Console.WriteLine(updateText);
        }

        if (!string.IsNullOrWhiteSpace(continuationToken))
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($" token: {Truncate(continuationToken, 60)}");
            Console.ResetColor();
        }
    }

    public static void PrintLabeled(string label, string content)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"{label}:");
        Console.ResetColor();
        Console.WriteLine(content);
    }

    public static void PrintInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void PrintFooter(string text)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine();
        Console.WriteLine(new string('=', 60));
        Console.WriteLine(text);
        Console.WriteLine(new string('=', 60));
        Console.ResetColor();
    }

    // Accumulation helpers for continuation output
    public static void StartAccumulatedStream()
    {
        _accum.Clear();
        _firstTimestamp = null;
    }

    public static void AccumulateAndPrint(string updateText, string? continuationToken)
    {
        if (string.IsNullOrEmpty(updateText)) return;

        if (_accum.Length == 0)
        {
            _firstTimestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        }

        _accum.Append(updateText);

        var trimmed = updateText.TrimEnd();
        bool endsWithSentence = trimmed.EndsWith('.') || trimmed.EndsWith('!') || trimmed.EndsWith('?');
        bool containsNewline = updateText.Contains("\n");
        bool tooLong = _accum.Length > 250;

        if (endsWithSentence || containsNewline || tooLong)
        {
            FlushAccumulatedInternal(continuationToken);
        }
    }

    public static void FlushAccumulated()
    {
        if (_accum.Length > 0)
        {
            FlushAccumulatedInternal(null);
        }
    }

    private static void FlushAccumulatedInternal(string? continuationToken)
    {
        var ts = _firstTimestamp ?? DateTime.Now.ToString("HH:mm:ss.fff");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"[{ts}] ");
        Console.ResetColor();

        var textToPrint = _accum.ToString().Replace("\n", " ").Trim();
        Console.WriteLine(textToPrint);

        if (!string.IsNullOrWhiteSpace(continuationToken))
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($" token: {Truncate(continuationToken, 60)}");
            Console.ResetColor();
        }

        _accum.Clear();
        _firstTimestamp = null;
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
    }
}