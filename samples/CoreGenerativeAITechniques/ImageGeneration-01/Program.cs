using Microsoft.Extensions.Configuration;
using Azure.AI.OpenAI;
using OpenAI.Images;

var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
var configuration = builder.Build();

var model = configuration["model"];
var url = configuration["api_url"];
var apiKey = configuration["api_key"];

AzureOpenAIClient azureClient = new(new Uri(url), new System.ClientModel.ApiKeyCredential(apiKey));
var client = azureClient.GetImageClient(model);

string prompt = "A kitten playing soccer in the moon. Use a comic style";

// generate an image using the prompt
ImageGenerationOptions options = new()
{
    Size = GeneratedImageSize.W1024xH1024,
    Quality = "medium"
};
GeneratedImage image = await client.GenerateImageAsync(prompt, options);

// Save the image to a file
string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/genimage{DateTimeOffset.Now.Ticks}.png";
File.WriteAllBytes(path, image.ImageBytes.ToArray());

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