// This sample shows the value of grounding a model with the public Microsoft Learn
// MCP Server (https://learn.microsoft.com/api/mcp). It asks the SAME question twice:
//
//   1. WITHOUT MCP  -> the model answers only from its (frozen) training knowledge,
//                      so for fast-moving topics the answer is often stale or vague.
//   2. WITH MCP     -> the model calls the Microsoft Learn documentation tools
//                      (microsoft_docs_search, microsoft_docs_fetch,
//                       microsoft_code_sample_search) and answers from the live docs,
//                      including an up-to-date version and a docs link.
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

// The question is intentionally about a fast-moving topic so the difference between
// "model knowledge only" and "grounded in Microsoft Learn" is easy to see live.
const string question =
    "What is the latest version of Microsoft Agent Framework for C#? " +
    "Answer with the version number and a Microsoft Learn docs link.";

// A short system instruction so the model commits to an answer (instead of asking
// clarifying questions) and, when tools are available, grounds itself in the docs.
const string systemPrompt =
    "You are a .NET documentation assistant. Answer directly and concisely. " +
    "Do not ask clarifying questions. 'Microsoft Agent Framework for C#' refers to the " +
    "Microsoft.Agents.AI NuGet packages. If documentation tools are available, you MUST " +
    "use them to find the current version and cite a Microsoft Learn link.";

// Create an IChatClient and enable automatic function (tool) invocation.
IChatClient client = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient()
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

// =====================================================================
// 1. BEFORE: ask WITHOUT MCP. The model can only use its training data.
// =====================================================================
Console.WriteLine("============================================================");
Console.WriteLine(" BEFORE  -  no MCP  (answer from model knowledge only)");
Console.WriteLine("============================================================");
Console.WriteLine($"Question: {question}");
Console.WriteLine();

var beforeMessages = new List<ChatMessage>
{
    new(ChatRole.System, systemPrompt),
    new(ChatRole.User, question)
};

// Stream the answer token-by-token so the response appears live in the console.
await foreach (var update in client.GetStreamingResponseAsync(
    beforeMessages,
    new ChatOptions { ModelId = deploymentName }))
{
    Console.Write(update.Text);
}
Console.WriteLine();
Console.WriteLine();

// =====================================================================
// 2. AFTER: ask WITH the Microsoft Learn MCP Server tools attached.
// =====================================================================

// Connect to the public Microsoft Learn MCP Server (no auth needed).
var clientTransport = new HttpClientTransport(new HttpClientTransportOptions
{
    Name = "Microsoft Learn MCP",
    Endpoint = new Uri("https://learn.microsoft.com/api/mcp")
});
await using var mcpClient = await McpClient.CreateAsync(clientTransport);

// Discover the tools the server exposes.
var tools = await mcpClient.ListToolsAsync();

Console.WriteLine("============================================================");
Console.WriteLine(" AFTER  -  with Microsoft Learn MCP  (grounded in live docs)");
Console.WriteLine("============================================================");
Console.WriteLine("Connected to the Microsoft Learn MCP Server. Available tools:");
foreach (var tool in tools)
{
    Console.WriteLine($"  - {tool.Name}: {tool.Description}");
}
Console.WriteLine();
Console.WriteLine($"Question: {question}");
Console.WriteLine();
Console.WriteLine("Asking the model (it will call the MCP tools as needed)...");
Console.WriteLine();

var afterMessages = new List<ChatMessage>
{
    new(ChatRole.System, systemPrompt),
    new(ChatRole.User, question)
};

// Stream the grounded answer. Tool calls happen automatically behind the scenes; the
// final text streams in live once the model has gathered what it needs from the docs.
await foreach (var update in client.GetStreamingResponseAsync(
    afterMessages,
    new ChatOptions { Tools = [.. tools], ModelId = deploymentName }))
{
    Console.Write(update.Text);
}
Console.WriteLine();
