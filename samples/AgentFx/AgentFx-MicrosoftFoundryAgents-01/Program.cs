using Azure.AI.Agents.Persistent;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var azureFoundryProjectEndpoint = config["azureFoundryProjectEndpoint"];
var deploymentName = config["deploymentName"];
var agentName = config["agentName"];

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