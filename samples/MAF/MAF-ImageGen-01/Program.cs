using OpenTelemetry;
using OpenTelemetry.Trace;
using MAF_ImageGen_01;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

// get mcp tools
var (mcpClient, tools) = await HuggingFaceMCP.GetHuggingFaceMCPClientAndToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine($"Connected to server with tools: {tool.Name}");
}
Console.WriteLine($"===");

// get chat client
IChatClient chatClient = ChatClientProvider.GetChatClient();

// Create a TracerProvider that exports to the console
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("agent-telemetry-source")
    .AddConsoleExporter()
    .Build();

// create image generator agent
var imageGenerator = chatClient.CreateAIAgent(
    name: "Image Generator",
    instructions: "You are an image generator agent that uses the tools from the Hugging Face MCP Server. If the user ask to create an image, use the Flux1 tool [gr1_flux1_schnell_infer]. If the user ask to generate an image, the image should always be pixelated.",
    description: "An AI agent that uses the Hugging Face MCP tools to generate images.",
    tools: [.. tools])
    .AsBuilder()
    .UseOpenTelemetry(sourceName: "agent-telemetry-source")
    .Build();

// test agent
//var message = "tell me a joke about kittens";
//var message = "create an image of a racoon in Canada";
var message = "What is the name of the user logged in to the Hugging Face MCP Server";

AgentRunResponse response = await imageGenerator.RunAsync(
    message: message);

Console.WriteLine(response.Text);

await mcpClient.DisposeAsync();