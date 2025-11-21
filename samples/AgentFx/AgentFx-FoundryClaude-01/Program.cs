using elbruno.Extensions.AI.Claude;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

// AgentFx Basic Chat with Claude via Microsoft Foundry
// Demonstrates using ChatClientAgent with Claude models deployed in Microsoft Foundry
// Uses elbruno.Extensions.AI.Claude package for seamless Claude integration

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpointClaude = config["endpointClaude"] ?? throw new InvalidOperationException("Missing 'endpointClaude' configuration");
var apiKey = config["apikey"] ?? throw new InvalidOperationException("Missing 'apikey' configuration");
var deploymentName = config["deploymentName"] ?? "claude-haiku-4-5";

Console.WriteLine("=".PadRight(60, '='));
Console.WriteLine("AgentFx with Claude via Microsoft Foundry");
Console.WriteLine("=".PadRight(60, '='));
Console.WriteLine($"Model: {deploymentName}");
Console.WriteLine($"Endpoint: {endpointClaude}");
Console.WriteLine("=".PadRight(60, '='));
Console.WriteLine();

// Create IChatClient using elbruno.Extensions.AI.Claude package
IChatClient chatClient = new AzureClaudeClient(
    endpoint: new Uri(endpointClaude),
    modelId: deploymentName,
    apiKey: apiKey);

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
