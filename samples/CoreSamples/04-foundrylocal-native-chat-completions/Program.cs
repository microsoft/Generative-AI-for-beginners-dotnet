using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.Logging.Abstractions;

const string ModelEnvVar = "FOUNDRY_LOCAL_MODEL";
const string NativeModelEnvVar = "FOUNDRY_LOCAL_NATIVE_MODEL";
const string DefaultModelAlias = "qwen2.5-0.5b";

var modelAlias =
    Environment.GetEnvironmentVariable(ModelEnvVar)
    ?? Environment.GetEnvironmentVariable(NativeModelEnvVar)
    ?? DefaultModelAlias;

Console.WriteLine("Foundry Local native chat completions sample");
Console.WriteLine($"Requested model alias: {modelAlias}");

var config = new Configuration
{
    AppName = "dotnet-local-ai-native-chat-completions",
    LogLevel = LogLevel.Information
};

await FoundryLocalManager.CreateAsync(config, NullLogger.Instance);
var manager = FoundryLocalManager.Instance;

var executionProviders = manager.DiscoverEps();
const int maxNameLength = 30;
Console.WriteLine();
Console.WriteLine("Execution providers:");
Console.WriteLine($"  {"Name".PadRight(maxNameLength)}  Registered");
Console.WriteLine($"  {new string('─', maxNameLength)}  ----------");
foreach (var ep in executionProviders)
{
    Console.WriteLine($"  {ep.Name.PadRight(maxNameLength)}  {ep.IsRegistered}");
}

Console.WriteLine();
Console.WriteLine("Downloading/registering execution providers:");
if (executionProviders.Length > 0)
{
    var activeEp = string.Empty;
    await manager.DownloadAndRegisterEpsAsync((epName, percent) =>
    {
        if (!string.Equals(activeEp, epName, StringComparison.Ordinal))
        {
            if (!string.IsNullOrEmpty(activeEp))
            {
                Console.WriteLine();
            }

            activeEp = epName;
        }

        Console.Write($"\r  {epName.PadRight(maxNameLength)}  {percent,6:F1}%");
    });
    Console.WriteLine();
}
else
{
    Console.WriteLine("  No execution providers to download.");
}

var catalog = await manager.GetCatalogAsync();
var model = await catalog.GetModelAsync(modelAlias);
if (model is null)
{
    Console.Error.WriteLine();
    Console.Error.WriteLine($"Model '{modelAlias}' was not found in the local catalog.");
    Console.Error.WriteLine("Optional environment overrides (PowerShell):");
    Console.Error.WriteLine($"  $env:{NativeModelEnvVar}=\"{DefaultModelAlias}\"");
    Console.Error.WriteLine($"  # or: $env:{ModelEnvVar}=\"{DefaultModelAlias}\"");
    manager.Dispose();
    return;
}

Console.WriteLine();
Console.WriteLine($"Resolved model: {model.Alias} ({model.Id})");

await model.DownloadAsync(progress =>
{
    Console.Write($"\rDownloading model: {progress,6:F2}%");
    if (progress >= 100f)
    {
        Console.WriteLine();
    }
});

Console.Write($"Loading model '{model.Id}'...");
await model.LoadAsync();
Console.WriteLine("done.");

try
{
    var chatClient = await model.GetChatClientAsync();
    var messages = new List<ChatMessage>
    {
        new() { Role = "user", Content = "Give one short tip for running local AI models efficiently." }
    };

    Console.WriteLine();
    Console.WriteLine("Streaming chat completion:");
    await foreach (var chunk in chatClient.CompleteChatStreamingAsync(messages, CancellationToken.None))
    {
        if (chunk.Choices is null || chunk.Choices.Count == 0)
        {
            continue;
        }

        var content = chunk.Choices[0].Message?.Content;
        if (!string.IsNullOrEmpty(content))
        {
            Console.Write(content);
            Console.Out.Flush();
        }
    }

    Console.WriteLine();
}
finally
{
    Console.WriteLine();
    Console.WriteLine("Unloading model...");
    await model.UnloadAsync();
    Console.WriteLine("Model unloaded.");
    manager.Dispose();
}
