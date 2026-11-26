# Foundry Local learning path

Follow the five course lessons with **Foundry Local instead of Ollama or Azure**. Begin with [setup](../01-IntroductionToGenerativeAI/setup-foundry-local.md). Commands below run in PowerShell from the repository root. No Azure credentials, Ollama server or Foundry CLI is required.

## Route through the lessons

| Lesson and outcome | Command / written sample | Expected evidence |
|---|---|---|
| 1: SDK-native Hello World | [Native chat](../samples/CoreSamples/FoundryLocal-00NativeChat/README.md): `dotnet run --project .\samples\CoreSamples\FoundryLocal-00NativeChat -- --prepare` | Actual local arithmetic answer, model ID, cached/download status |
| 2: MEAI chat | [Techniques](../samples/CoreSamples/FoundryLocal-01Techniques/README.md): `dotnet run --project .\samples\CoreSamples\FoundryLocal-01Techniques -- hello` | `Answer: 4` |
| 2: History | Same project with `-- history` | Second turn remembers `ORCHID` |
| 2: Streaming | Same project with `-- stream` | Nonempty incremental text and positive chunk count |
| 2: Structured output | Same project with `-- structured` | Parsed JSON: `Sentiment=Positive`, `Rating=5`, no extra properties |
| 2: Middleware/cache | Same project with `-- cache` | Two matching answers, `Model calls: 1` |
| 2: Functions | Same project with `-- tools` | Math without tools, then one observable library function invocation |
| 3: Model preparation | [Local RAG](../samples/CoreSamples/FoundryLocal-02RAG/README.md): `dotnet run --project .\samples\CoreSamples\FoundryLocal-02RAG -- prepare --prepare` | Chat and embedding models cached |
| 3: Embeddings, semantic search, RAG | Same project with `-- answer` | Actual vectors; library ranks first; answer contains `9 AM` and `[library]` |
| 3: Unanswerable/injected data | Same project with `-- unknown` or `-- injection` | Abstention, or source-grounded answer ignoring injected instruction |
| 4: Actual Agent Framework agent | [Agents](../samples/MAF/MAF-FoundryLocal-01/README.md): `dotnet run --project .\samples\MAF\MAF-FoundryLocal-01 -- story` | Writer returns a short Spanish story |
| 4: Agent tools | Same project with `-- tools` | One actual library tool invocation and opening time |
| 4: Sequential agents | Same project with `-- workflow` | Writer output followed by editor output |
| 4: Local MCP | [Local tools](../samples/CoreSamples/MCP-FoundryLocal-LocalTools/README.md): `dotnet run --project .\samples\CoreSamples\MCP-FoundryLocal-LocalTools` | Tool listed, client call and server invocation over stdio |
| 5: Responsible local AI | [Safety guidance](../05-ResponsibleAI/readme.md#foundry-local-privacy-and-boundaries) | Understand telemetry, tool boundaries and model licenses |

Core defaults: `phi-4-mini` for chat/tools and `qwen3-embedding-0.6b` for embeddings. The observed Windows CPU IDs were `Phi-4-mini-instruct-generic-cpu:5` and `qwen3-embedding-0.6b-generic-cpu:1`; actual resolved IDs are printed on every run. Model overrides are explicit environment variables described in setup, never automatic substitutions. Run samples sequentially to limit memory contention.

## What stays the same, and what changes

Read [lesson 2](../02-GenerativeAITechniques/readme.md), [lesson 3](../03-AIPatternsAndApplications/readme.md), and [lesson 4](../04-AgentsWithMAF/readme.md) alongside the samples. MEAI operations and concepts are shared; Azure initialization and provisioning are alternative branches, not local prerequisites.

The native introduction demonstrates embedded `ChatSession` inference. Main samples use **SDK-owned lifecycle -> ephemeral loopback OpenAI-compatible endpoint -> official OpenAI .NET client -> MEAI**. Agent samples use generic `AsAIAgent`, not a dedicated .NET Foundry Local Agent Framework provider or the older community adapter. The MCP server is a local child process with one read-only synthetic tool.

The SDK 2.3.1 JSON-schema endpoint envelope differs from the standard OpenAI client's schema envelope; see the [released request conversion code](https://github.com/microsoft/Foundry-Local/blob/v2.3.1/sdk_v2/cpp/src/contracts/chat_completions_converter.cc?wt.mc_id=dotnet-0000-brunocapuano). The structured lesson requests **JSON-object decoding**, then deserializes and validates a strict application contract. It does **not** claim that a standard OpenAI `json_schema` request enforces that schema in this runtime. Invalid output fails rather than returning a canned answer.

RAG uses real local embeddings and a tiny in-memory cosine-similarity index. It derives dimensions from vectors (1024 in the qualified model), rejects mismatched/nonfinite/zero vectors, and never reuses Ollama's 384-dimensional schema. This intentionally exposes retrieval mechanics before introducing a vector database. Citation/abstention assertions are teaching checks, not a universal hallucination or prompt-injection defense.

## Optional branches and limits

| Branch | Guidance |
|---|---|
| Vision | The [released native C# vision sample](https://github.com/microsoft/Foundry-Local/blob/v2.3.1/samples/cs/vision/Program.cs?wt.mc_id=dotnet-0000-brunocapuano) demonstrates `ImageItem`; `qwen3-vl-2b-instruct` appeared in the catalog. This course route does not yet qualify its image accuracy. Use a reviewed image and require grounded content before calling a run successful. |
| File audio | [Existing native transcription sample](../samples/CoreSamples/05-foundrylocal-audio-transcription/README.md) is a legacy WinML/API track with separate prerequisites. Start with a known recording; it is not acceptance coverage for the new 2.x text route. |
| Live audio | [Live](../samples/CoreSamples/11-foundrylocal-live-transcription/README.md) and [multilingual](../samples/CoreSamples/12-foundrylocal-multilingual-live-transcription/README.md) examples have Windows-specific microphone/native dependencies. They are optional, not part of core inference acceptance. |
| GPU/NPU or another OS | Product runtime support is not course qualification. The new route starts Windows x64 CPU-first; install external acceleration prerequisites only as a separate choice. |
| Managed cloud features | Azure AI Search, managed PDF processing, hosted agents and remote MCP remain separate branches. Local inference does not make remote tools local. |
| Videos/translations | Videos can show old/cloud setup. These current English written instructions are authoritative for this route; translations require separately reviewed complete contributions. |

## Validate the route

Python 3.9+ is needed only for automation. From repository root:

```powershell
python .\scripts\test_foundry_local.py
python .\scripts\test_foundry_local.py --build-only
python .\scripts\test_foundry_local.py --prepare
python .\scripts\test_foundry_local.py --run
```

The default checks documentation and runs offline .NET contract/loop tests without initializing the native runtime or downloading models. `--build-only` also builds all five samples, runs a local MCP protocol-only check, compiles the setup snippet, and audits dependencies. `--prepare` explicitly permits chat/embedding downloads and builds first. `--run` builds and exercises all core modes against cached models; it never downloads models. Missing core assets are failures, not passes or silent skips.

Use `--timeout 600` for a larger positive per-process budget (default 300 seconds) or `--sdk-version 10.0.403` to select an installed .NET SDK in a temporary working directory without changing repository SDK policy. Each mode has cooperative cancellation plus the runner's process-tree watchdog. Samples print model IDs; `--log-dir` preserves per-check logs. Cloud credential variables are removed for child processes without changing user secrets or settings.

PR CI performs offline/build checks. Manually triggered Windows CPU inference deliberately prepares models and reports runtime acceptance separately. Both jobs preserve per-check logs as workflow artifacts, including resolved model IDs and inference output when executed. Local PASS does not imply all external GitHub jobs or other hardware have run.

**Other routes:** [Ollama-only](./ollama-learning-path.md), [Azure setup](../01-IntroductionToGenerativeAI/setup-azure-openai.md), [Foundry sample inventory](../samples/CoreSamples/FOUNDRY-LOCAL-SAMPLES-README.md).
