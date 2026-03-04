#:package Azure.AI.OpenAI@2.8.0-beta.1
#:package Azure.Identity@1.18.0
#:package Microsoft.Extensions.AI@10.3.0
#:package Microsoft.Extensions.AI.OpenAI@10.3.0
#:package Microsoft.Extensions.Configuration.UserSecrets@10.0.3
#:property UserSecretsId=genai-beginners-dotnet

﻿using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.Text;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["AzureOpenAI:Endpoint"]
    ?? throw new InvalidOperationException("Set AzureOpenAI:Endpoint in User Secrets. See: https://github.com/microsoft/Generative-AI-for-beginners-dotnet/blob/main/01-IntroductionToGenerativeAI/setup-azure-openai.md");
var deploymentName = config["AzureOpenAI:Deployment"] ?? "gpt-5-mini";

IChatClient client = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
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
