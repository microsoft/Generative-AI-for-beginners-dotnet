using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["endpoint"];
var apiKey = new ApiKeyCredential(config["apikey"]);
var deploymentName = !string.IsNullOrEmpty(config["deploymentName"]) ? config["deploymentName"] : "gpt-4o-mini";

AIAgent agent = new AzureOpenAIClient(
    new Uri(endpoint),
    new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient()
    .CreateAIAgent(instructions: "You are a useful agent that replies in short and direct sentences.");

while (true)
{
    Console.Write("User: ");
    var userInput = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userInput))
    {
        break;
    }
    var response = await agent.RunAsync(userInput);
    Console.WriteLine($"Agent: {response.Text}");
}