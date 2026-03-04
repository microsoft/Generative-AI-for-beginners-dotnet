#:package Microsoft.Extensions.Configuration.UserSecrets@10.0.3
#:package Azure.AI.OpenAI@2.8.0-beta.1
#:package Azure.Identity@1.18.0
#:property UserSecretsId=genai-beginners-dotnet

﻿using Microsoft.Extensions.Configuration;
using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI.Images;

var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
var configuration = builder.Build();

var model = configuration["AzureOpenAI:Deployment"]
    ?? throw new InvalidOperationException("Set AzureOpenAI:Deployment in User Secrets. See: https://github.com/microsoft/Generative-AI-for-beginners-dotnet/blob/main/01-IntroductionToGenerativeAI/setup-azure-openai.md");
var url = configuration["AzureOpenAI:Endpoint"]
    ?? throw new InvalidOperationException("Set AzureOpenAI:Endpoint in User Secrets. See: https://github.com/microsoft/Generative-AI-for-beginners-dotnet/blob/main/01-IntroductionToGenerativeAI/setup-azure-openai.md");

AzureOpenAIClient azureClient = new(new Uri(url), new AzureCliCredential());
var client = azureClient.GetImageClient(model);

string prompt = "A kitten playing soccer in the moon. Use a comic style";

// generate an image using the prompt
ImageGenerationOptions options = new()
{
    Size = GeneratedImageSize.W1024xH1024,
    Quality = "standard"
};
GeneratedImage image = await client.GenerateImageAsync(prompt, options);

// Download the image bytes from the URL
using var httpClient = new HttpClient();
var imageBytes = await httpClient.GetByteArrayAsync(image.ImageUri);

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
