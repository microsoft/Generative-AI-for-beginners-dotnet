using LlmTornado;
using LlmTornado.Chat;
using LlmTornado.Chat.Models;
using Microsoft.Extensions.Configuration;
using System.Text;

var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
if (string.IsNullOrEmpty(githubToken))
{
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    githubToken = config["GITHUB_TOKEN"];
}

TornadoApi tornadoApi = new(
    serverUri: new Uri("https://models.github.ai/inference"),
    apiKey: githubToken,
    provider: LlmTornado.Code.LLmProviders.OpenAi);

Conversation chat = tornadoApi.Chat.CreateConversation(new ChatRequest
{
    Model = ChatModel.OpenAi.
});

// here we're building the prompt
StringBuilder prompt = new();
prompt.AppendLine("You will analyze the sentiment of the following product reviews. Each line is its own review. Output the sentiment of each review in a bulleted list and then provide a generate sentiment of all reviews. ");
prompt.AppendLine("I bought this product and it's amazing. I love it!");
prompt.AppendLine("This product is terrible. I hate it.");
prompt.AppendLine("I'm not sure about this product. It's okay.");
prompt.AppendLine("I found this product based on the other reviews. It worked for a bit, and then it didn't.");

chat.AppendUserInput(prompt.ToString());
string? response = await chat.GetResponse();

Console.WriteLine("GitHub Models gpt-4.1-mini:");
Console.WriteLine(response);