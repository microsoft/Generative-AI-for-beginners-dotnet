# Image Generation with Azure OpenAI

In this lesson, we'll explore how to use Azure OpenAI to generate images using DALL-E in your .NET applications. Image generation allows you to create original images based on text descriptions, opening up creative possibilities for various applications.

---

## Image Generation with Azure OpenAI

[![Image Generation with Azure OpenAI](./images/LIM_GAN_07_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson3-imagegen)

_⬆️Click the image to watch the video⬆️_

Image generation AI allows you to create original images from text descriptions or prompts. Using services like DALL-E through Azure OpenAI, you can specify exactly what you want to see in an image, including style, composition, objects, and more. This can be useful for creating illustrations, concept art, design mockups, and other visual content.

### Generating Images with Azure OpenAI

Let's explore how to generate images using Azure OpenAI in a .NET application:

> 🧑‍💻**Sample code**: [Here is a working example of this application](../samples/CoreSamples/ImageGeneration-01/) you can follow along with.

#### How to run the sample code

To run the sample code, you'll need to:

1. Make sure you have set up a GitHub Codespace with the appropriate environment as described in the [Setup guide](../02-SetupDevEnvironment/readme.md)
2. Ensure you have configured your Azure OpenAI API key and settings as described in the [Azure OpenAI setup guide](../02-SetupDevEnvironment/getting-started-azure-openai.md)
3. Open a terminal in your codespace (Ctrl+` or Cmd+`)
4. Navigate to the sample code directory:
   ```bash
   cd samples/CoreSamples/ImageGeneration-01
   ```
   ```
5. Run the application:
   ```bash
   dotnet run
   ```

#### Setting up the Azure OpenAI Client

First, we need to set up our configuration and create an Azure OpenAI client:

```csharp
var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
var configuration = builder.Build();

var model = configuration["model"];
var url = configuration["api_url"];
var apiKey = configuration["api_key"];

AzureOpenAIClient azureClient = new(new Uri(url), new System.ClientModel.ApiKeyCredential(apiKey));
var client = azureClient.GetImageClient(model);
```

In this code:
1. We load the configuration from user secrets
2. We extract the model name, API URL, and API key from the configuration
3. We create an Azure OpenAI client using the URL and API key
4. We get an image client specifically for the model we want to use

#### Creating a Prompt and Options

Next, we define the prompt that describes the image we want to generate, and set options for the image generation:

```csharp
string prompt = "A kitten playing soccer in the moon. Use a comic style";

// generate an image using the prompt
ImageGenerationOptions options = new()
{
    Size = GeneratedImageSize.W1024xH1024,
    Quality = "medium"
};
```

The prompt is a text description of the image we want to generate. The options specify:
- The size of the image (1024x1024 pixels in this case)
- The quality of the image (medium in this case)

#### Generating and Saving the Image

With our client, prompt, and options set up, we can generate the image:

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

#### Opening the Generated Image

Finally, we open the generated image in the default image viewer for the user's operating system:

```csharp
// open the image in the default viewer
if (OperatingSystem.IsWindows())
{
    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(path) { UseShellExecute = true });
}
else if (OperatingSystem.IsLinux())
{
    System.Diagnostics.Process.Start("xdg-open", path);
}
else if (OperatingSystem.IsMacOS())
{
    System.Diagnostics.Process.Start("open", path);
}
else
{
    Console.WriteLine("Unsupported OS");
}
```

This code checks the operating system and opens the image using the appropriate command for Windows, Linux, or macOS.

> 🗒️**Note:** The image generation capabilities may vary depending on the model you're using. Azure OpenAI offers different model versions with varying capabilities for image generation.

> 🙋 **Need help?**: If you encounter any issues running this example, [open an issue in the repository](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new?template=Blank+issue) and we'll help you troubleshoot.

## Additional Options for Image Generation

When generating images with Azure OpenAI, you can customize several parameters to get the results you want:

- **Size**: Control the dimensions of the generated image (e.g., 1024x1024, 1024x1792, 1792x1024)
- **Quality**: Choose between "standard" and "hd" quality levels
- **Style**: Select between "natural" and "vivid" styles
- **Number**: Generate multiple images in a single request

You can find more details about these options in the [official documentation](https://learn.microsoft.com/azure/ai-services/openai/how-to/dall-e?tabs=gpt-image-1).

## Summary

In this lesson, we learned how to use Azure OpenAI to generate images from text descriptions. We saw how to:

1. Set up an Azure OpenAI client for image generation
2. Create a prompt and options for the image
3. Generate an image based on the prompt
4. Save the generated image to a file
5. Open the image in the default viewer

Image generation opens up new creative possibilities for your applications, allowing you to create visual content on demand based on text descriptions.

## Additional resources

- [Generate images with AI and .NET](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-openai-generate-images?tabs=azd&pivots=openai)
- [DALL-E on Azure AI services](https://learn.microsoft.com/azure/ai-services/openai/how-to/dall-e?tabs=gpt-image-1)
- [Image generation overview](https://learn.microsoft.com/azure/ai-services/openai/concepts/models)

## Up next

You've learned how to generate images with Azure OpenAI in your .NET applications. In the next lesson, you'll learn how to run AI models locally on your machine.

👉 [Running models locally with AI Toolkit, Docker, and Foundry Local](./06-LocalModelRunners.md)
