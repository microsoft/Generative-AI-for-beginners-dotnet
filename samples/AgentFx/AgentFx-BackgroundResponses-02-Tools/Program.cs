using AgentFx_04;
using AgentFx_BackgroundResponses_01_Simple;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenTelemetry;
using OpenTelemetry.Trace;
using System.ComponentModel;

// get chat response client
var responseClient = ResponseClientProvider.GetResponseClient();

// Create a TracerProvider that exports to the console
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("agent-telemetry-source")
    .AddConsoleExporter()
    .Build();

var agent = responseClient.CreateAIAgent(
    name: "agent",
    instructions: @"You are a helpful assistant",
    tools: [AIFunctionFactory.Create(GetWeatherAsync)]
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

var question = "How is the the weather in Toronto";
//var question = "Write a long story about .NET Conf";
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

[Description("Get the weather for a given location.")]
static async Task<string> GetWeatherAsync(
    [Description("The location to get the weather for.")] string location)
{
    Console.WriteLine($"  >> Function Start");
    Console.WriteLine($"  >> Get weather for >> {location}");
    var response = $"The weather in {location} is cloudy with a high of 15°C.";
    Console.WriteLine($"  >> {response}");
    Console.WriteLine($"  >> Function End");
    return response;
}