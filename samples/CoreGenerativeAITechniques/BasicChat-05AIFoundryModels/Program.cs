using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ClientModel;
using System.Text;



var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["endpoint"];
var apiKey = new ApiKeyCredential(config["apikey"]);
var deploymentName = config["deploymentName"];

IChatClient client = new AzureOpenAIClient(new Uri(endpoint), apiKey)
    .GetChatClient(deploymentName)
    .AsIChatClient()
    .AsBuilder()
    .Build();

var history = new List<ChatMessage>
{
    new(ChatRole.Assistant, "You are a useful chatbot. If you don't know an answer, say 'I don't know!'. Always reply in a funny way. Use emojis if possible.")
};

while (true)
{
    Console.Write("Q: ");
    var userQ = Console.ReadLine();
    if (string.IsNullOrEmpty(userQ))
    {
        break;
    }
    history.Add(new ChatMessage(ChatRole.User, userQ));

    var sb = new StringBuilder();
    var result = client.GetStreamingResponseAsync(history);
    Console.Write("AI: ");
    await foreach (var item in result)
    {
        // validate if the item is null or has no contents
        if (item == null || item.Contents.Count == 0)
        {
            continue; // skip to the next item if it's null or empty
        }
        sb.Append(item);
        Console.Write(item.Contents[0].ToString());
    }
    Console.WriteLine();

    history.Add(new ChatMessage(ChatRole.Assistant, sb.ToString()));
}