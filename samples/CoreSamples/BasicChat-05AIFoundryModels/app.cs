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

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

// One Microsoft Foundry endpoint can host MANY models behind it.
var endpoint = config["AzureOpenAI:Endpoint"]
    ?? throw new InvalidOperationException("Set AzureOpenAI:Endpoint in User Secrets. See: https://github.com/microsoft/Generative-AI-for-beginners-dotnet/blob/main/01-IntroductionToGenerativeAI/setup-azure-openai.md");

// Swap the model by changing ONLY the deployment name.
// In Microsoft Foundry you deploy several models behind the same endpoint, e.g.:
//   gpt-5.5  ->  grok-4.3  ->  phi-4  ->  ...   (same code, same endpoint, just this string).
var deploymentName = config["AzureOpenAI:Deployment"] ?? "gpt-5-mini";
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
    .AsIChatClient();

Console.WriteLine($"Microsoft Foundry chat  ·  model: {deploymentName}  ·  auth: {authMode}");
Console.WriteLine("(swap the model with AzureOpenAI:Deployment, e.g. gpt-5.5 -> grok-4)");
Console.WriteLine("(switch auth with AzureOpenAI:AuthMode = apikey or integrated)");
Console.WriteLine();

var question = "what is your model name?";
Console.WriteLine($"Q: {question}");

var response = await client.GetResponseAsync(question);

Console.WriteLine();
Console.WriteLine($"AI [{deploymentName}]: {response.Text}");
