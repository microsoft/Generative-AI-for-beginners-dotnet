using ElBruno.MAF.FoundryLocal;
using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

const string ModelEnvVar = "FOUNDRY_LOCAL_MODEL";
const string FallbackModelEnvVar = "FOUNDRY_LOCAL_AGENT_FALLBACK_MODEL";
const string PromptEnvVar = "FOUNDRY_LOCAL_AGENT_PROMPT";
const string CleanupModelEnvVar = "FOUNDRY_LOCAL_CLEANUP_MODEL";
const string DefaultModelAlias = "qwen2.5-0.5b";
const string DefaultFallbackModelAlias = "qwen2.5-0.5b";
const string DefaultPrompt = "I'm in Pacific Standard Time. My bill is 42.50 and I want 18% tip. Use tools and return JSON with keys localTime, tipSummary, fact.";
const int ExpectedToolCalls = 3;

var modelAlias = Environment.GetEnvironmentVariable(ModelEnvVar) ?? DefaultModelAlias;
var fallbackModelAlias = Environment.GetEnvironmentVariable(FallbackModelEnvVar) ?? DefaultFallbackModelAlias;
var prompt = Environment.GetEnvironmentVariable(PromptEnvVar) ?? DefaultPrompt;
var cleanupOverride = ParseCleanupOverride(Environment.GetEnvironmentVariable(CleanupModelEnvVar));
var ct = CancellationToken.None;

Console.WriteLine("Foundry Local agent + tools sample");
Console.WriteLine("Step 1/6: Checking model cache status");
Console.WriteLine($"Model alias: {modelAlias}");
Console.WriteLine($"Fallback model alias: {fallbackModelAlias}");
Console.WriteLine($"Prompt: {prompt}");
var turnResult = await RunAgentTurnAsync(modelAlias, prompt, ct);
if (turnResult.ToolCallsDetected == 0 &&
    !string.Equals(modelAlias, fallbackModelAlias, StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine();
    Console.WriteLine($"No tool calls were detected with '{modelAlias}'.");
    Console.WriteLine($"Try running again with fallback model: $env:{ModelEnvVar}=\"{fallbackModelAlias}\"");
}

modelAlias = turnResult.UsedModelAlias;
if (turnResult.ToolCallsDetected < ExpectedToolCalls)
{
    Console.WriteLine();
    Console.WriteLine($"Detected {turnResult.ToolCallsDetected}/{ExpectedToolCalls} tool calls.");
    Console.WriteLine($"Tip: use a stricter prompt in {PromptEnvVar} to force all tool calls in demo mode.");
}

Console.WriteLine();
Console.WriteLine("Step 6/6: Cleanup");
var shouldCleanup = cleanupOverride ?? AskToDeleteDownloadedModel();
if (shouldCleanup)
{
    Console.WriteLine("Removing model from local cache...");
    var cleanupCatalog = await FoundryLocalManager.Instance.GetCatalogAsync();
    var cleanupModel = await cleanupCatalog.GetModelAsync(modelAlias);
    if (cleanupModel is null)
    {
        Console.WriteLine("Model alias was not found in local catalog for cleanup.");
    }
    else
    {
        await cleanupModel.RemoveFromCacheAsync();
        Console.WriteLine("Model cache removed.");
    }
}
else
{
    Console.WriteLine("Done.");
}

return 0;

static async Task<(string UsedModelAlias, int ToolCallsDetected)> RunAgentTurnAsync(
    string selectedModelAlias,
    string prompt,
    CancellationToken ct)
{
    // Adapter options: local model alias and automatic download behavior.
    var foundryOptions = Options.Create(new FoundryLocalOptions
    {
        ModelAlias = selectedModelAlias,
        DownloadIfMissing = true,
        UnloadOnExit = false
    });

    // Runtime defaults for the underlying local chat client.
    var runtimeOptions = Options.Create(new ChatRuntimeOptions
    {
        Temperature = 0.1,
        MaxOutputTokens = 256,
        Streaming = false
    });

    var lifecycle = new FoundryLocalModelLifecycleService(
        foundryOptions,
        runtimeOptions,
        NullLogger<FoundryLocalModelLifecycleService>.Instance);

    using var adapterClient = new FoundryLocalChatClientAdapter(
        lifecycle,
        NullLogger<FoundryLocalChatClientAdapter>.Instance);

    Console.WriteLine();
    Console.WriteLine("Step 2/6: Preparing local model lifecycle");
    Console.WriteLine($"Active model alias: {selectedModelAlias}");
    await lifecycle.GetChatClientAsync(ct);
    var diagnostics = lifecycle.GetDiagnosticsSnapshot();
    Console.WriteLine(diagnostics.DownloadedThisSession
        ? "Model cache: not present. Downloaded during startup preparation."
        : "Model cache: already available locally.");

    Console.WriteLine();
    Console.WriteLine("Step 3/6: Creating agent-style chat client with tool invocation middleware");

    // Wrap the adapter with MEAI function invocation middleware so the model
    // can call .NET functions defined as AITools.
    var agentClient = adapterClient
        .AsBuilder()
        .UseFunctionInvocation(NullLoggerFactory.Instance, cfg =>
        {
            cfg.MaximumIterationsPerRequest = 8;
            cfg.IncludeDetailedErrors = true;
        })
        .Build();

    Console.WriteLine();
    Console.WriteLine("Step 4/6: Registering tools");
    var tools = AgentSampleTools.BuildTools();
    Console.WriteLine($"Registered tools: {string.Join(", ", tools.Select(t => t.Name))}");

    var options = new ChatOptions
    {
        Instructions = "You are a concise local assistant. You must call the available tools before answering, then return compact JSON only.",
        ToolMode = ChatToolMode.RequireAny,
        Temperature = 0.1f,
        Tools = tools
    };

    Console.WriteLine();
    Console.WriteLine("Step 5/6: Running local agent turn");
    AgentSampleTools.ResetInvocationCounter();
    var response = await agentClient.GetResponseAsync(prompt, options, ct);
    var toolCalls = AgentSampleTools.GetInvocationCount();

    Console.WriteLine();
    Console.WriteLine("Agent response:");
    Console.WriteLine(response.Text);
    Console.WriteLine();
    Console.WriteLine($"Tool calls detected: {toolCalls}");

    await lifecycle.DisposeAsync();

    return (selectedModelAlias, toolCalls);
}

static bool? ParseCleanupOverride(string? value)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        return null;
    }

    if (value.Equals("true", StringComparison.OrdinalIgnoreCase))
    {
        return true;
    }

    if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
    {
        return false;
    }

    return null;
}

static bool AskToDeleteDownloadedModel()
{
    Console.Write("Delete downloaded model? [Y/n] ");
    var answer = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(answer))
    {
        return true;
    }

    return !answer.Equals("n", StringComparison.OrdinalIgnoreCase)
           && !answer.Equals("no", StringComparison.OrdinalIgnoreCase);
}
