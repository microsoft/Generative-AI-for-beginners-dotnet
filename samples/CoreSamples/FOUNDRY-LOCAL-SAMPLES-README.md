# Foundry Local samples

**New learners:** start with [Foundry Local setup](../../01-IntroductionToGenerativeAI/setup-foundry-local.md) and the [five-lesson Foundry Local learning path](../../docs/foundry-local-learning-path.md). It is an alternative to [Ollama](../../docs/ollama-learning-path.md) and [Azure](../../01-IntroductionToGenerativeAI/setup-azure-openai.md), not an additional prerequisite.

For typed decision examples across Foundry Local and Llama App, see the [decision model samples](./DECISION-MODEL-SAMPLES-README.md).

## Recommended course route

| Sample | Concepts |
|---|---|
| [FoundryLocal-00NativeChat](./FoundryLocal-00NativeChat/README.md) | SDK-native Hello World, explicit model preparation and lifecycle |
| [FoundryLocal-01Techniques](./FoundryLocal-01Techniques/README.md) | MEAI chat, history, streaming, validated JSON, cache and tools |
| [FoundryLocal-02RAG](./FoundryLocal-02RAG/README.md) | Real local embeddings, dimension-safe semantic retrieval and source-cited generation |
| [MAF-FoundryLocal-01](../MAF/MAF-FoundryLocal-01/README.md) | Actual Agent Framework agents, tools and sequential writer/editor |
| [MCP-FoundryLocal-LocalTools](./MCP-FoundryLocal-LocalTools/README.md) | Local stdio server and observable model-to-tool invocation |

From repository root, with .NET 10+:

```powershell
dotnet run --project .\samples\CoreSamples\FoundryLocal-00NativeChat -- --prepare
dotnet run --project .\samples\CoreSamples\FoundryLocal-01Techniques -- stream
```

The new route uses SDK 2.3.1, a private CPU-first runtime, retained model cache, and optional SDK-owned loopback HTTP. It does not require the CLI, a manually started service, a fixed port, Azure secrets, or GPU drivers. Model overrides are `FOUNDRY_LOCAL_MODEL` and `FOUNDRY_LOCAL_EMBEDDING_MODEL`; these samples do not read user secrets or `appsettings.json`.

Initial NuGet/catalog/model acquisition requires internet. Prepared local inference is not a promise of air-gapped first launch. Nonessential runtime telemetry is disabled by the course helper. Windows x64 CPU is the qualified course baseline; SDK runtime availability does not imply every sample works on every platform.

These are educational examples, not blanket production-readiness or managed safety guarantees. Read [privacy and boundaries](../../05-ResponsibleAI/readme.md#foundry-local-privacy-and-boundaries).

## Earlier and optional reference demos

These retain their original APIs and prerequisites; they are not the core route's acceptance targets. Read each project's source/manifest and README before running. Some prompts/configuration and WinML 1.x packages differ from the new 2.x route.

| Demo | Scope and caveats |
|---|---|
| [01-foundrylocal-hello-world](./01-foundrylocal-hello-world/) | Earlier OpenAI endpoint example; separately managed service and endpoint configuration |
| [02-foundrylocal-streaming](./02-foundrylocal-streaming/) | Earlier endpoint streaming demonstration |
| [03-foundrylocal-scenarios](./03-foundrylocal-scenarios/) | Summarization/sentiment and prompted JSON; not verified schema enforcement |
| [04-foundrylocal-native-chat-completions](./04-foundrylocal-native-chat-completions/) | Earlier Windows-targeted native WinML API track |
| [05-foundrylocal-audio-transcription](./05-foundrylocal-audio-transcription/README.md) | Optional file audio with separate native prerequisites |
| [06-foundrylocal-native-auto-chat](./06-foundrylocal-native-auto-chat/) | Earlier Windows lifecycle/selection demo; canned fallback output is not inference evidence; review cache-deletion prompts |
| [07-foundrylocal-agent-tools](./07-foundrylocal-agent-tools/) | Community native adapter and MEAI function invocation; agent-style, not an actual `AIAgent` |
| [10-live-speech-to-text](./10-live-speech-to-text/) | Windows microphone/NAudio and ElBruno.Whisper; not proof of Foundry SDK inference |
| [11-foundrylocal-live-transcription](./11-foundrylocal-live-transcription/README.md) | Optional Windows live ASR |
| [12-foundrylocal-multilingual-live-transcription](./12-foundrylocal-multilingual-live-transcription/README.md) | Optional Windows multilingual live ASR |
| [AIFoundryLocal-01-MEAI-Chat](./AIFoundryLocal-01-MEAI-Chat/) | Older raw OpenAI client with hardcoded GPU model/endpoint; use the new techniques sample for MEAI |

Do not apply one common setup command or claim unrestricted Linux/macOS compatibility for this mixed inventory. CLI commands belong to the separately versioned CLI; the recommended course's SDK preparation commands are in setup.

## References

- [Current Foundry Local SDK documentation](https://learn.microsoft.com/azure/foundry-local/reference/reference-sdk-current?wt.mc_id=dotnet-0000-brunocapuano)
- [SDK 2.3.1 samples and release prerequisites](https://github.com/microsoft/Foundry-Local/releases/tag/v2.3.1?wt.mc_id=dotnet-0000-brunocapuano)
- [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/ai-extensions?wt.mc_id=dotnet-0000-brunocapuano)
- [Contribution guidance](../../CONTRIBUTING.MD)

The repository's MIT license covers course code; model licenses and third-party runtime redistribution terms must be reviewed independently.
