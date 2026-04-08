# MAF Hosted Agent 01 — TimeZone Agent

A .NET console agent built with the **Microsoft Agent Framework (MAF) v1.0** that answers time and date questions using a local tool function. This sample demonstrates how to create a containerized agent suitable for deployment to [Azure Foundry Agent Service](https://learn.microsoft.com/en-us/azure/foundry/agents/concepts/hosted-agents) as a **Hosted Agent**.

## What This Sample Demonstrates

- Creating an AI agent with **MAF v1.0** (`AsAIAgent()`, `RunAsync()`, `AgentResponse`)
- Registering a **tool function** (`GetCurrentTime`) that the agent can call
- Using `TimeZoneInfo` to convert between timezones
- Supporting both **Azure OpenAI** and **Ollama** as AI providers via environment variables
- Packaging the agent as a **Docker container** for Hosted Agent deployment
- Using the **Foundry agent manifest** (`agent.yaml`)

## Prerequisites

| Requirement | Details |
|---|---|
| **.NET 10 SDK** | [Download](https://dotnet.microsoft.com/download) |
| **Docker** | Required for container build and Foundry deployment |
| **Azure OpenAI** or **Ollama** | At least one AI provider configured |
| **Azure subscription** | Required only for Azure OpenAI and Foundry deployment |

## Project Structure

```
MAF-HostedAgent-01-TimeZone/
├── Program.cs                    # Agent logic, tool function, chat loop
├── Dockerfile                    # Multi-stage Docker build
├── agent.yaml                    # Foundry agent manifest
├── MAF-HostedAgent-01-TimeZone.csproj
├── README.md
└── Properties/
    └── launchSettings.json       # Environment variable defaults
```

## Getting Started

### 1. Configure an AI Provider

**Option A — Azure OpenAI** (set environment variables):

```bash
export AZURE_OPENAI_ENDPOINT="https://<your-resource>.openai.azure.com/"
export AZURE_OPENAI_MODEL="gpt-4o-mini"
export AZURE_OPENAI_APIKEY="<your-api-key>"
```

**Option B — Ollama** (local, no Azure needed):

```bash
ollama pull phi4-mini
# Defaults: OLLAMA_ENDPOINT=http://localhost:11434/ OLLAMA_MODEL=phi4-mini
```

### 2. Build and Run Locally

```bash
cd samples/MAF/MAF-HostedAgent-01-TimeZone
dotnet build
dotnet run
```

You will see an interactive prompt:

```
TimeZone Agent — Ask me for the current time in any timezone!
Type 'exit' to quit.

You: What time is it in Tokyo?
Agent: The current time in Tokyo Standard Time is 2025-04-08 21:30:15 (Tokyo Standard Time).
```

### 3. Build the Docker Container

```bash
docker build -t timezone-agent:latest .
```

### 4. Run in Docker

```bash
docker run -it --rm \
  -e AZURE_OPENAI_ENDPOINT="https://<your-resource>.openai.azure.com/" \
  -e AZURE_OPENAI_MODEL="gpt-4o-mini" \
  -e AZURE_OPENAI_APIKEY="<your-api-key>" \
  timezone-agent:latest
```

### 5. Deploy to Azure Foundry Agent Service

Follow the [Hosted Agents deployment guide](https://learn.microsoft.com/en-us/azure/foundry/agents/concepts/hosted-agents) to push your container image and register the agent using the `agent.yaml` manifest.

## Code Walkthrough

### Chat Client Factory

The app checks environment variables to determine which AI provider to use. Azure OpenAI takes priority; if not configured, it falls back to Ollama:

```csharp
IChatClient chatClient = CreateChatClient();
```

### Agent with Tool Function

The agent is created using MAF's `AsAIAgent()` extension method. A `GetCurrentTime` tool function is registered via `AIFunctionFactory.Create()`:

```csharp
AIAgent timeAgent = chatClient.AsAIAgent(
    name: "TimeZoneAgent",
    instructions: "You are a helpful assistant that provides the current date and time...",
    tools: [AIFunctionFactory.Create(GetCurrentTime)]);
```

### Tool Function

The `GetCurrentTime` function uses `TimeZoneInfo.FindSystemTimeZoneById()` to convert UTC to the requested timezone:

```csharp
[Description("Gets the current date and time for the specified timezone.")]
static string GetCurrentTime(string timezoneName)
{
    var tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneName);
    var now = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, tz);
    return $"Current time in {tz.DisplayName}: {now:yyyy-MM-dd HH:mm:ss}";
}
```

### Interactive Chat Loop

The agent runs in a simple chat loop, processing each user message independently:

```csharp
AgentResponse response = await timeAgent.RunAsync(input);
Console.WriteLine($"Agent: {response.Text}");
```

## Related Resources

- [Microsoft Agent Framework Documentation](https://github.com/microsoft/agents)
- [Hosted Agents Overview](https://learn.microsoft.com/en-us/azure/foundry/agents/concepts/hosted-agents)
- [Microsoft.Extensions.AI](https://learn.microsoft.com/en-us/dotnet/ai/ai-extensions)
- [Azure OpenAI Setup](../../01-IntroductionToGenerativeAI/setup-azure-openai.md)
- [Ollama Setup](../../01-IntroductionToGenerativeAI/setup-local-ollama.md)
