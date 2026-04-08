# Microsoft Agent Framework Samples

Welcome to the comprehensive guide for **Microsoft Agent Framework (MAF)** samples! These samples demonstrate how to build intelligent agents using .NET 10, Microsoft.Extensions.AI, and various AI providers (Azure OpenAI, Ollama, Claude, and Microsoft Foundry).

## Overview

Microsoft Agent Framework v1.0 GA provides a modern, extensible foundation for building AI agents in .NET. These samples showcase:

- **Simple agents** — Single-shot and conversational AI agents
- **Multi-agent workflows** — Sequential and parallel agent orchestration
- **Tool integration** — Function calling and tool use
- **Persistence** — Saving and resuming conversation state
- **Web applications** — Blazor Server chat interfaces
- **Hosted deployment** — Docker containerization for Azure Foundry Agent Service
- **Multiple AI providers** — Azure OpenAI, Ollama, Claude, Microsoft Foundry, and more

## Prerequisites

### Required
- **.NET 10 SDK** or later ([Download](https://dotnet.microsoft.com/download))

### Choose at least one AI provider:
- **Azure OpenAI** — Cloud-based GPT models (requires Azure subscription)
- **Ollama** — Local LLM inference ([ollama.com](https://ollama.com))
- **Claude via Microsoft Foundry** — Anthropic models deployed in Azure
- **Microsoft Foundry** — Azure AI Foundry projects with multiple models

### Optional
- **Docker** — For building and deploying hosted agents
- **Visual Studio 2025** or **VS Code** — Recommended for development

## Setup Guides

Before running samples, configure your AI provider:

- **[Azure OpenAI Setup](../../01-IntroductionToGenerativeAI/setup-azure-openai.md)** — Configure Azure OpenAI credentials
- **[Ollama Setup](../../01-IntroductionToGenerativeAI/setup-local-ollama.md)** — Install and run Ollama locally

### Quick Setup (User Secrets)

```bash
cd samples/MAF/<sample-name>
dotnet user-secrets set "endpoint" "https://<your-endpoint>.openai.azure.com/"
dotnet user-secrets set "apikey" "<your-api-key>"
dotnet user-secrets set "deploymentName" "gpt-4o-mini"
```

---

## Sample Categories

### 1. Getting Started — Foundation Samples

Start here to learn the basics of MAF with simple agents.

| Sample | Description | Difficulty | Key Concepts |
|--------|-------------|------------|--------------|
| **[MAF01](MAF01/)** | Basic single-agent chat with Azure OpenAI | 🟢 Beginner | Agent creation, `RunAsync()`, Azure OpenAI |
| **[MAF02](MAF02/)** | Sequential workflow with two agents (Writer & Editor) | 🟢 Beginner | `AgentWorkflowBuilder`, sequential workflows, agent composition |
| **[MAF-Ollama-01](MAF-Ollama-01/)** | Single agent using local Ollama model | 🟢 Beginner | Ollama integration, local inference, provider flexibility |

**Quick Start:**
```bash
cd samples/MAF/MAF01
dotnet user-secrets set "endpoint" "https://<your-endpoint>.openai.azure.com/"
dotnet user-secrets set "apikey" "<your-api-key>"
dotnet run
```

---

### 2. Azure AI Foundry Integration

Work with Azure AI Foundry projects and managed deployments.

| Sample | Description | Difficulty | Key Concepts |
|--------|-------------|------------|--------------|
| **[MAF-AIFoundry-01](MAF-AIFoundry-01/)** | Basic agent using Azure Foundry with managed identity | 🟡 Intermediate | Azure Foundry, managed identity, `AzureCliCredential` |
| **[MAF-AIFoundry-02](MAF-AIFoundry-02/)** | Advanced Foundry integration with AI Projects API | 🟡 Intermediate | `AIProjectClient`, Foundry-specific APIs, advanced setup |
| **[MAF-AIFoundryAgents-01](MAF-AIFoundryAgents-01/)** | Creating and managing agents in Microsoft Foundry | 🔴 Advanced | Persistent agents, Foundry resource management, agent lifecycle |

**Setup:**
```bash
az login  # Authenticate with Azure CLI
dotnet user-secrets set "azureFoundryProjectEndpoint" "https://<resource>.services.ai.azure.com/"
dotnet user-secrets set "AzureOpenAI:Deployment" "gpt-5-mini"
```

---

### 3. Microsoft Foundry Agents

Deploy persistent agents in Microsoft Foundry with full lifecycle management.

| Sample | Description | Difficulty | Key Concepts |
|--------|-------------|------------|--------------|
| **[MAF-MicrosoftFoundryAgents-01](MAF-MicrosoftFoundryAgents-01/)** | Create agents in Microsoft Foundry with conversation loop | 🟡 Intermediate | `AIProjectClient`, agent persistence, interactive chat |
| **[MAF-MicrosoftFoundryAgents-02](MAF-MicrosoftFoundryAgents-02/)** | Advanced agent management and orchestration | 🔴 Advanced | Agent scaling, resource management, advanced workflows |

**Run:**
```bash
cd samples/MAF/MAF-MicrosoftFoundryAgents-01
dotnet user-secrets set "azureFoundryProjectEndpoint" "https://<resource>.services.ai.azure.com/"
dotnet user-secrets set "agentName" "my-agent"
dotnet run
```

---

### 4. Multi-Agent Workflows

Orchestrate multiple specialized agents working together.

| Sample | Description | Difficulty | Key Concepts |
|--------|-------------|------------|--------------|
| **[MAF-MultiAgents](MAF-MultiAgents/)** | Three agents (Researcher, Writer, Reviewer) using multiple providers | 🔴 Advanced | Multi-provider orchestration, sequential workflows, OpenTelemetry, Foundry agents |
| **[MAF-MultiAgents-Factory-01](MAF-MultiAgents-Factory-01/)** | Agent factory pattern for dynamic agent creation | 🔴 Advanced | Factory design pattern, agent composition, configuration-driven setup |
| **[MAF-MultiModel](MAF-MultiModel/)** | Researcher, Writer, and Reviewer agents with mixed providers | 🔴 Advanced | Azure OpenAI + Ollama, workflow composition, multi-model inference |

**Run:**
```bash
cd samples/MAF/MAF-MultiAgents
dotnet user-secrets set "AZURE_FOUNDRY_PROJECT_ENDPOINT" "https://<resource>.services.ai.azure.com/"
dotnet user-secrets set "endpoint" "https://<openai-resource>.openai.azure.com/"
dotnet user-secrets set "apikey" "<your-key>"
ollama pull llama3.2
dotnet run
```

---

### 5. Background Responses & Streaming

Demonstrate streaming responses and background processing.

| Sample | Description | Difficulty | Key Concepts |
|--------|-------------|------------|--------------|
| **[MAF-BackgroundResponses-01-Simple](MAF-BackgroundResponses-01-Simple/)** | Stream responses with continuation tokens | 🟡 Intermediate | `RunStreamingAsync()`, continuation tokens, response management |
| **[MAF-BackgroundResponses-02-Tools](MAF-BackgroundResponses-02-Tools/)** | Streaming with tool integration | 🔴 Advanced | Streaming + tool calling, token continuation with functions |
| **[MAF-BackgroundResponses-03-Complex](MAF-BackgroundResponses-03-Complex/)** | Advanced streaming with complex workflows | 🔴 Advanced | Complex streaming scenarios, error handling, performance optimization |

**Run:**
```bash
cd samples/MAF/MAF-BackgroundResponses-01-Simple
dotnet user-secrets set "endpoint" "https://<your-endpoint>.openai.azure.com/"
dotnet user-secrets set "apikey" "<your-api-key>"
dotnet run
```

---

### 6. Conversation Persistence

Save and resume agent conversations across sessions.

| Sample | Description | Difficulty | Key Concepts |
|--------|-------------|------------|--------------|
| **[MAF-Persisting-01-Simple](MAF-Persisting-01-Simple/)** | Serialize and deserialize agent threads | 🟢 Beginner | `AgentThread`, serialization, `DeserializeThread()` |
| **[MAF-Persisting-02-Menu](MAF-Persisting-02-Menu/)** | Interactive menu-driven persistence demo | 🟡 Intermediate | File-based persistence, session management, user interaction |
| **[MAF-FoundryClaude-Persisting-01](MAF-FoundryClaude-Persisting-01/)** | Persist Claude agent conversations | 🟡 Intermediate | Thread serialization with Claude, conversation resumption |

**Run:**
```bash
cd samples/MAF/MAF-Persisting-01-Simple
dotnet user-secrets set "endpoint" "https://<your-endpoint>.openai.azure.com/"
dotnet user-secrets set "apikey" "<your-api-key>"
dotnet run
```

---

### 7. Web Chat Applications

Full-featured Blazor Server web applications with chat interfaces.

| Sample | Description | Difficulty | Key Concepts |
|--------|-------------|------------|--------------|
| **[MAF-AIWebChatApp-Simple](MAF-AIWebChatApp-Simple/)** | Basic Blazor chat with OpenAI models | 🟡 Intermediate | Blazor Server, ASP.NET Aspire, OpenAI integration, web UI |
| **[MAF-AIWebChatApp-Middleware](MAF-AIWebChatApp-Middleware/)** | Web chat with custom middleware | 🟡 Intermediate | Middleware patterns, request/response processing, custom HTTP handlers |
| **[MAF-AIWebChatApp-MutliAgent](MAF-AIWebChatApp-MutliAgent/)** | Web interface for multi-agent workflows | 🔴 Advanced | Multi-agent web UI, workflow orchestration in web context |
| **[MAF-AIWebChatApp-AG-UI](MAF-AIWebChatApp-AG-UI/)** | Advanced UI with rich components | 🔴 Advanced | Complex Blazor components, rich UI interactions, performance optimization |
| **[MAF-AIWebChatApp-Persisting](MAF-AIWebChatApp-Persisting/)** | Web chat with conversation persistence | 🔴 Advanced | Web-based persistence, session management, state tracking |

**Run:**
```bash
cd samples/MAF/MAF-AIWebChatApp-Simple/ChatApp20
dotnet run  # Opens Aspire dashboard and web app
```

---

### 8. Image Generation

AI-powered image generation samples.

| Sample | Description | Difficulty | Key Concepts |
|--------|-------------|------------|--------------|
| **[MAF-ImageGen-01](MAF-ImageGen-01/)** | Basic image generation with Azure OpenAI | 🟡 Intermediate | DALL-E API, image generation, Azure OpenAI vision |
| **[MAF-ImageGen-02](MAF-ImageGen-02/)** | Advanced image generation with refinement | 🔴 Advanced | Image generation workflows, prompt engineering, advanced DALL-E features |

**Run:**
```bash
cd samples/MAF/MAF-ImageGen-01
dotnet user-secrets set "endpoint" "https://<your-endpoint>.openai.azure.com/"
dotnet user-secrets set "apikey" "<your-api-key>"
dotnet run
```

---

### 9. Claude Integration

Work with Anthropic Claude models via Microsoft Foundry.

| Sample | Description | Difficulty | Key Concepts |
|--------|-------------|------------|--------------|
| **[MAF-FoundryClaude-01](MAF-FoundryClaude-01/)** | Basic chat with Claude via Microsoft Foundry | 🟡 Intermediate | Claude integration, `AzureClaudeClient`, elbruno.Extensions.AI.Claude |
| **[MAF-AIWebChatApp-FoundryClaude](MAF-AIWebChatApp-FoundryClaude/)** | Blazor web chat with Claude models | 🟡 Intermediate | Web chat UI, Claude in Blazor, dependency injection, real-time chat |

**Setup:**
```bash
cd samples/MAF/MAF-FoundryClaude-01
dotnet user-secrets set "endpointClaude" "https://<resource>.services.ai.azure.com/anthropic/v1/messages"
dotnet user-secrets set "apikey" "<your-api-key>"
dotnet user-secrets set "deploymentName" "claude-haiku-4-5"
dotnet run
```

---

### 10. Hosted Agents (NEW)

Deploy agents as containers to Azure Foundry Agent Service.

| Sample | Description | Difficulty | Key Concepts |
|--------|-------------|------------|--------------|
| **[MAF-HostedAgent-01-TimeZone](MAF-HostedAgent-01-TimeZone/)** | Single agent with tool calling, containerized for Foundry | 🟡 Intermediate | Tool functions, Docker containerization, agent manifests, `agent.yaml` |
| **[MAF-HostedAgent-02-MultiAgent](MAF-HostedAgent-02-MultiAgent/)** | Multi-agent workflow for containerized deployment | 🔴 Advanced | Sequential workflows in containers, agent orchestration, Foundry deployment |

**Build and Run Locally:**
```bash
cd samples/MAF/MAF-HostedAgent-01-TimeZone
dotnet build
dotnet run

# Build Docker image
docker build -t timezone-agent:latest .

# Run in Docker
docker run -it --rm \
  -e AZURE_OPENAI_ENDPOINT="https://<your-resource>.openai.azure.com/" \
  -e AZURE_OPENAI_MODEL="gpt-4o-mini" \
  -e AZURE_OPENAI_APIKEY="<your-api-key>" \
  timezone-agent:latest
```

---

## Recommended Learning Path

### For Beginners
1. **MAF01** — Create your first agent
2. **MAF02** — Chain agents with workflows
3. **MAF-Ollama-01** — Work with local models
4. **MAF-Persisting-01-Simple** — Save conversations

### For Building Production Apps
1. **MAF-AIWebChatApp-Simple** — Build a web chat app
2. **MAF-MultiAgents** — Orchestrate multiple agents
3. **MAF-HostedAgent-01-TimeZone** — Deploy as a container
4. **MAF-AIFoundry-01** — Use Microsoft Foundry

### For Advanced Scenarios
1. **MAF-MultiModel** — Mix Azure OpenAI + Ollama
2. **MAF-FoundryClaude-01** — Integrate Claude
3. **MAF-BackgroundResponses-01-Simple** — Implement streaming
4. **MAF-HostedAgent-02-MultiAgent** — Complex hosted agents

---

## Common Tasks

### Create a Simple Agent
```csharp
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using Azure.AI.OpenAI;

var client = new AzureOpenAIClient(...).GetChatClient("gpt-4o-mini").AsIChatClient();

AIAgent agent = client.AsAIAgent(
    name: "MyAgent",
    instructions: "You are a helpful assistant...");

AgentResponse response = await agent.RunAsync("What is AI?");
Console.WriteLine(response.Text);
```

### Create a Multi-Agent Workflow
```csharp
using Microsoft.Agents.AI.Workflows;

AIAgent agent1 = ...;
AIAgent agent2 = ...;

Workflow workflow = AgentWorkflowBuilder.BuildSequential(agent1, agent2);
AIAgent workflowAgent = workflow.AsAIAgent();

AgentResponse result = await workflowAgent.RunAsync("Start processing...");
```

### Persist Agent Conversations
```csharp
var thread = agent.GetNewThread();
var response = await agent.RunAsync("Hello!", thread);

// Save thread
var json = thread.Serialize(JsonSerializerOptions.Web).GetRawText();
await File.WriteAllTextAsync("thread.json", json);

// Load thread later
var loaded = JsonSerializer.Deserialize<JsonElement>(await File.ReadAllTextAsync("thread.json"));
thread = agent.DeserializeThread(loaded, JsonSerializerOptions.Web);
response = await agent.RunAsync("Continue...", thread);
```

### Add Tool Functions
```csharp
AIAgent agent = client.AsAIAgent(
    name: "ToolAgent",
    instructions: "You are helpful...",
    tools: [
        AIFunctionFactory.Create(GetWeather),
        AIFunctionFactory.Create(GetTime)
    ]);

static string GetWeather(string location) => $"Sunny in {location}";
static string GetTime() => DateTime.Now.ToString();
```

---

## Key Resources

### Official Documentation
- **[Microsoft Agent Framework](https://learn.microsoft.com/agent-framework/)** — Complete MAF documentation
- **[Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/ai-extensions)** — AI extensions guide
- **[Azure Foundry Agents](https://learn.microsoft.com/azure/ai-foundry/agents/)** — Foundry agent documentation

### Related Lessons
- **[Lesson 01: Introduction to Generative AI](../../01-IntroductionToGenerativeAI/)** — Foundational concepts
- **[Lesson 02: Generative AI Techniques](../../02-GenerativeAITechniques/)** — Advanced patterns
- **[Lesson 04: Agents with MAF](../../04-AgentsWithMAF/)** — Agent-specific content

### Quick Links
- **[Setup Azure OpenAI](../../01-IntroductionToGenerativeAI/setup-azure-openai.md)**
- **[Setup Ollama](../../01-IntroductionToGenerativeAI/setup-local-ollama.md)**
- **[Project README](../../README.md)** — Course overview
- **[CONTRIBUTING.MD](../../CONTRIBUTING.MD)** — Contribution guidelines

---

## Troubleshooting

### Authentication Errors

**Problem:** `InvalidOperationException: Missing configuration`
- **Solution:** Check user secrets are set correctly: `dotnet user-secrets list`
- For Azure: `az login` to authenticate

**Problem:** `403 Forbidden` from Azure OpenAI
- **Solution:** Verify API key, endpoint, and deployment name match your Azure resource

### Provider Issues

**Problem:** Ollama connection refused
- **Solution:** Ensure Ollama is running: `ollama run llama3.2`

**Problem:** Claude model not found
- **Solution:** Verify Claude is deployed in Microsoft Foundry and deployment name is correct

### Build/Run Issues

**Problem:** `.csproj` build fails
- **Solution:** Run `dotnet restore` first, then `dotnet build`

**Problem:** Docker build fails
- **Solution:** Ensure Docker is installed and running: `docker --version`

---

## Contributing

Found an issue or have an improvement? Please see [CONTRIBUTING.MD](../../CONTRIBUTING.MD) for guidelines.

## License

These samples are part of the **Generative AI for Beginners .NET** course and follow the repository's **MIT License**.

---

**Happy building! 🚀**
