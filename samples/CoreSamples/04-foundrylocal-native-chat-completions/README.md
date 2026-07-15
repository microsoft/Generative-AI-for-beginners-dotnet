# 04-foundrylocal-native-chat-completions

Foundry Local **native SDK** sample (in-process manager flow) for streaming chat completions.

## Prerequisites

- .NET SDK 8.0+ (this project targets `net8.0-windows10.0.18362`)
- Foundry Local installed and available on this machine
- Windows package path (this sample uses `Microsoft.AI.Foundry.Local.WinML`)

> Cross-platform note: on macOS/Linux, swap package reference to `Microsoft.AI.Foundry.Local`.

## Configuration

Optional environment variables:

- `FOUNDRY_LOCAL_MODEL` (or `FOUNDRY_LOCAL_NATIVE_MODEL`) — default: `qwen2.5-0.5b`

## Run

```powershell
cd samples\04-foundrylocal-native-chat-completions
# optional overrides
$env:FOUNDRY_LOCAL_MODEL="qwen2.5-0.5b"
$env:FOUNDRY_LOCAL_NATIVE_MODEL="qwen2.5-0.5b"
dotnet restore
dotnet run
```

The sample initializes `FoundryLocalManager`, discovers execution providers, downloads/registers providers, downloads + loads model, streams chat output, and unloads the model.
