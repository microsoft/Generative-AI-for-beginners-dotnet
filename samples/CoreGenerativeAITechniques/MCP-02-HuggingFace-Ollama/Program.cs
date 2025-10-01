#pragma warning disable SKEXP0001, SKEXP0070  

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel.ChatCompletion;
using ModelContextProtocol.Client;
using OllamaSharp;

// To run the sample, you need to set the following environment variables or user secrets:
// "HF_API_KEY": " your HF token"
// "deploymentName" : "llama3.2" // Optional, defaults to "llama3.2" if not specified

var builder = Host.CreateApplicationBuilder(args);
var config = builder.Configuration
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();
var deploymentName = config["deploymentName"] ?? "llama3.2"; // Default to llama3.2 if not specified

// create MCP Client using Hugging Face endpoint
var hfHeaders = new Dictionary<string, string>
{
    { "Authorization", $"Bearer {config["HF_API_KEY"]}" }
};
var clientTransport = new SseClientTransport(
    new()
    {
        Name = "HF Server",
        Endpoint = new Uri("https://huggingface.co/mcp"),
        AdditionalHeaders = hfHeaders
    });
await using var mcpClient = await McpClientFactory.CreateAsync(clientTransport);

// Display the available server tools
var tools = await mcpClient.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine($"Connected to server with tools: {tool.Name}");
}
Console.WriteLine("Press Enter to continue...");
Console.ReadLine();
Console.WriteLine();

// create an IChatClient using the MCP tools
var uri = new Uri("http://localhost:11434");
var client = new OllamaApiClient(uri, deploymentName)
    .AsChatCompletionService()
    .AsChatClient()
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

var chatOptions = new ChatOptions
{
    Tools = [.. tools],
    ModelId = deploymentName
};

// Create image
Console.WriteLine("Starting the process to generate an image of a pixelated puppy...");
var query = "Create an image of a pixelated puppy.";
var result = await client.GetResponseAsync(query, chatOptions);
Console.Write($"AI response: {result}");
Console.WriteLine();

