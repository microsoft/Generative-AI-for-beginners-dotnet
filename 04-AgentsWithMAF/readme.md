# Lesson 4: AI Agents with Microsoft Agent Framework

Everything you've learned (chat, function calling, RAG, vision) comes together here. **Agents** are the natural evolution of AI applications: systems that don't just respond, but reason, plan, and act.

---

## What You'll Learn

| Concept | What It Means |
|---------|---------------|
| **Agents vs Chatbots** | Why agents are fundamentally different from chat applications |
| **Agent Framework** | Microsoft's unified foundation for building AI agents |
| **Tools & Actions** | How agents call functions and interact with external systems |
| **Multi-Agent Workflows** | Orchestrating multiple specialized agents together |
| **Model Context Protocol (MCP)** | Extending agent capabilities with standardized tool servers |

---

## From Chatbot to Agent

Think about the difference:

| Chatbot | Agent |
|---------|-------|
| Responds to questions | Accomplishes objectives |
| Stateless conversation | Maintains context and memory |
| You provide all information | Retrieves information autonomously |
| Single interaction | Multi-step reasoning |
| Follows your instructions | Plans and executes on its own |

**A chatbot answers. An agent acts.**

---

## The Agent Architecture

An agent combines everything we've learned:

```
                    ┌─────────────────────────────────────┐
                    │           AI AGENT                  │
                    │                                     │
User Input ────────►│  ┌─────────────────────────────┐   │
                    │  │   LLM (Reasoning Engine)    │   │
                    │  │   - Understands context     │   │
                    │  │   - Plans next steps        │   │
                    │  │   - Decides which tools     │   │
                    │  └─────────────────────────────┘   │
                    │           │         │              │
                    │     ┌─────┴─────┬───┴────┐         │
                    │     ▼           ▼        ▼         │
                    │  ┌─────┐   ┌─────┐   ┌─────┐       │
                    │  │Tools│   │Memory│  │Context│     │
                    │  └─────┘   └─────┘   └─────┘       │
                    │                                     │
                    └─────────────────────────────────────┘
                                     │
                                     ▼
                              Response + Actions
```

**Learn more:** [What are agents?](https://learn.microsoft.com/dotnet/ai/conceptual/agents) on Microsoft Learn.

---

## Why Microsoft Agent Framework?

[Microsoft Agent Framework](https://github.com/microsoft/agent-framework) is an open-source development kit that:

- **Provides** a unified framework for building agents and multi-agent systems in .NET
- **Provides** enterprise-grade features (telemetry, filters, type safety)
- **Supports** multiple AI providers (Azure OpenAI, OpenAI, GitHub Models, Ollama)
- **Enables** both single agents and complex multi-agent workflows
- **Integrates** with Model Context Protocol for extended capabilities

**Learn more:** [Agent Framework Overview](https://learn.microsoft.com/agent-framework/overview/agent-framework-overview) provides a comprehensive introduction to the framework's architecture and capabilities.

```bash
# Install the Agent Framework
dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
```

---

## Lesson Structure

This lesson is divided into four parts:

### [Part 1: Building Your First Agent](./01-building-first-agent.md)
Create a simple agent, understand the `AIAgent` abstraction, and see how agents differ from raw chat completions.

### [Part 2: Agents with Tools](./02-agents-with-tools.md)
Give your agent the ability to take actions (query databases, call APIs, execute code) through function tools.

### [Part 3: Multi-Agent Workflows](./03-multi-agent-workflows.md)
Orchestrate multiple specialized agents working together: sequential pipelines, handoffs, and group collaboration.

### [Part 4: Model Context Protocol (MCP)](./04-model-context-protocol.md)
Extend your agents with standardized tool servers, enabling integration with external systems and services.

---

## Sample Code Reference

All lesson code is in the [samples/AgentFx](../samples/AgentFx/) folder:

| Sample | Description |
|--------|-------------|
| [AgentFx01](../samples/AgentFx/AgentFx01/) | Basic agent with GitHub Models |
| [AgentFx02](../samples/AgentFx/AgentFx02/) | Sequential workflow (writer → editor) |
| [AgentFx-Ollama-01](../samples/AgentFx/AgentFx-Ollama-01/) | Agent using local Ollama model |
| [AgentFx-MultiAgents](../samples/AgentFx/AgentFx-MultiAgents/) | Multi-model orchestration demo |
| [AgentFx-BackgroundResponses-*](../samples/AgentFx/) | Agents with tools and async responses |
| [AgentFx-Persisting-*](../samples/AgentFx/) | Agents with conversation persistence |

---

## Prerequisites

Before starting this lesson, ensure you have:

- Completed Lessons 1-3 (or understand chat, function calling, and RAG concepts)
- .NET 9 SDK installed
- Access to at least one AI provider:
  - GitHub Models (recommended) - Free tier available
  - Azure OpenAI - Requires Azure subscription
  - Ollama - For local agent execution

---

## Let's Begin

Start with the fundamentals:

[Continue to Part 1: Building Your First Agent →](./01-building-first-agent.md)
