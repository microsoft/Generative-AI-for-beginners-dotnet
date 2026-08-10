using elbruno.Extensions.AI.Claude;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

// MAF Basic Chat with Claude via Microsoft Foundry
// Demonstrates using ChatClientAgent with Claude models deployed in Microsoft Foundry
// Uses elbruno.Extensions.AI.Claude package for seamless Claude integration

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

// All samples in this repo share the user-secrets store "genai-beginners-dotnet", so this sample
// reads its own scoped "Claude:*" keys first and only then falls back to the old generic key names.
var endpointClaude = config["Claude:Endpoint"] ?? config["endpointClaude"];
var apiKey = config["Claude:ApiKey"] ?? config["apikey"];
var deploymentName = config["Claude:Deployment"] ?? config["deploymentName"] ?? "claude-haiku-4-5";

var missing = new List<string>();
if (string.IsNullOrWhiteSpace(endpointClaude)) missing.Add("Claude:Endpoint");
if (string.IsNullOrWhiteSpace(apiKey)) missing.Add("Claude:ApiKey");

if (missing.Count > 0)
{
    Console.WriteLine($"""
        Claude via Microsoft Foundry is not configured. Missing: {string.Join(", ", missing)}

        Prerequisite: a Claude model deployment in Azure AI Foundry (for example "claude-haiku-4-5").
        Create it in the Azure AI Foundry portal under Deployments > Deploy model, then copy the
        project/resource endpoint and one of its keys. Without that deployment these keys cannot be
        filled in.

        Then set the missing values (all samples share the user-secrets id "genai-beginners-dotnet"):

        dotnet user-secrets set --id genai-beginners-dotnet "Claude:Endpoint" "https://<your-resource>.services.ai.azure.com"
        dotnet user-secrets set --id genai-beginners-dotnet "Claude:ApiKey" "<your-api-key>"
        dotnet user-secrets set --id genai-beginners-dotnet "Claude:Deployment" "claude-haiku-4-5"

        Claude:Deployment is optional and defaults to "claude-haiku-4-5".
        The legacy keys "endpointClaude", "apikey" and "deploymentName" still work, but "apikey" is
        shared with other samples in this store, so the scoped "Claude:*" keys are recommended.
        """);
    return 1;
}

Console.WriteLine("=".PadRight(60, '='));
Console.WriteLine("MAF with Claude via Microsoft Foundry");
Console.WriteLine("=".PadRight(60, '='));
Console.WriteLine($"Model: {deploymentName}");
Console.WriteLine($"Endpoint: {endpointClaude}");
Console.WriteLine("=".PadRight(60, '='));
Console.WriteLine();

// Create IChatClient using elbruno.Extensions.AI.Claude package
IChatClient chatClient = new AzureClaudeClient(
    endpoint: new Uri(endpointClaude!),
    modelId: deploymentName,
    apiKey: apiKey!);

// Create AI Agent with ChatClientAgent
AIAgent writer = chatClient.AsAIAgent(
    name: "Writer",
    instructions: "You are a creative writer who crafts engaging and imaginative stories. Keep responses concise but vivid.");

// Run the agent with a prompt
Console.WriteLine("Prompt: Write a short story about a haunted house with a character named Lucia.");
Console.WriteLine();
Console.WriteLine("Response:");
Console.WriteLine("-".PadRight(60, '-'));

AgentResponse response = await writer.RunAsync("Write a short story about a haunted house with a character named Lucia.");

Console.WriteLine(response.Text);
Console.WriteLine("-".PadRight(60, '-'));
Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
return 0;
