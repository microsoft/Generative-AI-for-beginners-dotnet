using MAF_BackgroundResponses_01_Simple;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenTelemetry;
using OpenTelemetry.Trace;
using System.ComponentModel;

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

        StreamConsoleHelper.PrintHeader("Background Responses (tools) — streaming with interruption and continuation");
        StreamConsoleHelper.PrintLabeled("Question", question);

        // STREAMING PHASE
        StreamConsoleHelper.PrintSection("Start streaming (background responses enabled)");

        await foreach (var update in agent.RunStreamingAsync(question, thread, options))
        {
            // print raw streaming chunks (helpful for debugging)
            var tokenString = update.ContinuationToken?.ToString();
            StreamConsoleHelper.PrintUpdate(update.Text, tokenString);

            latestReceivedUpdate = update;

            // Simulate interruption when we receive some text
            if (!string.IsNullOrWhiteSpace(update.Text))
            {
                StreamConsoleHelper.PrintInfo(">> interruption simulated — capturing continuation token and stopping streaming");
                break;
            }
        }

        StreamConsoleHelper.PrintLabeled("After interruption", latestReceivedUpdate?.Text ?? "<no content received>");
        Console.WriteLine();

        // CONTINUATION PHASE
        StreamConsoleHelper.PrintSection("Start continuation (resume using continuation token)");

        if (latestReceivedUpdate?.ContinuationToken is null)
        {
            StreamConsoleHelper.PrintError("No continuation token available to resume the response. Aborting continuation.");
        }
        else
        {
            options.ContinuationToken = latestReceivedUpdate!.ContinuationToken;

            // Prepare accumulator to join token fragments into readable sentences
            StreamConsoleHelper.StartAccumulatedStream();

            await foreach (var update in agent.RunStreamingAsync(thread, options))
            {
                var tokenString = update.ContinuationToken?.ToString();
                StreamConsoleHelper.AccumulateAndPrint(update.Text, tokenString);
            }

            StreamConsoleHelper.FlushAccumulated();
        }

        Console.WriteLine();
        StreamConsoleHelper.PrintFooter("End of demo — press any key to exit");
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