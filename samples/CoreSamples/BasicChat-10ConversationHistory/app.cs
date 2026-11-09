#:package Azure.AI.OpenAI@2.9.0-beta.1
#:package Azure.Identity@1.21.0
#:package Microsoft.Extensions.AI@10.11.0
#:package Microsoft.Extensions.AI.OpenAI@10.11.0
#:package OllamaSharp@5.5.2
#:package Microsoft.Extensions.Configuration.UserSecrets@10.0.14
#:property UserSecretsId=genai-beginners-dotnet
#:property JsonSerializerIsReflectionEnabledByDefault=true

using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OllamaSharp;
using System.Text;

// Multi-turn conversation with Microsoft.Extensions.AI.
//
// The point of this sample is the CONVERSATION HISTORY pattern, which is identical for
// every provider because everything is an IChatClient. Set the Foundry secrets below and
// it runs against Microsoft Foundry; leave them unset and it falls back to local Ollama.
// Pass --ollama to select local inference explicitly without reading those secrets.
//      dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-endpoint>.openai.azure.com/"
//      dotnet user-secrets set "AzureOpenAI:Deployment" "gpt-5-mini"
// Then sign in with: az login

bool useOllama = args.Contains("--ollama");
if (args.Any(arg => arg != "--ollama"))
{
    throw new ArgumentException("Usage: dotnet run app.cs -- [--ollama]");
}
var config = useOllama
    ? new ConfigurationBuilder().Build()
    : new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["AzureOpenAI:Endpoint"];
var deploymentName = config["AzureOpenAI:Deployment"] ?? "gpt-5-mini";

// Same abstraction, different backend — the conversation code below never changes.
IChatClient client;
string modelLabel;
using var httpClient = new HttpClient
{
    BaseAddress = new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434"),
    Timeout = TimeSpan.FromMinutes(2)
};
if (!useOllama && !string.IsNullOrEmpty(endpoint))
{
    client = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
        .GetChatClient(deploymentName)
        .AsIChatClient();
    modelLabel = $"Microsoft Foundry · {deploymentName}";
}
else
{
    var model = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "phi4-mini";
    client = new OllamaApiClient(httpClient, model);
    modelLabel = $"Ollama · {model}";
}
using var clientLifetime = client;

Console.WriteLine($"Chat with history  ·  {modelLabel}");
Console.WriteLine();

// Create a conversation history list
// This is the CORRECT way to initialize a conversation with a system message
List<ChatMessage> conversation = new()
{
    new ChatMessage(ChatRole.System, "You are a good assistance with short and smart answers")
};

while (true)
{
    Console.Write("Q (empty to exit): ");
    string question = Console.ReadLine() ?? "";
    if (string.IsNullOrWhiteSpace(question))
    {
        break;
    }

    // Add user message to conversation history
    conversation.Add(new ChatMessage(ChatRole.User, question));

    // Stream the answer token-by-token, accumulating it so the full turn can be stored.
    var sb = new StringBuilder();
    Console.Write("AI: ");
    using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    await foreach (var update in client.GetStreamingResponseAsync(conversation, cancellationToken: timeout.Token))
    {
        Console.Write(update.Text);
        sb.Append(update.Text);
    }
    Console.WriteLine();
    if (sb.Length == 0)
    {
        throw new InvalidOperationException("The model returned no streaming text.");
    }

    // Append the accumulated assistant turn so the next question keeps the full context.
    conversation.Add(new ChatMessage(ChatRole.Assistant, sb.ToString()));
}
