# 12-foundrylocal-multilingual-live-transcription

Foundry Local native streaming sample for real-time, multilingual microphone transcription.
It builds on [sample 11](../11-foundrylocal-live-transcription/) and follows the
[Microsoft Learn live transcription guide](https://learn.microsoft.com/azure/foundry-local/how-to/how-to-live-transcribe-audio?tabs=windows&pivots=programming-language-csharp&wt.mc_id=dotnet-35129-website).

The sample uses the `nemotron-3.5-asr-streaming-0.6b` Foundry Local SDK catalog alias
for the `nvidia-nemotron-3.5-asr-streaming-multilingual-0.6b` model and defaults to
automatic language detection. Speech is transcribed in its original language; this
sample does not translate between languages.

## Language selection

At startup, choose one of these options:

```text
Choose the spoken language:
  1. Auto-detect (auto, default)
  2. English (en)
  3. Spanish (es)
  4. Italian (it)
  5. More languages (enter a locale)
Selection [1]:
```

Press ENTER to use automatic detection. The predefined choices use `en`, `es`, and `it`.
Choose **More languages** to enter another locale recognized by .NET and supported by the
model, such as `de`, `fr`, or `zh-CN`.

When the language is known, selecting it explicitly can improve accuracy and reduce language
detection work. Auto-detection is convenient when the speaker's language is not known in advance.

## Prerequisites

- .NET SDK 8.0 or later
- Windows (microphone capture uses NAudio's Windows-only `WaveInEvent`)
- Foundry Local installed and available
- A working microphone/input device
- Internet access on the first run to download the model and execution providers

## Configuration

The interactive language menu appears when `FOUNDRY_LOCAL_SPEECH_LANGUAGE` is not set.
For scripted or non-interactive runs, set the variable to `auto` or a locale:

| Variable | Default | Description |
|---|---|---|
| `FOUNDRY_LOCAL_SPEECH_MODEL` | `nemotron-3.5-asr-streaming-0.6b` | Multilingual streaming ASR model alias |
| `FOUNDRY_LOCAL_SPEECH_LANGUAGE` | *(startup menu; ENTER selects `auto`)* | `auto` or an explicit locale such as `en`, `es`, `it`, or `zh-CN` |
| `FOUNDRY_LOCAL_CLEANUP_MODEL` | *(prompt)* | `true` or `false` to bypass the end-of-run model cleanup prompt |

Examples:

```powershell
# Auto-detect without showing the menu
$env:FOUNDRY_LOCAL_SPEECH_LANGUAGE="auto"
dotnet run

# Spanish
$env:FOUNDRY_LOCAL_SPEECH_LANGUAGE="es"
dotnet run

# Italian
$env:FOUNDRY_LOCAL_SPEECH_LANGUAGE="it"
dotnet run
```

## Run

```powershell
cd samples\CoreSamples\12-foundrylocal-multilingual-live-transcription
dotnet run
```

Select a language, speak into the microphone, and press ENTER to stop. Interim text appears
in cyan, and finalized phrases are printed as `[FINAL]`.

## Expected output

```text
===========================================================
 Foundry Local -- Multilingual Live Transcription
===========================================================

Choose the spoken language:
  1. Auto-detect (auto, default)
  2. English (en)
  3. Spanish (es)
  4. Italian (it)
  5. More languages (enter a locale)
Selection [1]:

Requested model alias: nemotron-3.5-asr-streaming-0.6b
Language:              Auto-detect (auto)
Mode:                  transcription (no translation)

Ensuring model is available locally (downloads automatically when not cached)...
Downloading model: 100.00%
Model is available locally.

...

  [FINAL] Buongiorno, questa e una prova.
```

The exact model variant and execution-provider output depend on the machine.

## Model cleanup

After the model unloads, the sample asks:

```text
Delete downloaded model? [Y/n]
```

Press ENTER to remove it from the Foundry Local cache, or enter `n` to retain it for a faster
next startup. Set `FOUNDRY_LOCAL_CLEANUP_MODEL=true` or `false` to make this non-interactive.

## Implementation notes

- Audio is captured at 16 kHz, 16-bit, mono.
- A bounded channel moves PCM chunks from NAudio's synchronous callback to Foundry Local's
  asynchronous `AppendAsync` API while respecting backpressure.
- The SDK catalog uses `nemotron-3.5-asr-streaming-0.6b` as the model alias. The sample also
  accepts the qualified `nvidia-nemotron-3.5-asr-streaming-multilingual-0.6b` name and falls
  back to the catalog alias when needed.
- `DownloadAsync` downloads the model into Foundry Local's private cache when it is absent and
  skips the download when it is already cached.
- The currently published CUDA GPU variant `nemotron-3.5-asr-streaming-0.6b-cuda-gpu:2`
  has a confirmed upstream bug: its built-in Silero VAD is incorrectly placed on CUDA and
  terminates the process when the first audio chunk arrives. The sample detects affected
  GPU versions and uses CPU instead. It will prefer GPU automatically when a newer corrected
  model version is published. See [Foundry Local issue #850](https://github.com/microsoft/Foundry-Local/issues/850?wt.mc_id=dotnet-35129-website).
- Automatic detection selects the spoken language; it does not translate the transcript.
