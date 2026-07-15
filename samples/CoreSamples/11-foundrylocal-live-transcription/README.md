# 11-foundrylocal-live-transcription

Foundry Local **native streaming** sample — real-time **microphone → text** transcription.
This is the Microsoft Learn parity sample for
[live transcribe audio](https://learn.microsoft.com/en-us/azure/foundry-local/how-to/how-to-live-transcribe-audio?tabs=windows&pivots=programming-language-csharp).

## How this differs from sample 10

| | Sample 10 (`10-live-speech-to-text`) | Sample 11 (this) |
|---|---|---|
| Runtime | ElBruno.Whisper + ONNX Runtime | **Foundry Local** streaming ASR |
| Model | Whisper (HuggingFace) | **nemotron-speech-streaming-en-0.6b** (Foundry catalog) |
| Download | ElBruno.HuggingFace.Downloader | Foundry Local's own downloader |
| Style | batch: transcribe each utterance after a pause | **true streaming**: interim + final results as you speak |

Both are fully local (no cloud). Sample 11 uses Foundry Local's real streaming session
(`CreateLiveTranscriptionSession()`), which emits **interim** results in real time plus a **final**
result per utterance.

> **Why not download the model with ElBruno here?** Foundry Local loads models from its own private
> cache using catalog-specific metadata and file layout. The streaming session requires a model
> obtained from `catalog.GetModelAsync(...)` and downloaded via `model.DownloadAsync()` — there is no
> API to hand Foundry a model that was downloaded separately from HuggingFace. So Foundry Local
> downloads the model itself (automatically, on first run). For the ElBruno/HuggingFace download
> route, see sample 10.

## Prerequisites

- .NET SDK 8.0+ (this project targets `net8.0-windows10.0.18362`)
- **Windows** — microphone capture uses NAudio's `WaveInEvent`, which is Windows-only
- Foundry Local installed and available on this machine
- A working microphone / input device
- Internet access on the **first** run (to download the model)

> The streaming ASR models are surfaced by the Foundry Local **SDK** (`Microsoft.AI.Foundry.Local.WinML`
> 1.2.3). An older `foundry` CLI may not list them via `foundry model list`, but the SDK catalog
> resolves them.

## Configuration

Optional environment variables:

| Variable | Default | Description |
|---|---|---|
| `FOUNDRY_LOCAL_SPEECH_MODEL` | `nemotron-speech-streaming-en-0.6b` | Streaming ASR model alias |
| `FOUNDRY_LOCAL_SPEECH_LANGUAGE` | `en` | Language (`en`, `es`, `de`, `zh-CN`, … or `auto` for multilingual models) |
| `FOUNDRY_LOCAL_CLEANUP_MODEL` | *(prompt)* | `true`/`false` — non-interactive override for the end-of-run "delete model" prompt |

### Deleting the downloaded model

At the end of the session (after the model is unloaded), the sample asks:

```text
Delete downloaded model? [Y/n]
```

The default is **Yes** — pressing ENTER removes the model from the Foundry Local cache
(`model.RemoveFromCacheAsync()`), so it re-downloads on the next run. Answer `n` to keep it cached.
For non-interactive runs, set `FOUNDRY_LOCAL_CLEANUP_MODEL=true` (always delete) or `false` (always keep).

### Available streaming ASR models (Foundry Local catalog)

| Alias | Notes |
|---|---|
| `nemotron-speech-streaming-en-0.6b` (default) | English |
| `nemotron-speech-streaming-es-0.6b` | Spanish |
| `nemotron-3.5-asr-streaming-0.6b` | Multilingual (use `FOUNDRY_LOCAL_SPEECH_LANGUAGE=auto`) |

## Run

```powershell
cd samples\11-foundrylocal-live-transcription
# optional overrides (defaults shown)
$env:FOUNDRY_LOCAL_SPEECH_MODEL="nemotron-speech-streaming-en-0.6b"
$env:FOUNDRY_LOCAL_SPEECH_LANGUAGE="en"
dotnet run
```

Then **speak into your microphone**. Interim words stream in cyan; each finalized phrase prints as
`[FINAL]`. Press **ENTER** to stop.

## Expected output

```
===========================================================
   Foundry Local -- Live Audio Transcription (streaming)
===========================================================

Requested model alias: nemotron-speech-streaming-en-0.6b
Language:              en

Downloading/registering execution providers:
  CUDAExecutionProvider              100.0%

Resolved model: nemotron-speech-streaming-en-0.6b (nemotron-speech-streaming-en-0.6b-generic-cpu:3)
Selected CPU variant: nemotron-speech-streaming-en-0.6b-generic-cpu:3
Downloading model: 100.00%
Loading model 'nemotron-speech-streaming-en-0.6b-generic-cpu:3'...done.

===========================================================
  LIVE TRANSCRIPTION ACTIVE
  Speak into your microphone.
  Interim results appear in real-time (cyan);
  finalized phrases are printed as [FINAL].
  Press ENTER to stop.
===========================================================

hello this is a live transcription test
  [FINAL] Hello, this is a live transcription test.

Unloading model...
Model unloaded.

Delete downloaded model? [Y/n]
Removing model from local cache...
Model cache removed.
```

## Notes

- **First run downloads the model + execution providers**; later runs load from the Foundry Local
  cache and start quickly.
- Audio is captured at 16 kHz / 16-bit / mono — the format the streaming session expects.
- NAudio's `DataAvailable` callback is synchronous, so PCM is enqueued into a bounded channel and
  streamed to the session on a dedicated task (respects SDK backpressure; oldest chunks drop if the
  session falls behind).
