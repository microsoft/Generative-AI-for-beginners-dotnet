# Ollama with a local MCP server

Requires .NET 11 or later, Ollama, and `ollama pull llama3.2`. Run from the repository root:

```powershell
dotnet run --project .\samples\CoreSamples\MCP-04-LocalTools
```

The application starts a child copy of itself in `--server` mode, connects using the MCP SDK's stdio transport, discovers `get_library_hours` (the SDK's tool name for `GetLibraryHours`), and offers the tool to the local model via MEAI function invocation.

Expect the discovered tool name, `Local MCP GetLibraryHours invoked.` on stderr, an answer containing the demo opening time, 9 AM, and a positive `MCP tool calls` count. Server logs use stderr because stdout carries the JSON-RPC protocol. The MCP client owns and disposes its session transport when finished; it does not leave a server running.

The server exposes only a read-only function returning synthetic data. There is no remote MCP endpoint, token, shell tool, database mutation, or cloud dependency. First package/model downloads still need internet. `OLLAMA_ENDPOINT` and `OLLAMA_MODEL` override local defaults. The whole client operation has a two-minute cancellation bound.

Local inference does not make arbitrary remote MCP servers offline or private. Review each server's permissions and destination before offering its tools to an agent.

[MCP lesson](../../../04-AgentsWithMAF/04-model-context-protocol.md) | [Ollama path](../../../docs/ollama-learning-path.md)
