# Foundry Local with a local MCP tool

The client launches a child .NET MCP server over **stdio** and exposes only `GetLibraryHours` with fixed synthetic data. Both model inference and the tool run locally.

Follow [setup](../../../01-IntroductionToGenerativeAI/setup-foundry-local.md). From repository root:

```powershell
dotnet run --project .\samples\CoreSamples\FoundryLocal-01Techniques -- prepare --prepare
dotnet run --project .\samples\CoreSamples\MCP-FoundryLocal-LocalTools
dotnet run --project .\samples\CoreSamples\MCP-FoundryLocal-LocalTools -- protocol
```

The main mode uses default `phi-4-mini` through SDK-managed MEAI and must show one MCP call, the server's `Local MCP GetLibraryHours invoked.` message on stderr, and an answer based on `9 AM`. A plausible answer without a call fails.

`protocol` directly invokes the MCP tool without model initialization/downloads. It tests local protocol wiring, **not AI tool-selection behavior**. `--server` is an internal child mode; stdout is reserved for JSON-RPC, while diagnostic output goes to stderr.

The client owns and disposes its transport/child process. Inference/tool rounds are bounded with a three-minute cancellation budget and three middleware iterations. The acceptance runner adds a process watchdog. No remote MCP token, hosted search, Azure key or Ollama server is used.

See [Program.cs](./Program.cs), [responsible local tools](../../../05-ResponsibleAI/readme.md#foundry-local-privacy-and-boundaries), and the [full route](../../../docs/foundry-local-learning-path.md).
