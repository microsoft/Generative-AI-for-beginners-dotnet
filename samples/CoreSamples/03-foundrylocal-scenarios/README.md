# 03-foundrylocal-scenarios

Practical Foundry Local sample with three tiny scenario flows in one console app.

## What it does

1. Reuses shared bootstrap/preflight from `samples\Common\FoundryLocalSampleSupport.cs`
2. Supports scenario selection by argument or tiny menu
3. Runs concise deterministic prompts for:
   - `summarize` (exactly two short sentences)
   - `sentiment` (`LABEL | REASON`)
   - `structured` (strict minified JSON with `title` + 3 `keywords`)
4. Prints clean `Input` / `Output` sections

## Configuration

Environment variables (all optional):

- `FOUNDRY_LOCAL_BASE_URL` (default: `http://127.0.0.1:5273/v1`)
- `FOUNDRY_LOCAL_MODEL` (default: `qwen2.5-0.5b`)
- `FOUNDRY_LOCAL_API_KEY` (default: `local-dev-key`)

If `FOUNDRY_LOCAL_MODEL` is unavailable, the app falls back to the first **chat-capable** model it can detect from `/v1/models`.

## Run

```powershell
cd samples\03-foundrylocal-scenarios
# optional overrides
$env:FOUNDRY_LOCAL_BASE_URL="http://127.0.0.1:5273/v1"
$env:FOUNDRY_LOCAL_MODEL="qwen2.5-0.5b"
$env:FOUNDRY_LOCAL_API_KEY="local-dev-key"
dotnet restore
```

### 1) Summarize scenario

```powershell
dotnet run -- summarize
dotnet run -- summarize "Foundry Local gives me private low-latency AI for dev workflows."
```

### 2) Sentiment scenario

```powershell
dotnet run -- sentiment
dotnet run -- sentiment "I love how fast this local model responds."
```

### 3) Structured JSON scenario

```powershell
dotnet run -- structured
dotnet run -- structured "Local AI helps summarize docs and extract useful tags."
```

### Interactive tiny menu

```powershell
dotnet run
```

## Offline / service-not-running path

If Foundry Local is down, preflight fails fast and prints actionable steps:

```text
Preflight check failed.
Could not reach Foundry Local service at 'http://127.0.0.1:5273/v1': ...

Try this:
  1) Verify Foundry Local CLI is installed: foundry --help
  2) Start or restart the local service: foundry service start
  3) Check status and endpoint: foundry service status
  4) Optional environment overrides (PowerShell):
     $env:FOUNDRY_LOCAL_BASE_URL="http://127.0.0.1:5273/v1"
     $env:FOUNDRY_LOCAL_MODEL="qwen2.5-0.5b"
     $env:FOUNDRY_LOCAL_API_KEY="local-dev-key"
```
