using Azure.AI.Agents.Persistent;
using Azure.AI.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;

#pragma warning disable CA2252 // Opt-in for preview features used by AIProjectClient in samples

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var azureFoundryProjectEndpoint = config["azureFoundryProjectEndpoint"] ?? throw new InvalidOperationException("Missing 'azureFoundryProjectEndpoint' configuration");
var deploymentName = config["deploymentName"] ?? throw new InvalidOperationException("Missing 'deploymentName' configuration");
var agentName = config["agentName"] ?? throw new InvalidOperationException("Missing 'agentName' configuration");

AIProjectClient projectClient = new(
    endpoint: new Uri(azureFoundryProjectEndpoint),
    tokenProvider: new AzureCliCredential());

// create agent
//AIAgent aiAgent = projectClient.CreateAIAgent(
//    model: deploymentName,
//    name: "Agent",
//    instructions: "You are a useful agent that replies in short and direct sentences.");

// get existing agent
AIAgent aiAgent = await projectClient.GetAIAgentAsync(agentName);

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