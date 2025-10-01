using NAudio.Wave;

/// <summary>
/// Uses the NAudio library (https://github.com/naudio/NAudio) to provide a rudimentary abstraction to output
/// BinaryData audio segments to the default output (speaker/headphone) device.
/// </summary>
public class SpeakerOutput : IDisposable
{
    BufferedWaveProvider _waveProvider;
    WaveOutEvent _waveOutEvent;

    public SpeakerOutput()
    {
        WaveFormat outputAudioFormat = new(
            rate: 24000,
            bits: 16,
            channels: 1);
        _waveProvider = new(outputAudioFormat)
        {
            BufferDuration = TimeSpan.FromMinutes(2),
        };
        _waveOutEvent = new();
        _waveOutEvent.Init(_waveProvider);
        _waveOutEvent.Play();
    }

    public void EnqueueForPlayback(BinaryData audioData)
    {
        if (audioData == null)
        {
            return;
        }
        byte[] buffer = audioData.ToArray();
        _waveProvider.AddSamples(buffer, 0, buffer.Length);
    }

    public void ClearPlayback()
    {
        _waveProvider.ClearBuffer();
    }

    public void Dispose()
    {
        _waveOutEvent?.Dispose();
    }
}