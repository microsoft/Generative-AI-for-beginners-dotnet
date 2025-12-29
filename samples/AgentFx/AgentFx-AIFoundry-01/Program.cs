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

IChatClient chatClient =
    new AzureOpenAIClient(
        new Uri(endpoint),
        new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient();


AIAgent writer = chatClient.CreateAIAgent(
    name: "Writer",
    instructions: "Write stories that are engaging and creative.");

AgentRunResponse response = await writer.RunAsync("Write a short story about a haunted house.");

Console.WriteLine(response.Text);