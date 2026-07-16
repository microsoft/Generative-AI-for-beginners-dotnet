# 05-foundrylocal-audio-transcription

Foundry Local **native SDK** sample (in-process manager flow) for streaming audio transcription.

## Prerequisites

- .NET SDK 8.0+ (this project targets `net8.0-windows10.0.18362`)
- Foundry Local installed and available on this machine
- Windows package path (this sample uses `Microsoft.AI.Foundry.Local.WinML`)

> Cross-platform note: on macOS/Linux, swap package reference to `Microsoft.AI.Foundry.Local`.

## Configuration

Optional environment variables:

- `FOUNDRY_LOCAL_WHISPER_MODEL` — preferred model alias override
- `FOUNDRY_LOCAL_AUDIO_MODEL` — fallback model alias override
- `FOUNDRY_LOCAL_MODEL` — final fallback alias override
- `FOUNDRY_LOCAL_AUDIO_LANGUAGE` — transcription language hint (default: `en`)

Default alias is `whisper-tiny`.

## Input audio file

- If a CLI argument is provided, that path is used.
- If no argument is provided, sample defaults to:
  - `samples\05-foundrylocal-audio-transcription\Recording.mp3`

If the file is missing, the sample prints an actionable message with the expected path and a runnable command.

## Run

```powershell
cd samples\05-foundrylocal-audio-transcription
# optional overrides
$env:FOUNDRY_LOCAL_WHISPER_MODEL="whisper-tiny"
$env:FOUNDRY_LOCAL_AUDIO_MODEL="whisper-tiny"
$env:FOUNDRY_LOCAL_MODEL="whisper-tiny"
$env:FOUNDRY_LOCAL_AUDIO_LANGUAGE="en"
dotnet restore
dotnet run
```

Run with a custom file:

```powershell
dotnet run -- "C:\path\to\audio.mp3"
```

## Expected output

Output includes:

- `Foundry Local native audio transcription sample`
- `Execution providers:`
- `Resolved model:`
- `Selected CPU variant:` (when available)
- `Transcribing audio with streaming output:`
- streamed transcript text
- `Model unloaded.`
