#pragma warning disable SKEXP0001, SKEXP0070  

using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;
using Microsoft.SemanticKernel.Connectors.Ollama;
using OllamaSharp;

var modelId = "phi4-mini";
var uri = "http://localhost:11434/";


// create client
var chat = new OllamaApiClient(uri, modelId)
    .AsChatCompletionService();

var history = new ChatHistory();
history.AddSystemMessage("You are a useful chatbot. If you don't know an answer, say 'I don't know!'. Always reply in a funny way. Use emojis if possible.");

while (true)
{
    Console.Write("Q: ");
    var userQ = Console.ReadLine();
    if (string.IsNullOrEmpty(userQ))
    {
        break;
    }
    history.AddUserMessage(userQ);

    var sb = new StringBuilder();
    var result = chat.GetStreamingChatMessageContentsAsync(history);
    Console.Write($"AI [{modelId}]: ");
    await foreach (var item in result)
    {
        sb.Append(item);
        Console.Write(item.Content);
    }
    Console.WriteLine();

    history.AddAssistantMessage(sb.ToString());
}