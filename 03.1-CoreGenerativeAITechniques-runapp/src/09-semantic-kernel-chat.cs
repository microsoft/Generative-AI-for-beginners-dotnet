using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using OpenAI;
using System.ClientModel;
using Microsoft.Extensions.Configuration;
using System.Text;

// Interactive Chat with Semantic Kernel
// This demo shows streaming chat with conversation memory using SK

Console.WriteLine("💬 Interactive Chat with Semantic Kernel");
Console.WriteLine("Type 'exit' or 'quit' to end the conversation");
Console.WriteLine();

try
{
    // Get GitHub token for GitHub Models
    var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
    if (string.IsNullOrEmpty(githubToken))
    {
        var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        githubToken = config["GITHUB_TOKEN"];
    }

    if (string.IsNullOrEmpty(githubToken))
    {
        Console.WriteLine("❌ Missing GitHub token");
        Console.WriteLine("Please set your GitHub token:");
        Console.WriteLine("  export GITHUB_TOKEN='your_token_here'");
        Console.WriteLine("  OR");
        Console.WriteLine("  dotnet user-secrets set 'GITHUB_TOKEN' 'your_token_here'");
        return;
    }

    // Configure GitHub Models with Semantic Kernel
    var modelId = "Phi-3.5-mini-instruct";
    var uri = "https://models.github.ai/inference";

    var client = new OpenAIClient(new ApiKeyCredential(githubToken), 
        new OpenAIClientOptions { Endpoint = new Uri(uri) });

    var builder = Kernel.CreateBuilder();
    builder.AddOpenAIChatCompletion(modelId, client);

    var kernel = builder.Build();
    var chat = kernel.GetRequiredService<IChatCompletionService>();

    // Initialize chat history with system message
    var history = new ChatHistory();
    history.AddSystemMessage("You are a helpful, creative, and friendly AI assistant. You explain things clearly and ask follow-up questions when helpful. Use emojis occasionally to make the conversation more engaging. If you don't know something, say so honestly.");

    Console.WriteLine("🤖 AI Assistant ready! How can I help you today?");
    Console.WriteLine();

    // Main chat loop
    while (true)
    {
        Console.Write("👤 You: ");
        var userInput = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(userInput) || 
            userInput.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
            userInput.Equals("quit", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("👋 Goodbye! Thanks for chatting!");
            break;
        }

        // Add user message to history
        history.AddUserMessage(userInput);

        // Get streaming response from AI
        Console.Write("🤖 AI: ");
        var responseBuilder = new StringBuilder();
        
        try
        {
            var streamingResponse = chat.GetStreamingChatMessageContentsAsync(history);
            
            await foreach (var chunk in streamingResponse)
            {
                var content = chunk.Content;
                if (!string.IsNullOrEmpty(content))
                {
                    Console.Write(content);
                    responseBuilder.Append(content);
                }
            }
            
            Console.WriteLine();
            Console.WriteLine();

            // Add AI response to history
            history.AddAssistantMessage(responseBuilder.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error getting response: {ex.Message}");
            Console.WriteLine("Let's try again!");
            Console.WriteLine();
        }
    }

    // Display conversation summary
    Console.WriteLine($"📊 Conversation ended. Total messages: {history.Count}");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Startup error: {ex.Message}");
    Console.WriteLine();
    Console.WriteLine("🔧 Troubleshooting:");
    Console.WriteLine("• Make sure your GitHub token has access to GitHub Models");
    Console.WriteLine("• Check your internet connection");
    Console.WriteLine("• Verify the GitHub Models service is available");
}