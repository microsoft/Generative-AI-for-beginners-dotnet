// Foundry Local multilingual live microphone transcription.
//
// This sample uses Foundry Local's multilingual streaming ASR model. It defaults
// to automatic language detection and transcribes speech in its source language.
// It does not translate speech into another language.
//
// Microphone capture uses NAudio (Windows-only WaveInEvent) at 16 kHz / 16-bit / mono.

using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.Logging.Abstractions;
using NAudio.Wave;
using System.Globalization;
using System.Threading.Channels;

const string SpeechModelEnvVar = "FOUNDRY_LOCAL_SPEECH_MODEL";
const string SpeechLanguageEnvVar = "FOUNDRY_LOCAL_SPEECH_LANGUAGE";
const string CleanupModelEnvVar = "FOUNDRY_LOCAL_CLEANUP_MODEL";
const string DefaultModelAlias = "nemotron-3.5-asr-streaming-0.6b";
const string QualifiedModelName = "nvidia-nemotron-3.5-asr-streaming-multilingual-0.6b";
const int FirstExpectedStableGpuVariantVersion = 3;
const int SampleRate = 16000;
const int MaxNameLength = 30;

var modelAlias = FirstNonEmpty(Environment.GetEnvironmentVariable(SpeechModelEnvVar)) ?? DefaultModelAlias;
var languageOverride = FirstNonEmpty(Environment.GetEnvironmentVariable(SpeechLanguageEnvVar));
var cleanupOverride = ParseCleanupOverride(Environment.GetEnvironmentVariable(CleanupModelEnvVar));

Console.WriteLine("===========================================================");
Console.WriteLine(" Foundry Local -- Multilingual Live Transcription");
Console.WriteLine("===========================================================");
Console.WriteLine();

string language;
if (languageOverride is null)
{
    language = PromptForLanguage();
}
else if (!TryNormalizeLanguage(languageOverride, out language))
{
    Console.Error.WriteLine(
        $"Environment variable {SpeechLanguageEnvVar} must be 'auto' or a valid locale such as en, es, it, or zh-CN.");
    return 1;
}

Console.WriteLine();
Console.WriteLine($"Requested model alias: {modelAlias}");
Console.WriteLine($"Language:              {DescribeLanguage(language)}");
Console.WriteLine("Mode:                  transcription (no translation)");
Console.WriteLine();

if (!OperatingSystem.IsWindows())
{
    Console.Error.WriteLine("Microphone capture uses NAudio's WaveInEvent, which is Windows-only.");
    Console.Error.WriteLine("Run this sample on Windows, or adapt it to another capture library.");
    return 1;
}

if (WaveInEvent.DeviceCount == 0)
{
    Console.Error.WriteLine("No microphone/input device was detected. Connect a microphone and try again.");
    return 1;
}

var config = new Configuration
{
    AppName = "dotnet-local-ai-multilingual-live-transcription",
    LogLevel = LogLevel.Information
};

await FoundryLocalManager.CreateAsync(config, NullLogger.Instance);
var manager = FoundryLocalManager.Instance;
IModel? loadedModel = null;

try
{
    var executionProviders = manager.DiscoverEps();
    if (executionProviders.Length > 0)
    {
        Console.WriteLine("Downloading/registering execution providers:");
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

            Console.Write($"\r  {epName.PadRight(MaxNameLength)}  {percent,6:F1}%");
        });
        Console.WriteLine();
        Console.WriteLine();
    }

    var catalog = await manager.GetCatalogAsync();
    IModel? model = null;
    foreach (var candidateAlias in GetCompatibleModelAliases(modelAlias))
    {
        model = await catalog.GetModelAsync(candidateAlias);
        if (model is not null)
        {
            if (!string.Equals(candidateAlias, modelAlias, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Catalog alias '{modelAlias}' was not found; using compatible alias '{candidateAlias}'.");
            }

            break;
        }
    }

    if (model is null)
    {
        Console.Error.WriteLine();
        Console.Error.WriteLine($"Model '{modelAlias}' was not found in the Foundry Local catalog.");
        Console.Error.WriteLine("Known names for the multilingual streaming model:");
        Console.Error.WriteLine($"  {DefaultModelAlias} (Foundry Local SDK catalog alias)");
        Console.Error.WriteLine($"  {QualifiedModelName} (qualified model name)");
        Console.Error.WriteLine($"Override with: $env:{SpeechModelEnvVar}=\"{DefaultModelAlias}\"");
        return 1;
    }

    Console.WriteLine($"Resolved model: {model.Alias} ({model.Id})");

    var selectedVariant = SelectBestStableVariant(model, out var gpuFallbackReason);
    if (gpuFallbackReason is not null)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"GPU variant skipped: {gpuFallbackReason}");
        Console.WriteLine("Using CPU until a corrected GPU model version is available.");
        Console.ResetColor();
    }

    if (selectedVariant is not null)
    {
        model.SelectVariant(selectedVariant);
        Console.WriteLine($"Selected variant: {selectedVariant.Id} ({selectedVariant.Info.Runtime?.DeviceType})");
    }

    Console.WriteLine("Ensuring model is available locally (downloads automatically when not cached)...");
    await model.DownloadAsync(progress =>
    {
        Console.Write($"\rDownloading model: {progress,6:F2}%");
        if (progress >= 100f)
        {
            Console.WriteLine();
        }
    });
    Console.WriteLine("Model is available locally.");

    Console.Write($"Loading model '{model.Id}'...");
    await model.LoadAsync();
    loadedModel = model;
    Console.WriteLine("done.");

    var audioClient = await model.GetAudioClientAsync();
    var session = audioClient.CreateLiveTranscriptionSession();
    session.Settings.SampleRate = SampleRate;
    session.Settings.Channels = 1;
    session.Settings.Language = language;

    await session.StartAsync(CancellationToken.None);

    var readTask = Task.Run(async () =>
    {
        try
        {
            await foreach (var result in session.GetStream(CancellationToken.None))
            {
                var text = result.Content?[0]?.Text;
                if (result.IsFinal)
                {
                    Console.WriteLine();
                    Console.WriteLine($"  [FINAL] {text}");
                    Console.Out.Flush();
                }
                else if (!string.IsNullOrEmpty(text))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(text);
                    Console.ResetColor();
                    Console.Out.Flush();
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    });

    using var waveIn = new WaveInEvent
    {
        WaveFormat = new WaveFormat(SampleRate, 16, 1),
        BufferMilliseconds = 100
    };

    var audioChannel = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(50)
    {
        FullMode = BoundedChannelFullMode.DropOldest
    });

    var appendTask = Task.Run(async () =>
    {
        await foreach (var chunk in audioChannel.Reader.ReadAllAsync())
        {
            await session.AppendAsync(chunk);
        }
    });

    waveIn.DataAvailable += (_, e) =>
    {
        if (e.BytesRecorded <= 0)
        {
            return;
        }

        var buffer = new byte[e.BytesRecorded];
        Buffer.BlockCopy(e.Buffer, 0, buffer, 0, e.BytesRecorded);
        audioChannel.Writer.TryWrite(buffer);
    };

    Console.WriteLine();
    Console.WriteLine("===========================================================");
    Console.WriteLine("  MULTILINGUAL TRANSCRIPTION ACTIVE");
    Console.WriteLine($"  Language: {DescribeLanguage(language)}");
    Console.WriteLine("  Speak into your microphone.");
    Console.WriteLine("  Interim results appear in real-time (cyan);");
    Console.WriteLine("  finalized phrases are printed as [FINAL].");
    Console.WriteLine("  Press ENTER to stop.");
    Console.WriteLine("===========================================================");
    Console.WriteLine();

    try
    {
        waveIn.StartRecording();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Failed to start microphone capture: {ex.Message}");
        audioChannel.Writer.Complete();
        return 1;
    }

    Console.ReadLine();

    waveIn.StopRecording();
    audioChannel.Writer.Complete();
    await appendTask;

    await session.StopAsync(CancellationToken.None);
    await readTask;
}
finally
{
    Console.WriteLine();
    Console.WriteLine("Unloading model...");
    if (loadedModel is not null)
    {
        await loadedModel.UnloadAsync();
        Console.WriteLine("Model unloaded.");

        var shouldCleanup = cleanupOverride ?? AskToDeleteDownloadedModel();
        if (shouldCleanup)
        {
            Console.WriteLine();
            Console.WriteLine("Removing model from local cache...");
            await loadedModel.RemoveFromCacheAsync();
            Console.WriteLine("Model cache removed.");
        }
    }
    else
    {
        Console.WriteLine("Model unloaded.");
    }

    manager.Dispose();
}

return 0;

static string PromptForLanguage()
{
    while (true)
    {
        Console.WriteLine("Choose the spoken language:");
        Console.WriteLine("  1. Auto-detect (auto, default)");
        Console.WriteLine("  2. English (en)");
        Console.WriteLine("  3. Spanish (es)");
        Console.WriteLine("  4. Italian (it)");
        Console.WriteLine("  5. More languages (enter a locale)");
        Console.Write("Selection [1]: ");

        switch (Console.ReadLine()?.Trim())
        {
            case null or "" or "1":
                return "auto";
            case "2":
                return "en";
            case "3":
                return "es";
            case "4":
                return "it";
            case "5":
                Console.Write("Locale (for example de, fr, or zh-CN): ");
                var locale = Console.ReadLine()?.Trim();
                if (TryNormalizeLanguage(locale, out var normalizedLocale))
                {
                    return normalizedLocale;
                }

                Console.Error.WriteLine("Enter a valid locale supported by the model.");
                Console.WriteLine();
                break;
            default:
                Console.Error.WriteLine("Choose a number from 1 to 5.");
                Console.WriteLine();
                break;
        }
    }
}

static bool TryNormalizeLanguage(string? value, out string language)
{
    language = string.Empty;
    if (string.IsNullOrWhiteSpace(value))
    {
        return false;
    }

    if (value.Equals("auto", StringComparison.OrdinalIgnoreCase))
    {
        language = "auto";
        return true;
    }

    try
    {
        language = CultureInfo.GetCultureInfo(value).Name;
        return !string.IsNullOrEmpty(language);
    }
    catch (CultureNotFoundException)
    {
        return false;
    }
}

static string DescribeLanguage(string language)
{
    if (language.Equals("auto", StringComparison.OrdinalIgnoreCase))
    {
        return "Auto-detect (auto)";
    }

    var culture = CultureInfo.GetCultureInfo(language);
    return $"{culture.EnglishName} ({culture.Name})";
}

static IEnumerable<string> GetCompatibleModelAliases(string requestedAlias)
{
    yield return requestedAlias;

    if (requestedAlias.Equals(QualifiedModelName, StringComparison.OrdinalIgnoreCase))
    {
        yield return DefaultModelAlias;
    }
    else if (requestedAlias.Equals(DefaultModelAlias, StringComparison.OrdinalIgnoreCase))
    {
        yield return QualifiedModelName;
    }
}

static IModel? SelectBestStableVariant(IModel model, out string? gpuFallbackReason)
{
    gpuFallbackReason = null;
    var variants = model.Variants.ToList();
    var gpuVariant = variants.FirstOrDefault(
        static variant => variant.Info.Runtime?.DeviceType == DeviceType.GPU);

    if (gpuVariant is not null
        && TryGetVariantVersion(gpuVariant.Id, out var gpuVersion)
        && gpuVersion < FirstExpectedStableGpuVariantVersion)
    {
        gpuFallbackReason =
            $"{gpuVariant.Id} has a known CUDA VAD crash when the first audio chunk is processed.";
        return variants.FirstOrDefault(
                   static variant => variant.Info.Runtime?.DeviceType == DeviceType.CPU)
               ?? gpuVariant;
    }

    return gpuVariant
           ?? variants.FirstOrDefault(
               static variant => variant.Info.Runtime?.DeviceType == DeviceType.CPU)
           ?? variants.FirstOrDefault();
}

static bool TryGetVariantVersion(string variantId, out int version)
{
    version = 0;
    var separatorIndex = variantId.LastIndexOf(':');
    return separatorIndex >= 0
           && int.TryParse(
               variantId.AsSpan(separatorIndex + 1),
               NumberStyles.None,
               CultureInfo.InvariantCulture,
               out version);
}

static string? FirstNonEmpty(params string?[] values) =>
    values.FirstOrDefault(static value => !string.IsNullOrWhiteSpace(value));

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
    Console.WriteLine();
    Console.Write("Delete downloaded model? [Y/n] ");
    var answer = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(answer))
    {
        return true;
    }

    return !answer.Equals("n", StringComparison.OrdinalIgnoreCase)
           && !answer.Equals("no", StringComparison.OrdinalIgnoreCase);
}
