# MAF Hosted Agent 02 — Multi-Agent Research Assistant

A .NET console application built with the **Microsoft Agent Framework (MAF) v1.0** that orchestrates three collaborating agents in a sequential workflow. This sample demonstrates how to build a multi-agent pipeline suitable for deployment to [Azure Foundry Agent Service](https://learn.microsoft.com/en-us/azure/foundry/agents/concepts/hosted-agents) as a **Hosted Agent**.

## What This Sample Demonstrates

- Building a **multi-agent workflow** with MAF v1.0 (`AgentWorkflowBuilder.BuildSequential()`)
- Creating specialized agents with distinct roles (Researcher, Writer, Reviewer)
- Chaining agents so each one builds on the previous agent's output
- Converting a `Workflow` into a single `AIAgent` with `workflow.AsAIAgent()`
- Supporting both **Azure OpenAI** and **Ollama** as AI providers
- Packaging the pipeline as a **Docker container** for Hosted Agent deployment

## Architecture

```
 ┌─────────────┐     ┌─────────────┐     ┌─────────────┐
 │  Researcher  │────▶│    Writer    │────▶│   Reviewer   │
 │              │     │              │     │              │
 │ Gathers key  │     │ Drafts a     │     │ Polishes the │
 │ facts and    │     │ structured   │     │ article and  │
 │ questions    │     │ article      │     │ provides the │
 │ about topic  │     │ from notes   │     │ final output │
 └─────────────┘     └─────────────┘     └─────────────┘
       ▲                                        │
       │           User provides topic          │
       └────────────────────────────────────────┘
                   Final article returned
```

### How It Works

1. The user provides a research topic
2. **Researcher** — Generates key questions, facts, and talking points
3. **Writer** — Takes the research notes and produces a structured article
4. **Reviewer** — Reviews, edits, and polishes the final article
5. The polished article is returned to the user

## Prerequisites

| Requirement | Details |
|---|---|
| **.NET 10 SDK** | [Download](https://dotnet.microsoft.com/download) |
| **Docker** | Required for container build and Foundry deployment |
| **Azure OpenAI** or **Ollama** | At least one AI provider configured |
| **Azure subscription** | Required only for Azure OpenAI and Foundry deployment |

## Project Structure

```
MAF-HostedAgent-02-MultiAgent/
├── Program.cs                    # Workflow orchestration and chat loop
├── Agents/
│   ├── ResearcherAgent.cs        # Researcher agent definition
│   ├── WriterAgent.cs            # Writer agent definition
│   └── ReviewerAgent.cs          # Reviewer agent definition
├── Dockerfile                    # Multi-stage Docker build
├── agent.yaml                    # Foundry agent manifest
├── MAF-HostedAgent-02-MultiAgent.csproj
├── README.md
└── Properties/
    └── launchSettings.json       # Environment variable defaults
```

## Getting Started

### 1. Configure an AI Provider

**Option A — Azure OpenAI** (set environment variables):

```bash
export AZURE_OPENAI_ENDPOINT="https://<your-resource>.openai.azure.com/"
export AZURE_OPENAI_MODEL="gpt-5-mini"
export AZURE_OPENAI_APIKEY="<your-api-key>"
```

**Option B — Ollama** (local, no Azure needed):

```bash
ollama pull phi4-mini
# Defaults: OLLAMA_ENDPOINT=http://localhost:11434/ OLLAMA_MODEL=phi4-mini
```

### 2. Build and Run Locally

```bash
cd samples/MAF/MAF-HostedAgent-02-MultiAgent
dotnet build
dotnet run
```

You will see an interactive prompt:

```
Multi-Agent Research Assistant
Workflow: Researcher → Writer → Reviewer
Type a topic to research, or 'exit' to quit.

Topic: quantum computing
--- Running workflow ---
[1/3] Researcher is gathering information...
[2/3] Writer is drafting the article...
[3/3] Reviewer is polishing the output...

=== Final Output ===

# Quantum Computing: The Next Frontier
...
```

### 3. Build the Docker Container

```bash
docker build -t multi-agent-research:latest .
```

### 4. Run in Docker

```bash
docker run -it --rm \
  -e AZURE_OPENAI_ENDPOINT="https://<your-resource>.openai.azure.com/" \
  -e AZURE_OPENAI_MODEL="gpt-5-mini" \
  -e AZURE_OPENAI_APIKEY="<your-api-key>" \
  multi-agent-research:latest
```

### 5. Deploy to Azure Foundry Agent Service

Follow the [Hosted Agents deployment guide](https://learn.microsoft.com/en-us/azure/foundry/agents/concepts/hosted-agents) to push your container image and register the agent using the `agent.yaml` manifest.

## Code Walkthrough

### Agent Definitions

Each agent is defined as a static factory class in the `Agents/` folder. The factory method receives an `IChatClient` and returns a configured `AIAgent`:

```csharp
static class ResearcherAgent
{
    public static AIAgent Create(IChatClient client) =>
        client.AsAIAgent(
            name: "Researcher",
            instructions: "You are a thorough research assistant...");
}
```

### Workflow Orchestration

The three agents are wired into a sequential workflow using `AgentWorkflowBuilder`. The output of each agent flows into the next:

```csharp
Workflow workflow = AgentWorkflowBuilder.BuildSequential(researcher, writer, reviewer);
AIAgent workflowAgent = workflow.AsAIAgent();
```

### Running the Workflow

The workflow agent behaves like any single `AIAgent`—call `RunAsync()` with a prompt and receive the final `AgentResponse`:

```csharp
AgentResponse response = await workflowAgent.RunAsync(
    $"Research and write an article about: {input}");
Console.WriteLine(response.Text);
```

### Chat Client Factory

The same dual-provider pattern from other Hosted Agent samples. Azure OpenAI takes priority; if not configured, it falls back to Ollama:

```csharp
IChatClient chatClient = CreateChatClient();
```

## Key Concepts

| Concept | Description |
|---|---|
| **Sequential Workflow** | Agents run in order; each receives the previous agent's output |
| **Agent Specialization** | Each agent has a focused role (research, writing, editing) |
| **Workflow as Agent** | `workflow.AsAIAgent()` wraps the pipeline as a single callable agent |
| **Provider Flexibility** | Swap between Azure OpenAI and Ollama via environment variables |

## Related Resources

- [Microsoft Agent Framework Documentation](https://github.com/microsoft/agents)
- [Hosted Agents Overview](https://learn.microsoft.com/en-us/azure/foundry/agents/concepts/hosted-agents)
- [Microsoft.Extensions.AI](https://learn.microsoft.com/en-us/dotnet/ai/ai-extensions)
- [MAF-HostedAgent-01-TimeZone](../MAF-HostedAgent-01-TimeZone/) — Single-agent Hosted Agent sample
- [MAF-MultiAgents](../MAF-MultiAgents/) — Multi-model agent orchestration
- [Azure OpenAI Setup](../../01-IntroductionToGenerativeAI/setup-azure-openai.md)
- [Ollama Setup](../../01-IntroductionToGenerativeAI/setup-local-ollama.md)
