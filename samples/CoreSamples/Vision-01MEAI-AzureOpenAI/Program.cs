// Multimodal (vision) chat against Microsoft Foundry using Microsoft.Extensions.AI.
//
// Keyless by default: uses your `az login` credentials (Microsoft Entra ID).
//      dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-endpoint>.openai.azure.com/"
//      dotnet user-secrets set "AzureOpenAI:Deployment" "gpt-5-mini"
// Then sign in with: az login
//
// Optional key auth: also set "AzureOpenAI:ApiKey" and it will be used instead.

using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ClientModel;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["AzureOpenAI:Endpoint"]
    ?? throw new InvalidOperationException("Set AzureOpenAI:Endpoint in User Secrets. See: https://github.com/microsoft/Generative-AI-for-beginners-dotnet/blob/main/01-IntroductionToGenerativeAI/setup-azure-openai.md");
var deploymentName = config["AzureOpenAI:Deployment"] ?? "gpt-5-mini";
var apiKey = config["AzureOpenAI:ApiKey"];

// Keyless (Microsoft Entra ID) is the recommended path; fall back to a key when provided.
AzureOpenAIClient azureClient = string.IsNullOrEmpty(apiKey)
    ? new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    : new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey));

IChatClient chatClient = azureClient
        .GetChatClient(deploymentName)
        .AsIChatClient();


// images
string imgRunningShoes = "running-shoes.jpg";
string imgCarLicense = "license.jpg";
string imgReceipt = "german-receipt.jpg";

// prompts
var promptDescribe = "Describe the image";
var promptAnalyze = "How many red shoes are in the picture? and what other shoes colors are there?";
var promptOcr = "What is the text in this picture? Is there a theme for this?";
var promptReceipt = "I bought the coffee and the sausage. How much do I owe? Add a 18% tip.";

// The extra images and prompts above are ready-to-use variants: swap them into
// `prompt` and `imageFileName` below to demo OCR (license), receipt math
// (german-receipt), or counting (promptAnalyze). They are intentionally left
// unassigned until you swap, so silence the "assigned but never used" warning.
#pragma warning disable CS0219
_ = (imgCarLicense, imgReceipt, promptAnalyze, promptOcr, promptReceipt);
#pragma warning restore CS0219

// prompts
string systemPrompt = @"You are a useful assistant that describes images using a direct style.";
var prompt = promptDescribe;
string imageFileName = imgRunningShoes;
string image = Path.Combine(AppContext.BaseDirectory, "images", imageFileName);


List<ChatMessage> messages =
[
    new ChatMessage(Microsoft.Extensions.AI.ChatRole.System, systemPrompt),
    new ChatMessage(Microsoft.Extensions.AI.ChatRole.User, prompt),
];

// read the image bytes, create a new image content part and add it to the messages
AIContent aic = new DataContent(File.ReadAllBytes(image), "image/jpeg");
var message = new ChatMessage(Microsoft.Extensions.AI.ChatRole.User, [aic]);
messages.Add(message);

// send the messages to the assistant
var response = await chatClient.GetResponseAsync(messages);
Console.WriteLine($"Prompt: {prompt}");
Console.WriteLine($"Image: {imageFileName}");
Console.WriteLine($"Response: {response.Text}");
