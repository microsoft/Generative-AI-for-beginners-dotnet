using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["AzureOpenAI:Endpoint"];
var deploymentName = config["AzureOpenAI:Deployment"] ?? "gpt-5-mini";

IChatClient chatClient =
    new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
        .GetChatClient(deploymentName)
        .AsIChatClient();

AIAgent writer = chatClient.AsAIAgent(
    name: "Writer",
    instructions: "Write stories that are engaging and creative.");

AgentResponse response = await writer.RunAsync("Write a short story about a haunted house with a character named Lucia.");

Console.WriteLine(response.Text);