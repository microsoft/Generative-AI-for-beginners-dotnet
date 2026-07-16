# 10-live-speech-to-text

Live **microphone → text** transcription that runs fully locally. This is the "live" sibling of
[sample 05](../05-foundrylocal-audio-transcription) (which transcribes an audio *file*).

Like [sample 06](../06-foundrylocal-native-auto-chat), the model is **downloaded automatically on
first run** — no manual model preparation. The download uses
[`ElBruno.HuggingFace.Downloader`](https://www.nuget.org/packages/ElBruno.HuggingFace.Downloader)
(pulled in transitively by [`ElBruno.Whisper`](https://www.nuget.org/packages/ElBruno.Whisper)).

## ElBruno.Whisper vs. Foundry Local streaming (sample 11)

This sample downloads an ONNX **Whisper** model from HuggingFace (via `ElBruno.HuggingFace.Downloader`)
and runs inference locally with ONNX Runtime. It transcribes each spoken *utterance* in a batch after
a short pause — a simple, dependency-light approach that doesn't require the Foundry Local runtime.

If you want **true real-time streaming** (interim + final results as you speak) through Foundry
Local's native ASR API (`nemotron-speech-streaming-en-0.6b`), see
[sample 11](../11-foundrylocal-live-transcription) — the Microsoft Learn parity sample. That path
uses Foundry Local's own model download; this one uses ElBruno/HuggingFace and no Foundry runtime.

Everything runs on-device: **no cloud, no API keys.**

## How it works

1. **Auto-download** — `WhisperClient.CreateAsync` downloads the model from HuggingFace on first run
   (~75 MB for `whisper-tiny.en`) and caches it. Later runs load instantly from cache.
2. **Capture** — [NAudio](https://www.nuget.org/packages/NAudio)'s `WaveInEvent` records the
   microphone at 16 kHz / 16-bit / mono (Whisper's expected format).
3. **Segment** — Whisper is not a streaming model, so raw PCM is grouped into spoken *utterances*
   using a simple silence detector (RMS energy). An utterance is flushed when a pause is detected or
   the max-segment length is reached.
4. **Transcribe** — each utterance is wrapped in a WAV stream and passed to `TranscribeAsync`. The
   recognized text is printed with a timestamp.

## Prerequisites

- .NET SDK 8.0+ (this project targets `net8.0-windows10.0.18362`)
- **Windows** — microphone capture uses NAudio's `WaveInEvent`, which is Windows-only
- A working microphone / input device
- Internet access on the **first** run (to download the model)

## Configuration

All optional, via environment variables:

| Variable | Default | Description |
|---|---|---|
| `WHISPER_MODEL` | `tiny.en` | Model alias — see table below |
| `WHISPER_LANGUAGE` | model default | Language hint (e.g. `en`, `es`). Leave unset for English-only models. |
| `WHISPER_SILENCE_THRESHOLD` | `500` | RMS energy below which audio is treated as silence |
| `WHISPER_SILENCE_MS` | `800` | Pause length (ms) that ends an utterance |
| `WHISPER_MAX_SEGMENT_SECONDS` | `20` | Force-flush a long utterance after this many seconds (keep < 30) |
| `WHISPER_MIN_SPEECH_MS` | `300` | Utterances shorter than this are ignored as noise |

### Available models

Set `WHISPER_MODEL` to any alias below (larger = more accurate, slower, bigger download):

| Alias | English-only | Approx size |
|---|---|---|
| `tiny.en` (default) | yes | ~75 MB |
| `tiny` | no | ~75 MB |
| `base.en` | yes | ~145 MB |
| `base` | no | ~145 MB |
| `small.en` | yes | ~470 MB |
| `small` | no | ~470 MB |
| `medium.en` | yes | ~1.5 GB |
| `medium` | no | ~1.5 GB |
| `large-v3` | no | ~3 GB |
| `large-v3-turbo` | no | ~1.6 GB |

## Run

```powershell
cd samples\10-live-speech-to-text
# optional overrides
$env:WHISPER_MODEL="tiny.en"
$env:WHISPER_LANGUAGE="en"
dotnet run
```

Then **speak into your microphone**. Each phrase is transcribed after a short pause. Press **ENTER**
to stop.

## Expected output

```
===========================================================
   Local Live Speech-to-Text (ElBruno.Whisper + NAudio)
===========================================================

Model:    Whisper Tiny (English) (whisper-tiny.en)
Source:   HuggingFace repo 'onnx-community/whisper-tiny.en' (auto-download)
Language: en (model default)

Preparing model (first run downloads it from HuggingFace; later runs load from cache)...
  Downloading onnx/encoder_model.onnx...  100.0%
Model ready.

===========================================================
  LIVE TRANSCRIPTION ACTIVE
  Speak into your microphone.
  Each spoken phrase is transcribed after a short pause.
  Press ENTER to stop.
===========================================================

[14:32:07] hello this is a live transcription test
[14:32:11] it runs completely on my machine

Stopped. Model unloaded.
```

## Notes

- **First run downloads the model** (cached at `%LOCALAPPDATA%\ElBruno\Whisper\models\`); subsequent
  runs are offline and instant to start.
- Transcription runs inline per utterance. If speech arrives faster than it can be transcribed, the
  bounded capture buffer drops the oldest audio (`DropOldest`) — acceptable for a live demo.
- Whisper occasionally emits placeholders like `[BLANK_AUDIO]` on near-silent windows; these are
  filtered out before printing.
