# Image and Video Generation with Azure OpenAI New Models

In this lesson, we'll explore how to use the latest Azure OpenAI models for generating images and videos in your .NET applications. We'll cover `gpt-image-1` for advanced image generation and `sora` for video generation, providing you with cutting-edge capabilities for creating visual content.

---

## Image and Video Generation with New Azure OpenAI Models

Image and video generation AI represents the next frontier in creative applications. Using the latest models like `gpt-image-1` for images and `sora` for videos through Azure OpenAI, you can create high-quality visual content from text descriptions. These new models offer improved quality, better understanding of complex prompts, and enhanced creative capabilities.

### Image Generation with gpt-image-1

The `gpt-image-1` model represents a significant advancement in image generation capabilities. Let's explore how to use it in a .NET application:

> 🧑‍💻**Sample code**: [Here is a working example of this application](../samples/CoreSamples/ImageGeneration-01/) you can follow along with.

#### How to run the sample code

To run the sample code, you'll need to:

1. Make sure you have set up a GitHub Codespace with the appropriate environment as described in the [Setup guide](../02-SetupDevEnvironment/readme.md)
2. Ensure you have configured your Azure OpenAI API key and settings as described in the [Azure OpenAI setup guide](../02-SetupDevEnvironment/getting-started-azure-openai.md)
3. Open a terminal in your codespace (Ctrl+` or Cmd+`)
4. Navigate to the sample code directory:
   
   If you're using Windows Command Prompt (CMD) or PowerShell:
   ```bash
   cd samples\CoreSamples\ImageGeneration-01
   ```
   
   If you're using Linux, macOS, Git Bash, WSL, or the VS Code terminal:
   ```bash
   cd samples/CoreSamples/ImageGeneration-01
   ```
   
   > **Note**: GitHub Codespaces runs a Linux environment, so always use forward slashes (`/`) in paths when working in Codespaces, regardless of your local operating system.

5. Run the application:
   ```bash
   dotnet run
   ```

#### Setting up the Azure OpenAI Client for gpt-image-1

First, we need to set up our configuration and create an Azure OpenAI client specifically for the `gpt-image-1` model:

```csharp
var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
var configuration = builder.Build();

var model = configuration["model"]; // Set to "gpt-image-1" in your configuration
var url = configuration["api_url"];
var apiKey = configuration["api_key"];

AzureOpenAIClient azureClient = new(new Uri(url), new System.ClientModel.ApiKeyCredential(apiKey));
var client = azureClient.GetImageClient(model);
```

In this code:

1. We load the configuration from user secrets

1. We extract the model name (should be "gpt-image-1"), API URL, and API key from the configuration

1. We create an Azure OpenAI client using the URL and API key

1. We get an image client specifically for the `gpt-image-1` model

#### Creating Advanced Prompts and Options

The `gpt-image-1` model supports more sophisticated prompts and enhanced options:

```csharp
string prompt = "A kitten playing soccer in the moon. Use a comic style";

// generate an image using the prompt with advanced options
ImageGenerationOptions options = new()
{
    Size = GeneratedImageSize.W1024xH1024,
    Quality = "medium"
};
```

The `gpt-image-1` model provides:

- Enhanced understanding of complex prompts
- Better style interpretation
- Improved image quality and consistency
- More accurate object placement and composition

#### Generating and Processing the Image

With our client, prompt, and options configured for `gpt-image-1`, we can generate the image:

```csharp
GeneratedImage image = await client.GenerateImageAsync(prompt, options);

// Save the image to a file
string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/genimage{DateTimeOffset.Now.Ticks}.png";
File.WriteAllBytes(path, image.ImageBytes.ToArray());
```

This code:

1. Calls the `GenerateImageAsync` method with our prompt and options
2. Creates a file path on the desktop with a unique filename
3. Saves the generated image bytes to the file

### Video Generation with Sora

The `sora` model enables video generation from text prompts, bringing motion and temporal dynamics to your AI-generated content. Let's explore how to use it:

> 🧑‍💻**Sample code**: [Here are working examples for video generation](../samples/CoreSamples/VideoGeneration-AzureSora-01/) and [using the AzureSoraSDK](../samples/CoreSamples/VideoGeneration-AzureSoraSDK-02/) you can follow along with.

#### Using REST API Approach

The first approach uses direct REST API calls to interact with the Sora model:

```csharp
// Configuration setup
var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
var configuration = builder.Build();
string endpoint = configuration["endpoint"];
string apiKey = configuration["api_key"];
string model = "sora";

// HTTP client setup
var client = new HttpClient();
client.DefaultRequestHeaders.Add("api-key", apiKey);
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
```

#### Creating a Video Generation Job

To generate a video with Sora, we first create a video generation job:

```csharp
string prompt = "Two puppies playing soccer in the moon. Use a cartoon style.";

// Create a video generation job
string createUrl = $"{endpoint}/openai/v1/video/generations/jobs?api-version=preview";
var body = new
{
    prompt = prompt,
    width = 480,
    height = 480,
    n_seconds = 5,
    model = model
};

var bodyJson = JsonSerializer.Serialize(body);
var response = await client.PostAsync(createUrl, new StringContent(bodyJson, Encoding.UTF8, "application/json"));
response.EnsureSuccessStatusCode();

var responseJson = await response.Content.ReadAsStringAsync();
using var doc = JsonDocument.Parse(responseJson);
string jobId = doc.RootElement.GetProperty("id").GetString();
```

#### Polling for Job Completion

Video generation takes time, so we need to poll for the job status:

```csharp
// Poll for job status
string statusUrl = $"{endpoint}/openai/v1/video/generations/jobs/{jobId}?api-version=preview";
string status = null;
JsonElement statusResponse = default;

do
{
    await Task.Delay(5000); // Wait 5 seconds between polls
    var statusResp = await client.GetAsync(statusUrl);
    var statusJson = await statusResp.Content.ReadAsStringAsync();
    var statusDoc = JsonDocument.Parse(statusJson);
    statusResponse = statusDoc.RootElement;
    status = statusResponse.GetProperty("status").GetString();
    Console.WriteLine($"{DateTime.Now:dd-MMM-yyyy HH:mm:ss} Job status: {status}");
} while (status != "succeeded" && status != "failed" && status != "cancelled");
```

#### Downloading the Generated Video

Once the job succeeds, we can download the generated video:

```csharp
if (status == "succeeded")
{
    string generationId = statusResponse.GetProperty("generations")[0].GetProperty("id").GetString();
    string videoUrl = $"{endpoint}/openai/v1/video/generations/{generationId}/content/video?api-version=preview";
    
    var videoResp = await client.GetAsync(videoUrl);
    if (videoResp.IsSuccessStatusCode)
    {
        string outputFilename = Path.Combine(outputDir, $"sora_{DateTime.Now:ddMMMyyyy_HHmmss}.mp4");
        using (var fs = new FileStream(outputFilename, FileMode.Create, FileAccess.Write))
        {
            await videoResp.Content.CopyToAsync(fs);
        }
        Console.WriteLine($"SORA Generated video saved: '{outputFilename}'");
    }
}
```


#### Using AzureSoraSDK (Alternative Approach)

For a more streamlined experience, you can use the [AzureSoraSDK](https://github.com/DrHazemAli/AzureSoraSDK) by [DrHazemAli](https://github.com/DrHazemAli). **Note:** This is *not* an official SDK from Microsoft or OpenAI, but it works well and provides a convenient way to generate videos with Sora in .NET applications.

> ℹ️ **Learn more:** Visit the [AzureSoraSDK repository](https://github.com/DrHazemAli/AzureSoraSDK) and check out the [Wiki](https://github.com/DrHazemAli/AzureSoraSDK/wiki) for detailed documentation, usage examples, and the latest updates from the author.

---

```csharp
// Configure the client
var options = new SoraClientOptions
{
    Endpoint = endpoint,
    ApiKey = apiKey,
    DeploymentName = "sora",
    ApiVersion = "preview"
};

// Create client
using var client = new SoraClient(options.Endpoint, options.ApiKey, options.DeploymentName);

// Submit video generation job
var jobId = await client.SubmitVideoJobAsync(
    prompt: "A serene waterfall in a lush forest with sunlight filtering through trees",
    width: 1280,
    height: 720,
    nSeconds: 10);

// Wait for completion and download
var videoUri = await client.WaitForCompletionAsync(jobId);
var outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "sora_videos", "output.mp4");
await client.DownloadVideoAsync(videoUri, outputPath);

---

**References:**

- Author: [DrHazemAli](https://github.com/DrHazemAli)
- Repository: [AzureSoraSDK](https://github.com/DrHazemAli/AzureSoraSDK)
- Wiki: [AzureSoraSDK Wiki](https://github.com/DrHazemAli/AzureSoraSDK/wiki)

> 📝 **Note:** AzureSoraSDK is a community-developed SDK and not an official Microsoft or OpenAI product. However, it is well-maintained and provides a user-friendly way to generate videos with Sora. For the latest features, troubleshooting, and advanced usage, please visit the author's [repository](https://github.com/DrHazemAli/AzureSoraSDK) and [wiki](https://github.com/DrHazemAli/AzureSoraSDK/wiki).
```

> 🗒️**Note:** Video generation with Sora typically takes several minutes to complete, depending on the length and complexity of the requested video.

> 🙋 **Need help?**: If you encounter any issues running these examples, [open an issue in the repository](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new?template=Blank+issue) and we'll help you troubleshoot.

#### Ready-to-Use SORA Applications

There are community-driven projects (not officially affiliated with Microsoft) that leverage the SORA APIs to generate videos. These projects require users to configure their own credentials and provide support for various AI model services.

One such project is [Viral Videos Demo](https://github.com/hprez21/ViralVideosDemo), a cross-platform application built with **.NET MAUI**, which allows you to create viral videos (or any other type of content) by editing prompts directly in the code or through a user-friendly graphical interface. 

### Integration with SORA APIs

The Viral Videos Demo integrates with the SORA APIs to generate videos based on user-provided prompts. To use the application, you will need:

1. **Azure OpenAI API Key**: Obtain your API key from the [Azure OpenAI portal](https://learn.microsoft.com/azure/ai-services/openai/).
2. **SORA API Endpoint**: Configure the endpoint URL for the SORA video generation service.
3. **Credential Configuration**: Update the application's configuration file or environment variables with your API key and endpoint details.

For detailed setup instructions, refer to the [Viral Videos Demo repository](https://github.com/hprez21/ViralVideosDemo). The repository includes step-by-step guidance on configuring the application and using the SORA APIs effectively.
<img width="376" height="762" alt="The video viewer page for videos generated by the SORA model" src="https://github.com/user-attachments/assets/e0e45a39-31e9-4509-b09e-b75b3e0f9fb5" />

Here’s a [video](https://youtu.be/Rrc2rOrUa50) demonstrating how it works.

## Key Differences and Capabilities

### gpt-image-1 vs Previous Models
- **Enhanced Prompt Understanding**: Better interpretation of complex, detailed prompts
- **Improved Quality**: Higher resolution and more detailed images
- **Better Style Consistency**: More accurate representation of artistic styles
- **Object Placement**: Better spatial understanding and object relationships

### Sora Video Generation Features
- **Temporal Consistency**: Maintains object and scene consistency across frames
- **Complex Motion**: Handles intricate movements and interactions
- **Style Flexibility**: Supports various artistic styles and aesthetics
- **Duration Control**: Generate videos from a few seconds to longer sequences

## Summary

In this lesson, we explored the latest Azure OpenAI models for visual content generation:

1. **gpt-image-1 for Image Generation**:
   - Set up Azure OpenAI client for the latest image model
   - Create sophisticated prompts with enhanced understanding
   - Generate high-quality images with improved consistency

2. **Sora for Video Generation**:
   - Create video generation jobs using REST API
   - Monitor job progress and handle asynchronous processing
   - Download and save generated videos
   - Use AzureSoraSDK for simplified integration

These new models represent significant advances in AI-generated visual content, offering enhanced quality, better prompt understanding, and new creative possibilities for your applications.

## Additional resources

- [Microsoft Learn: How to use Azure OpenAI image generation models](https://learn.microsoft.com/azure/ai-services/openai/how-to/dall-e?tabs=gpt-image-1)
- [OpenAI-dotnet image generation](https://github.com/openai/openai-dotnet?tab=readme-ov-file#how-to-generate-images)
- [Microsoft Learn: Sora video generation (preview)](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation)
- [Azure Sora SDK documentation](https://github.com/DrHazemAli/AzureSoraSDK/tree/main)
- [AzureSoraSDK Official Repository](https://github.com/DrHazemAli/AzureSoraSDK)

## Up next

You've learned how to use the latest Azure OpenAI models for image and video generation in your .NET applications. These capabilities open up new possibilities for creating dynamic, engaging visual content in your applications.

👉 [Return to the main lesson overview](./readme.md) to explore more generative AI techniques.
