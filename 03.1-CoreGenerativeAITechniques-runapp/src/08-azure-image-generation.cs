using Microsoft.Extensions.Configuration;
using Azure.AI.OpenAI;
using OpenAI.Images;

// Azure OpenAI Image Generation Demo
// This demo shows how to generate images using Azure OpenAI DALL-E models

Console.WriteLine("🎨 Azure OpenAI Image Generation Demo");
Console.WriteLine();

try
{
    // Load configuration from user secrets
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    
    var endpoint = config["AZURE_OPENAI_ENDPOINT"];
    var apiKey = config["AZURE_OPENAI_APIKEY"];
    var model = config["AZURE_OPENAI_IMAGE_MODEL"] ?? "dall-e-3"; // Default to DALL-E 3

    if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
    {
        Console.WriteLine("❌ Missing Azure OpenAI configuration");
        Console.WriteLine("Please set the following user secrets:");
        Console.WriteLine("  dotnet user-secrets set 'AZURE_OPENAI_ENDPOINT' 'https://your-resource.openai.azure.com/'");
        Console.WriteLine("  dotnet user-secrets set 'AZURE_OPENAI_APIKEY' 'your-api-key'");
        Console.WriteLine("  dotnet user-secrets set 'AZURE_OPENAI_IMAGE_MODEL' 'dall-e-3' (optional)");
        return;
    }

    // Create Azure OpenAI client
    var azureClient = new AzureOpenAIClient(new Uri(endpoint), new System.ClientModel.ApiKeyCredential(apiKey));
    var imageClient = azureClient.GetImageClient(model);

    // Define the image prompt
    string prompt = "A futuristic robot coding on a laptop in a coffee shop, digital art style with warm lighting";
    
    Console.WriteLine($"🖼️ Generating image with prompt: {prompt}");
    Console.WriteLine($"🤖 Using model: {model}");
    Console.WriteLine();

    // Configure image generation options
    var options = new ImageGenerationOptions()
    {
        Size = GeneratedImageSize.W1024xH1024,
        Quality = GeneratedImageQuality.Standard,
        Style = GeneratedImageStyle.Vivid,
        ResponseFormat = GeneratedImageFormat.Bytes
    };

    // Generate the image
    Console.WriteLine("⏳ Generating image... This may take a few seconds");
    var generatedImage = await imageClient.GenerateImageAsync(prompt, options);

    // Save the image to a file
    string fileName = $"generated-image-{DateTimeOffset.Now:yyyyMMdd-HHmmss}.png";
    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    string fullPath = Path.Combine(desktopPath, fileName);
    
    await File.WriteAllBytesAsync(fullPath, generatedImage.ImageBytes.ToArray());
    
    Console.WriteLine($"✅ Image generated successfully!");
    Console.WriteLine($"📁 Saved to: {fullPath}");
    Console.WriteLine();

    // Attempt to open the image with the default viewer
    try
    {
        if (OperatingSystem.IsWindows())
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(fullPath) { UseShellExecute = true });
        }
        else if (OperatingSystem.IsLinux())
        {
            System.Diagnostics.Process.Start("xdg-open", fullPath);
        }
        else if (OperatingSystem.IsMacOS())
        {
            System.Diagnostics.Process.Start("open", fullPath);
        }
        
        Console.WriteLine("🖼️ Image opened in default viewer");
    }
    catch
    {
        Console.WriteLine("📝 Please manually open the image file to view it");
    }

    Console.WriteLine();
    Console.WriteLine("💡 Try experimenting with different prompts:");
    Console.WriteLine("   - 'A serene mountain landscape at sunset, oil painting style'");
    Console.WriteLine("   - 'A cute cartoon cat wearing glasses reading a book'");
    Console.WriteLine("   - 'Abstract geometric patterns in blue and gold, modern art style'");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error generating image: {ex.Message}");
    Console.WriteLine();
    Console.WriteLine("🔧 Troubleshooting:");
    Console.WriteLine("• Verify your Azure OpenAI endpoint and API key are correct");
    Console.WriteLine("• Ensure your Azure resource has DALL-E models deployed");
    Console.WriteLine("• Check that you have sufficient quota for image generation");
}