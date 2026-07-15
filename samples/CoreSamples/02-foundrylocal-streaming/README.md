# 02-foundrylocal-streaming

Streaming Foundry Local sample using the OpenAI .NET SDK (`OpenAI` package).

## What it does

1. Runs a preflight check against `GET /v1/models`
2. Prints clear offline guidance if Foundry Local is unavailable
3. Streams token-by-token output from a chat completion
4. Supports two tiny prompt variants:
   - `eli5` (or `1`)
   - `bullets` (or `2`)

## Configuration

Environment variables (all optional):

- `FOUNDRY_LOCAL_BASE_URL` (default: `http://127.0.0.1:5273/v1`)
- `FOUNDRY_LOCAL_MODEL` (default: `qwen2.5-0.5b`)
- `FOUNDRY_LOCAL_API_KEY` (default: `local-dev-key`)
- `FOUNDRY_LOCAL_PROMPT_VARIANT` (default: `eli5`; values: `eli5`, `bullets`, `1`, `2`)

If `FOUNDRY_LOCAL_MODEL` is unavailable, the sample falls back to the first **chat-capable** model it can detect from `/v1/models`.

## Run

```powershell
cd samples\02-foundrylocal-streaming
# optional overrides
$env:FOUNDRY_LOCAL_BASE_URL="http://127.0.0.1:5273/v1"
$env:FOUNDRY_LOCAL_MODEL="qwen2.5-0.5b"
$env:FOUNDRY_LOCAL_API_KEY="local-dev-key"
$env:FOUNDRY_LOCAL_PROMPT_VARIANT="eli5"
dotnet restore
dotnet run
```

Run with variant override:

```powershell
dotnet run -- bullets
```

## Expected output (success)

```text
Foundry Local streaming sample
Endpoint: http://127.0.0.1:5273/v1

Prompt variants:
  1) eli5    - Explain like I'm 5
  2) bullets - Three tiny bullets
Selected variant: eli5

Streaming response:

[streamed text appears progressively here]
```

## Expected output (preflight failure / offline)

```text
Foundry Local streaming sample
Endpoint: http://127.0.0.1:5273/v1
...
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
