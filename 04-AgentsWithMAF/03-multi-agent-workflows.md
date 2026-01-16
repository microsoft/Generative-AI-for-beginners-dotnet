# Multi-Agent Workflows

In this lesson, you'll learn how to orchestrate multiple specialized agents working together to accomplish complex tasks.

---

[![Multi-Agent Workflows](./images/LIM_GAN_08_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson4-multiagent)

_Click the image to watch the video_

---

## Why Multiple Agents?

A single agent has limits:
- Too many tools become unwieldy (20+ tools confuse the model)
- Complex tasks require different expertise
- Some steps need different models or capabilities

Multi-agent systems solve this by dividing responsibilities:

```
Complex Task
     │
     ▼
┌─────────────────────────────────────────────────────────────┐
│                    Multi-Agent System                       │
│                                                             │
│  ┌──────────┐    ┌──────────┐    ┌──────────┐              │
│  │Researcher│───►│  Writer  │───►│  Editor  │              │
│  │  Agent   │    │  Agent   │    │  Agent   │              │
│  └──────────┘    └──────────┘    └──────────┘              │
│                                                             │
│  Each agent has specialized instructions and capabilities   │
└─────────────────────────────────────────────────────────────┘
     │
     ▼
Polished Output
```

**Learn more:** [Multi-Agent Orchestrations](https://learn.microsoft.com/agent-framework/user-guide/workflows/orchestrations/overview) on Microsoft Learn.

---

## Part 1: Workflow Orchestration Patterns

Agent Framework provides five orchestration patterns:

| Pattern | Description | Use Case |
|---------|-------------|----------|
| **Sequential** | Agents process in order, passing results forward | Pipelines, step-by-step workflows |
| **Concurrent** | Agents work in parallel on the same input | Parallel analysis, ensemble decisions |
| **Handoff** | Agents dynamically pass control based on context | Expert escalation, dynamic routing |
| **Group Chat** | Agents collaborate in shared conversation | Iterative refinement, collaborative problem-solving |
| **Magentic** | Lead agent directs other agents (planner-based) | Complex, generalist collaboration |

Let's explore each pattern with code examples.

---

## Part 2: Sequential Workflow

The simplest pattern: output from one agent becomes input to the next.

```csharp
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;

// Create specialized agents
AIAgent writer = chatClient.CreateAIAgent(
    name: "Writer",
    instructions: "Write stories that are engaging and creative.");

AIAgent editor = chatClient.CreateAIAgent(
    name: "Editor",
    instructions: "Make the story more engaging, fix grammar, and enhance the plot.");

// Build a sequential workflow
Workflow workflow = AgentWorkflowBuilder.BuildSequential(writer, editor);

// Run the workflow
AIAgent workflowAgent = workflow.AsAgent();

AgentRunResponse response = await workflowAgent.RunAsync(
    "Write a short story about a haunted house.");

Console.WriteLine(response.Text);
```

The story flows from Writer → Editor, producing a polished result.

### Adding More Steps

```csharp
AIAgent researcher = chatClient.CreateAIAgent(
    name: "Researcher",
    instructions: "Research and gather key facts about the topic.");

AIAgent writer = chatClient.CreateAIAgent(
    name: "Writer",
    instructions: "Write content based on the research provided.");

AIAgent editor = chatClient.CreateAIAgent(
    name: "Editor",
    instructions: "Polish the writing for clarity and engagement.");

AIAgent factChecker = chatClient.CreateAIAgent(
    name: "FactChecker",
    instructions: "Verify claims and add [citation needed] where facts cannot be confirmed.");

// Four-step pipeline
Workflow pipeline = AgentWorkflowBuilder.BuildSequential(
    researcher, writer, editor, factChecker);

var response = await pipeline.AsAgent().RunAsync(
    "Create an article about the history of quantum computing.");
```

---

## Part 3: Concurrent Workflow

When agents can work independently on the same input:

```csharp
AIAgent sentimentAnalyst = chatClient.CreateAIAgent(
    name: "SentimentAnalyst",
    instructions: "Analyze the emotional tone and sentiment of the text.");

AIAgent summaryAgent = chatClient.CreateAIAgent(
    name: "Summarizer",
    instructions: "Provide a concise summary of the text.");

AIAgent keywordExtractor = chatClient.CreateAIAgent(
    name: "KeywordExtractor",
    instructions: "Extract the main keywords and topics from the text.");

// Build concurrent workflow - all agents process in parallel
Workflow concurrent = AgentWorkflowBuilder.BuildConcurrent(
    sentimentAnalyst, summaryAgent, keywordExtractor);

var response = await concurrent.AsAgent().RunAsync("""
    The new product launch exceeded all expectations. Sales were 
    up 200% compared to last year, and customer feedback has been 
    overwhelmingly positive. The marketing team's innovative 
    campaign drove significant social media engagement.
    """);

// Response contains aggregated results from all agents
Console.WriteLine(response.Text);
```

All three agents analyze the text simultaneously, and results are combined.

---

## Part 4: Handoff Workflow

Dynamic routing where agents decide when to pass control:

```csharp
// Support agent for general questions
AIAgent generalSupport = chatClient.CreateAIAgent(
    name: "GeneralSupport",
    instructions: """
        You handle general customer questions.
        If the customer has a billing issue, hand off to BillingSpecialist.
        If the customer has a technical issue, hand off to TechnicalSupport.
        """);

// Specialist for billing issues
AIAgent billingSpecialist = chatClient.CreateAIAgent(
    name: "BillingSpecialist",
    instructions: """
        You are a billing expert. Handle payment, invoice, and 
        subscription questions. If the issue is resolved, you can 
        hand back to GeneralSupport for any follow-up questions.
        """);

// Specialist for technical issues
AIAgent technicalSupport = chatClient.CreateAIAgent(
    name: "TechnicalSupport",
    instructions: """
        You are a technical support expert. Handle software bugs,
        configuration issues, and how-to questions.
        """);

// Build handoff workflow - agents can transfer control
Workflow handoff = AgentWorkflowBuilder.BuildHandoff(
    generalSupport, billingSpecialist, technicalSupport);

// First query goes to GeneralSupport, which may route to specialists
var response = await handoff.AsAgent().RunAsync(
    "I was charged twice for my subscription last month.");
```

The workflow dynamically routes based on the conversation content.

---

## Part 5: Multi-Model Orchestration

Different agents can use different AI models:

```csharp
using OllamaSharp;
using Azure.AI.OpenAI;
using Azure.Identity;

// Agent 1: Researcher using Azure OpenAI
var azureClient = new AzureOpenAIClient(
    new Uri("https://your-resource.openai.azure.com/"),
    new AzureCliCredential());

IChatClient azureChatClient = azureClient
    .GetChatClient("gpt-4o")
    .AsIChatClient();

AIAgent researcher = azureChatClient.CreateAIAgent(
    name: "Researcher",
    instructions: "Research topics thoroughly using your broad knowledge.");

// Agent 2: Writer using GitHub Models
IChatClient githubChatClient = new ChatClient(
        "gpt-4o-mini",
        new ApiKeyCredential(githubToken),
        new OpenAIClientOptions { Endpoint = new Uri("https://models.github.ai/inference") })
    .AsIChatClient();

AIAgent writer = githubChatClient.CreateAIAgent(
    name: "Writer",
    instructions: "Write engaging content based on research.");

// Agent 3: Reviewer using local Ollama
IChatClient ollamaClient = new OllamaApiClient(
    new Uri("http://localhost:11434/"), 
    "llama3.2");

AIAgent reviewer = ollamaClient.CreateAIAgent(
    name: "Reviewer",
    instructions: "Review content for accuracy and suggest improvements.");

// Combine in a workflow - each agent uses a different model!
Workflow multiModel = AgentWorkflowBuilder.BuildSequential(
    researcher, writer, reviewer);

var response = await multiModel.AsAgent().RunAsync(
    "Create an article about renewable energy innovations.");
```

This is powerful for:
- **Cost optimization** - Use expensive models only where needed
- **Privacy** - Keep sensitive data on local models
- **Capabilities** - Match model strengths to tasks

---

## Part 6: Custom Workflow Graphs

For complex routing, build custom workflow graphs:

```csharp
using Microsoft.Agents.AI.Workflows;

// Create a workflow builder
var builder = new WorkflowBuilder();

// Define nodes (agents)
var intake = builder.AddAgent(intakeAgent, "intake");
var analyzer = builder.AddAgent(analyzerAgent, "analyzer");
var simpleHandler = builder.AddAgent(simpleAgent, "simple");
var complexHandler = builder.AddAgent(complexAgent, "complex");
var responder = builder.AddAgent(responderAgent, "responder");

// Define edges (connections)
builder.AddEdge("intake", "analyzer");

// Conditional routing based on analysis
builder.AddConditionalEdge("analyzer", context =>
{
    var lastMessage = context.Thread.Messages.Last().Content;
    return lastMessage.Contains("complex") ? "complex" : "simple";
});

builder.AddEdge("simple", "responder");
builder.AddEdge("complex", "responder");

// Build and run
Workflow customWorkflow = builder.Build();
```

---

## Part 7: Workflow Best Practices

### Clear Agent Responsibilities

```csharp
// ❌ Bad - overlapping responsibilities
AIAgent agent1 = chatClient.CreateAIAgent(
    instructions: "Write and edit content.");

AIAgent agent2 = chatClient.CreateAIAgent(
    instructions: "Edit and improve content.");

// ✅ Good - distinct responsibilities
AIAgent writer = chatClient.CreateAIAgent(
    instructions: "Write creative, engaging first drafts. Focus on ideas and flow.");

AIAgent editor = chatClient.CreateAIAgent(
    instructions: "Polish writing for grammar, clarity, and consistency. Don't change the core ideas.");
```

### Handle Agent Failures

```csharp
try
{
    var response = await workflowAgent.RunAsync(userInput);
    Console.WriteLine(response.Text);
}
catch (AgentException ex)
{
    Console.WriteLine($"Agent {ex.AgentName} failed: {ex.Message}");
    // Fallback logic
}
```

### Limit Workflow Depth

```
✅ Good: 2-4 agents in sequence
❌ Risky: 10+ agents in sequence (latency, error propagation)
```

---

## Let's Review: What You Learned

| Concept | Key Takeaway |
|---------|-------------|
| **Sequential** | Agents process in order, like a pipeline |
| **Concurrent** | Agents work in parallel on same input |
| **Handoff** | Dynamic routing between specialists |
| **Multi-Model** | Different agents can use different AI providers |
| **AgentWorkflowBuilder** | Factory for creating workflow patterns |

### Quick Self-Check

1. When would you use concurrent vs sequential workflows?
2. How does handoff differ from sequential?
3. Why might you use different models for different agents?

---

## Sample Code Reference

| Sample | Description |
|--------|-------------|
| [AgentFx02](../samples/AgentFx/AgentFx02/) | Simple sequential workflow |
| [AgentFx-MultiAgents](../samples/AgentFx/AgentFx-MultiAgents/) | Multi-model orchestration |
| [AgentFx-MultiAgents-Factory-01](../samples/AgentFx/AgentFx-MultiAgents-Factory-01/) | Advanced workflow patterns |

---

## Additional Resources

- [Workflow Orchestrations Overview](https://learn.microsoft.com/agent-framework/user-guide/workflows/orchestrations/overview): Introduction to multi-agent orchestration patterns
- [Sequential Workflows](https://learn.microsoft.com/agent-framework/user-guide/workflows/orchestrations/sequential): Building pipeline-style agent workflows
- [Handoff Workflows](https://learn.microsoft.com/agent-framework/user-guide/workflows/orchestrations/handoff): Dynamic routing between specialized agents
- [Group Chat Orchestration](https://learn.microsoft.com/agent-framework/user-guide/workflows/orchestrations/group-chat): Collaborative multi-agent conversations

---

## Up Next

Multi-agent systems are powerful, but how do we extend them with external capabilities? The Model Context Protocol (MCP) provides a standardized way:

[Continue to Part 4: Model Context Protocol (MCP) →](./04-model-context-protocol.md)
