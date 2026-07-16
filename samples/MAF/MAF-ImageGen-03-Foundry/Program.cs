// MAF-ImageGen-03-Foundry — a Microsoft Agent Framework agent that generates images with GPT-Image-2.
//
// A Microsoft Agent Framework (MAF) AIAgent is given an image
// tool that wraps ElBruno.Text2Image.Foundry's GptImage2Generator (MEAI IImageGenerator over
// Microsoft Foundry / GPT-Image-2). The agent decides when to call the tool, generates the
// image, and saves it as a PNG.
//
// Two building blocks, composed:
//   - IChatClient        (Microsoft.Extensions.AI) — the agent's brain, keyless via `az login`.
//   - IImageGenerator    (ElBruno.Text2Image.Foundry / GPT-Image-2) — the image tool.
//
// To run the sample, set the following user secrets:
//      dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-endpoint>.services.ai.azure.com/"
//      dotnet user-secrets set "AzureOpenAI:Deployment" "gpt-5-mini"        # chat model (keyless)
//      dotnet user-secrets set "AzureOpenAI:ApiKey" "<your-api-key>"        # GPT-Image-2 uses key auth
// Optional (defaults shown):
//      dotnet user-secrets set "AzureOpenAI:ImageDeployment" "gpt-image-2"
// Then sign in for the keyless chat client: az login
//
// Usage:
//      dotnet run                       # uses the built-in default prompt
//      dotnet run -- "a corgi astronaut floating over Mars, comic style"

using System.ComponentModel;
using Azure.AI.OpenAI;
using Azure.Identity;
using ElBruno.Text2Image.Foundry;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var endpoint = config["AzureOpenAI:Endpoint"]
    ?? throw new InvalidOperationException("Set AzureOpenAI:Endpoint in User Secrets.");
var chatDeployment = config["AzureOpenAI:Deployment"] ?? "gpt-5-mini";
var imageApiKey = config["AzureOpenAI:ApiKey"]
    ?? throw new InvalidOperationException("Set AzureOpenAI:ApiKey in User Secrets (GPT-Image-2 uses key auth).");
var imageDeployment = config["AzureOpenAI:ImageDeployment"] ?? "gpt-image-2";
var imageModelName = config["AzureOpenAI:ImageModelName"] ?? "GPT-Image-2";

// --- Image generation building block: IImageGenerator over Microsoft Foundry / GPT-Image-2 ---
// GptImage2Generator builds an AzureOpenAIClient that expects the BARE resource URL (it appends
// "/openai/deployments/{deployment}/images/generations" itself), so strip any "/openai" suffix.
using var httpClient = new HttpClient();
ElBruno.Text2Image.IImageGenerator imageGenerator = new GptImage2Generator(
    endpoint: NormalizeEndpoint(endpoint),
    apiKey: imageApiKey,
    httpClient: httpClient,
    modelName: imageModelName,
    deploymentName: imageDeployment,
    timeoutSeconds: 300);

var outputDir = Path.Combine(AppContext.BaseDirectory, "images");
Directory.CreateDirectory(outputDir);

// Tracks the most recent image saved by the tool, so we can open it after the agent finishes.
string? lastImagePath = null;

// The tool the agent can call: generate an image from a prompt and save it as a PNG.
[Description("Generates an image from a detailed text prompt using GPT-Image-2 and saves it as a PNG. Returns the saved file path.")]
async Task<string> GenerateImage(
    [Description("A vivid, detailed description of the image to generate.")] string prompt)
{
    Console.WriteLine($"\n[tool] Generating image with GPT-Image-2...\n[tool] Prompt: {prompt}\n");
    var result = await imageGenerator.GenerateAsync(prompt, options: null, CancellationToken.None);

    var path = Path.Combine(outputDir, $"image-{DateTime.Now:yyyyMMdd-HHmmss}.png");
    await File.WriteAllBytesAsync(path, result.ImageBytes);
    lastImagePath = path;

    Console.WriteLine($"[tool] Image generated in {result.InferenceTimeMs}ms and saved to:\n[tool] {path}\n");
    return path;
}

// --- The agent: a chat model (keyless) composed with the image tool, via MAF ---
IChatClient chatClient = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(chatDeployment)
    .AsIChatClient();

AIAgent imageAgent = chatClient.AsAIAgent(
    name: "ImageStudio",
    instructions:
        "You are a creative studio agent. When the user asks for an image, call the GenerateImage " +
        "tool with a vivid, detailed prompt, then tell the user the file path where the image was " +
        "saved. Do not ask clarifying questions; infer a good prompt and generate the image.",
    tools: [AIFunctionFactory.Create(GenerateImage)]);

// The request: a CLI argument, or the default built-in prompt.
var request = args.Length > 0
    ? string.Join(' ', args)
    : "Create an incident-response hero image: a calm on-call engineer reviewing a runbook next to a " +
      "rising error-rate line chart crossing a 5% threshold. Flat vector style, blue and teal palette, " +
      "professional, high contrast, no text artifacts.";

Console.WriteLine($"User: {request}\n");
Console.WriteLine("Agent is thinking (it will call the GPT-Image-2 tool as needed)...");

AgentResponse response = await imageAgent.RunAsync(request);
Console.WriteLine($"\nAgent: {response.Text}");

// Open the generated image in the OS default viewer.
if (lastImagePath is not null && File.Exists(lastImagePath))
{
    Console.WriteLine($"\nOpening image: {lastImagePath}");
    OpenFile(lastImagePath);
}

// Strip any "/openai..." suffix so GptImage2Generator gets the bare Foundry resource URL.
static string NormalizeEndpoint(string endpoint)
{
    var trimmed = endpoint.Trim().TrimEnd('/');
    var openAiIndex = trimmed.IndexOf("/openai", StringComparison.OrdinalIgnoreCase);
    return openAiIndex >= 0 ? trimmed[..openAiIndex] : trimmed;
}

// Open a file using the OS default application (cross-platform).
static void OpenFile(string path)
{
    try
    {
        if (OperatingSystem.IsWindows())
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(path) { UseShellExecute = true });
        }
        else if (OperatingSystem.IsMacOS())
        {
            System.Diagnostics.Process.Start("open", path);
        }
        else if (OperatingSystem.IsLinux())
        {
            System.Diagnostics.Process.Start("xdg-open", path);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Could not open the image automatically: {ex.Message}");
    }
}
