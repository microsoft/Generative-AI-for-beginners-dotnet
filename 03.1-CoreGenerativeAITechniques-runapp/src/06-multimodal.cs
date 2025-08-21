// Multimodal AI Demo (Vision)
// This demo shows how to analyze images using vision-capable AI models
// 
// Prerequisites: Set GITHUB_TOKEN environment variable or user secret
// Run with: dotnet run 06-multimodal.cs (requires .NET 10)

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

// Create chat client with vision capabilities
IChatClient client = new ChatCompletionsClient(
        endpoint: new Uri("https://models.inference.ai.azure.com"),
        new AzureKeyCredential(githubToken))
        .AsIChatClient("gpt-4o-mini");

Console.WriteLine("👁️  Multimodal AI Demo - Vision Analysis");
Console.WriteLine("========================================");
Console.WriteLine("This demo analyzes images using AI vision capabilities.\n");

// Demo 1: Analyze a sample image from URL
await AnalyzeImageFromUrl(client);

// Demo 2: Analyze a local image (if available)
await AnalyzeLocalImage(client);

Console.WriteLine("✨ Vision analysis demo completed!");

static async Task AnalyzeImageFromUrl(IChatClient client)
{
    Console.WriteLine("🖼️  Demo 1: Analyzing Image from URL");
    Console.WriteLine("───────────────────────────────────");
    
    // Using a sample image from the web
    var imageUrl = "https://raw.githubusercontent.com/microsoft/Generative-AI-for-beginners-dotnet/main/03-CoreGenerativeAITechniques/images/vision-sample.jpg";
    
    try
    {
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a helpful AI assistant that can analyze images. Provide detailed, accurate descriptions of what you see."),
            new ChatMessage(ChatRole.User, [
                new TextContent("Please analyze this image and describe what you see. Include details about:"),
                new TextContent("- Main subjects or objects"),
                new TextContent("- Setting/environment"),  
                new TextContent("- Colors and lighting"),
                new TextContent("- Any text or signs visible"),
                new TextContent("- Overall mood or atmosphere"),
                new ImageContent(new Uri(imageUrl))
            ])
        };

        Console.WriteLine($"🔗 Analyzing image: {imageUrl}");
        Console.WriteLine("🤖 AI Analysis:");
        Console.WriteLine(new string('─', 50));
        
        var response = await client.CompleteAsync(messages);
        Console.WriteLine(response.Message.Text);
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error analyzing image from URL: {ex.Message}");
        Console.WriteLine("Note: The image URL might not be accessible or the model might not support vision.");
        Console.WriteLine();
    }
}

static async Task AnalyzeLocalImage(IChatClient client)
{
    Console.WriteLine("📁 Demo 2: Analyzing Local Image");
    Console.WriteLine("────────────────────────────────");
    
    // Check for common image file types in current directory
    var imageExtensions = new[] { "*.jpg", "*.jpeg", "*.png", "*.gif", "*.bmp" };
    var currentDir = Directory.GetCurrentDirectory();
    
    string? imageFile = null;
    foreach (var extension in imageExtensions)
    {
        var files = Directory.GetFiles(currentDir, extension, SearchOption.TopDirectoryOnly);
        if (files.Length > 0)
        {
            imageFile = files[0];
            break;
        }
    }
    
    if (imageFile == null)
    {
        Console.WriteLine("💡 No local image files found in current directory.");
        Console.WriteLine("   To test with a local image, place a .jpg, .png, or other image file in this directory.");
        Console.WriteLine();
        return;
    }
    
    try
    {
        // Read image as base64
        var imageBytes = await File.ReadAllBytesAsync(imageFile);
        var base64Image = Convert.ToBase64String(imageBytes);
        var mimeType = GetMimeType(Path.GetExtension(imageFile));
        
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a helpful AI assistant that can analyze images. Be specific and detailed in your analysis."),
            new ChatMessage(ChatRole.User, [
                new TextContent("Please analyze this local image and provide:"),
                new TextContent("1. A general description of what you see"),
                new TextContent("2. Technical details (resolution, format if visible)"),
                new TextContent("3. Suggestions for improvement if it's a photo"),
                new TextContent("4. Any interesting details you notice"),
                new ImageContent(Convert.FromBase64String(base64Image), mimeType)
            ])
        };

        Console.WriteLine($"📸 Analyzing local image: {Path.GetFileName(imageFile)}");
        Console.WriteLine("🤖 AI Analysis:");
        Console.WriteLine(new string('─', 50));
        
        var response = await client.CompleteAsync(messages);
        Console.WriteLine(response.Message.Text);
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error analyzing local image: {ex.Message}");
        Console.WriteLine("Note: The model might not support vision or the image format might not be supported.");
        Console.WriteLine();
    }
}

static string GetMimeType(string extension)
{
    return extension.ToLower() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        ".bmp" => "image/bmp",
        ".webp" => "image/webp",
        _ => "image/jpeg"
    };
}

// Helper method to create a simple test image if none exists
static async Task CreateSampleImageIfNeeded()
{
    var sampleImagePath = "sample-image.txt";
    if (!File.Exists(sampleImagePath))
    {
        var sampleImageInfo = """
            📝 Sample Image Instructions
            ═══════════════════════════
            
            To test the multimodal demo with a local image:
            
            1. Place any image file (.jpg, .png, .gif, .bmp) in this directory
            2. Run the demo again
            3. The AI will analyze your image and provide detailed insights
            
            Suggested test images:
            • Screenshots of applications
            • Photos of objects or scenes  
            • Charts or diagrams
            • Artwork or designs
            • Documents with text
            
            The AI can describe content, read text, analyze composition,
            and provide detailed observations about what it sees.
            """;
            
        await File.WriteAllTextAsync(sampleImagePath, sampleImageInfo);
    }
}