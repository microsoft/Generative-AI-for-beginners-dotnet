using Microsoft.Extensions.AI;
using OllamaSharp;

using var httpClient = new HttpClient
{
    BaseAddress = new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434"),
    Timeout = TimeSpan.FromMinutes(3)
};
using IChatClient chatClient =
    new OllamaApiClient(httpClient, Environment.GetEnvironmentVariable("OLLAMA_VISION_MODEL") ?? "llama3.2-vision");

// images
string imgRunningShoes = "running-shoes.jpg";
string imgCarLicense = "license.jpg";
string imgReceipt = "german-receipt.jpg";

// prompts
var promptDescribe = "Describe the main objects in the image in one short sentence.";
var promptAnalyze = "How many red shoes are in the picture? and what other shoes colors are there?";
var promptOcr = "What is the text in this picture? Is there a theme for this?";
var promptReceipt = "I bought the coffee and the sausage. How much do I owe? Add a 18% tip.";

// prompts
string systemPrompt = @"You are a useful assistant that describes images using a direct style.";
var prompt = promptDescribe;
string imageFileName = imgRunningShoes;
string image = Path.Combine(AppContext.BaseDirectory, "images", imageFileName);

// read the image bytes, create a new image content part and add it to the messages
AIContent aic = new DataContent(File.ReadAllBytes(image), "image/jpeg");
List<ChatMessage> messages =
[
    new ChatMessage(ChatRole.System, systemPrompt),
    new ChatMessage(ChatRole.User, [new TextContent(prompt), aic])
 ];

// send the messages to the assistant
using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(3));
var imageAnalysis = await chatClient.GetResponseAsync(messages, cancellationToken: timeout.Token);
if (string.IsNullOrWhiteSpace(imageAnalysis.Text))
{
    throw new InvalidOperationException("The vision model returned no image description.");
}

Console.WriteLine($"Prompt: {prompt}");
Console.WriteLine($"Image: {imageFileName}");
Console.WriteLine($"Response: {imageAnalysis.Text}");