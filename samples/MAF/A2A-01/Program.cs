// A2A-01 — a minimal end-to-end Agent-to-Agent (A2A) sample in one console run.
//
// This single process does BOTH sides of the A2A protocol:
//   1. SERVER: hosts a "writer-agent" (Azure OpenAI) and exposes it over the
//      Agent-to-Agent (A2A) HTTP+JSON protocol binding.
//   2. CLIENT: connects to that same endpoint with an A2AClient, wraps it as a
//      standard AIAgent, and calls RunAsync — exactly how a remote app (even one
//      written in another language/framework) would talk to the agent.
//
// This is the "HTTP of agent communication": the client never references the
// agent's implementation, only its A2A endpoint.
//
// To run the sample, set the following user secrets (Azure OpenAI, keyless via Azure CLI):
//      dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-endpoint>.openai.azure.com/"
//      dotnet user-secrets set "AzureOpenAI:Deployment" "gpt-5-mini"
// Then sign in with: az login

using A2A;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

const string agentName = "writer-agent";
const string baseUrl = "http://localhost:5099";
const string a2aPath = "/a2a/writer-agent";

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls(baseUrl);
builder.Logging.ClearProviders(); // keep the console output focused on the demo

var endpoint = builder.Configuration["AzureOpenAI:Endpoint"]
    ?? throw new InvalidOperationException("Set AzureOpenAI:Endpoint in User Secrets.");
var deployment = builder.Configuration["AzureOpenAI:Deployment"] ?? "gpt-5-mini";

// 1. SERVER — build the agent and expose it over the A2A protocol.
IChatClient chatClient = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deployment)
    .AsIChatClient();

AIAgent writerAgent = chatClient.AsAIAgent(
    name: agentName,
    instructions: "You are a creative writer. Keep answers short and vivid.");

var app = builder.Build();
app.MapA2A(writerAgent, a2aPath);

await app.StartAsync();
Console.WriteLine($"A2A server is hosting '{agentName}' at {baseUrl}{a2aPath}");
Console.WriteLine();

// 2. CLIENT — talk to the agent purely over the A2A protocol.
var a2aClient = new A2AClient(new Uri(baseUrl + a2aPath));
AIAgent remoteAgent = a2aClient.AsAIAgent(
    name: "remote-writer",
    description: "The writer agent, reached over A2A.");

const string prompt = "Write a two-line poem about .NET and AI.";
Console.WriteLine($"Client -> A2A: {prompt}");
Console.WriteLine();

AgentResponse response = await remoteAgent.RunAsync(prompt);
Console.WriteLine("A2A -> Client:");
Console.WriteLine(response.Text);

await app.StopAsync();
