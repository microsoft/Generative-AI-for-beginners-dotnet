// MAF-MCP-01 — a Microsoft Agent Framework agent that uses MCP tools.
//
// This sample wires a Microsoft Agent Framework (MAF) AIAgent to the public
// Microsoft Learn MCP Server (https://learn.microsoft.com/api/mcp). The agent
// receives the Learn documentation tools (microsoft_docs_search,
// microsoft_docs_fetch, microsoft_code_sample_search) and calls them on its own
// to ground its answers in the live Microsoft Learn docs.
//
// The Microsoft Learn MCP Server is public and keyless, so no API key or token is
// required to connect to it.
//
// To run the sample, set the following user secrets (Azure OpenAI, keyless via Azure CLI):
//      dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-endpoint>.openai.azure.com/"
//      dotnet user-secrets set "AzureOpenAI:Deployment" "gpt-5-mini"
// Then sign in with: az login

using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
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

// 3. Create the chat client and build a MAF agent that owns the MCP tools.
//    The agent invokes the tools automatically while answering.
IChatClient chatClient = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient();

AIAgent docsAgent = chatClient.AsAIAgent(
    name: "LearnDocsAgent",
    instructions: "You are a .NET documentation assistant. " +
                  "Use the Microsoft Learn tools to answer questions about .NET and AI. " +
                  "Always ground your answer in the docs and include a Microsoft Learn link.",
    tools: [.. tools]);

// 4. Ask the agent a question — it decides when to call the Learn MCP tools.
const string question =
    "What is the latest version of Microsoft Agent Framework for C#? " +
    "Answer with the version number and a Microsoft Learn docs link.";

Console.WriteLine($"Question: {question}");
Console.WriteLine();
Console.WriteLine("Agent is thinking (it will call the MCP tools as needed)...");
Console.WriteLine();

// Stream the grounded answer so it appears token-by-token in the console. Tool calls
// happen automatically behind the scenes; the final text streams in live.
await foreach (var update in docsAgent.RunStreamingAsync(question))
{
    Console.Write(update.Text);
}
Console.WriteLine();
