# Setting Up: Local Development with Ollama

You can follow the core course without an Azure account, Azure login, cloud API keys, or Codespaces. Ollama runs the model on your machine. Initial model downloads and NuGet restores require internet access; inference can then run offline.

## 1. Install and start the tools

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download?wt.mc_id=dotnet-0000-brunocapuano) or later and [Ollama](https://ollama.com/download). Clone or download this repository and open a terminal at its root.

```powershell
dotnet --version
ollama --version
ollama list
```

The .NET SDK version must be at least 10. `ollama --version` checks the CLI, not server readiness. `ollama list` must reach the server and list your models, even if the list is initially empty. You can also open the local API at `http://localhost:11434/api/tags`.

The desktop Ollama application normally starts the server. If no server is running, run `ollama serve` in a separate terminal and leave it open. Do not start a second server when the desktop application is already serving requests.

## 2. Download models as you need them

| Model tag | Used for | When needed |
|---|---|---|
| `phi4-mini` | Chat, history, streaming, structured output, caching, RAG answers | First sample |
| `all-minilm` | 384-dimensional text embeddings | Semantic search and RAG |
| `llama3.2` | Function calling, agents, local MCP | Tool and agent lessons |
| `llama3.2-vision` | Image understanding | Optional vision sample; substantially larger download and memory requirements |

```powershell
ollama pull phi4-mini
```

Later, before RAG and the tool lessons:

```powershell
ollama pull all-minilm
ollama pull llama3.2
```

Do not use a chat model for embeddings, or assume a text-only model understands images. Different models support different features and produce different results.

## 3. Run your first .NET sample

From the repository root:

```powershell
dotnet run .\samples\CoreSamples\BasicChat-03Ollama\app.cs
```

This is a .NET file-based application, so include `app.cs`. It sends four product reviews to `phi4-mini` and prints their sentiments. Expect positive, negative, and neutral classifications, not exactly the same wording each time.

The sample uses Ollama directly. Do not run `setup.ps1`, `az login`, or configure Azure secrets for this route.

Most later samples have a `.csproj` instead. For those, use `dotnet run --project <folder>` from the repository root, or `dotnet run` inside the project directory.

## 4. Continue through the course

Follow the [Ollama-only learning path](../docs/ollama-learning-path.md). It connects each lesson to runnable local examples and shows the commands, expected outputs, and optional branches.

For multi-turn chat, explicitly select Ollama even if Azure secrets already exist:

```powershell
dotnet run .\samples\CoreSamples\BasicChat-10ConversationHistory\app.cs -- --ollama
```

Ask "Remember that my project is called Cedar." Then ask "What is my project called?" The second response should remember Cedar. Submit an empty line to exit.

## Optional configuration

The local route defaults to `http://localhost:11434`. These PowerShell variables apply to commands in the current terminal only:

```powershell
$env:OLLAMA_ENDPOINT = "http://localhost:11434"
$env:OLLAMA_MODEL = "phi4-mini"
```

`OLLAMA_MODEL` overrides the sample's default chat model. Before tool or agent examples, set it to `llama3.2` or remove the override with `Remove-Item Env:OLLAMA_MODEL`. RAG separately uses `OLLAMA_EMBEDDING_MODEL` (default `all-minilm`); vision uses `OLLAMA_VISION_MODEL` (default `llama3.2-vision`). An override must name an installed model with the required capability.

On macOS/Linux, use `export OLLAMA_MODEL=phi4-mini` and shell-appropriate path separators instead of the PowerShell assignment and Windows paths. Set variables only if you need to change defaults.

## Troubleshooting

| Symptom | Action |
|---|---|
| Connection refused | Check `ollama list`, start Ollama if stopped, and check `OLLAMA_ENDPOINT`. |
| Model not found / HTTP 404 | Run `ollama list` and `ollama pull <exact-model-tag>`. Check overrides as well as sample defaults. |
| "Couldn't find a project" | File-based examples need `dotnet run app.cs`, not just `dotnet run`. Project examples need the correct directory or `--project`. |
| Reflection-based JSON serialization disabled | Update to this revision of the file-based examples. They explicitly enable the serializer support required by OllamaSharp. |
| Slow first response or timeout | The model may be loading. Check memory use, close unused models/apps, and try a smaller model with the required capabilities. CPU inference is slower. Requests are bounded; inspect errors rather than assuming success. |
| Tool not called or wrong structured result | Verify the model capability, keep the sample's explicit prompt, and try a more capable model. A tool-capable model can still make mistakes. |
| Embedding dimension mismatch | The local RAG schema expects `all-minilm`'s 384 dimensions. Changing models requires changing the schema and rebuilding the index, not mixing old and new vectors. |
| Running in a container or Codespace | `localhost` means that environment, not your laptop. Run Ollama in the same environment or configure a reachable endpoint. Do not expose an unauthenticated Ollama server publicly. |
| Package restore fails offline | Restore packages and download models while online before disconnecting. No Azure credentials are needed. |

The [Ollama dev container](../.devcontainer/Ollama/devcontainer.json) installs local models inside the container and requests 16 GB of RAM. A Codespace is remote compute, not on-device execution. It may incur Codespaces charges even when no Azure AI service is used.

## Videos and provider differences

The existing setup and lesson videos may show Azure initialization or older file locations. They are not an Ollama command checklist. Use this guide and the linked learning path as the current written companion: initialize the local client, then follow the provider-neutral MEAI operations. Cloud deployment, managed document processing, and Azure filtering remain separate optional capabilities.

The setup, chat, RAG, and vision video shortcuts still resolve to YouTube. Other topic shortcuts were found to redirect to search and are marked unavailable in their chapters. The written local route is complete without those recordings; it does not claim that a new Ollama-specific video has been published.

[Back to Introduction](./readme.md) | [Continue with the Ollama learning path](../docs/ollama-learning-path.md)
