using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

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
        var deploymentName = config["deploymentName"] ?? "gpt-5-mini";

        // You will need to set these environment variables or edit the following values.
        var endpoint = config["endpoint"];
        var deployment = config["FLUX_DEPLOYMENT_NAME"];
        var apiVersion = config["FLUX_OPENAI_API_VERSION"];
        var apiKey = config["AZURE_OPENAI_API_KEY"];

        if (!endpoint.EndsWith('/'))
        {
            endpoint += '/';
        }

        var basePath = $"openai/deployments/{deployment}/images";
        var urlParams = $"?api-version={apiVersion}";

        using var client = new HttpClient();

        var generationUrl = $"{endpoint}{basePath}/generations{urlParams}";
        var generationBody = new
        {
            prompt = imageGenerationPrompt, // use provided prompt
            n =1,
            size = "1024x1024",
            quality = "standard",
            output_format = "png",
        };

        using (var genRequest = new HttpRequestMessage(HttpMethod.Post, generationUrl))
        {
            genRequest.Headers.Add("Api-Key", apiKey);
            var json = JsonSerializer.Serialize(generationBody);
            genRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var genResponse = await client.SendAsync(genRequest);
            genResponse.EnsureSuccessStatusCode();
            var genResult = await genResponse.Content.ReadAsStringAsync();

            // Save image(s) and return path of first image
            var savedPath = SaveImageFromGenerationResponse(genResult);
            return savedPath;
        }
    }

    /// <summary>
    /// Parses the JSON image generation response, extracts the first base64 image payload and saves it as a PNG file.
    /// Returns the absolute path to the saved image file.
    /// </summary>
    /// <param name="generationResponseJson">JSON returned by the image generation endpoint.</param>
    /// <returns>Absolute file path of saved PNG image.</returns>
    public static string SaveImageFromGenerationResponse(string generationResponseJson)
    {
        if (string.IsNullOrWhiteSpace(generationResponseJson))
            throw new ArgumentException("Generation response JSON is empty", nameof(generationResponseJson));

        using var doc = JsonDocument.Parse(generationResponseJson);
        var root = doc.RootElement;
        if (!root.TryGetProperty("data", out var dataArray) || dataArray.ValueKind != JsonValueKind.Array || dataArray.GetArrayLength() ==0)
            throw new InvalidOperationException("Image generation response does not contain a 'data' array with any items.");

        var first = dataArray[0];
        if (!first.TryGetProperty("b64_json", out var b64Element))
            throw new InvalidOperationException("Image generation response item does not contain 'b64_json'.");

        var b64 = b64Element.GetString();
        if (string.IsNullOrWhiteSpace(b64))
            throw new InvalidOperationException("'b64_json' value is empty.");

        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(b64);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("Failed to decode base64 image content.", ex);
        }

        var outputDir = Path.Combine(Environment.CurrentDirectory, "generated-images");
        Directory.CreateDirectory(outputDir);

        var fileName = $"image_{DateTime.UtcNow:yyyyMMdd_HHmmssfff}.png";
        var filePath = Path.Combine(outputDir, fileName);
        File.WriteAllBytes(filePath, bytes);
        return filePath;
    }
}
