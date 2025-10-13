# Microsoft Agent Framework (AgentFx)

Build powerful, orchestrated AI agents with Microsoft Agent Framework - the next evolution in .NET AI development.

---

## What you'll learn in this lesson

- 🤖 Understanding the Microsoft Agent Framework and its architecture
- 🔗 Creating and orchestrating multiple AI agents
- 🌐 Working with different AI providers (GitHub Models, Azure AI Foundry, Ollama)
- 🔄 Building sequential and concurrent agent workflows
- 🛠️ Integrating with Model Context Protocol (MCP) for enhanced capabilities

## Introduction to Microsoft Agent Framework

The Microsoft Agent Framework (AgentFx) is a powerful new framework for building AI agent systems in .NET. It provides a structured way to create, orchestrate, and manage AI agents that can work together to solve complex problems.

Unlike traditional single-model AI applications, AgentFx enables you to:

- **Create Specialized Agents**: Build agents with specific roles and expertise
- **Orchestrate Workflows**: Combine multiple agents in sequential or parallel workflows
- **Multi-Model Support**: Use different AI models for different tasks within the same workflow
- **Seamless Integration**: Work with various AI providers including Azure OpenAI, GitHub Models, and local models via Ollama

> 🧑‍🏫 **Learn more**: Explore the [Microsoft Agent Framework Overview](https://learn.microsoft.com/agent-framework/overview/agent-framework-overview) for in-depth documentation.

## Understanding AI Agents vs. Single-Model Interactions

Before diving into AgentFx, it's important to understand how agents differ from traditional AI model interactions:

### Traditional Single-Model Approach

```
User Input → AI Model → Response
```

In this approach, you send a prompt to an AI model and receive a response. This works well for simple tasks but becomes limited when dealing with complex, multi-step problems.

### Agent-Based Approach

```
User Input → Agent 1 (Research) → Agent 2 (Analysis) → Agent 3 (Writing) → Final Output
```

With agents, you can break down complex tasks into specialized steps, with each agent focusing on what it does best. Agents can also use tools, access external data, and make decisions about when to hand off work to other agents.

## Key Concepts in Microsoft Agent Framework

### 1. AIAgent

The core building block of AgentFx is the `AIAgent` class. Each agent has:

- **Name**: A unique identifier for the agent
- **Instructions**: System prompt defining the agent's behavior and expertise
- **Chat Client**: The underlying AI model (via `IChatClient` from Microsoft.Extensions.AI)

### 2. Agent Types

AgentFx provides several agent types:

- **ChatClientAgent**: A basic agent that wraps an `IChatClient`
- **Custom Agents**: You can extend the base `AIAgent` class to create specialized agents with custom logic

### 3. Agent Workflows

Agents can be orchestrated in different patterns:

- **Sequential Workflows**: Agents execute one after another, with each agent building on the previous agent's output
- **Concurrent Workflows**: Multiple agents work in parallel (future capability)
- **Conditional Workflows**: Agents decide the next step based on their output

## Getting Started with AgentFx

### Prerequisites

Before starting with AgentFx, ensure you have:

- **.NET 9 SDK** or later
- **AI Provider Access**: At least one of the following:
  - GitHub Token for GitHub Models
  - Azure AI Foundry endpoint using managed identities o using an API key
  - Ollama installed locally

### Installation

The Microsoft Agent Framework is available via NuGet:

```bash
dotnet add package Microsoft.Agents.AI
```

You'll also need the Microsoft.Extensions.AI package for chat client support:

```bash
dotnet add package Microsoft.Extensions.AI
```

## Practical Examples

This lesson includes several code samples demonstrating different aspects of AgentFx:

### Sample 1: Basic Single Agent ([AgentFx01](./src/AgentFx01/))

This sample demonstrates the simplest agent setup - a single "Writer" agent that creates stories.

**Key Concepts**:

- Creating a `ChatClientAgent`
- Configuring agent instructions
- Running a simple agent task

**How to run**:

```bash
cd 06-AgentFx/src/AgentFx01
dotnet user-secrets set "GITHUB_TOKEN" "your-github-token"
dotnet run
```

### Sample 2: Multi-Model Agent Workflow ([AgentFx-MultiModel](./src/AgentFx-MultiModel/))

This advanced sample shows how to orchestrate three different agents using three different AI providers in a sequential workflow.

**Workflow**:

1. **Researcher Agent** (GitHub Models) - Gathers information
2. **Writer Agent** (Azure AI Foundry) - Creates content
3. **Reviewer Agent** (Ollama local) - Provides feedback

**Key Concepts**:

- Multi-model orchestration
- Sequential agent workflows
- Provider-agnostic agent design

See the [detailed README](./src/AgentFx-MultiModel/README.md) for setup and configuration.

### Sample 3: Agent with Azure AI Foundry ([AgentFx-AIFoundry-01](./src/AgentFx-AIFoundry-01/))

Demonstrates how to create an agent using Azure AI Foundry as the provider.

**How to run**:

```bash
cd 06-AgentFx/src/AgentFx-AIFoundry-01
dotnet user-secrets set "endpoint" "https://<your-endpoint>.services.ai.azure.com/"
dotnet user-secrets set "apikey" "your-azure-api-key"
dotnet run
```

### Sample 4: Agent with Ollama ([AgentFx-Ollama-01](./src/AgentFx-Ollama-01/))

Shows how to use local AI models via Ollama with AgentFx.

**Prerequisites**:

```bash
# Install Ollama from https://ollama.com
ollama pull llama3.2
ollama run llama3.2
```

**How to run**:

```bash
cd 06-AgentFx/src/AgentFx-Ollama-01
dotnet run
```

### Sample 5: Agent with Image Generation ([AgentFx-ImageGen-01](./src/AgentFx-ImageGen-01/))

This sample demonstrates how agents can interact with tools like the Hugging Face MCP Server for image generation.

**Key Concepts**:

- Agent tool integration
- Model Context Protocol (MCP) usage
- Multi-modal agent capabilities

## Agent Workflows in Detail

### Sequential Workflow Pattern

A sequential workflow is the most common pattern, where agents execute in order:

```csharp
// Pseudo-code example
var researcher = new ChatClientAgent(researchClient, new() {
    Name = "Researcher",
    Instructions = "Research the given topic thoroughly."
});

var writer = new ChatClientAgent(writerClient, new() {
    Name = "Writer", 
    Instructions = "Write an engaging article based on research."
});

var reviewer = new ChatClientAgent(reviewerClient, new() {
    Name = "Reviewer",
    Instructions = "Review the article and provide constructive feedback."
});

// Execute in sequence
var researchResult = await researcher.RunAsync(userTopic);
var articleResult = await writer.RunAsync(researchResult.Text);
var reviewResult = await reviewer.RunAsync(articleResult.Text);
```

### Benefits of Multi-Agent Workflows

1. **Specialization**: Each agent can use a different model optimized for its task
2. **Cost Optimization**: Use expensive models only where needed
3. **Quality Control**: Agents can review and improve each other's work
4. **Flexibility**: Easy to add, remove, or modify agents in the workflow

## Integration with Model Context Protocol (MCP)

The Model Context Protocol (MCP) provides a standardized way for agents to interact with external tools and services. AgentFx seamlessly integrates with MCP to enable agents to:

- Access external APIs
- Generate images with services like Hugging Face
- Query databases
- Interact with file systems
- And more

> 🧑‍💻 **Sample code**: Check out the [AgentFx-ImageGen-01 sample](./src/AgentFx-ImageGen-01/) for a practical MCP integration example.

> 🧑‍🏫 **Learn more**: Explore the [Model Context Protocol C# SDK](https://github.com/modelcontextprotocol/csharp-sdk) for detailed information.

## Best Practices

When building with Microsoft Agent Framework, keep these best practices in mind:

### 1. Clear Agent Instructions

Write specific, clear instructions for each agent:

```csharp
Instructions = "You are a technical writer. Write clear, concise documentation " +
               "for a technical audience. Use examples and code snippets where appropriate."
```

### 2. Agent Naming

Use descriptive names that reflect the agent's role:

```csharp
Name = "TechnicalWriter"  // Good
Name = "Agent1"           // Avoid, please no!
```

### 3. Provider Selection

Choose AI providers based on your needs:

- **GitHub Models**: Quick prototyping and development
- **Azure AI Foundry**: Production workloads with enterprise features
- **Ollama**: Local development, privacy-sensitive data

### 4. Error Handling

Always handle agent failures gracefully:

```csharp
try
{
    var response = await agent.RunAsync(input);
    Console.WriteLine(response.Text);
}
catch (Exception ex)
{
    Console.WriteLine($"Agent failed: {ex.Message}");
}
```

### 5. Configuration Management

Use user secrets or environment variables for API keys:

```bash
dotnet user-secrets set "GITHUB_TOKEN" "your-token"
dotnet user-secrets set "endpoint" "your-endpoint"
dotnet user-secrets set "apikey" "your-key"
```

## Comparison with Semantic Kernel Agents

If you're familiar with Semantic Kernel agents from Lesson 3, you might wonder how AgentFx differs:

| Feature | Semantic Kernel Agents | Microsoft Agent Framework |
|---------|----------------------|---------------------------|
| **Purpose** | Plugin-based agent orchestration | Specialized agent workflows |
| **Complexity** | Higher-level abstractions | Lower-level control |
| **Use Case** | Complex reasoning, planning | Task-specific agent chains |
| **Learning Curve** | Steeper | Gentler |
| **Flexibility** | Very flexible with plugins | Focused on agent orchestration |

Both frameworks have their place:

- Use **Semantic Kernel** for complex, reasoning-heavy applications with many plugins
- Use **AgentFx** for focused, multi-step workflows with specialized agents

## Real-World Use Cases

Microsoft Agent Framework excels in scenarios like:

### 1. Content Creation Pipeline

- Research agent gathers information
- Writing agent creates draft
- Editor agent refines content
- SEO agent optimizes for search

### 2. Customer Support System

- Triage agent categorizes issues
- Research agent finds relevant documentation
- Response agent formulates answers
- Quality agent reviews responses

### 3. Data Analysis Workflow

- Extraction agent pulls data from sources
- Analysis agent processes and identifies patterns
- Visualization agent creates charts
- Summary agent generates insights

### 4. Software Development Assistant

- Requirements agent clarifies specifications
- Design agent creates architecture
- Implementation agent generates code
- Testing agent creates test cases
- Review agent checks quality

## Summary

In this lesson, you learned about the Microsoft Agent Framework and how to build powerful multi-agent systems in .NET. You explored:

- The core concepts of AgentFx
- How to create and configure agents
- Multi-model agent orchestration
- Integration with different AI providers
- Real-world workflow patterns
- Best practices for agent development

The Microsoft Agent Framework represents the next evolution in .NET AI development, enabling you to build sophisticated, specialized agent systems that can tackle complex, multi-step problems.

## Additional Resources

- [Microsoft Agent Framework Overview](https://learn.microsoft.com/agent-framework/overview/agent-framework-overview)
- [Model Context Protocol C# SDK](https://github.com/modelcontextprotocol/csharp-sdk)
- [Microsoft.Extensions.AI Documentation](https://learn.microsoft.com/dotnet/ai/ai-extensions)
- [Azure AI Foundry](https://ai.azure.com/)
- [GitHub Models](https://github.com/marketplace/models)
- [Ollama Documentation](https://ollama.ai/docs)

## Next Steps

Ready to put your agent skills into practice?

👉 [Continue to Lesson 07 - Responsible Use of Generative AI](../09-ResponsibleGenAI/readme.md)

Or explore more advanced agent scenarios:

- [Practical .NET Generative AI Samples](../04-PracticalSamples/readme.md)
- [Apps Created with GenAI Tools](../05-AppCreatedWithGenAI/readme.md)
