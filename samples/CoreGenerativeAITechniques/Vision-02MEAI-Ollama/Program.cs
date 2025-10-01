using Microsoft.Extensions.AI;

IChatClient chatClient =
    new OllamaChatClient(new Uri("http://localhost:11434/"), "llama3.2-vision");

// images
string imgRunningShoes = "running-shoes.jpg";
string imgCarLicense = "license.jpg";
string imgReceipt = "german-receipt.jpg";

// prompts
var promptDescribe = "Describe the image";
var promptAnalyze = "How many red shoes are in the picture? and what other shoes colors are there?";
var promptOcr = "What is the text in this picture? Is there a theme for this?";
var promptReceipt = "I bought the coffee and the sausage. How much do I owe? Add a 18% tip.";

// prompts
string systemPrompt = @"You are a useful assistant that describes images using a direct style.";
var prompt = promptDescribe;
string imageFileName = imgRunningShoes;
string image = Path.Combine(Directory.GetCurrentDirectory(), "images", imageFileName);

// read the image bytes, create a new image content part and add it to the messages
AIContent aic = new DataContent(File.ReadAllBytes(image), "image/jpeg");
List<ChatMessage> messages =
[
    new ChatMessage(ChatRole.User, prompt),
    new ChatMessage(ChatRole.User, [aic])
 ];

// send the messages to the assistant
var imageAnalysis = await chatClient.GetResponseAsync(messages);

Console.WriteLine($"Prompt: {prompt}");
Console.WriteLine($"Image: {imageFileName}");
Console.WriteLine($"Response: {imageAnalysis.Text}");