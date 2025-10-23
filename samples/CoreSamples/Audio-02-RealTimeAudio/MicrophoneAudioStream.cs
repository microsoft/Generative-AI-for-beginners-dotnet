using NAudio.Wave;

#nullable disable

/// <summary>
/// Uses the NAudio library (https://github.com/naudio/NAudio) to provide a rudimentary abstraction of microphone
/// input as a stream.
/// </summary>
public class MicrophoneAudioStream : Stream, IDisposable
{
    private const int SAMPLES_PER_SECOND = 24000;
    private const int BYTES_PER_SAMPLE = 2;
    private const int CHANNELS = 1;

    // For simplicity, this is configured to use a static 10-second ring buffer.
    private readonly byte[] _buffer = new byte[BYTES_PER_SAMPLE * SAMPLES_PER_SECOND * CHANNELS * 10];
    private readonly object _bufferLock = new();
    private int _bufferReadPos = 0;
    private int _bufferWritePos = 0;

    private readonly WaveInEvent _waveInEvent;

    private MicrophoneAudioStream()
    {
        _waveInEvent = new()
        {
            WaveFormat = new WaveFormat(SAMPLES_PER_SECOND, BYTES_PER_SAMPLE * 8, CHANNELS),
        };
        _waveInEvent.DataAvailable += (_, e) =>
        {
            lock (_bufferLock)
            {
                int bytesToCopy = e.BytesRecorded;
                if (_bufferWritePos + bytesToCopy >= _buffer.Length)
                {
                    int bytesToCopyBeforeWrap = _buffer.Length - _bufferWritePos;
                    Array.Copy(e.Buffer, 0, _buffer, _bufferWritePos, bytesToCopyBeforeWrap);
                    bytesToCopy -= bytesToCopyBeforeWrap;
                    _bufferWritePos = 0;
                }
                Array.Copy(e.Buffer, e.BytesRecorded - bytesToCopy, _buffer, _bufferWritePos, bytesToCopy);
                _bufferWritePos += bytesToCopy;
            }
        };
        _waveInEvent.StartRecording();
    }

    public static MicrophoneAudioStream Start() => new();

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => throw new NotImplementedException();

    public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int totalCount = count;

        int GetBytesAvailable() => _bufferWritePos < _bufferReadPos
            ? _bufferWritePos + (_buffer.Length - _bufferReadPos)
            : _bufferWritePos - _bufferReadPos;

        // For simplicity, we'll block until all requested data is available and not perform partial reads.
        while (GetBytesAvailable() < count)
        {
            Thread.Sleep(100);
        }

        lock (_bufferLock)
        {
            if (_bufferReadPos + count >= _buffer.Length)
            {
                int bytesBeforeWrap = _buffer.Length - _bufferReadPos;
                Array.Copy(
                    sourceArray: _buffer,
                    sourceIndex: _bufferReadPos,
                    destinationArray: buffer,
                    destinationIndex: offset,
                    length: bytesBeforeWrap);
                _bufferReadPos = 0;
                count -= bytesBeforeWrap;
                offset += bytesBeforeWrap;
            }

            Array.Copy(_buffer, _bufferReadPos, buffer, offset, count);
            _bufferReadPos += count;
        }

        return totalCount;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    protected override void Dispose(bool disposing)
    {
        _waveInEvent?.Dispose();
        base.Dispose(disposing);
    }
}