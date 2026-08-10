#:package Microsoft.Extensions.Configuration.UserSecrets@10.0.3
#:package Azure.AI.OpenAI@2.8.0-beta.1
#:package Azure.Identity@1.18.0
#:property UserSecretsId=genai-beginners-dotnet

﻿using Microsoft.Extensions.Configuration;
using System.ClientModel;
using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI.Images;

var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
var configuration = builder.Build();

// Image generation needs an image-capable deployment, not the shared chat deployment
// that the other samples use. AzureOpenAI:Deployment is only a backwards-compatible fallback.
var imageDeployment = configuration["AzureOpenAI:ImageDeployment"];
var legacyDeployment = configuration["AzureOpenAI:Deployment"];

if (string.IsNullOrWhiteSpace(imageDeployment))
{
    Console.WriteLine("""
        Missing configuration: AzureOpenAI:ImageDeployment

        This sample generates images, so it needs an IMAGE-CAPABLE deployment
        (for example: gpt-image-1, gpt-image-1.5 or gpt-image-2).
        A chat model such as gpt-4o-mini or gpt-5-mini will NOT work here.

        1. In the Azure AI Foundry portal (https://ai.azure.com), deploy an image model
           on the same Azure OpenAI resource you use for the other samples.
        2. Then set the deployment name in User Secrets:

           dotnet user-secrets set --id genai-beginners-dotnet "AzureOpenAI:ImageDeployment" "gpt-image-1"

        Setup guide: https://github.com/microsoft/Generative-AI-for-beginners-dotnet/blob/main/01-IntroductionToGenerativeAI/setup-azure-openai.md
        """);

    if (string.IsNullOrWhiteSpace(legacyDeployment))
    {
        return 1;
    }

    Console.WriteLine($"Falling back to the legacy key AzureOpenAI:Deployment ('{legacyDeployment}'). This only works if that deployment is an image model.");
    Console.WriteLine();
}

var model = imageDeployment ?? legacyDeployment!;

var url = configuration["AzureOpenAI:Endpoint"];
if (string.IsNullOrWhiteSpace(url))
{
    Console.WriteLine("""
        Missing configuration: AzureOpenAI:Endpoint

        Set it in User Secrets:

           dotnet user-secrets set --id genai-beginners-dotnet "AzureOpenAI:Endpoint" "https://<your-resource>.openai.azure.com/"

        Setup guide: https://github.com/microsoft/Generative-AI-for-beginners-dotnet/blob/main/01-IntroductionToGenerativeAI/setup-azure-openai.md
        """);
    return 1;
}

AzureOpenAIClient azureClient = new(new Uri(url), new AzureCliCredential());
var client = azureClient.GetImageClient(model);

string prompt = "A kitten playing soccer in the moon. Use a comic style";

// generate an image using the prompt
ImageGenerationOptions options = new()
{
    Size = GeneratedImageSize.W1024xH1024,
    Quality = "auto"
};

GeneratedImage image;
try
{
    image = await client.GenerateImageAsync(prompt, options);
}
catch (ClientResultException ex) when (ex.Status == 400 && ex.Message.Contains("imageGenerations", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine($"""
        The deployment '{model}' cannot generate images.

        Azure rejected the request because this deployment is not an image model
        (it is most likely a chat model such as gpt-5-mini).

        Deploy an image model in the Azure AI Foundry portal (https://ai.azure.com) -
        for example gpt-image-1, gpt-image-1.5 or gpt-image-2 - and point this sample at it:

           dotnet user-secrets set --id genai-beginners-dotnet "AzureOpenAI:ImageDeployment" "gpt-image-1"

        Original service message: {ex.Message}
        """);
    return 1;
}

var imageBytes = image.ImageBytes.ToArray();

// Save the image to a file
string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/genimage{DateTimeOffset.Now.Ticks}.png";
File.WriteAllBytes(path, imageBytes);

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

return 0;
