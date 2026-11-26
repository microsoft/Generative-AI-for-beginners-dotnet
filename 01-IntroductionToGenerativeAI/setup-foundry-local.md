# Set up Foundry Local for the .NET course

Foundry Local runs models on your computer. This route needs **.NET 10 or later**, but no Azure subscription, API key, Azure CLI, Ollama, Docker, or separate Foundry CLI/service installation.

Start from the repository root in PowerShell. These examples use SDK **2.3.1** and were qualified on **Windows x64 CPU**. The SDK also packages Linux x64/ARM64, Windows ARM64 and macOS Apple Silicon runtimes; those course inference paths are not yet qualified. Use native hardware, not a Linux container expecting Windows acceleration.

## 1. Prepare the chat model

```powershell
dotnet --version
dotnet run --project .\samples\CoreSamples\FoundryLocal-00NativeChat -- --prepare
```

The first run restores NuGet packages, resolves `phi-4-mini`, explicitly downloads it if missing, loads it, and answers `4` to a small arithmetic prompt. Download and model load can take significantly longer than subsequent requests.

The observed CPU variant is `Phi-4-mini-instruct-generic-cpu:5` (catalog size about **4,915 MB**). RAG additionally uses `qwen3-embedding-0.6b-generic-cpu:1` (about **495 MB**). These are catalog file sizes, **not RAM/VRAM requirements**; allow additional space for native runtime/NuGet files and enough available memory for both models during RAG. Model versions/sizes can change.

The smaller `qwen2.5-0.5b` is not the route default: it failed the arithmetic qualification. A model being listed or advertising tools does not guarantee lesson quality.

`--prepare` permits model acquisition; without it, a missing cached model is an error. Cached models are not downloaded again. The runtime keeps the cache under its `dotnet-beginners-foundry` application configuration. Do not delete it between lessons. Samples use a private runtime host and unload only their own models; they do not stop another application's service.

## 2. Run locally through MEAI

```powershell
dotnet run --project .\samples\CoreSamples\FoundryLocal-01Techniques -- hello
dotnet run --project .\samples\CoreSamples\FoundryLocal-01Techniques -- history
dotnet run --project .\samples\CoreSamples\FoundryLocal-01Techniques -- stream
```

The native Hello World uses `ChatSession` directly, without HTTP. The other samples use the SDK to start a **loopback-only endpoint on a free port**, discover that address, and construct an OpenAI client adapted to MEAI. No fixed port or cloud credential is needed.

Inside these course projects, the small [shared lifecycle helper](../samples/CoreSamples/Common/FoundryCourseRuntime.cs) owns the SDK and model resources. The application code is:

```csharp
using FoundryLocal.Course;
using Microsoft.Extensions.AI;

await using var runtime = await FoundryCourseRuntime.StartAsync(prepare: false);
using IChatClient chat = runtime.CreateChatClient();
using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(3));
var response = await chat.GetResponseAsync(
    "What is 2 + 2? Reply with only the number.",
    FoundryCourseChecks.TextOptions, timeout.Token);
Console.WriteLine(FoundryCourseChecks.RequireText(response.Text));
```

This snippet requires the course helper/import, not just a single NuGet reference. Read the [native Hello World source](../samples/CoreSamples/FoundryLocal-00NativeChat/Program.cs) and [bootstrap source](../samples/CoreSamples/Common/FoundryCourseRuntime.cs) to see initialization, alias resolution, downloads, loading, sessions and disposal. All new projects import [the same package versions](../samples/CoreSamples/Common/FoundryLocal.Course.props).

## 3. Add local embeddings when you reach lesson 3

```powershell
dotnet run --project .\samples\CoreSamples\FoundryLocal-02RAG -- prepare --prepare
dotnet run --project .\samples\CoreSamples\FoundryLocal-02RAG -- answer
```

Preparation loads both models but does not generate an answer. The next command generates actual vectors, reports their dimension, retrieves the library document, and generates a source-cited answer. No Azure vector service is used.

## Configuration and troubleshooting

The new route reads only these model overrides, not Azure secrets or `appsettings.json`:

```powershell
$env:FOUNDRY_LOCAL_MODEL = "phi-4-mini"
$env:FOUNDRY_LOCAL_EMBEDDING_MODEL = "qwen3-embedding-0.6b"
```

Clear an override with `Remove-Item Env:\FOUNDRY_LOCAL_MODEL` if you previously changed it. Empty, unknown, wrong-task or non-tool-capable selections fail explicitly; there is no fallback to another model/provider. Foundry aliases differ from Ollama names: use `phi-4-mini`, not `phi4-mini`.

| Symptom | Action |
|---|---|
| Model is not cached | Repeat the same command with `--prepare` while online; do not install a CLI to fix this SDK route |
| Model alias unavailable or wrong task | Check the emitted error and current catalog; use the documented defaults |
| Native library/architecture error | Check SDK, OS and process architecture against the supported runtime identifiers |
| Slow inference or memory pressure | Close other model workloads; CPU inference is slower, and RAG loads two models; do not confuse file size with memory use |
| Empty/invalid JSON, wrong answer, missed tool | Treat this as a failed lesson, not successful setup; retain logs and exact model/version |
| Timeouts | The runner has a configurable per-process watchdog; native cancellation is cooperative and is not an unlimited-hang guarantee |
| Want GPU/NPU acceleration | Follow the [versioned release prerequisites](https://github.com/microsoft/Foundry-Local/releases/tag/v2.3.1?wt.mc_id=dotnet-0000-brunocapuano) separately; these samples deliberately do not download EPs or install drivers |

Initial restore, catalog resolution, model downloads and optional EP installation need network access. Cached local inference and prepared dependency use are distinct from a fully air-gapped first launch. Do not assume offline catalog startup on every platform/runtime; qualify it in your deployment environment.

The helper disables nonessential runtime telemetry before initialization. See the [versioned privacy description](https://github.com/microsoft/Foundry-Local/blob/v2.3.1/sdk_v2/cpp/docs/Privacy.md?wt.mc_id=dotnet-0000-brunocapuano), review each model's license, and avoid logging sensitive prompts. The local OpenAI placeholder credential is not an Azure API key or an authentication boundary.

**Continue:** [Foundry Local learning path through all five lessons](../docs/foundry-local-learning-path.md). Other independent choices remain [Ollama](./setup-local-ollama.md) and [Azure OpenAI](./setup-azure-openai.md).
