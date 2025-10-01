#pragma warning disable SKEXP0001, SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0050, SKEXP0052
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text;

var model = "Phi-3.5-mini-instruct-cuda-gpu";
var baseUrl = "http://localhost:5273/v1";
var apiKey = "unused";

// Create a chat completion service
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(modelId: model, apiKey: apiKey, endpoint: new Uri(baseUrl))
    .Build();

var chat = kernel.GetRequiredService<IChatCompletionService>();
var history = new ChatHistory();
history.AddSystemMessage("You are a useful chatbot. Always reply in a funny way with short answers.");

var settings = new OpenAIPromptExecutionSettings
{
    MaxTokens = 50000,
    Temperature = 1
};

while (true)
{
    Console.Write("Q: ");
    var userQuestion = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userQuestion))
    {
        break;
    }
    history.AddUserMessage(userQuestion);

    var responseBuilder = new StringBuilder();
    Console.Write("AI: ");
    await foreach (var message in chat.GetStreamingChatMessageContentsAsync(history, settings, kernel))
    {
        responseBuilder.Append(message.Content);
        Console.Write(message.Content);
    }
    Console.WriteLine();

    history.AddAssistantMessage(responseBuilder.ToString());
}