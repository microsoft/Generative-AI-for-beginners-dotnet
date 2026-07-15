// Foundry Local — LIVE microphone → text transcription (real-time streaming).
//
// This is the native Foundry Local streaming sample (Microsoft Learn parity):
//   https://learn.microsoft.com/azure/foundry-local/how-to/how-to-live-transcribe-audio
//
// Unlike sample 10 (which uses ElBruno.Whisper batch transcription over silence-
// segmented windows), this sample uses Foundry Local's real streaming ASR API:
// the model runs as a live session that accepts raw PCM as you speak and yields
// partial (interim) results in real time plus a final result per utterance.
//
// The speech model (default: nemotron-speech-streaming-en-0.6b) is resolved from
// the Foundry Local catalog and auto-downloaded on first run via Foundry's own
// downloader (model.DownloadAsync). Foundry loads models from its private cache
// with catalog-specific metadata, so the model download must go through Foundry
// itself — it can't be substituted with a raw HuggingFace download.
//
// Microphone capture uses NAudio (Windows-only WaveInEvent) at 16 kHz / 16-bit / mono.

using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.Logging.Abstractions;
using NAudio.Wave;
using System.Threading.Channels;

const string SpeechModelEnvVar = "FOUNDRY_LOCAL_SPEECH_MODEL";
const string SpeechLanguageEnvVar = "FOUNDRY_LOCAL_SPEECH_LANGUAGE";
const string CleanupModelEnvVar = "FOUNDRY_LOCAL_CLEANUP_MODEL";
const string DefaultModelAlias = "nemotron-speech-streaming-en-0.6b";
const string DefaultLanguage = "en";
const int SampleRate = 16000;
const int MaxNameLength = 30;

var modelAlias = FirstNonEmpty(Environment.GetEnvironmentVariable(SpeechModelEnvVar)) ?? DefaultModelAlias;
var language = FirstNonEmpty(Environment.GetEnvironmentVariable(SpeechLanguageEnvVar)) ?? DefaultLanguage;
var cleanupOverride = ParseCleanupOverride(Environment.GetEnvironmentVariable(CleanupModelEnvVar));

Console.WriteLine("===========================================================");
Console.WriteLine("   Foundry Local -- Live Audio Transcription (streaming)");
Console.WriteLine("===========================================================");
Console.WriteLine();
Console.WriteLine($"Requested model alias: {modelAlias}");
Console.WriteLine($"Language:              {language}");
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
    AppName = "dotnet-local-ai-live-transcription",
    LogLevel = LogLevel.Information
};

await FoundryLocalManager.CreateAsync(config, NullLogger.Instance);
var manager = FoundryLocalManager.Instance;
IModel? loadedModel = null;

try
{
    // --- Register execution providers (GPU/CPU acceleration backends) ---
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

    // --- Resolve the streaming speech model from the Foundry Local catalog ---
    var catalog = await manager.GetCatalogAsync();
    var model = await catalog.GetModelAsync(modelAlias);
    if (model is null)
    {
        Console.Error.WriteLine();
        Console.Error.WriteLine($"Model '{modelAlias}' was not found in the Foundry Local catalog.");
        Console.Error.WriteLine("Streaming ASR models include:");
        Console.Error.WriteLine("  nemotron-speech-streaming-en-0.6b   (English, default)");
        Console.Error.WriteLine("  nemotron-speech-streaming-es-0.6b   (Spanish)");
        Console.Error.WriteLine("  nemotron-3.5-asr-streaming-0.6b     (multilingual)");
        Console.Error.WriteLine($"Override with:  $env:{SpeechModelEnvVar}=\"nemotron-speech-streaming-en-0.6b\"");
        manager.Dispose();
        return 1;
    }

    Console.WriteLine($"Resolved model: {model.Alias} ({model.Id})");

    var cpuVariant = model.Variants.FirstOrDefault(v => v.Info.Runtime?.DeviceType == DeviceType.CPU);
    if (cpuVariant is not null)
    {
        model.SelectVariant(cpuVariant);
        Console.WriteLine($"Selected CPU variant: {cpuVariant.Id}");
    }

    // --- Auto-download the model on first run (Foundry's own downloader) ---
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

    // --- Open a live streaming transcription session ---
    var audioClient = await model.GetAudioClientAsync();
    var session = audioClient.CreateLiveTranscriptionSession();
    session.Settings.SampleRate = SampleRate;
    session.Settings.Channels = 1;
    session.Settings.Language = language;

    await session.StartAsync(CancellationToken.None);

    // Consume transcription results: interim (cyan, in-place) + final (new line).
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

    // --- Capture the microphone and stream PCM into the session ---
    // NAudio's DataAvailable callback is synchronous, so enqueue chunks into a
    // bounded channel and await AppendAsync on a dedicated task (respects backpressure).
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
    Console.WriteLine("  LIVE TRANSCRIPTION ACTIVE");
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
