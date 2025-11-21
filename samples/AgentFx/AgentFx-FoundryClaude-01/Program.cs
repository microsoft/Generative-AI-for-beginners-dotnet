using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ClientModel;
using System.ClientModel.Primitives;

// AgentFx Basic Chat with Claude via Azure AI Foundry
// Demonstrates using ChatClientAgent with Claude models deployed in Azure AI Foundry
// Uses ClaudeToOpenAIMessageHandler to bridge OpenAI and Claude API formats

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["endpoint"] ?? throw new InvalidOperationException("Missing 'endpoint' configuration");
var endpointClaude = config["endpointClaude"] ?? throw new InvalidOperationException("Missing 'endpointClaude' configuration");
var apiKey = config["apikey"] ?? throw new InvalidOperationException("Missing 'apikey' configuration");
var deploymentName = config["deploymentName"] ?? "claude-haiku-4-5";

Console.WriteLine("=".PadRight(60, '='));
Console.WriteLine("AgentFx with Claude via Azure AI Foundry");
Console.WriteLine("=".PadRight(60, '='));
Console.WriteLine($"Model: {deploymentName}");
Console.WriteLine($"Endpoint: {endpoint}");
Console.WriteLine("=".PadRight(60, '='));
Console.WriteLine();

// Create custom HTTP message handler for Claude endpoint in Azure
var customHttpMessageHandler = new ClaudeToOpenAIMessageHandler
{
    AzureClaudeDeploymentUrl = endpointClaude,
    ApiKey = apiKey,
    Model = deploymentName
};
HttpClient customHttpClient = new(customHttpMessageHandler);

// Wrap HttpClient in the pipeline transport
var transport = new HttpClientPipelineTransport(customHttpClient);

// Client options with custom transport
var clientOptions = new AzureOpenAIClientOptions
{
    Transport = transport
};

// Create IChatClient with the custom transport
IChatClient chatClient = new AzureOpenAIClient(
    endpoint: new Uri(endpoint),
    credential: new ApiKeyCredential(apiKey),
    options: clientOptions)
    .GetChatClient(deploymentName)
    .AsIChatClient()
    .AsBuilder()
    .Build();

// Create AI Agent with ChatClientAgent
AIAgent writer = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "Writer",
        Instructions = "You are a creative writer who crafts engaging and imaginative stories. Keep responses concise but vivid."
    });

// Run the agent with a prompt
Console.WriteLine("Prompt: Write a short story about a haunted house with a character named Lucia.");
Console.WriteLine();
Console.WriteLine("Response:");
Console.WriteLine("-".PadRight(60, '-'));

AgentRunResponse response = await writer.RunAsync("Write a short story about a haunted house with a character named Lucia.");

Console.WriteLine(response.Text);
Console.WriteLine("-".PadRight(60, '-'));
Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
