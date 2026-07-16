using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.Logging.Abstractions;

const string WhisperModelEnvVar = "FOUNDRY_LOCAL_WHISPER_MODEL";
const string AudioModelEnvVar = "FOUNDRY_LOCAL_AUDIO_MODEL";
const string ModelEnvVar = "FOUNDRY_LOCAL_MODEL";
const string AudioLanguageEnvVar = "FOUNDRY_LOCAL_AUDIO_LANGUAGE";
const string DefaultModelAlias = "whisper-tiny";
const string DefaultAudioLanguage = "en";

var modelAlias =
    FirstNonEmpty(
        Environment.GetEnvironmentVariable(WhisperModelEnvVar),
        Environment.GetEnvironmentVariable(AudioModelEnvVar),
        Environment.GetEnvironmentVariable(ModelEnvVar))
    ?? DefaultModelAlias;

var sampleDirectory = ResolveSampleDirectory();
var defaultAudioPath = Path.Combine(sampleDirectory, "Recording.mp3");
var audioPath = ResolveAudioPath(args, sampleDirectory, defaultAudioPath);

Console.WriteLine("Foundry Local native audio transcription sample");
Console.WriteLine($"Requested model alias: {modelAlias}");

if (!File.Exists(audioPath))
{
    Console.WriteLine();
    Console.WriteLine($"Audio file not found: {audioPath}");
    Console.WriteLine("Provide a valid .mp3 path or add a default file at:");
    Console.WriteLine($"  {defaultAudioPath}");
    Console.WriteLine("Example:");
    Console.WriteLine(@"  dotnet run -- .\Recording.mp3");
    return 1;
}

var config = new Configuration
{
    AppName = "dotnet-local-ai-audio-transcription",
    LogLevel = LogLevel.Information
};

await FoundryLocalManager.CreateAsync(config, NullLogger.Instance);
var manager = FoundryLocalManager.Instance;
IModel? loadedModel = null;

try
{
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
        Console.Error.WriteLine($"  $env:{WhisperModelEnvVar}=\"{DefaultModelAlias}\"");
        Console.Error.WriteLine($"  $env:{AudioModelEnvVar}=\"{DefaultModelAlias}\"");
        Console.Error.WriteLine($"  # or: $env:{ModelEnvVar}=\"{DefaultModelAlias}\"");
        manager.Dispose();
        return 1;
    }

    Console.WriteLine();
    Console.WriteLine($"Resolved model: {model.Alias} ({model.Id})");

    var cpuVariant = model.Variants.FirstOrDefault(v => v.Info.Runtime?.DeviceType == DeviceType.CPU);
    if (cpuVariant is not null)
    {
        model.SelectVariant(cpuVariant);
        Console.WriteLine($"Selected CPU variant: {cpuVariant.Id}");
    }
    else
    {
        Console.WriteLine("CPU variant not available; using default variant.");
    }

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
    loadedModel = model;
    Console.WriteLine("done.");

    var language = FirstNonEmpty(Environment.GetEnvironmentVariable(AudioLanguageEnvVar)) ?? DefaultAudioLanguage;
    var audioClient = await model.GetAudioClientAsync();
    audioClient.Settings.Language = language;

    Console.WriteLine();
    Console.WriteLine($"Transcribing audio with streaming output: {audioPath}");
    await foreach (var chunk in audioClient.TranscribeAudioStreamingAsync(audioPath, CancellationToken.None))
    {
        if (string.IsNullOrEmpty(chunk.Text))
        {
            continue;
        }

        Console.Write(chunk.Text);
        Console.Out.Flush();
    }

    Console.WriteLine();
}
finally
{
    Console.WriteLine();
    Console.WriteLine("Unloading model...");
    if (loadedModel is not null)
    {
        await loadedModel.UnloadAsync();
    }
    Console.WriteLine("Model unloaded.");
    manager.Dispose();
}

return 0;

static string? FirstNonEmpty(params string?[] values) =>
    values.FirstOrDefault(static value => !string.IsNullOrWhiteSpace(value));

static string ResolveSampleDirectory()
{
    var current = new DirectoryInfo(AppContext.BaseDirectory);
    while (current is not null)
    {
        if (File.Exists(Path.Combine(current.FullName, "FoundryLocal.AudioTranscription.csproj")))
        {
            return current.FullName;
        }

        current = current.Parent;
    }

    return Directory.GetCurrentDirectory();
}

static string ResolveAudioPath(string[] args, string sampleDirectory, string defaultAudioPath)
{
    if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
    {
        return defaultAudioPath;
    }

    var candidate = args[0].Trim();
    if (Path.IsPathRooted(candidate))
    {
        return Path.GetFullPath(candidate);
    }

    var fromWorkingDirectory = Path.GetFullPath(candidate, Directory.GetCurrentDirectory());
    if (File.Exists(fromWorkingDirectory))
    {
        return fromWorkingDirectory;
    }

    return Path.GetFullPath(candidate, sampleDirectory);
}
