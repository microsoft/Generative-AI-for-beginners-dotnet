using OpenTelemetry;
using OpenTelemetry.Trace;
using AgentFx_ImageGen_01;
using AgentFx_ImageGen_02;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

var tools = new[] { AIFunctionFactory.Create(ImageGenerator.GenerateImageFromPrompt) };

// get chat client
IChatClient chatClient = ChatClientProvider.GetChatClient();

// Create a TracerProvider that exports to the console
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("agent-telemetry-source")
    .AddConsoleExporter()
    .Build();

var imageGenerator = chatClient.CreateAIAgent(
    name: "Image Generator",
    instructions: "You are an agent that is specialized on image generation. If the user ask to create an image, the image should always be pixelated with big pixels.",
    description: "An AI agent that generate images using Microsoft Foundry models.",
    tools: [.. tools])
    .AsBuilder()
    .UseOpenTelemetry(sourceName: "agent-telemetry-source")
    .Build();

// test agent
var message = "create an image of a racoon in Canada";

AgentRunResponse response = await imageGenerator.RunAsync(
    message: message);

Console.WriteLine(response.Text);
