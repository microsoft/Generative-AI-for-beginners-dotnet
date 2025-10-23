using Azure;
using Azure.AI.Inference;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Client;
using System.ClientModel;

// To run the sample, you need to set the following environment variables or user secrets:
// Using GitHub models
//      "HF_API_KEY": " your HF token"
//      "GITHUB_TOKEN": " your GitHub Token "
// Using Azure OpenAI models
//      "endpoint": "https://<endpoint>.services.ai.azure.com/",
//      "apikey": " your key ",
//      "deploymentName": "a deployment name, ie: gpt-4.1-mini"

var builder = Host.CreateApplicationBuilder(args);
var config = builder.Configuration
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();
var deploymentName = config["deploymentName"] ?? "gpt-4.1-mini"; // Default to gpt-4.1-mini if not specified 

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
IChatClient client = GetChatClient();
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

IChatClient GetChatClient()
{
    IChatClient client = null;

    var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
    if (string.IsNullOrEmpty(githubToken))
    {
        githubToken = config["GITHUB_TOKEN"];
    }

    // create a client if githubToken is valid string
    if (!string.IsNullOrEmpty(githubToken))
    {
        client = new ChatCompletionsClient(
            endpoint: new Uri("https://models.github.ai/inference"),
            new AzureKeyCredential(githubToken))
            .AsIChatClient(deploymentName)
            .AsBuilder()
            .UseFunctionInvocation()
            .Build(); ;
    }
    else
    {
        // create an Azure OpenAI client if githubToken is not valid
        var endpoint = config["endpoint"];
        var apiKey = new ApiKeyCredential(config["apikey"]);

        client = new AzureOpenAIClient(new Uri(endpoint), apiKey)
            .GetChatClient(deploymentName)
            .AsIChatClient()
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();
    }
    return client;
}
