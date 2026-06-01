using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["AzureOpenAI:Endpoint"] ?? throw new InvalidOperationException(
    "Missing 'AzureOpenAI:Endpoint'. Run: dotnet user-secrets set \"AzureOpenAI:Endpoint\" \"https://<your-resource>.openai.azure.com/\"");
var deploymentName = config["AzureOpenAI:Deployment"] ?? "gpt-5-mini";

IChatClient chatClient =
    new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
        .GetChatClient(deploymentName)
        .AsIChatClient();

AIAgent writer = chatClient.AsAIAgent(
    name: "Writer",
    instructions: "Write stories that are engaging and creative.");

// Stream the response so the story appears token-by-token in the console — a livelier
// demo experience than waiting for the full response.
await foreach (var update in writer.RunStreamingAsync(
    "Write a short story about a haunted house with a character named Lucia."))
{
    Console.Write(update.Text);
}
Console.WriteLine();