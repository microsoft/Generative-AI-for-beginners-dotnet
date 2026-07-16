using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.Logging.Abstractions;

const string ModelEnvVar = "FOUNDRY_LOCAL_MODEL";
const string PromptEnvVar = "FOUNDRY_LOCAL_PROMPT";
const string CleanupModelEnvVar = "FOUNDRY_LOCAL_CLEANUP_MODEL";
const string DefaultModelAlias = "phi-3.5-mini";
const string DefaultPrompt = "Why is the sky blue?";

var modelAlias = Environment.GetEnvironmentVariable(ModelEnvVar) ?? DefaultModelAlias;
var prompt = Environment.GetEnvironmentVariable(PromptEnvVar) ?? DefaultPrompt;
var cleanupOverride = NativeAutoChatSupport.ParseCleanupOverride(Environment.GetEnvironmentVariable(CleanupModelEnvVar));
var ct = CancellationToken.None;

var config = new Configuration
{
    AppName = "dotnet-local-ai-native-auto-chat",
    LogLevel = LogLevel.Information
};

Console.WriteLine("Foundry Local native auto chat sample");
Console.WriteLine("Step 1/6: Initializing SDK and local runtime...");
Console.WriteLine($"Question: {prompt}");

// Create the Foundry Local runtime manager. This is the entry point for EP discovery,
// model catalog lookup, model download/load, and client creation.
await FoundryLocalManager.CreateAsync(config, NullLogger.Instance);
var manager = FoundryLocalManager.Instance;
IModel? loadedModel = null;

try
{
    var eps = manager.DiscoverEps();
    const int maxNameLength = 30;

    // Show execution providers available on this machine.
    Console.WriteLine();
    Console.WriteLine("Step 2/6: Discovering execution providers");
    Console.WriteLine($"  {"Name".PadRight(maxNameLength)}  Registered");
    Console.WriteLine($"  {new string('─', maxNameLength)}  ----------");
    foreach (var ep in eps)
    {
        Console.WriteLine($"  {ep.Name.PadRight(maxNameLength)}  {ep.IsRegistered}");
    }

    // Ensure providers are downloaded/registered before selecting model variants.
    Console.WriteLine();
    Console.WriteLine("Step 3/6: Downloading/registering execution providers");
    if (eps.Length > 0)
    {
        var completedEps = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        await manager.DownloadAndRegisterEpsAsync((epName, percent) =>
        {
            if (percent >= 100f && completedEps.Add(epName))
            {
                Console.WriteLine($"  {epName.PadRight(maxNameLength)}  100.0%");
            }
        });

        if (completedEps.Count == 0)
        {
            Console.WriteLine("  Execution providers already registered.");
        }
    }
    else
    {
        Console.WriteLine("  No execution providers to download.");
    }

    // Resolve alias -> model and choose the best variant (GPU when available, else CPU).
    Console.WriteLine();
    Console.WriteLine("Step 4/6: Resolving model + best variant for this machine");
    var catalog = await manager.GetCatalogAsync();
    var model = await catalog.GetModelAsync(modelAlias);
    if (model is null)
    {
        Console.Error.WriteLine();
        Console.Error.WriteLine($"Model '{modelAlias}' was not found in the local catalog.");
        Console.Error.WriteLine("Set FOUNDRY_LOCAL_MODEL to an installed alias and try again.");
        return 1;
    }

    Console.WriteLine($"Resolved model alias: {model.Alias}");
    var selectedVariant = NativeAutoChatSupport.SelectBestVariantForMachine(model, manager.DiscoverEps());
    if (selectedVariant is not null)
    {
        model.SelectVariant(selectedVariant);
        Console.WriteLine($"Selected variant: {selectedVariant.Id} ({selectedVariant.Info.Runtime?.DeviceType})");
    }
    else
    {
        Console.WriteLine("Selected variant: SDK default");
    }

    // Download happens only once; subsequent runs use local cache.
    Console.WriteLine();
    Console.WriteLine("Step 5/6: Downloading/loading model");
    var progressDone = false;
    var lastPrintedPercent = -1;
    await model.DownloadAsync(progress =>
    {
        if (progressDone)
        {
            return;
        }

        var percent = (int)Math.Floor(progress);
        if (percent > lastPrintedPercent)
        {
            lastPrintedPercent = percent;
            Console.Write($"\rDownloading model: {progress,6:F2}%");
        }

        if (progress >= 100f && !progressDone)
        {
            progressDone = true;
            Console.WriteLine();
        }
    });

    Console.Write($"Loading model {model.Id}...");
    await model.LoadAsync();
    loadedModel = model;
    Console.WriteLine("done.");

    // Ask one question using the native chat client with a quality guard + retry fallback.
    var chatClient = await model.GetChatClientAsync();
    chatClient.Settings.Temperature = 0.0f;
    chatClient.Settings.TopP = 0.1f;
    chatClient.Settings.MaxTokens = 96;
    chatClient.Settings.RandomSeed = 42;

    Console.WriteLine();
    Console.WriteLine("Step 6/6: Generating answer");
    Console.WriteLine($"Prompt: {prompt}");
    var answer = await NativeAutoChatSupport.AskQuestionWithQualityGuardAsync(chatClient, prompt, ct);
    Console.WriteLine();
    Console.WriteLine("Answer:");
    Console.WriteLine(answer);

    var shouldCleanup = cleanupOverride ?? NativeAutoChatSupport.AskToDeleteDownloadedModel();
    if (shouldCleanup)
    {
        Console.WriteLine();
        Console.WriteLine("Removing model from local cache...");
        await model.RemoveFromCacheAsync();
        Console.WriteLine("Model cache removed.");
    }
}
finally
{
    if (loadedModel is not null)
    {
        Console.WriteLine();
        Console.WriteLine("Unloading model...");
        await loadedModel.UnloadAsync();
        Console.WriteLine("Model unloaded.");
    }

    manager.Dispose();
}

return 0;
