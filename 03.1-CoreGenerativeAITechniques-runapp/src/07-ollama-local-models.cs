using Microsoft.Extensions.AI;
using System.Text;

// Local AI with Ollama - no API keys needed!
// Make sure Ollama is running with: ollama serve
// And the model is pulled: ollama pull phi4-mini

Console.WriteLine("🔧 Local AI Demo with Ollama");
Console.WriteLine("Using phi4-mini model locally via Ollama");
Console.WriteLine();

try
{
    // Create Ollama client pointing to local server
    IChatClient client = new OllamaChatClient(new Uri("http://localhost:11434/"), "phi4-mini");

    // Build a comprehensive prompt for sentiment analysis
    StringBuilder prompt = new StringBuilder();
    prompt.AppendLine("You will analyze the sentiment of the following product reviews. Each line is its own review. Output the sentiment of each review in a bulleted list including the original text and the sentiment, and then provide a general sentiment of all reviews.");
    prompt.AppendLine();
    prompt.AppendLine("Reviews:");
    prompt.AppendLine("I bought this product and it's amazing. I love it!");
    prompt.AppendLine("This product is terrible. I hate it.");
    prompt.AppendLine("I'm not sure about this product. It's okay.");
    prompt.AppendLine("I found this product based on the other reviews. It worked for a bit, and then it didn't.");

    Console.WriteLine("📝 Prompt sent to Ollama:");
    Console.WriteLine(prompt.ToString());
    Console.WriteLine("🤖 Ollama Response:");
    Console.WriteLine();

    // Send prompt to local model
    var response = await client.GetResponseAsync(prompt.ToString());
    Console.WriteLine(response.Text);

    Console.WriteLine();
    Console.WriteLine("✅ Local AI demo completed successfully!");
    Console.WriteLine("💡 Try modifying the reviews or prompt to experiment with different scenarios");
}
catch (HttpRequestException ex) when (ex.Message.Contains("Connection refused"))
{
    Console.WriteLine("❌ Error: Could not connect to Ollama server");
    Console.WriteLine("Please ensure Ollama is running:");
    Console.WriteLine("  1. Start Ollama: ollama serve");
    Console.WriteLine("  2. Pull the model: ollama pull phi4-mini");
    Console.WriteLine("  3. Try again");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine("Make sure Ollama is properly installed and the phi4-mini model is available");
}