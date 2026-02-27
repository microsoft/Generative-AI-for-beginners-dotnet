using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var azureFoundryProjectEndpoint = config["azureFoundryProjectEndpoint"];
var deploymentName = config["deploymentName"];
var assistantId = config["assistantId"];

var persistentAgentClient = new PersistentAgentsClient(
    azureFoundryProjectEndpoint!,
    new AzureCliCredential());

// create agent
//AIAgent aiAgent = persistentAgentClient.CreateAIAgent(
//    model: deploymentName,
//    name: "Agent",
//    instructions: "You are a useful agent that replies in short and direct sentences.");

// get existing agent
AIAgent aiAgent = await persistentAgentClient.GetAIAgentAsync(assistantId);

while (true)
{
    Console.Write("User: ");
    var userInput = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userInput))
    {
        break;
    }
    var response = await aiAgent.RunAsync(userInput);
    Console.WriteLine($"Agent: {response.Text}");
}