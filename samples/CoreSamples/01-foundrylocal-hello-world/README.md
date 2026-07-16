# 01-foundrylocal-hello-world

Minimal Foundry Local non-streaming hello-world sample using the OpenAI .NET SDK (`OpenAI` package).

## What it does

1. Runs a **preflight** check against `GET /v1/models`
2. Prints actionable guidance if Foundry Local is not reachable
3. Sends one simple non-streaming prompt and prints the response
4. Uses shared helper code from `samples\Common\FoundryLocalSampleSupport.cs`

## Configuration

Environment variables (all optional):

- `FOUNDRY_LOCAL_BASE_URL` (default: `http://127.0.0.1:5273/v1`)
- `FOUNDRY_LOCAL_MODEL` (default: `qwen2.5-0.5b`)
- `FOUNDRY_LOCAL_API_KEY` (default: `local-dev-key`)

If `FOUNDRY_LOCAL_MODEL` is unavailable, the sample falls back to the first **chat-capable** model it can detect from `/v1/models`.

## Run

```powershell
cd samples\01-foundrylocal-hello-world
# optional overrides
$env:FOUNDRY_LOCAL_BASE_URL="http://127.0.0.1:5273/v1"
$env:FOUNDRY_LOCAL_MODEL="qwen2.5-0.5b"
$env:FOUNDRY_LOCAL_API_KEY="local-dev-key"
dotnet restore
dotnet run
```

## Expected output (success)

```text
Foundry Local hello world (non-streaming)
Endpoint: http://127.0.0.1:5273/v1

Model response:
Hello from Foundry Local!
```

## Expected output (preflight failure)

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

## Expected output (model invocation failure)

```text
Model invocation failed for '...' with HTTP 500 (InternalServerError).
This usually means the selected model is not chat-capable or is not loaded.
Try setting FOUNDRY_LOCAL_MODEL to an installed chat model.
```

## Assumptions

- Foundry Local exposes an OpenAI-compatible API at `/v1`.
- `foundry service start` / `foundry service status` are available on your installed Foundry Local CLI.
