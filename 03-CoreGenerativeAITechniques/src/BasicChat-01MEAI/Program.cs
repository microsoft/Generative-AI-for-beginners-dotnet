using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.Text;
using ChatRole = Microsoft.Extensions.AI.ChatRole;

var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
if(string.IsNullOrEmpty(githubToken))
{
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    githubToken = config["GITHUB_TOKEN"];
}

IChatClient client = new ChatCompletionsClient(
        endpoint: new Uri("https://models.inference.ai.azure.com"),
        new AzureKeyCredential(githubToken))
        .AsChatClient("Phi-3.5-MoE-instruct");

// assume IChatClient is instantiated as before

List<ChatMessage> conversation =
[
    new (ChatRole.System, "You are a product review assistant. Your job is to help people write great product reviews. Keep asking questions on the person's experience with the product until you have enough information to write a review. Then write the review for them and ask if they are happy with it.")
];

Console.Write("Start typing a review (type 'q' to quit): ");

// Loop to read messages from the console
while (true)
{    
    string message = Console.ReadLine();

    if (message.ToLower() == "q")
    {
        break;
    }

    conversation.Add(new ChatMessage(ChatRole.User, message));

    // Process the message with the chat client (example)
    var response = await client.GetResponseAsync(conversation);
    conversation.Add(response.Message);
    
    Console.WriteLine(response.Message.Text);    
}
