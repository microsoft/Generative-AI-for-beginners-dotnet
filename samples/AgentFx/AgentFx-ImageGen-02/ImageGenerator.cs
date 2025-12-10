using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using OpenAI;
using OpenAI.Images;
using System.ClientModel;

#pragma warning disable OPENAI001

namespace AgentFx_ImageGen_02;

public static class ImageGenerator
{
    [Description("Generates an image from a prompt. Returns the absolute path to the saved image file.")]
    public static async Task<string> GenerateImageFromPrompt(
        [Description("The prompt to generate the image from.")]
        string imageGenerationPrompt) 
    {
        var builder = Host.CreateApplicationBuilder();
        var config = builder.Configuration
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>()
            .Build();

        // You will need to set these environment variables or edit the following values.
        var endpoint = config["endpoint"];
        var deployment = config["FLUX_DEPLOYMENT_NAME"];
        var apiKey = config["AZURE_OPENAI_API_KEY"];

        // Ensure endpoint ends with /openai/v1/
        if (!endpoint.EndsWith("/openai/v1/"))
        {
            if (endpoint.EndsWith('/'))
            {
                endpoint += "openai/v1/";
            }
            else
            {
                endpoint += "/openai/v1/";
            }
        }

        ImageClient client = new(
            credential: new ApiKeyCredential(apiKey),
            model: deployment,
            options: new OpenAIClientOptions()
            {
                Endpoint = new Uri(endpoint),
            }
        );

        ImageGenerationOptions options = new()
        {
            Size = GeneratedImageSize.W1024xH1024,
        };

        GeneratedImage image = await client.GenerateImageAsync(imageGenerationPrompt, options);
        BinaryData bytes = image.ImageBytes;

        var outputDir = Path.Combine(Environment.CurrentDirectory, "generated-images");
        Directory.CreateDirectory(outputDir);

        var fileName = $"image_{DateTime.UtcNow:yyyyMMdd_HHmmssfff}.jpg";
        var filePath = Path.Combine(outputDir, fileName);
        File.WriteAllBytes(filePath, bytes.ToArray());

        return filePath;
    }
}

