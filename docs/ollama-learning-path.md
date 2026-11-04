# Ollama-only learning path

Start with the [local setup guide](../01-IntroductionToGenerativeAI/setup-local-ollama.md). You need .NET 10 or later and a running Ollama server, not Azure credentials. All commands below run in PowerShell from the repository root. Unless overridden, each sample uses the model shown in the table.

## Your route through the lessons

| Lesson and concept | Sample and command from repository root | Model | Expected result |
|---|---|---|---|
| 1-2: First response | [BasicChat-03Ollama](../samples/CoreSamples/BasicChat-03Ollama/app.cs): `dotnet run .\samples\CoreSamples\BasicChat-03Ollama\app.cs` | `phi4-mini` | Four reviews classified by sentiment |
| 2: Conversation history | [BasicChat-10ConversationHistory](../samples/CoreSamples/BasicChat-10ConversationHistory/app.cs): `dotnet run .\samples\CoreSamples\BasicChat-10ConversationHistory\app.cs -- --ollama` | `phi4-mini` | Streaming chat remembers previous turns; empty input exits |
| 2: Streaming | [Ollama-01Techniques](../samples/CoreSamples/Ollama-01Techniques/README.md): `dotnet run --project .\samples\CoreSamples\Ollama-01Techniques -- stream` | `phi4-mini` | Text arrives incrementally and a positive chunk count is printed |
| 2: Structured output | Same project: `dotnet run --project .\samples\CoreSamples\Ollama-01Techniques -- structured` | `phi4-mini` | JSON with `Sentiment` = `Positive` and `Rating` = `5` |
| 2: Function calling | [MEAIFunctionsOllama](../samples/CoreSamples/MEAIFunctionsOllama/README.md): `dotnet run --project .\samples\CoreSamples\MEAIFunctionsOllama` | `llama3.2` | Math without tools, then an observable weather-tool call |
| 2: Middleware/cache | [Ollama-01Techniques](../samples/CoreSamples/Ollama-01Techniques/README.md): `dotnet run --project .\samples\CoreSamples\Ollama-01Techniques -- cache` | `phi4-mini` | Two matching answers but `Model calls: 1` |
| 3: Embeddings, search, RAG | [Ollama-02RAG](../samples/CoreSamples/Ollama-02RAG/README.md): `dotnet run --project .\samples\CoreSamples\Ollama-02RAG` | `all-minilm` + `phi4-mini` | 384-dimensional vectors, library retrieval, answer citing `[library]` |
| 4: First agent | [MAF-Ollama-01](../samples/MAF/MAF-Ollama-01/README.md): `dotnet run --project .\samples\MAF\MAF-Ollama-01` | `llama3.2` | A short Spanish story |
| 4: Agent tools | Same project: `dotnet run --project .\samples\MAF\MAF-Ollama-01 -- tools` | `llama3.2` | Library-hours function invoked; answer based on demo data |
| 4: Sequential agents | Same project: `dotnet run --project .\samples\MAF\MAF-Ollama-01 -- workflow` | `llama3.2` | Writer output followed by editor output, both local |
| 4: Local MCP | [MCP-04-LocalTools](../samples/CoreSamples/MCP-04-LocalTools/README.md): `dotnet run --project .\samples\CoreSamples\MCP-04-LocalTools` | `llama3.2` | A child MCP server exposes library hours over stdio; the model calls it |
| 5: Responsible local AI | [Responsible AI](../05-ResponsibleAI/readme.md#local-models-and-safety) | Any local model | Review safeguards and boundaries; no managed Azure filter is assumed |

These examples perform inference locally and use fixed demo data. They need internet for their first NuGet restores and model downloads, but not for inference once dependencies are present. No sample in this core route reads cloud secrets when explicitly running in local mode.

For every sample, `OLLAMA_ENDPOINT` changes the server, and `OLLAMA_MODEL` changes the chat model. Clear any override when you want the table's defaults. `BasicChat-10ConversationHistory --ollama` deliberately bypasses shared Azure secrets without modifying them.

## Follow the written lesson explanations

Read [Lesson 2](../02-GenerativeAITechniques/readme.md), [Lesson 3](../03-AIPatternsAndApplications/readme.md), and [Lesson 4](../04-AgentsWithMAF/readme.md) alongside these samples. Azure-specific client initialization in the chapters is an alternative, not a local prerequisite. The `IChatClient` operations are shared, but models differ in tool, schema, vision, and embedding support.

For a local initializer in project-based examples:

```csharp
using Microsoft.Extensions.AI;
using OllamaSharp;

using IChatClient client =
    new OllamaApiClient(new Uri("http://localhost:11434"), "phi4-mini");
var response = await client.GetResponseAsync("Hello!");
Console.WriteLine(response.Text);
```

Use the runnable samples for package versions and bounded requests. File-based applications also need the serializer property shown in the first sample.

## Optional branches and honest limits

| Branch | Local alternative or extra requirement |
|---|---|
| Vision | [Vision-02MEAI-Ollama](../samples/CoreSamples/Vision-02MEAI-Ollama/README.md). Download the default `llama3.2-vision`, or use the verified smaller `moondream` alternative with `OLLAMA_VISION_MODEL=moondream`. Check hardware capacity. `dotnet run --project .\samples\CoreSamples\Vision-02MEAI-Ollama`. |
| Local vector storage with Azure embeddings | `RAGSimple-02MEAIVectorsMemory` is not fully local AI. Use `Ollama-02RAG` for local embeddings and generation. |
| Azure AI Search, managed PDF processing, hosted agents | Optional cloud capabilities. Do not run Azure provisioning for the local route. Local vision is not a drop-in replacement for every cloud document API. |
| Mixed-model workflows | `MAF-MultiAgents` and `MAF-MultiModel` include cloud providers. `MAF-MultiAgents-Factory-01` uses another local model and an image. Start with the small local writer/editor example above. |
| Hugging Face / Microsoft Learn MCP | Local inference still calls a remote service. Hugging Face may require tokens or paid tools. Use `MCP-04-LocalTools` for a fully local protocol example. |
| Videos | Videos may show Azure setup or older paths. The current written companion is this route plus the setup guide; do not copy Azure credentials/provisioning steps into local examples. |
| Translations | This route is currently maintained in English. Translation updates should be complete, separately reviewed language contributions, not partial machine-generated edits. |

Local models do not automatically receive Azure content filtering or compliance guarantees. Keep tools read-only while learning, validate structured output, restrict access to sensitive data, and require human approval before consequential actions.

## Validate your environment

The opt-in smoke runner requires Python 3.9 or later and the .NET SDK. It never downloads models. Run from the repository root:

```powershell
python .\scripts\test_ollama.py
python .\scripts\test_ollama.py --build-only
python .\scripts\test_ollama.py --run
```

The first command runs offline regression tests and validates the local-path documentation links. `--build-only` also builds all eight route samples and compiles the six local-provider documentation snippets without requiring a running Ollama server; CI uses this mode. `--run` additionally exercises chat, history, streaming, structured output, cache, tools, local RAG, agents, local MCP, and negative cases with bounded timeouts. Its child processes use an empty user-secrets directory and clear cloud credential variables without modifying your configuration.

To select models and include optional vision:

```powershell
ollama pull moondream
python .\scripts\test_ollama.py --run --endpoint http://localhost:11434 --chat-model phi4-mini --tool-model llama3.2 --embedding-model all-minilm --vision-model moondream
```

Missing models are reported as `SKIP`, not `PASS`; the summary distinguishes failed and skipped checks. A failed assertion, process error, or timeout returns nonzero. Core inference coverage is complete only when all core checks pass (optional vision can be skipped). Vision acceptance requires the description to identify runners or footwear in the supplied photo, not merely return text. A fake model that repeatedly requests the weather tool also verifies the middleware's iteration limit and final tool-free request. If multiple SDKs are installed, use `--sdk-version 10.0.403` with your installed .NET 10 SDK version to select it in a temporary directory, without changing repository SDK settings. `--timeout 240` sets the positive per-process limit in seconds.
