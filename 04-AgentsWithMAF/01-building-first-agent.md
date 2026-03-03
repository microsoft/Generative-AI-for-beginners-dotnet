# Building Your First Agent

In this lesson, you'll create your first AI agent using Microsoft Agent Framework and understand what makes agents different from simple chat applications.

---

[![Building Your First Agent](./images/LIM_GAN_08_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson4-first-agent)

_Click the image to watch the video_

---

## Chat vs Agent: What's the Difference?

Let's be concrete. Here's a chat completion:

```csharp
var response = await chatClient.GetResponseAsync("Tell me a joke about pirates");
Console.WriteLine(response.Text);
```

And here's an agent:

```csharp
AIAgent joker = chatClient.CreateAIAgent(
    instructions: "You are good at telling jokes.");

var response = await joker.RunAsync("Tell me a joke about pirates");
Console.WriteLine(response.Text);
```

They look similar, but there are key differences:

| Chat Completion | Agent |
|-----------------|-------|
| Stateless - each call is independent | Maintains conversation thread |
| You manage the message history | Agent manages context internally |
| You handle tool calling manually | Agent handles tool invocation automatically |
| You write the orchestration logic | Agent reasons and decides next steps |

The agent abstracts away the complexity of managing conversations, tools, and multi-step reasoning.

---

## Part 1: Your First Agent

### Install the Agent Framework

```bash
dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
```

### Create a Simple Agent

```csharp
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

// Create a chat client (same as before)
IChatClient chatClient = new AzureOpenAIClient(
        new Uri(config["endpoint"]),
        new ApiKeyCredential(config["apikey"]))
    .GetChatClient("gpt-4o-mini")
    .AsIChatClient();

// Create an agent from the chat client
AIAgent writer = chatClient.CreateAIAgent(
    name: "Writer",
    instructions: "Write stories that are engaging and creative.");

// Run the agent
AgentRunResponse response = await writer.RunAsync(
    "Write a short story about a haunted house with a character named Lucia.");

Console.WriteLine(response.Text);
```

That's it! The `CreateAIAgent` extension method transforms any `IChatClient` into an agent.

---

## Part 2: Understanding AIAgent

The `AIAgent` class is the core abstraction in Agent Framework:

```csharp
public abstract class AIAgent
{
    // The agent's name (used in multi-agent scenarios)
    public string Name { get; }
    
    // Execute the agent with a message
    public Task<AgentRunResponse> RunAsync(string message, AgentRunOptions? options = null);
    
    // Execute with a thread (for conversation continuity)
    public Task<AgentRunResponse> RunAsync(AgentThread thread, string message, AgentRunOptions? options = null);
}
```

### Key Concepts

| Concept | Description |
|---------|-------------|
| **Name** | Identifies the agent (important for multi-agent workflows) |
| **Instructions** | The system prompt defining the agent's behavior |
| **RunAsync** | Executes the agent and returns a response |
| **AgentThread** | Maintains conversation history across multiple runs |

---

## Part 3: Agent with Conversation Thread

Agents can maintain conversation context using `AgentThread`:

```csharp
using Microsoft.Agents.AI;

// Create an agent
AIAgent assistant = chatClient.CreateAIAgent(
    name: "Assistant",
    instructions: "You are a helpful assistant that remembers our conversation.");

// Create a thread to maintain context
var thread = new AgentThread();

// First message
var response1 = await assistant.RunAsync(thread, "My name is Bruno.");
Console.WriteLine($"Agent: {response1.Text}");

// Second message - the agent remembers the name
var response2 = await assistant.RunAsync(thread, "What's my name?");
Console.WriteLine($"Agent: {response2.Text}");  // Should include "Bruno"

// Third message - still in context
var response3 = await assistant.RunAsync(thread, "Can you spell it backwards?");
Console.WriteLine($"Agent: {response3.Text}");  // Should spell "onurB"
```

The thread automatically manages the conversation history, so the agent can reference earlier parts of the conversation.

---

## Part 4: Agents with Different Providers

One of Agent Framework's strengths is provider flexibility. The same agent pattern works with any AI provider.

### Azure OpenAI

```csharp
using Azure.AI.OpenAI;
using Azure.Identity;

AIAgent agent = new AzureOpenAIClient(
    new Uri("https://your-resource.openai.azure.com/"),
    new AzureCliCredential())
    .GetChatClient("gpt-4o-mini")
    .CreateAIAgent(instructions: "You are a helpful assistant.");
```

### Azure OpenAI (with API Key)

```csharp
using OpenAI;
using System.ClientModel;

IChatClient chatClient = new AzureOpenAIClient(
        new Uri(config["endpoint"]),
        new ApiKeyCredential(config["apikey"]))
    .GetChatClient("gpt-4o-mini")
    .AsIChatClient();

AIAgent agent = chatClient.CreateAIAgent(
    instructions: "You are a helpful assistant.");
```

### Ollama (Local)

```csharp
using OllamaSharp;

IChatClient chatClient = new OllamaApiClient(
    new Uri("http://localhost:11434/"), 
    "llama3.2");

AIAgent agent = chatClient.CreateAIAgent(
    name: "LocalAgent",
    instructions: "You are a helpful assistant running locally.");

var response = await agent.RunAsync("What can you help me with?");
Console.WriteLine(response.Text);
```

**Learn more:** [Microsoft Agent Framework Quick-Start](https://learn.microsoft.com/agent-framework/tutorials/quick-start) on Microsoft Learn.

---

## Part 5: Agent Builder Pattern

For more control, use the builder pattern:

```csharp
using Microsoft.Agents.AI;
using OpenTelemetry;
using OpenTelemetry.Trace;

// Set up telemetry
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("my-agent-source")
    .AddConsoleExporter()
    .Build();

// Create an agent with telemetry
AIAgent agent = chatClient.CreateAIAgent(
    name: "TracedAgent",
    instructions: "You are a helpful assistant.")
    .AsBuilder()
    .UseOpenTelemetry(sourceName: "my-agent-source")
    .Build();

var response = await agent.RunAsync("Hello!");
```

The builder pattern allows you to:
- Add telemetry for observability
- Configure middleware for interception
- Add custom behaviors

---

## Part 6: When to Use Agents vs Chat

Not every AI application needs agents. Here's a decision guide:

### Use Chat Completions When:
- Single question → single answer
- You control the exact prompt
- No need for multi-step reasoning
- Simple, predictable workflows

### Use Agents When:
- Multi-turn conversations with context
- The AI needs to call tools/functions
- Complex, multi-step tasks
- You want the AI to plan and decide

> **Rule of thumb:** If you can write a function to do it, do that. Use agents when the task requires reasoning and flexibility.

---

## Let's Review: What You Learned

| Concept | Key Takeaway |
|---------|-------------|
| **AIAgent** | Core abstraction for building agents |
| **CreateAIAgent** | Extension method to create agents from any chat client |
| **AgentThread** | Maintains conversation context across runs |
| **Provider Flexibility** | Same code works with Azure OpenAI, Ollama |
| **When to Use** | Agents shine when tasks need reasoning and multi-step execution |

### Quick Self-Check

1. What's the difference between chat completions and agents?
2. Why would you use an `AgentThread`?
3. When should you NOT use an agent?

---

## Sample Code Reference

| Sample | Description |
|--------|-------------|
| [MAF01](../samples/MAF/MAF01/) | Basic agent with Azure OpenAI |
| [MAF-Ollama-01](../samples/MAF/MAF-Ollama-01/) | Agent using local Ollama model |
| [MAF-Persisting-01-Simple](../samples/MAF/MAF-Persisting-01-Simple/) | Agent with conversation persistence |

---

## Additional Resources

- [Microsoft Agent Framework Overview](https://learn.microsoft.com/agent-framework/overview/agent-framework-overview): Architecture and concepts of the Agent Framework
- [Agent Framework Quick-Start Guide](https://learn.microsoft.com/agent-framework/tutorials/quick-start): Build your first agent in minutes
- [What are agents? (.NET)](https://learn.microsoft.com/dotnet/ai/conceptual/agents): Conceptual guide to AI agents in .NET
- [AIAgent class reference](https://learn.microsoft.com/agent-framework/user-guide/agents/agent-overview): API documentation for the core agent abstraction

---

## Up Next

Your agent can talk, but can it *do* things? In the next part, we'll give agents the power to take actions:

[Continue to Part 2: Agents with Tools →](./02-agents-with-tools.md)
