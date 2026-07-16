using System.ClientModel;
using FoundryLocal.Samples.Common;
using OpenAI;
using OpenAI.Chat;

var settings = FoundryLocalSampleSupport.LoadSettings();
var selectedScenario = await ResolveScenarioAsync(args);
if (selectedScenario is null)
{
    PrintUsage();
    return 1;
}

var inputText = ResolveInputText(selectedScenario, args);

Console.WriteLine("Foundry Local practical scenarios");
Console.WriteLine($"Scenario: {selectedScenario.Key}");
Console.WriteLine($"Endpoint: {settings.BaseUrl}");

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

ClientResult<ChatCompletion> completion;
try
{
    completion = await chatClient.CompleteChatAsync(
    [
        new SystemChatMessage(selectedScenario.SystemPrompt),
        new UserChatMessage(selectedScenario.BuildUserPrompt(inputText))
    ]);
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

var response = string.Concat(completion.Value.Content.Select(c => c.Text)).Trim();

Console.WriteLine();
Console.WriteLine("Input:");
Console.WriteLine(inputText);
Console.WriteLine();
Console.WriteLine("Output:");
Console.WriteLine(string.IsNullOrWhiteSpace(response) ? "(empty response)" : response);

return 0;

static async Task<ScenarioDefinition?> ResolveScenarioAsync(string[] args)
{
    if (args.Length > 0)
    {
        return ScenarioDefinition.FromToken(args[0]);
    }

    Console.WriteLine("Select a scenario:");
    Console.WriteLine("  1) summarize  - concise summary");
    Console.WriteLine("  2) sentiment  - classify sentiment");
    Console.WriteLine("  3) structured - JSON title+keywords");
    Console.Write("Choice [1-3]: ");

    var choice = (await Console.In.ReadLineAsync())?.Trim();
    if (string.IsNullOrWhiteSpace(choice))
    {
        choice = "1";
    }

    return ScenarioDefinition.FromToken(choice);
}

static string ResolveInputText(ScenarioDefinition scenario, string[] args)
{
    if (args.Length > 1)
    {
        return string.Join(" ", args.Skip(1)).Trim();
    }

    return scenario.DefaultInput;
}

static void PrintUsage()
{
    Console.Error.WriteLine("Usage:");
    Console.Error.WriteLine("  dotnet run -- [summarize|sentiment|structured|1|2|3] [optional input text]");
    Console.Error.WriteLine();
    Console.Error.WriteLine("Examples:");
    Console.Error.WriteLine("  dotnet run -- summarize");
    Console.Error.WriteLine("  dotnet run -- sentiment \"I love how fast this runs locally.\"");
    Console.Error.WriteLine("  dotnet run -- structured \"Local AI keeps sensitive notes on my laptop.\"");
}

internal sealed class ScenarioDefinition(
    string key,
    string systemPrompt,
    string defaultInput,
    Func<string, string> buildUserPrompt)
{
    public string Key { get; } = key;
    public string SystemPrompt { get; } = systemPrompt;
    public string DefaultInput { get; } = defaultInput;
    public Func<string, string> BuildUserPrompt { get; } = buildUserPrompt;

    public static ScenarioDefinition? FromToken(string token)
    {
        var value = token.Trim().ToLowerInvariant();
        return value switch
        {
            "1" or "summarize" => Summarize,
            "2" or "sentiment" => Sentiment,
            "3" or "structured" or "json" => Structured,
            _ => null
        };
    }

    private static ScenarioDefinition Summarize { get; } = new(
        "summarize",
        "You are a deterministic summarizer. Return exactly two short sentences.",
        "Foundry Local lets me run small AI models on-device for fast private prototyping.",
        input => $"Summarize this text in exactly two short sentences:\n{input}");

    private static ScenarioDefinition Sentiment { get; } = new(
        "sentiment",
        "You are a deterministic classifier. Output exactly one line: LABEL | REASON. Labels: Positive, Neutral, Negative.",
        "The setup took a few minutes, and now responses are quick and reliable.",
        input => $"Classify the sentiment for this text:\n{input}");

    private static ScenarioDefinition Structured { get; } = new(
        "structured",
        "Return only strict minified JSON with shape {\"title\":\"...\",\"keywords\":[\"...\",\"...\",\"...\"]}. Use exactly 3 lowercase single-word keywords.",
        "I built a tiny local AI helper to summarize meeting notes and tag action items.",
        input => $"Create structured output for this text:\n{input}");
}
