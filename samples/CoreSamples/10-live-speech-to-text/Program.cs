// Live speech-to-text sample — local Whisper over ONNX Runtime.
//
// This sample is a "live microphone" sibling to sample 05 (file-based audio
// transcription). Because the NVIDIA nemotron streaming ASR models and Whisper
// are NOT part of the Foundry Local catalog on this machine, the model is
// auto-downloaded from HuggingFace on first run via ElBruno.HuggingFace.Downloader
// (used transitively by ElBruno.Whisper) and inference runs locally with ONNX Runtime.
//
// Microphone capture uses NAudio (Windows-only WaveInEvent). Audio is grouped into
// spoken utterances with a simple silence detector, then each utterance is transcribed.

using System.Threading.Channels;
using ElBruno.HuggingFace;
using ElBruno.Whisper;
using LiveSpeechToText;
using NAudio.Wave;

const int SampleRate = 16000;   // Whisper expects 16 kHz mono
const int BitsPerSample = 16;
const int ChannelCount = 1;

// --- Configuration (environment overridable) ---
var modelAlias = Environment.GetEnvironmentVariable("WHISPER_MODEL");
var language = Environment.GetEnvironmentVariable("WHISPER_LANGUAGE");
var settings = new SegmentationSettings(
    SampleRate: SampleRate,
    BitsPerSample: BitsPerSample,
    Channels: ChannelCount,
    SilenceThreshold: LiveTranscriptionSupport.ReadDouble("WHISPER_SILENCE_THRESHOLD", 500),
    SilenceFlushMs: LiveTranscriptionSupport.ReadInt("WHISPER_SILENCE_MS", 800),
    MaxSegmentSeconds: LiveTranscriptionSupport.ReadInt("WHISPER_MAX_SEGMENT_SECONDS", 20),
    MinSpeechMs: LiveTranscriptionSupport.ReadInt("WHISPER_MIN_SPEECH_MS", 300));

var model = LiveTranscriptionSupport.ResolveModel(modelAlias);

Console.WriteLine("===========================================================");
Console.WriteLine("   Local Live Speech-to-Text (ElBruno.Whisper + NAudio)");
Console.WriteLine("===========================================================");
Console.WriteLine();
Console.WriteLine($"Model:    {model.DisplayName} ({model.Id})");
Console.WriteLine($"Source:   HuggingFace repo '{model.HuggingFaceRepoId}' (auto-download)");
Console.WriteLine($"Language: {(string.IsNullOrWhiteSpace(language) ? (model.IsEnglishOnly ? "en (model default)" : "auto-detect") : language)}");
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

// --- Prepare the model (auto-download from HuggingFace on first run) ---
var options = new WhisperOptions { Model = model };
if (!string.IsNullOrWhiteSpace(language))
{
    options.Language = language;
}

var lastStage = string.Empty;
var downloadProgress = new Progress<DownloadProgress>(p =>
{
    if (p.Stage == DownloadStage.Downloading)
    {
        Console.Write($"\r  Downloading {p.CurrentFile}... {p.PercentComplete,6:F1}%   ");
    }
    else if (!string.Equals(lastStage, p.Stage.ToString(), StringComparison.Ordinal))
    {
        lastStage = p.Stage.ToString();
        Console.WriteLine($"\r  {p.Stage}: {p.Message}".PadRight(60));
    }
});

Console.WriteLine("Preparing model (first run downloads it from HuggingFace; later runs load from cache)...");
using var whisper = await WhisperClient.CreateAsync(options, downloadProgress);
Console.WriteLine();
Console.WriteLine("Model ready.");
Console.WriteLine();

// --- Wire up microphone capture ---
// NAudio's DataAvailable callback is synchronous; enqueue PCM chunks into a bounded
// channel so the callback never blocks, and process them on a dedicated consumer task.
var audioChannel = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(200)
{
    FullMode = BoundedChannelFullMode.DropOldest
});

using var waveIn = new WaveInEvent
{
    WaveFormat = new WaveFormat(SampleRate, BitsPerSample, ChannelCount),
    BufferMilliseconds = 100
};

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

var consumer = Task.Run(() =>
    LiveTranscriptionSupport.ConsumeAsync(audioChannel.Reader, whisper, settings, CancellationToken.None));

Console.WriteLine("===========================================================");
Console.WriteLine("  LIVE TRANSCRIPTION ACTIVE");
Console.WriteLine("  Speak into your microphone.");
Console.WriteLine("  Each spoken phrase is transcribed after a short pause.");
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
    return 1;
}

Console.ReadLine();

waveIn.StopRecording();
audioChannel.Writer.Complete();
await consumer;

Console.WriteLine();
Console.WriteLine("Stopped. Model unloaded.");
return 0;
