# Local streaming, structured output, and caching

Requires .NET 10 or later, a running Ollama server, and `ollama pull phi4-mini`. No Azure setup or secrets are used.

From the repository root:

```powershell
dotnet run --project .\samples\CoreSamples\Ollama-01Techniques -- stream
dotnet run --project .\samples\CoreSamples\Ollama-01Techniques -- structured
dotnet run --project .\samples\CoreSamples\Ollama-01Techniques -- cache
```

`stream` prints text as chunks arrive. `structured` uses MEAI's typed response API, then validates the classification and rating before printing JSON. Model output is still untrusted even when it parses.

`cache` wraps a counting `DelegatingChatClient` in MEAI's distributed-cache middleware, backed by in-memory storage for this process. Two identical requests should print matching answers and `Model calls: 1`. The counter is inside the cache so it counts only requests that reach the model. This demonstrates middleware ordering, not a persistent or shared production cache.

Set `OLLAMA_ENDPOINT` or `OLLAMA_MODEL` to override the local defaults. Requests have a two-minute bound. Failed inference or invalid results raise an error instead of printing a success marker.

[Continue along the Ollama path](../../../docs/ollama-learning-path.md)
