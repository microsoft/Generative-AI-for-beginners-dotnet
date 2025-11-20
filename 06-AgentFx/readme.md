# Microsoft Agent Framework (AgentFx)

Build powerful, orchestrated AI agents with Microsoft Agent Framework - the next evolution in .NET AI development.

---

## What you'll learn in this lesson

- 🤖 Understanding the Microsoft Agent Framework and its architecture
- 🔗 Creating and orchestrating multiple AI agents
- 🌐 Working with different AI providers (GitHub Models, Microsoft Foundry, Ollama)
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

```text
User Input → AI Model → Response
```

In this approach, you send a prompt to an AI model and receive a response. This works well for simple tasks but becomes limited when dealing with complex, multi-step problems.

### Agent-Based Approach

```text
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
  - Microsoft Foundry endpoint using managed identities o using an API key
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

## Code Samples

This lesson includes multiple code samples demonstrating different aspects of the Microsoft Agent Framework. Each sample is self-contained and can be run independently.

### Basic Samples

| Sample | Description | Key Concepts | Provider |
|--------|-------------|-------------|----------|
| [AgentFx01](../samples/AgentFx/AgentFx01/) | Single agent that writes creative stories | Basic agent setup, ChatClientAgent | GitHub Models |
| [AgentFx02](../samples/AgentFx/AgentFx02/) | Two-agent workflow: Writer + Editor | Sequential workflows, agent chaining | GitHub Models |
| [AgentFx-AIFoundry-01](../samples/AgentFx/AgentFx-AIFoundry-01/) | Single agent using Microsoft Foundry | Azure CLI authentication, managed identity | Microsoft Foundry |
| [AgentFx-Ollama-01](../samples/AgentFx/AgentFx-Ollama-01/) | Single agent using local Ollama models | Local AI inference, privacy-focused | Ollama (local) |
| [AgentFx-BackgroundResponses-01-Simple](../samples/AgentFx/AgentFx-BackgroundResponses-01-Simple/) | Background responses with continuation tokens | Streaming interruption, response continuation | Configurable |
| [AgentFx-BackgroundResponses-02-Tools](../samples/AgentFx/AgentFx-BackgroundResponses-02-Tools/) | Background responses with tool integration | Tool usage during background processing | Configurable |
| [AgentFx-BackgroundResponses-03-Complex](../samples/AgentFx/AgentFx-BackgroundResponses-03-Complex/) | Complex background responses with multiple queries | Interleaved queries, continuation management | Configurable |
| [AgentFx-Persisting-01-Simple](../samples/AgentFx/AgentFx-Persisting-01-Simple/) | Persisting conversations: serialize and resume AgentThread | Thread serialization, resume across restarts | Configurable |
| [AgentFx-Persisting-02-Menu](../samples/AgentFx/AgentFx-Persisting-02-Menu/) | Interactive console for persisting and resuming AgentThreads | Menu-driven persist/load flow, Temp storage demo | Configurable |

### Advanced Multi-Provider Samples

| Sample | Description | Key Concepts | Providers |
|--------|-------------|-------------|-----------|
| [AgentFx-MultiModel](../samples/AgentFx/AgentFx-MultiModel/) | Three-agent workflow across multiple providers | Multi-provider orchestration, OpenTelemetry tracing | GitHub Models or Azure OpenAI + Ollama |
| [AgentFx-MultiAgents](../samples/AgentFx/AgentFx-MultiAgents/) | Researcher-Writer-Reviewer workflow with persistent agents | Microsoft Foundry persistent agents, flexible configuration | Microsoft Foundry + Azure OpenAI/GitHub Models + Ollama |

### Integration & Web Samples

| Sample | Description | Key Concepts | Provider |
|--------|-------------|-------------|----------|
| [AgentFx-ImageGen-01](../samples/AgentFx/AgentFx-ImageGen-01/) | Agent that generates images using Hugging Face MCP | MCP integration, tool usage, multi-modal | GitHub Models or Azure OpenAI |
| [AgentFx-AIWebChatApp-Simple](../samples/AgentFx/AgentFx-AIWebChatApp-Simple/) | Simple web-based chat application | Web integration, Blazor UI | Configurable |
| [AgentFx-AIWebChatApp-Middleware](../samples/AgentFx/AgentFx-AIWebChatApp-Middleware/) | Web chat with middleware pattern | Custom middleware, request processing | Configurable |
| [AgentFx-AIWebChatApp-MutliAgent](../samples/AgentFx/AgentFx-AIWebChatApp-MutliAgent/) | Web chat with multiple agents | Multi-agent web apps, agent selection | Configurable |
| [AgentFx-AIWebChatApp-Persisting](../samples/AgentFx/AgentFx-AIWebChatApp-Persisting/) | Web chat with persisted conversation threads | Thread serialization, per-session resume | Configurable |

### Quick Start Guide

**For beginners**, start with:

1. **AgentFx01** - Learn basic agent creation
2. **AgentFx02** - Understand agent workflows
3. **AgentFx-Ollama-01** - Try local models
4. **AgentFx-BackgroundResponses-01-Simple** - Basic background responses

**For advanced scenarios**, explore:

1. **AgentFx-MultiAgents** - See [detailed README](../samples/AgentFx/AgentFx-MultiAgents/README.md) for multi-provider orchestration
2. **AgentFx-BackgroundResponses-03-Complex** - Advanced background response patterns
3. **AgentFx-ImageGen-01** - Learn MCP integration
4. **AgentFx-AIWebChatApp-MutliAgent** - Build web-based agent systems
5. **AgentFx-AIWebChatApp-Persisting** - Full web chat with durable thread history (per-session persistence)

### Running the Samples

Each sample includes configuration instructions in its source code comments. Generally:

> **Note**: Replace `<sample-folder>` with the specific sample directory name (e.g., `AgentFx01`).
> Use the appropriate path separator for your platform:

- **Windows (CMD/PowerShell)**: `cd samples\AgentFx\<sample-folder>`
- **Linux/macOS/Git Bash/WSL/Codespaces**: `cd samples/AgentFx/<sample-folder>`

**GitHub Models** (requires GitHub token):

Windows (CMD/PowerShell):

```bash
cd samples\AgentFx\<sample-folder>
dotnet user-secrets set "GITHUB_TOKEN" "your-github-token"
dotnet run
```

Linux/macOS/Git Bash/WSL/Codespaces:

```bash
cd samples/AgentFx/<sample-folder>
dotnet user-secrets set "GITHUB_TOKEN" "your-github-token"
dotnet run
```

**Microsoft Foundry** (requires Azure CLI login):

Windows (CMD/PowerShell):

```bash
cd samples\AgentFx\<sample-folder>
dotnet user-secrets set "endpoint" "https://<your-endpoint>.services.ai.azure.com/"
dotnet user-secrets set "deploymentName" "gpt-4o-mini"
az login
dotnet run
```

Linux/macOS/Git Bash/WSL/Codespaces:

```bash
cd samples/AgentFx/<sample-folder>
dotnet user-secrets set "endpoint" "https://<your-endpoint>.services.ai.azure.com/"
dotnet user-secrets set "deploymentName" "gpt-4o-mini"
az login
dotnet run
```

**Ollama** (requires local installation):

Windows (CMD/PowerShell):

```bash
# Install Ollama from https://ollama.com
ollama pull llama3.2
cd samples\AgentFx\<sample-folder>
dotnet run
```

Linux/macOS/Git Bash/WSL/Codespaces:

```bash
# Install Ollama from https://ollama.com
ollama pull llama3.2
cd samples/AgentFx/<sample-folder>
dotnet run
```

> **Note**: GitHub Codespaces runs a Linux environment, so always use forward slashes (`/`) in paths when working in Codespaces, regardless of your local operating system.

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

## Agent Background Responses

AgentFx supports background responses, allowing agents to continue generating output even after you stop listening to streaming updates. This feature is useful for:

- **Interrupting long-running responses** to handle immediate queries
- **Resuming generation** from where you left off using continuation tokens
- **Interleaving short queries** with long-running background tasks
- **Managing complex workflows** that require both real-time and background processing

Background responses enable more sophisticated agent interactions by letting you:

1. Start streaming a response from an agent
2. Capture a continuation token during streaming
3. Stop the stream to handle other tasks
4. Resume the original response later using the token

> 🧑‍💻 **Sample code**: Explore the background responses samples:
>
> - [AgentFx-BackgroundResponses-01-Simple](../samples/AgentFx/AgentFx-BackgroundResponses-01-Simple/) - Basic background response pattern
> - [AgentFx-BackgroundResponses-03-Complex](../samples/AgentFx/AgentFx-BackgroundResponses-03-Complex/) - Advanced workflows with interleaved queries
>
> 🧑‍🏫 **Learn more**: See the [Background Responses detailed guide](./README-BackgroundResponses.md) for comprehensive documentation and examples.

## Persisted Conversations

AgentFx supports persisting and resuming conversations using the `AgentThread` object. This feature enables you to:

- **Save conversation state** across sessions or after application restarts
- **Resume conversations** with full context preserved
- **Store conversations** in databases, files, or cloud storage
- **Maintain agent memory** for long-running interactions

Persisted conversations work by serializing the `AgentThread` to JSON, which can then be stored and later deserialized to continue the conversation.

### How It Works

1. **Create an agent and thread**: Initialize your agent and get a new thread
2. **Run conversations**: Execute prompts against the thread to build conversation history
3. **Serialize the thread**: Convert the thread state to JSON using `Serialize()` or `SerializeAsync()`
4. **Store the state**: Save the JSON to your preferred storage (file, database, blob storage)
5. **Deserialize and resume**: Load the JSON and rehydrate the thread using `DeserializeThread()`
6. **Continue the conversation**: Use the resumed thread to maintain full context

### Example

```csharp
// Create an agent
var agent = client.CreateAIAgent(
    name: "Assistant",
    instructions: "You are a helpful assistant");

// Create a new thread and run a conversation
var thread = agent.GetNewThread();
var response = await agent.RunAsync("My name is Bruno", thread);

// Serialize and save the thread
var threadJson = thread.Serialize(JsonSerializerOptions.Web).GetRawText();
await File.WriteAllTextAsync("agent_thread.json", threadJson);

// Later: Load and deserialize the thread
var loadedJson = await File.ReadAllTextAsync("agent_thread.json");
var reloaded = JsonSerializer.Deserialize<JsonElement>(loadedJson);
var resumedThread = agent.DeserializeThread(reloaded, JsonSerializerOptions.Web);

// Continue the conversation with context
var nextResponse = await agent.RunAsync("What is my name?", resumedThread);
// The agent remembers: "Your name is Bruno"
```

### Key Considerations

- **Agent consistency**: You must use the same agent type to deserialize threads, as different agents may have unique internal implementations
- **Storage flexibility**: The framework abstracts storage details - some agents (like Microsoft Foundry) persist history in the service, while others (like OpenAI chat completion) manage state in-memory
- **Production storage**: For production scenarios, use reliable storage like cloud databases or blob storage instead of local files

> 🧑‍💻 **Sample code**: Explore the persisted conversation samples:
>
> - [AgentFx-Persisting-01-Simple](../samples/AgentFx/AgentFx-Persisting-01-Simple/) - Basic thread serialization and resumption
> - [AgentFx-Persisting-02-Menu](../samples/AgentFx/AgentFx-Persisting-02-Menu/) - Interactive menu-driven persist/load workflow
> - [AgentFx-AIWebChatApp-Persisting](../samples/AgentFx/AgentFx-AIWebChatApp-Persisting/) - Blazor web chat demonstrating per-user AgentThread persistence and resume across restarts
>
> 🧑‍🏫 **Learn more**: See the official [Persisted Conversations tutorial](https://learn.microsoft.com/agent-framework/tutorials/agents/persisted-conversation) and [Agent Memory guide](https://learn.microsoft.com/agent-framework/user-guide/agents/agent-memory) for detailed documentation.

## Integration with Model Context Protocol (MCP)

The Model Context Protocol (MCP) provides a standardized way for agents to interact with external tools and services. AgentFx seamlessly integrates with MCP to enable agents to:

- Access external APIs
- Generate images with services like Hugging Face
- Query databases
- Interact with file systems
- And more

> 🧑‍💻 **Sample code**: Check out the [AgentFx-ImageGen-01 sample](../samples/AgentFx/AgentFx-ImageGen-01/) for a practical MCP integration example.

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
- **Microsoft Foundry**: Production workloads with enterprise features
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
- [Microsoft Foundry](https://ai.azure.com/)
- [GitHub Models](https://github.com/marketplace/models)
- [Ollama Documentation](https://ollama.ai/docs)

## Next Steps

Ready to put your agent skills into practice?

👉 [Continue to Lesson 07 - Responsible Use of Generative AI](../09-ResponsibleGenAI/readme.md)

Or explore more advanced agent scenarios:

- [Practical .NET Generative AI Samples](../04-PracticalSamples/readme.md)
- [Apps Created with GenAI Tools](../05-AppCreatedWithGenAI/readme.md)
