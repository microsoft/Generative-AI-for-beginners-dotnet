# Foundry Local with Microsoft.Extensions.AI

Follow [setup](../../../01-IntroductionToGenerativeAI/setup-foundry-local.md); commands run from repository root:

```powershell
dotnet run --project .\samples\CoreSamples\FoundryLocal-01Techniques -- prepare --prepare
dotnet run --project .\samples\CoreSamples\FoundryLocal-01Techniques -- hello
dotnet run --project .\samples\CoreSamples\FoundryLocal-01Techniques -- history
dotnet run --project .\samples\CoreSamples\FoundryLocal-01Techniques -- stream
dotnet run --project .\samples\CoreSamples\FoundryLocal-01Techniques -- structured
dotnet run --project .\samples\CoreSamples\FoundryLocal-01Techniques -- cache
dotnet run --project .\samples\CoreSamples\FoundryLocal-01Techniques -- tools
```

| Mode | What to observe |
|---|---|
| `prepare --prepare` | Download a missing default `phi-4-mini` and retain it; no inference |
| `hello` | Arithmetic response `4` |
| `history` | Explicit message history remembers `ORCHID` across two turns |
| `stream` | Incremental nonempty text and a positive chunk count |
| `structured` | JSON-object decoding plus typed validation: Positive/5, correct types and no additional fields |
| `cache` | Two matching responses with only one model invocation |
| `tools` | Math without tools, then one real read-only library function call |

The SDK starts an ephemeral loopback server and the official OpenAI adapter supplies `IChatClient`; inference stays local. History is sent explicitly; it is not also replayed into a stateful native session.

For SDK 2.3.1, the standard OpenAI client's JSON-schema envelope is incompatible with the endpoint's expected envelope. This sample uses JSON-object decoding and application validation, **not advertised schema-constrained decoding**. JSON errors and incorrect semantics fail visibly.

Tools have at most three middleware iterations and a three-minute cancellation budget. `RequireAny` is used only for the explicit library request; the middleware manages subsequent turns. Tests exercise repeated requests and the final tool-free turn. These boundaries are not authorization for consequential tools.

`FOUNDRY_LOCAL_MODEL` overrides the model explicitly. A missing cache requires `--prepare`; an invalid/wrong-task/non-tool-capable model fails instead of selecting another model. The helper does not read cloud secrets.

Read [Program.cs](./Program.cs), then continue to [local RAG](../FoundryLocal-02RAG/README.md) or the [full route](../../../docs/foundry-local-learning-path.md).
