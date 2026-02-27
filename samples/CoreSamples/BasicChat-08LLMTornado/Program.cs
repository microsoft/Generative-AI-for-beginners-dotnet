using LlmTornado;
using LlmTornado.Chat;
using LlmTornado.Chat.Models;
using Microsoft.Extensions.Configuration;
using System.Text;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["endpoint"];
var apiKey = config["apikey"];

TornadoApi tornadoApi = new(
    serverUri: new Uri(endpoint),
    apiKey: apiKey,
    provider: LlmTornado.Code.LLmProviders.AzureOpenAi);

Conversation chat = tornadoApi.Chat.CreateConversation(new ChatRequest
{
    Model = ChatModel.OpenAi.Gpt41.V41Mini
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

Console.WriteLine("Azure OpenAI gpt-4.1-mini:");
Console.WriteLine(response);