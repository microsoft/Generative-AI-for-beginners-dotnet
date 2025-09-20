// LLM Text Completion Demo
// This demo shows how to use Microsoft.Extensions.AI for basic text completions
// 
// Prerequisites: Set GITHUB_TOKEN environment variable or user secret
// Run with: dotnet run 01-llm-completion.cs (requires .NET 10)

using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.Text;

// Get GitHub token from environment or user secrets
var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
if (string.IsNullOrEmpty(githubToken))
{
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    githubToken = config["GITHUB_TOKEN"];
}

if (string.IsNullOrEmpty(githubToken))
{
    Console.WriteLine("Error: GITHUB_TOKEN not found. Please set it as an environment variable or user secret.");
    return;
}

// Create chat client using GitHub Models
IChatClient client = new ChatCompletionsClient(
        endpoint: new Uri("https://models.inference.ai.azure.com"),
        new AzureKeyCredential(githubToken))
        .AsIChatClient("Phi-3.5-MoE-instruct");

// Build the sentiment analysis prompt
StringBuilder prompt = new StringBuilder();
prompt.AppendLine("You will analyze the sentiment of the following product reviews. Each line is its own review. Output the sentiment of each review in a bulleted list and then provide a general sentiment of all reviews.");
prompt.AppendLine("I bought this product and it's amazing. I love it!");
prompt.AppendLine("This product is terrible. I hate it.");
prompt.AppendLine("I'm not sure about this product. It's okay.");
prompt.AppendLine("I found this product based on the other reviews. It worked for a bit, and then it didn't.");

Console.WriteLine("🤖 Analyzing product review sentiments...\n");
Console.WriteLine("📝 Reviews to analyze:");
var reviewLines = prompt.ToString().Split('\n').Skip(1).Take(4);
foreach (var review in reviewLines)
{
    if (!string.IsNullOrWhiteSpace(review))
        Console.WriteLine($"   • {review}");
}

Console.WriteLine("\n🔄 Sending to AI model...\n");

try
{
    // Send the prompt to the model and wait for the text completion
    var response = await client.GetResponseAsync(prompt.ToString());
    
    // Display the response
    Console.WriteLine("🎯 AI Analysis Result:");
    Console.WriteLine(new string('=', 50));
    Console.WriteLine(response.Text);
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine("Make sure your GITHUB_TOKEN is valid and has access to GitHub Models.");
}