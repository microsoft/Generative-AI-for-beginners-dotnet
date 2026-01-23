# Generative AI for Beginners .NET - Sample Code

This directory contains all the code samples for the **Generative AI for Beginners .NET** workshop. The samples are organized into four main categories based on their purpose and complexity.

---

## Directory Structure

```text
samples/
├── CoreSamples/          # Core AI techniques and fundamental concepts
├── AgentFx/              # Microsoft Agent Framework samples
├── PracticalSamples/     # Real-world practical applications
└── AppsWithGenAI/        # Applications created with GenAI assistance
```

---

## CoreSamples

**Location:** [`samples/CoreSamples/`](./CoreSamples/)  
**Associated Lessons:** [Lesson 02](../02-SetupDevEnvironment/) & [Lesson 03](../03-CoreGenerativeAITechniques/)

This folder contains fundamental samples demonstrating core Generative AI techniques with .NET. These samples cover the essential building blocks for AI-enabled applications and are referenced throughout Lessons 02 and 03.

### What you'll find in CoreSamples

- **Chat & Completions** - Basic text generation and conversational AI using various providers (GitHub Models, Azure OpenAI, Ollama) and frameworks (Microsoft.Extensions.AI, Semantic Kernel)
- **Function Calling** - Extending AI capabilities by allowing models to call custom functions and external tools
- **Retrieval-Augmented Generation (RAG)** - Integrating external knowledge stores with AI models using different vector databases (in-memory, Azure AI Search, Qdrant)
- **Vision & Audio** - Image analysis, speech-to-text, and real-time audio conversations
- **Agents** - Basic agent implementations with custom functions and OpenAPI integrations
- **Image & Video Generation** - Creating visual content with DALL-E and Sora models
- **Model Context Protocol (MCP)** - Integrating with external services using MCP
- **Local Model Runners** - Running AI models locally using AI Toolkit, Docker, and Microsoft Foundry Local

This folder includes over 40 samples demonstrating different techniques, providers, and integration patterns.

---

## AgentFx

**Location:** [`samples/AgentFx/`](./AgentFx/)  
**Associated Lesson:** [Lesson 06](../06-AgentFx/)

This folder contains samples demonstrating the **Microsoft Agent Framework** for building sophisticated multi-agent AI systems.

### What you'll find in AgentFx

- **Basic Samples** - Single and dual-agent workflows demonstrating fundamental agent creation and orchestration patterns
- **Advanced Multi-Provider Samples** - Complex workflows combining multiple AI providers (Azure OpenAI, GitHub Models, Ollama) in sophisticated agent orchestrations
- **Integration & Web Samples** - Web-based chat applications with Blazor UI, middleware patterns, and MCP integrations for tool usage (e.g., image generation)

This folder includes 12 samples ranging from simple single-agent scenarios to complex multi-agent systems with persistent state and web interfaces.

## PracticalSamples

**Location:** [`samples/PracticalSamples/`](./PracticalSamples/)  
**Associated Lesson:** [Lesson 04](../04-PracticalSamples/)

This folder contains practical, real-world samples demonstrating AI integration patterns in production-ready scenarios.

### What you'll find in PracticalSamples

A comprehensive **.NET Aspire MCP Sample** showcasing the **Model Context Protocol (MCP)** integration with a complete multi-project solution including:

- .NET Aspire orchestration for managing distributed services
- Blazor-based chat interface for user interactions
- ASP.NET Core backend integrating with MCP servers
- Shared service configurations and defaults

**Run the sample:**

```bash
cd samples/PracticalSamples/src
dotnet run --project McpSample.AppHost/McpSample.AppHost.csproj
```

---

## AppsWithGenAI

**Location:** [`samples/AppsWithGenAI/`](./AppsWithGenAI/)  
**Associated Lesson:** [Lesson 05](../05-AppCreatedWithGenAI/)

This folder showcases complete, production-quality applications created with the assistance of Generative AI tools like GitHub Copilot Agent, demonstrating "vibe coding" - building real software through AI-assisted development.

### What you'll find

- **SpaceAINet** - A retro space invaders console game with AI-powered enemy behavior that can analyze game state in real-time and make strategic decisions using Azure OpenAI or Ollama
- **HFMCP.GenImage** - An image generation application using Hugging Face's MCP Server, demonstrating multi-modal AI capabilities with .NET Aspire orchestration
- **ConsoleGpuViewer** - A GPU diagnostics and monitoring tool that integrates with local AI models for intelligent hardware analysis

Each application is fully functional and can be run independently with proper AI provider configuration.

---

## Getting Started

### Prerequisites

- **.NET 9 SDK** or later
- **AI Provider Access** (at least one):
  - GitHub Personal Access Token for GitHub Models
  - Microsoft Foundry endpoint (with managed identity or API key)
  - Ollama installed locally

### Quick Start by Category

> **Note**: Use the appropriate path separator for your platform:
>
> - **Windows (CMD/PowerShell)**: Use backslashes `\` (e.g., `samples\CoreSamples\BasicChat-01MEAI`)
> - **Linux/macOS/Git Bash/WSL**: Use forward slashes `/` (e.g., `samples/CoreSamples/BasicChat-01MEAI`)
> - **GitHub Codespaces**: Always use forward slashes `/` (runs Linux environment)

#### 1. **Learning Core Concepts** (Start here!)

```bash
cd samples/CoreSamples/BasicChat-01MEAI
dotnet run
```

#### 2. **Building Agents**

```bash
cd samples/AgentFx/AgentFx01
dotnet run
```

#### 3. **Practical Applications**

```bash
cd samples/PracticalSamples/src
dotnet run --project McpSample.AppHost
```

#### 4. **Complete Applications**

```bash
cd samples/AppsWithGenAI/SpaceAINet/SpaceAINet.Console
dotnet run
```

---

## Sample Organization by Lesson

| Lesson | Sample Folders | Key Topics |
|--------|---------------|------------|
| [Lesson 01: Introduction](../01-IntroductionToGenerativeAI/) | (Setup guides) | Environment setup, concepts |
| [Lesson 02: Techniques](../02-GenerativeAITechniques/) | CoreSamples/BasicChat-*, MEAIFunctions | Chat, streaming, function calling |
| [Lesson 03: Patterns](../03-AIPatternsAndApplications/) | CoreSamples/RAG*, Vision-*, Audio-* | RAG, embeddings, vision, audio |
| [Lesson 04: Agents](../04-AgentsWithMAF/) | AgentFx/* | Microsoft Agent Framework, MCP |
| [Lesson 05: Responsible AI](../05-ResponsibleAI/) | (Conceptual) | Ethics, safety, transparency |

---

## Additional Resources

- [Main Course README](../README.md)
- [Glossary](../GLOSSARY.md)
- [Microsoft.Extensions.AI Documentation](https://learn.microsoft.com/dotnet/ai/ai-extensions)
- [Semantic Kernel Documentation](https://learn.microsoft.com/semantic-kernel/)
- [Microsoft Agent Framework](https://learn.microsoft.com/agent-framework/)

- **Web chat with persisted conversations** - `AgentFx-AIWebChatApp-Persisting` demonstrates per-user `AgentThread` serialization, storage, and resume across restarts
