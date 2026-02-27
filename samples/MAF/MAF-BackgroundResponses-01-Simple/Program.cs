using MAF_BackgroundResponses_01_Simple;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System.Text;

// Simple demo that shows how to use Agent Framework's Background Responses feature.
// Background responses allow the agent to continue generating responses in the background
// after you receive partial streaming updates. You can interrupt, capture the continuation
// token, and later resume the generation from that token to get the rest of the response.
// See: https://learn.microsoft.com/en-us/agent-framework/user-guide/agents/agent-background-responses

await BackgroundResponsesDemo.RunAsync();

internal static class BackgroundResponsesDemo
{
    public static async Task RunAsync()
    {
        // get chat response client
        var responseClient = ResponseClientProvider.GetResponseClient();

        // create a simple agent with basic instructions
        var agent = responseClient.CreateAIAgent(
            name: "agent",
            instructions: "You are a helpful assistant");

        // Enable background responses so the agent can keep generating after an interruption.
        var options = new AgentRunOptions
        {
            AllowBackgroundResponses = true
        };

        //1) Create a thread for the conversation
        var thread = agent.GetNewThread();

        //2) The question we'll ask
        var question = "Write a 2 lines story about .NET Conf";

        StreamConsoleHelper.PrintHeader("Background Responses — streaming with interruption and continuation");
        StreamConsoleHelper.PrintLabeled("Question", question);

        // Streaming phase: start streaming and simulate an interruption when we receive some text
        StreamConsoleHelper.PrintSection("Start streaming (background responses enabled)");

        AgentRunResponseUpdate? latestReceivedUpdate = null;

        await foreach (var update in agent.RunStreamingAsync(question, thread, options))
        {
            // Convert continuation token to string for display and storage
            var tokenString = update.ContinuationToken?.ToString();

            // Print each streaming update in a clean, easy-to-read format
            StreamConsoleHelper.PrintUpdate(update.Text, tokenString);

            latestReceivedUpdate = update;

            // Simulate an interruption if we have received any non-empty text
            if (!string.IsNullOrWhiteSpace(update.Text))
            {
                StreamConsoleHelper.PrintInfo(">> interruption simulated — capturing continuation token and stopping streaming");
                break;
            }
        }

        StreamConsoleHelper.PrintLabeled("After interruption", latestReceivedUpdate?.Text ?? "<no content received>");
        Console.WriteLine();

        // Continuation phase: resume from the captured continuation token to receive the rest
        StreamConsoleHelper.PrintSection("Start continuation (resume using continuation token)");

        if (latestReceivedUpdate?.ContinuationToken is null)
        {
            StreamConsoleHelper.PrintError("No continuation token available to resume the response. Aborting continuation.");
        }
        else
        {
            options.ContinuationToken = latestReceivedUpdate!.ContinuationToken;

            // Prepare accumulator in StreamConsoleHelper to join token fragments into sentences
            StreamConsoleHelper.StartAccumulatedStream();

            // When resuming we pass the thread and options; the agent will continue from the token
            await foreach (var update in agent.RunStreamingAsync(thread, options))
            {
                var tokenString = update.ContinuationToken?.ToString();

                // Accumulate tokens and print flushed sentences for more readable output
                StreamConsoleHelper.AccumulateAndPrint(update.Text, tokenString);
            }

            // Flush any remaining buffered fragments so the last partial sentence is printed
            StreamConsoleHelper.FlushAccumulated();
        }

        Console.WriteLine();
        StreamConsoleHelper.PrintFooter("End of demo — press any key to exit");
        Console.ReadKey();
    }
}
