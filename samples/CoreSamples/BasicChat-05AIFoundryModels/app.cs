#:package Azure.AI.OpenAI@2.8.0-beta.1
#:package Azure.Identity@1.18.0
#:package Microsoft.Extensions.AI@10.3.0
#:package Microsoft.Extensions.AI.OpenAI@10.3.0
#:package Microsoft.Extensions.Configuration.UserSecrets@10.0.3
#:property UserSecretsId=genai-beginners-dotnet

using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.Text;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

// One Microsoft Foundry endpoint can host MANY models behind it.
var endpoint = config["AzureOpenAI:Endpoint"]
    ?? throw new InvalidOperationException("Set AzureOpenAI:Endpoint in User Secrets. See: https://github.com/microsoft/Generative-AI-for-beginners-dotnet/blob/main/01-IntroductionToGenerativeAI/setup-azure-openai.md");

// Swap the model by changing ONLY the deployment name.
// In Microsoft Foundry you deploy several models behind the same endpoint, e.g.:
//   gpt-5.5  ->  grok-4.3  ->  phi-4  ->  ...   (same code, same endpoint, just this string).
//var deploymentName = config["AzureOpenAI:Deployment"] ?? "gpt-5-mini";
var deploymentName = "gpt-5-mini";
//var deploymentName = "Kimi-K2.6";

// Auth mode. "integrated" = Microsoft Entra ID (recommended for Foundry: no keys to leak).
// "apikey" = key from the Foundry portal (endpoint + deployment + apikey).
var authMode = config["AzureOpenAI:AuthMode"] ?? "integrated";

AzureOpenAIClient azureClient = authMode.Equals("apikey", StringComparison.OrdinalIgnoreCase)
    // Key auth: deployment name + endpoint + apikey.
    ? new AzureOpenAIClient(new Uri(endpoint),
        new AzureKeyCredential(config["AzureOpenAI:ApiKey"]
            ?? throw new InvalidOperationException("Set AzureOpenAI:ApiKey in User Secrets when AuthMode=apikey.")))
    // Integrated Security (recommended): Microsoft Entra ID via `az login`, no secrets.
    : new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential());

IChatClient client = azureClient
    .GetChatClient(deploymentName)
    .AsIChatClient()
    .AsBuilder()
    .Build();

Console.WriteLine($"Microsoft Foundry chat  ·  model: {deploymentName}  ·  auth: {authMode}");
Console.WriteLine("(swap the model with AzureOpenAI:Deployment, e.g. gpt-5.5 -> grok-4)");
Console.WriteLine("Streaming is enabled for a better live demo experience.");
Console.WriteLine("Suggested demo prompt:");
Console.WriteLine("hi, my name is Bruno, tell me your model name and something about your model card information");
Console.WriteLine();

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
    Console.Write($"AI [{deploymentName}] (streaming): ");
    await foreach (var item in result)
    {
        // validate if the item is null or has no contents
        if (item == null || item.Contents.Count == 0)
        {
            continue; // skip to the next item if it's null or empty
        }

        foreach (var content in item.Contents)
        {
            var text = content?.ToString();
            if (string.IsNullOrWhiteSpace(text))
            {
                continue;
            }

            sb.Append(text);
            Console.Write(text);
        }
    }
    Console.WriteLine();

    history.Add(new ChatMessage(ChatRole.Assistant, sb.ToString()));
}
