# Local agents with Ollama

Requires .NET 10 or later, a running Ollama server, and `ollama pull llama3.2`. No cloud secrets or Azure services are used.

From the repository root, restore once and run:

```powershell
dotnet restore .\samples\MAF\MAF-Ollama-01
dotnet run --project .\samples\MAF\MAF-Ollama-01
dotnet run --project .\samples\MAF\MAF-Ollama-01 -- tools
dotnet run --project .\samples\MAF\MAF-Ollama-01 -- workflow
```

The default `story` mode prints a short story in Spanish. `tools` creates a librarian agent with one read-only function and prints an observable invocation and positive call count. The library's weekday opening time is fixed demo data: 9 AM.

`workflow` runs a writer followed by an editor, passing the first agent's response to the second. This is explicit sequential orchestration in ordinary C#, not the richer workflow-builder API or autonomous handoff. Both agents share the same local `IChatClient`. The lesson's Azure and mixed-provider workflows remain optional advanced examples.

Set `OLLAMA_ENDPOINT` and `OLLAMA_MODEL` to change the server/model. Tools require a tool-capable model. Requests are bounded by a three-minute cancellation token and two-minute HTTP timeout; a missing tool invocation fails explicitly.

[First agent](../../../04-AgentsWithMAF/01-building-first-agent.md) | [Ollama path](../../../docs/ollama-learning-path.md)
