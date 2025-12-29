using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
if (string.IsNullOrEmpty(githubToken))
{
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    githubToken = config["GITHUB_TOKEN"];
}

IChatClient chatClient =
    new ChatClient(
            "gpt-4o-mini",
            new ApiKeyCredential(githubToken!),
            new OpenAIClientOptions { Endpoint = new Uri("https://models.github.ai/inference") })
        .AsIChatClient();

AIAgent writer = chatClient.CreateAIAgent(
    name: "Writer",
    instructions: "Write stories that are engaging and creative.");

AgentRunResponse response = await writer.RunAsync("Write a short story about a haunted house with a character named Lucia.");

Console.WriteLine(response.Text);