#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0070 

using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using System.Text;
using SKFunctions01;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
if (string.IsNullOrEmpty(githubToken))
{
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    githubToken = config["GITHUB_TOKEN"];
}
var modelId = "gpt-4o-mini";
var uri = "https://models.github.ai/inference";

// create client
var client = new OpenAIClient(new ApiKeyCredential(githubToken),
    new OpenAIClientOptions { Endpoint = new Uri(uri) });

// Create a chat completion service
var builder = Kernel.CreateBuilder();
builder.Plugins.AddFromType<CityTemperaturePlugIn>();
builder.AddOpenAIChatCompletion(modelId, client);

// Get the chat completion service
Kernel kernel = builder.Build();
var chat = kernel.GetRequiredService<IChatCompletionService>();

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

    // Get the chat completions
    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };

    var sb = new StringBuilder();
    var result = chat.GetStreamingChatMessageContentsAsync(history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);
    Console.Write("AI: ");
    await foreach (var item in result)
    {
        sb.Append(item);
        Console.Write(item.Content);
    }
    Console.WriteLine();

    history.AddAssistantMessage(sb.ToString());
}