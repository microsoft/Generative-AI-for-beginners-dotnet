using AgentFx_BackgroundResponses_01_Simple;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;

// get chat response client
var responseClient = ResponseClientProvider.GetResponseClient();

var agent = responseClient.CreateAIAgent(
    name: "agent",
    instructions: @"You are a helpful assistant");

AgentRunOptions options = new()
{
    AllowBackgroundResponses = true
};

AgentThread thread = agent.GetNewThread();
AgentRunResponseUpdate? latestReceivedUpdate = null;

var question = "Write a 2 paragraphs story about .NET Conf";

Console.WriteLine("====================");
Console.WriteLine("start streaming");
Console.WriteLine("");
await foreach (var update in agent.RunStreamingAsync(question, thread, options))
{
    Console.WriteLine("streaming update: " + update.Text);

    latestReceivedUpdate = update;

    // Simulate an interruption if the updateText have some text on it
    if (!string.IsNullOrWhiteSpace(update.Text))
    {
        Console.WriteLine(" >> interruption simulated");
        break;
    }

}

Console.WriteLine("after interruption: " + latestReceivedUpdate?.Text);
Console.WriteLine("====================");
Console.WriteLine("");

Console.WriteLine("====================");
Console.WriteLine("start continuation");
Console.WriteLine("");

// Resume from interruption point captured by the continuation token
options.ContinuationToken = latestReceivedUpdate?.ContinuationToken;
await foreach (var update in agent.RunStreamingAsync(thread, options))
{
    Console.Write(update.Text);
}
Console.WriteLine("");
Console.WriteLine("====================");
Console.WriteLine("end");
Console.ReadKey();