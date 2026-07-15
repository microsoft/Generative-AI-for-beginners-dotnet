# Foundry Local Samples - Running AI Models Locally from .NET

This collection demonstrates how to run **Generative AI models locally** from .NET using **Microsoft Foundry Local**. These samples showcase practical techniques for local model inference—from simple chat completions and streaming responses to advanced scenarios like audio transcription and live speech-to-text—all powered by Foundry Local's OpenAI-compatible endpoint and native SDK.

## Overview

**Microsoft Foundry Local** enables you to run AI models on your machine without cloud dependencies, API costs, or internet connectivity. It provides two integration paths:

1. **OpenAI-Compatible Endpoint** — Use the standard OpenAI SDK with Foundry Local's `/v1` API (works with any OpenAI-compatible code)
2. **Native Foundry Local SDK** — Direct model lifecycle management, auto-download, GPU/CPU optimization, and audio streaming

All samples are production-ready and follow .NET best practices with proper error handling, configuration management, and resource cleanup.

## Samples Included

| Sample | Location | Description |
|--------|----------|-------------|
| **01-foundrylocal-hello-world** | [./01-foundrylocal-hello-world/](./01-foundrylocal-hello-world/) | Simple non-streaming prompt/response using OpenAI SDK against Foundry Local's OpenAI-compatible endpoint. Includes preflight checks. |
| **02-foundrylocal-streaming** | [./02-foundrylocal-streaming/](./02-foundrylocal-streaming/) | Token-by-token streaming output demonstrating multiple prompt variants (eli5, bullets, detailed explanations). |
| **03-foundrylocal-scenarios** | [./03-foundrylocal-scenarios/](./03-foundrylocal-scenarios/) | Practical use cases: text summarization, sentiment analysis, structured JSON output with deterministic prompts. |
| **04-foundrylocal-native-chat-completions** | [./04-foundrylocal-native-chat-completions/](./04-foundrylocal-native-chat-completions/) | Native SDK chat completions with in-process `FoundryLocalManager` for model lifecycle control. Microsoft Learn parity. |
| **05-foundrylocal-audio-transcription** | [./05-foundrylocal-audio-transcription/](./05-foundrylocal-audio-transcription/) | Native SDK audio transcription—download/load a Whisper model, stream transcript from an audio file. |
| **06-foundrylocal-native-auto-chat** | [./06-foundrylocal-native-auto-chat/](./06-foundrylocal-native-auto-chat/) | Native SDK demonstration of model alias resolution, GPU/CPU auto-selection, auto-download, and unload. |
| **07-foundrylocal-agent-tools** | [./07-foundrylocal-agent-tools/](./07-foundrylocal-agent-tools/) | Local agent-style chat using `ElBruno.MAF.FoundryLocal.Adapter` + Microsoft.Extensions.AI tool/function invocation. |
| **10-live-speech-to-text** | [./10-live-speech-to-text/](./10-live-speech-to-text/) | ⚠️ **Windows-only** — Live microphone input to text transcription fully local (ONNX Whisper via ElBruno.Whisper + NAudio). |
| **11-foundrylocal-live-transcription** | [./11-foundrylocal-live-transcription/](./11-foundrylocal-live-transcription/) | ⚠️ **Windows-only** — Native Foundry Local streaming speech-to-text (nemotron-speech-streaming ASR) with real-time interim + final results. |

## Prerequisites

All samples require:

### 1. Foundry Local Setup

- **Foundry Local Service**: Download and install from [Microsoft Foundry Local](https://learn.microsoft.com/azure/ai-foundry/foundry-local/)
- **Default Endpoint**: `http://127.0.0.1:5273/v1` (ensure service is running)
- **Models**: Pre-download required models via Foundry Local CLI (e.g., `foundrylocal model pull qwen2.5-0.5b`)

### 2. Development Environment

- **.NET 10 SDK** or later: [Download .NET](https://dotnet.microsoft.com/download?wt.mc_id=dotnet-35129-website)
- **IDE**: Visual Studio 2022, VS Code, or Rider (optional but recommended)

### 3. Environment Variables & Configuration

Each sample uses the following configuration (set via environment variables or `appsettings.json`):

```bash
FOUNDRY_LOCAL_BASE_URL=http://127.0.0.1:5273/v1
FOUNDRY_LOCAL_MODEL=qwen2.5-0.5b
FOUNDRY_LOCAL_API_KEY=<your-api-key>
```

### ⚠️ Platform-Specific Notes

- **Samples 10 & 11** (live speech-to-text) are **Windows-only** due to NAudio and native audio codec dependencies.
- **Linux/macOS**: Samples 01–09 (chat, streaming, scenarios, agent tools) work without restriction.

## Quick Start

### Running a Console Sample

```bash
# Set environment variables (or edit appsettings.json)
$env:FOUNDRY_LOCAL_BASE_URL = "http://127.0.0.1:5273/v1"
$env:FOUNDRY_LOCAL_MODEL = "qwen2.5-0.5b"

# Example: Basic chat with OpenAI SDK
cd samples/CoreSamples/01-foundrylocal-hello-world
dotnet run

# Example: Streaming responses
cd samples/CoreSamples/02-foundrylocal-streaming
dotnet run

# Example: Native SDK chat
cd samples/CoreSamples/04-foundrylocal-native-chat-completions
dotnet run
```

### Verifying Foundry Local is Running

```bash
# Health check
curl http://127.0.0.1:5273/v1/models

# Should return a list of available models in OpenAI format
```

## Common Patterns

### Using the OpenAI SDK with Foundry Local

```csharp
using OpenAI;

var client = new OpenAIClient(
    apiKey: Environment.GetEnvironmentVariable("FOUNDRY_LOCAL_API_KEY") ?? "not-needed",
    new OpenAIClientOptions
    {
        Endpoint = new Uri(Environment.GetEnvironmentVariable("FOUNDRY_LOCAL_BASE_URL") 
            ?? "http://127.0.0.1:5273/v1")
    });

var messages = new[] 
{ 
    new ChatCompletionMessage(ChatRole.System, "You are a helpful assistant."),
    new ChatCompletionMessage(ChatRole.User, "What is Foundry Local?")
};

var completion = await client.GetChatCompletionsAsync(
    new ChatCompletionOptions
    {
        Model = Environment.GetEnvironmentVariable("FOUNDRY_LOCAL_MODEL") ?? "qwen2.5-0.5b",
        Messages = messages.ToList()
    });

Console.WriteLine(completion.Content[0].Text);
```

### Using the Native Foundry Local SDK

```csharp
using Foundry.LocalSDK;
using Microsoft.Extensions.AI;

var manager = new FoundryLocalManager(
    baseUrl: new Uri(Environment.GetEnvironmentVariable("FOUNDRY_LOCAL_BASE_URL") 
        ?? "http://127.0.0.1:5273/v1"),
    apiKey: Environment.GetEnvironmentVariable("FOUNDRY_LOCAL_API_KEY") ?? "");

var modelId = Environment.GetEnvironmentVariable("FOUNDRY_LOCAL_MODEL") ?? "qwen2.5-0.5b";
var client = manager.GetChatClient(modelId);

var response = await client.CompleteAsync("What can you do?");
Console.WriteLine(response.Message.Text);
```

## Architecture & Integration Patterns

### Two Paths to Local AI

```
Your .NET Application
       ↓
    ┌──┴────────────────────────────┐
    ↓                               ↓
OpenAI SDK                   FoundryLocalManager (Native SDK)
    ↓                               ↓
Foundry Local /v1 API         Direct Model Lifecycle
(HTTP REST)                    (In-process)
    ↓                               ↓
Local Model Inference ← ─ ─ ─ ─ ─ ─ ┘
```

**OpenAI SDK Path** — Familiar, drop-in compatible with existing OpenAI code; uses HTTP `/v1` endpoint.

**Native SDK Path** — Direct control over model lifecycle, GPU/CPU selection, auto-download, streaming audio.

## Configuration

### Setting User Secrets (Local Development)

```bash
cd samples/CoreSamples/<sample-name>

# For local development
dotnet user-secrets set "FOUNDRY_LOCAL_BASE_URL" "http://127.0.0.1:5273/v1"
dotnet user-secrets set "FOUNDRY_LOCAL_MODEL" "qwen2.5-0.5b"
dotnet user-secrets set "FOUNDRY_LOCAL_API_KEY" "your-key-here"
```

### Using appsettings.json

```json
{
  "FoundryLocal": {
    "BaseUrl": "http://127.0.0.1:5273/v1",
    "Model": "qwen2.5-0.5b",
    "ApiKey": "your-api-key"
  }
}
```

## Best Practices

### 1. **Model Selection**

- **qwen2.5-0.5b** — Fast, lightweight (0.5B parameters, ~300MB); good for quick responses and demo
- **phi4-mini** — Balanced performance (3.8B parameters, ~2GB); excellent reasoning
- **mistral-nemo** — High quality (12B parameters, ~8GB); complex tasks

Choose based on available VRAM and latency requirements.

### 2. **Error Handling**

```csharp
try
{
    var response = await client.CompleteAsync("Your prompt");
}
catch (HttpRequestException ex)
{
    Console.Error.WriteLine("Foundry Local service not reachable. Is it running?");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
}
```

### 3. **Resource Management**

- Use `using` statements for clients and managers
- Call `Unload()` on models when finished (native SDK)
- Monitor GPU/CPU usage, especially with large models

### 4. **Streaming for Better UX**

Always use streaming for user-facing applications:

```csharp
await foreach (var chunk in client.CompleteStreamingAsync("Your prompt"))
{
    Console.Write(chunk.Message.Text);
}
Console.WriteLine();
```

### 5. **Configuration Security**

- Never hardcode API keys or endpoints
- Use user secrets in development
- Use environment variables or Azure Key Vault in production

## Troubleshooting

### "Connection refused" / "Could not connect to http://127.0.0.1:5273"

**Cause**: Foundry Local service is not running.

**Solution**:
1. Verify Foundry Local is installed
2. Start the Foundry Local service: `foundrylocal start`
3. Check service status: `foundrylocal status`
4. Verify endpoint is reachable: `curl http://127.0.0.1:5273/v1/models`

### Model "qwen2.5-0.5b" not found

**Cause**: Model is not downloaded on your machine.

**Solution**:
1. List available models: `foundrylocal model list`
2. Pull the model: `foundrylocal model pull qwen2.5-0.5b`
3. Verify it's downloaded: `foundrylocal model list --local`

### High latency / Slow responses

**Cause**: Model is running on CPU instead of GPU.

**Solution**:
1. Check Foundry Local status: `foundrylocal status`
2. Verify GPU availability: `foundrylocal gpu list` (native SDK only)
3. Use a smaller model if GPU is unavailable (e.g., qwen2.5-0.5b vs. mistral-nemo)

### Sample 10/11 won't run (Windows-only samples)

**Cause**: You're on Linux or macOS.

**Solution**: Use samples 01–09, which are cross-platform.

### API Key Authentication Errors

**Cause**: Incorrect `FOUNDRY_LOCAL_API_KEY`.

**Solution**:
1. Check Foundry Local configuration: `foundrylocal config show`
2. Update user secrets: `dotnet user-secrets set "FOUNDRY_LOCAL_API_KEY" "..."`
3. Restart your application

## Additional Resources

### Documentation

- [Microsoft Foundry Local Documentation](https://learn.microsoft.com/azure/ai-foundry/foundry-local/?wt.mc_id=ai_copilot_web_wwwplatform_aml-55852-website)
- [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/ai-extensions?wt.mc_id=ai_copilot_web_wwwplatform_aml-55852-website)
- [OpenAI .NET SDK Documentation](https://github.com/openai/openai-dotnet)

### Related Samples

- **[BasicChat Samples](./BasicChat-01MEAI/)** — Microsoft.Extensions.AI chat patterns
- **[MAF Foundry Local Adapter](../MAF/)** — Agent Framework + Foundry Local
- **[Ollama Samples](./BasicChat-03Ollama/)** — Alternative local model runner

### Learning Resources

- [Generative AI for Beginners .NET](https://github.com/microsoft/Generative-AI-for-beginners-dotnet)
- [Microsoft Agent Framework](https://learn.microsoft.com/agent-framework/?wt.mc_id=ai_copilot_web_wwwplatform_aml-55852-website)
- [Microsoft Foundry Quickstart](https://learn.microsoft.com/azure/ai-foundry/quickstarts/?wt.mc_id=ai_copilot_web_wwwplatform_aml-55852-website)

## Contributing

Contributions are welcome! Please follow the repository's contribution guidelines:

1. Fork the repository
2. Create a feature branch
3. Make your changes and format with `dotnet format`
4. Submit a pull request

For details, see [CONTRIBUTING.MD](../../CONTRIBUTING.MD).

## License

This sample collection is part of the Generative AI for Beginners .NET course and follows the repository's MIT license.

---

**Ready to run AI models locally?** Start with [01-foundrylocal-hello-world](./01-foundrylocal-hello-world/) for a quick introduction, then explore streaming, scenarios, and advanced patterns!
