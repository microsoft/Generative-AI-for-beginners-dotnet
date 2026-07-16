using System.Text;
using System.Threading.Channels;
using ElBruno.Whisper;

namespace LiveSpeechToText;

/// <summary>
/// Audio format and segmentation settings for live microphone transcription.
/// Whisper is not a streaming model: it transcribes fixed windows (up to 30s).
/// To feel "live" on the console, we capture microphone PCM, detect the end of a
/// spoken utterance using a simple energy (RMS) based silence detector, and send
/// each completed utterance to <see cref="WhisperClient"/> for transcription.
/// </summary>
internal sealed record SegmentationSettings(
    int SampleRate,
    int BitsPerSample,
    int Channels,
    double SilenceThreshold,
    int SilenceFlushMs,
    int MaxSegmentSeconds,
    int MinSpeechMs);

internal static class LiveTranscriptionSupport
{
    /// <summary>
    /// Resolves a friendly model alias (e.g. "tiny.en", "base", "whisper-small.en",
    /// "large-v3-turbo") to a known Whisper model. Defaults to Whisper Tiny (English).
    /// </summary>
    internal static WhisperModelDefinition ResolveModel(string? alias)
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            return KnownWhisperModels.WhisperTinyEn;
        }

        var normalized = alias.Trim();
        var id = normalized.StartsWith("whisper-", StringComparison.OrdinalIgnoreCase)
            ? normalized
            : $"whisper-{normalized}";

        var model = KnownWhisperModels.FindById(id);
        if (model is null)
        {
            Console.WriteLine($"Unknown model alias '{alias}'. Falling back to '{KnownWhisperModels.WhisperTinyEn.Id}'.");
            Console.WriteLine("Valid aliases: " + string.Join(", ", KnownWhisperModels.All.Select(m => m.Id)));
            return KnownWhisperModels.WhisperTinyEn;
        }

        return model;
    }

    internal static int ReadInt(string envVar, int fallback)
    {
        var raw = Environment.GetEnvironmentVariable(envVar);
        return int.TryParse(raw, out var value) && value > 0 ? value : fallback;
    }

    internal static double ReadDouble(string envVar, double fallback)
    {
        var raw = Environment.GetEnvironmentVariable(envVar);
        return double.TryParse(raw, out var value) && value >= 0 ? value : fallback;
    }

    /// <summary>
    /// Consumes raw PCM chunks from the microphone channel, groups them into spoken
    /// utterances using silence detection, and transcribes each completed utterance.
    /// Runs until the channel is completed (user pressed ENTER), then flushes any
    /// remaining buffered speech.
    /// </summary>
    internal static async Task ConsumeAsync(
        ChannelReader<byte[]> reader,
        WhisperClient whisper,
        SegmentationSettings settings,
        CancellationToken cancellationToken)
    {
        var bytesPerMs = settings.SampleRate * (settings.BitsPerSample / 8.0) * settings.Channels / 1000.0;
        var prerollMs = 200.0; // keep a little audio before speech starts so word onsets survive
        var maxSegmentMs = settings.MaxSegmentSeconds * 1000.0;

        var chunks = new List<byte[]>();
        var bufferedMs = 0.0;
        var speechMs = 0.0;
        var trailingSilenceMs = 0.0;
        var hasSpeech = false;

        void Reset()
        {
            chunks.Clear();
            bufferedMs = 0;
            speechMs = 0;
            trailingSilenceMs = 0;
            hasSpeech = false;
        }

        try
        {
            await foreach (var chunk in reader.ReadAllAsync(cancellationToken))
            {
                var chunkMs = chunk.Length / bytesPerMs;
                var isSpeech = Rms(chunk) >= settings.SilenceThreshold;

                chunks.Add(chunk);
                bufferedMs += chunkMs;

                if (isSpeech)
                {
                    hasSpeech = true;
                    speechMs += chunkMs;
                    trailingSilenceMs = 0;
                }
                else if (hasSpeech)
                {
                    trailingSilenceMs += chunkMs;
                }
                else
                {
                    // Still waiting for the user to start speaking. Drop old leading
                    // silence so the buffer stays small, keeping only a short preroll.
                    while (chunks.Count > 1 && bufferedMs - (chunks[0].Length / bytesPerMs) > prerollMs)
                    {
                        bufferedMs -= chunks[0].Length / bytesPerMs;
                        chunks.RemoveAt(0);
                    }
                }

                var endOfUtterance = hasSpeech && trailingSilenceMs >= settings.SilenceFlushMs;
                var tooLong = bufferedMs >= maxSegmentMs;

                if (endOfUtterance || tooLong)
                {
                    if (hasSpeech && speechMs >= settings.MinSpeechMs)
                    {
                        await FlushAsync(chunks, whisper, settings, cancellationToken);
                    }

                    Reset();
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected on shutdown.
        }

        // Drain: transcribe whatever speech remains in the buffer.
        if (hasSpeech && speechMs >= settings.MinSpeechMs)
        {
            await FlushAsync(chunks, whisper, settings, CancellationToken.None);
        }
    }

    private static async Task FlushAsync(
        List<byte[]> chunks,
        WhisperClient whisper,
        SegmentationSettings settings,
        CancellationToken cancellationToken)
    {
        var pcm = Concat(chunks);
        var wav = BuildWav(pcm, settings.SampleRate, (short)settings.Channels, (short)settings.BitsPerSample);

        using var stream = new MemoryStream(wav, writable: false);
        var result = await whisper.TranscribeAsync(stream, cancellationToken);
        var text = CleanTranscript(result.Text);

        if (!string.IsNullOrEmpty(text))
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {text}");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Root-mean-square amplitude of a little-endian 16-bit mono PCM buffer.
    /// Used as a cheap speech/silence discriminator.
    /// </summary>
    private static double Rms(byte[] pcm16)
    {
        var sampleCount = pcm16.Length / 2;
        if (sampleCount == 0)
        {
            return 0;
        }

        double sumSquares = 0;
        for (var i = 0; i + 1 < pcm16.Length; i += 2)
        {
            var sample = (short)(pcm16[i] | (pcm16[i + 1] << 8));
            sumSquares += (double)sample * sample;
        }

        return Math.Sqrt(sumSquares / sampleCount);
    }

    private static byte[] Concat(List<byte[]> chunks)
    {
        var total = chunks.Sum(c => c.Length);
        var result = new byte[total];
        var offset = 0;
        foreach (var chunk in chunks)
        {
            Buffer.BlockCopy(chunk, 0, result, offset, chunk.Length);
            offset += chunk.Length;
        }

        return result;
    }

    /// <summary>
    /// Wraps raw PCM samples in a minimal 44-byte WAV (RIFF) container so the audio
    /// can be handed to <see cref="WhisperClient.TranscribeAsync(Stream, CancellationToken)"/>,
    /// which expects a WAV stream.
    /// </summary>
    internal static byte[] BuildWav(byte[] pcm, int sampleRate, short channels, short bitsPerSample)
    {
        using var memory = new MemoryStream();
        using var writer = new BinaryWriter(memory, Encoding.ASCII, leaveOpen: true);

        var byteRate = sampleRate * channels * bitsPerSample / 8;
        var blockAlign = (short)(channels * bitsPerSample / 8);

        writer.Write("RIFF"u8.ToArray());
        writer.Write(36 + pcm.Length);
        writer.Write("WAVE"u8.ToArray());
        writer.Write("fmt "u8.ToArray());
        writer.Write(16);                 // PCM fmt chunk size
        writer.Write((short)1);           // audio format = PCM
        writer.Write(channels);
        writer.Write(sampleRate);
        writer.Write(byteRate);
        writer.Write(blockAlign);
        writer.Write(bitsPerSample);
        writer.Write("data"u8.ToArray());
        writer.Write(pcm.Length);
        writer.Write(pcm);
        writer.Flush();

        return memory.ToArray();
    }

    /// <summary>
    /// Normalizes a transcript and drops Whisper's non-speech placeholders
    /// (e.g. "[BLANK_AUDIO]", "(silence)") that appear when a window is mostly quiet.
    /// </summary>
    private static string CleanTranscript(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var trimmed = text.Trim();

        string[] placeholders =
        {
            "[BLANK_AUDIO]", "(silence)", "[silence]", "(no speech)", "[no speech]", "..."
        };

        foreach (var placeholder in placeholders)
        {
            if (trimmed.Equals(placeholder, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }
        }

        return trimmed;
    }
}
