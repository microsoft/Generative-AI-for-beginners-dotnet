using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Foundry;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var azureFoundryProjectEndpoint = config["azureFoundryProjectEndpoint"] ?? throw new InvalidOperationException("Missing 'azureFoundryProjectEndpoint' configuration");
var deploymentName = config["AzureOpenAI:Deployment"] ?? throw new InvalidOperationException("Missing 'AzureOpenAI:Deployment' configuration");
var agentName = config["agentName"] ?? throw new InvalidOperationException("Missing 'agentName' configuration");

AIProjectClient projectClient = new(
    new Uri(azureFoundryProjectEndpoint),
    new AzureCliCredential());

// create agent using Responses API
AIAgent aiAgent = projectClient.AsAIAgent(
    model: deploymentName,
    instructions: "You are a useful agent that replies in short and direct sentences.",
    name: agentName);

while (true)
{
    Console.Write("User: ");
    var userInput = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userInput))
    {
        break;
    }
    var response = await aiAgent.RunAsync(userInput);
    Console.WriteLine($"Agent [{agentName}]: {response.Text}");
}