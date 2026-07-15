using System.ClientModel;
using FoundryLocal.Samples.Common;
using OpenAI;
using OpenAI.Chat;

const string DefaultPromptVariant = "eli5";

var settings = FoundryLocalSampleSupport.LoadSettings();
var variantInput = args.FirstOrDefault()
    ?? Environment.GetEnvironmentVariable("FOUNDRY_LOCAL_PROMPT_VARIANT")
    ?? DefaultPromptVariant;

Console.WriteLine("Foundry Local streaming sample");
Console.WriteLine($"Endpoint: {settings.BaseUrl}");
Console.WriteLine();
Console.WriteLine("Prompt variants:");
Console.WriteLine("  1) eli5    - Explain like I'm 5");
Console.WriteLine("  2) bullets - Three tiny bullets");

var selectedPrompt = ResolvePromptVariant(variantInput);
if (selectedPrompt is null)
{
    Console.Error.WriteLine();
    Console.Error.WriteLine($"Unknown prompt variant '{variantInput}'. Use: eli5 | bullets");
    return 1;
}

Console.WriteLine($"Selected variant: {selectedPrompt.Key}");

var preflight = await FoundryLocalSampleSupport.RunPreflightAsync(settings.BaseUrl, settings.Model);
if (!preflight.Ok)
{
    Console.Error.WriteLine();
    Console.Error.WriteLine(FoundryLocalSampleSupport.BuildPreflightGuidance(preflight.ErrorMessage!, settings));
    return 1;
}

if (!string.Equals(preflight.SelectedModel, settings.Model, StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine($"Configured model '{settings.Model}' was not found. Using '{preflight.SelectedModel}' instead.");
}

var chatClient = new ChatClient(
    preflight.SelectedModel!,
    new ApiKeyCredential(settings.ApiKey),
    new OpenAIClientOptions { Endpoint = new Uri(settings.BaseUrl) });

Console.WriteLine();
Console.WriteLine("Streaming response:");
var firstChunk = true;
try
{
    await foreach (var update in chatClient.CompleteChatStreamingAsync(
    [
        new SystemChatMessage("You are concise and presenter-friendly."),
        new UserChatMessage(selectedPrompt.Prompt)
    ]))
    {
        foreach (var content in update.ContentUpdate)
        {
            if (string.IsNullOrEmpty(content.Text))
            {
                continue;
            }

            if (firstChunk)
            {
                Console.WriteLine();
                firstChunk = false;
            }

            Console.Write(content.Text);
        }
    }
}
catch (ClientResultException ex)
{
    Console.Error.WriteLine();
    Console.Error.WriteLine(
        $"Model invocation failed for '{preflight.SelectedModel}' with HTTP {(int)ex.Status} ({ex.Status}).");
    Console.Error.WriteLine("This usually means the selected model is not chat-capable or is not loaded.");
    Console.Error.WriteLine("Try setting FOUNDRY_LOCAL_MODEL to an installed chat model.");
    return 1;
}

if (firstChunk)
{
    Console.WriteLine();
    Console.WriteLine("(empty response)");
}
else
{
    Console.WriteLine();
}

return 0;

static PromptVariant? ResolvePromptVariant(string input)
{
    var value = input.Trim().ToLowerInvariant();
    return value switch
    {
        "1" or "eli5" => new PromptVariant("eli5", "In one short paragraph, explain what a GPU does like I'm 5."),
        "2" or "bullets" => new PromptVariant("bullets", "Summarize why local AI inference is useful in exactly 3 short bullet points."),
        _ => null
    };
}

internal sealed record PromptVariant(string Key, string Prompt);
