// Interactive Chat Flow Demo
// This demo shows how to create an interactive chat conversation using Microsoft.Extensions.AI
// 
// Prerequisites: Set GITHUB_TOKEN environment variable or user secret
// Run with: dotnet run 02-chat-flow.cs (requires .NET 10)

using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

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

// Initialize conversation history
var chatHistory = new List<ChatMessage>
{
    new ChatMessage(ChatRole.System, "You are a helpful AI assistant specializing in .NET development and programming best practices. Keep your responses concise and practical.")
};

Console.WriteLine("💬 Interactive .NET Programming Assistant");
Console.WriteLine("=========================================");
Console.WriteLine("Ask me anything about .NET development! Type 'quit' to exit.\n");

try
{
    while (true)
    {
        // Get user input
        Console.Write("👤 You: ");
        var userInput = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(userInput) || userInput.ToLower() == "quit")
        {
            Console.WriteLine("👋 Goodbye! Happy coding!");
            break;
        }

        // Add user message to conversation history
        chatHistory.Add(new ChatMessage(ChatRole.User, userInput));

        Console.Write("🤖 Assistant: ");
        
        // Get streaming response from the AI
        await foreach (var update in client.CompleteStreamingAsync(chatHistory))
        {
            Console.Write(update.Text);
        }
        
        // Add the complete response to conversation history
        var response = await client.CompleteAsync(chatHistory);
        chatHistory.Add(new ChatMessage(ChatRole.Assistant, response.Message.Text));
        
        Console.WriteLine("\n");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine("Make sure your GITHUB_TOKEN is valid and has access to GitHub Models.");
}