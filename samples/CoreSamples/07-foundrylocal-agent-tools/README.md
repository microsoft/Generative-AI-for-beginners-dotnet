# 07-foundrylocal-agent-tools

Local **agent-style** sample that uses `ElBruno.MAF.FoundryLocal.Adapter` + `Microsoft.Extensions.AI` tool invocation.

## What it does

1. Creates a local chat client using `FoundryLocalChatClientAdapter`
2. Wraps it with MEAI function-invocation middleware
3. Registers sample tools from an external file (`AgentSampleTools.cs`)
4. Prints whether the selected model is already cached or will be downloaded
5. Runs one agent turn in local mode (no OpenAI-compatible REST URL)
6. Logs each tool invocation to the console
7. Optionally removes the downloaded model cache

## Tools in this sample

- `get_time_in_timezone`
- `calculate_tip`
- `get_demo_fact`

## Configuration

Environment variables (all optional):

- `FOUNDRY_LOCAL_MODEL` (default: `qwen2.5-0.5b`)
- `FOUNDRY_LOCAL_AGENT_FALLBACK_MODEL` (default: `qwen2.5-0.5b`)
- `FOUNDRY_LOCAL_AGENT_PROMPT` (default demo prompt using time + tip + fact)
- `FOUNDRY_LOCAL_CLEANUP_MODEL` (`true`/`false`) non-interactive cleanup override

## Run

```powershell
cd samples\07-foundrylocal-agent-tools
# optional overrides
$env:FOUNDRY_LOCAL_MODEL="qwen2.5-0.5b"
$env:FOUNDRY_LOCAL_AGENT_FALLBACK_MODEL="qwen2.5-0.5b"
$env:FOUNDRY_LOCAL_AGENT_PROMPT="I am in Pacific Standard Time. Bill is 42.50 with 18% tip. Use tools and return JSON."
dotnet restore
dotnet run
```

If no tools are called with the selected model, the sample prints guidance to rerun with `FOUNDRY_LOCAL_AGENT_FALLBACK_MODEL`.

At the end, the sample asks:

```text
Delete downloaded model? [Y/n]
```

Default is **Yes**.

For non-interactive runs:

```powershell
$env:FOUNDRY_LOCAL_CLEANUP_MODEL="true"
dotnet run
```
