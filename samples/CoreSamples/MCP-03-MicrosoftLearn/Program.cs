// This sample shows how to use the C# MCP SDK to connect to the public
// Microsoft Learn MCP Server (https://learn.microsoft.com/api/mcp) and let an
// Azure OpenAI model call its documentation tools (microsoft_docs_search,
// microsoft_docs_fetch, microsoft_code_sample_search) via MEAI function invocation.
//
// The Microsoft Learn MCP Server is a public, keyless, streamable-HTTP MCP server,
// so no API key or Authorization header is required to connect to it.
//
// To run the sample, set the following user secrets (Azure OpenAI, keyless via Azure CLI):
//      dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-endpoint>.openai.azure.com/"
//      dotnet user-secrets set "AzureOpenAI:Deployment" "gpt-5-mini"
// Then sign in with: az login

using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Client;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["AzureOpenAI:Endpoint"]
    ?? throw new InvalidOperationException("Set AzureOpenAI:Endpoint in User Secrets.");
var deploymentName = config["AzureOpenAI:Deployment"] ?? "gpt-5-mini";

// 1. Connect to the public Microsoft Learn MCP Server (no auth needed).
var clientTransport = new HttpClientTransport(new HttpClientTransportOptions
{
    Name = "Microsoft Learn MCP",
    Endpoint = new Uri("https://learn.microsoft.com/api/mcp")
});
await using var mcpClient = await McpClient.CreateAsync(clientTransport);

// 2. Discover the tools the server exposes.
var tools = await mcpClient.ListToolsAsync();
Console.WriteLine("Connected to the Microsoft Learn MCP Server. Available tools:");
foreach (var tool in tools)
{
    Console.WriteLine($"  - {tool.Name}: {tool.Description}");
}
Console.WriteLine();

// 3. Create an IChatClient and enable automatic function (tool) invocation.
IChatClient client = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient()
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

var chatOptions = new ChatOptions
{
    Tools = [.. tools],
    ModelId = deploymentName
};

// 4. Ask a question that the model can only answer well by calling the Learn tools.
var question = "Using the Microsoft Learn docs, what is Microsoft.Extensions.AI " +
               "and which interface do I use to chat with a model? Include a docs link.";
Console.WriteLine($"Question: {question}");
Console.WriteLine();
Console.WriteLine("Asking the model (it will call the MCP tools as needed)...");
Console.WriteLine();

var response = await client.GetResponseAsync(question, chatOptions);
Console.WriteLine(response.Text);
