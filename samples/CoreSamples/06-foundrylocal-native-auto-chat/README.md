# 06-foundrylocal-native-auto-chat

Foundry Local **native SDK** sample that does in-process chat with automatic runtime/model lifecycle steps.

## What it does

1. Initializes `FoundryLocalManager` (no REST endpoint URL required)
2. Discovers and registers execution providers
3. Resolves model alias from catalog
4. Selects the best model **variant** for your machine (GPU when available/registered, otherwise CPU)
5. Downloads model if needed (cached on next run)
6. Loads model, asks one question, validates response quality, retries once if malformed, and unloads model

## Configuration

Environment variables (all optional):

- `FOUNDRY_LOCAL_MODEL` (default: `phi-3.5-mini`)
- `FOUNDRY_LOCAL_PROMPT` (default: `Why is the sky blue?`)
- `FOUNDRY_LOCAL_CLEANUP_MODEL` (`true`/`false`) — optional non-interactive override for cache cleanup

## Run

```powershell
cd samples\06-foundrylocal-native-auto-chat
# optional overrides
$env:FOUNDRY_LOCAL_MODEL="phi-3.5-mini"
$env:FOUNDRY_LOCAL_PROMPT="Why is the sky blue?"
dotnet restore
dotnet run
```

At the end, the sample asks:

```text
Delete downloaded model? [Y/n]
```

Default is **Yes**.

For non-interactive runs, force behavior with environment override:

```powershell
$env:FOUNDRY_LOCAL_CLEANUP_MODEL="true"
dotnet run
```

## How this "automatic" flow works

1. You provide a model alias (or use the default).
2. Foundry Local resolves that alias to a concrete model and its available variants.
3. The sample inspects registered execution providers and selects a matching variant:
   - GPU preferred when available
   - CPU fallback when GPU isn't available
4. `DownloadAsync` only downloads on first run; later runs reuse cache.

## Expected output

Output includes:

- `Foundry Local native auto chat sample`
- `Available execution providers:`
- `Downloading/registering execution providers:`
- `Resolved model alias:`
- `Selected variant: ... (GPU|CPU)`
- `Downloading model:`
- `Question: Why is the sky blue?` (or your `FOUNDRY_LOCAL_PROMPT`)
- `Prompt: Why is the sky blue?` (shown in Step 6)
- `Answer:`
- `Primary response looked malformed or off-topic. Retrying once...` (only when fallback is needed)
- `Model unloaded.`
- `Delete downloaded model? [Y/n]`
- `Model cache removed.` (when answer is yes, or override is `true`)
