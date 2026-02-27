# Model Context Protocol (MCP)

In this lesson, you'll learn how to extend your agents with standardized tool servers using Model Context Protocol, enabling integration with external systems without custom code for each integration.

---

[![Model Context Protocol](./images/LIM_GAN_08_thumb_w480.png)](https://aka.ms/genainet/videos/lesson4-mcp)

_Click the image to watch the video_

---

## What is Model Context Protocol?

**Model Context Protocol (MCP)** is an open standard that defines how applications provide tools and contextual data to large language models. Think of it as a "USB port for AI"—a standardized way to connect AI to external capabilities.

Instead of writing custom integration code for every tool, you connect to MCP servers that expose capabilities in a standard format.

```
┌──────────────────────────────────────────────────────────────────┐
│                        Your Agent                                │
│                                                                  │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐              │
│  │ MCP Client  │  │ MCP Client  │  │ MCP Client  │              │
│  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘              │
└─────────┼────────────────┼────────────────┼──────────────────────┘
          │                │                │
          ▼                ▼                ▼
   ┌────────────┐  ┌────────────┐  ┌────────────┐
   │ GitHub MCP │  │ Slack MCP  │  │ Database   │
   │   Server   │  │   Server   │  │ MCP Server │
   └────────────┘  └────────────┘  └────────────┘
```

**Learn more:** [Model Context Protocol](https://learn.microsoft.com/agent-framework/user-guide/model-context-protocol/) on Microsoft Learn.

---

## Why MCP Matters

Without MCP, every tool integration requires:
- Custom code for each API
- Authentication handling
- Error management
- Documentation and maintenance

With MCP:
- **Standardized** interface for all tools
- **Reusable** servers that work with any MCP-compatible client
- **Community** ecosystem of pre-built servers
- **Security** through standardized authentication patterns

---

## Part 1: Understanding MCP Components

### MCP Servers

MCP servers expose tools, resources, and prompts:

| Component | Description | Example |
|-----------|-------------|---------|
| **Tools** | Functions the agent can call | `search_files`, `create_issue` |
| **Resources** | Data the agent can read | File contents, database records |
| **Prompts** | Templates for common tasks | Code review checklist |

### MCP Clients

MCP clients (like Agent Framework) connect to servers and make their capabilities available to agents.

---

## Part 2: Connecting to MCP Servers

The `ModelContextProtocol.Client` package provides the MCP client. Here's the general pattern for connecting to any MCP server:

```csharp
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

// Create MCP Client using HTTP transport
var clientTransport = new HttpClientTransport(
    new HttpClientTransportOptions  
    {
        Name = "Server Name",
        Endpoint = new Uri("https://your-mcp-server.com/mcp"),
        AdditionalHeaders = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {apiKey}" }
        }
    });

await using var mcpClient = await McpClient.CreateAsync(clientTransport);

// List available tools from the server
var tools = await mcpClient.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine($"Available tool: {tool.Name}");
}
```

The MCP client automatically discovers all tools exposed by the server. You can then pass these tools to your chat client.

---

## Part 3: Common MCP Servers

The MCP ecosystem includes servers for many popular services:

| Server | Capabilities |
|--------|-------------|
| **GitHub** | Create issues, search code, manage PRs |
| **Hugging Face** | Image generation, model inference |
| **Filesystem** | Read/write files, search directories |
| **PostgreSQL** | Query databases, run SQL |
| **Slack** | Send messages, read channels |
| **Memory** | Persistent storage for agent context |

### Example: GitHub MCP Server

```csharp
// Connect to GitHub MCP server
var clientTransport = new HttpClientTransport(
    new HttpClientTransportOptions
    {
        Name = "GitHub",
        Endpoint = new Uri("https://api.githubcopilot.com/mcp"),
        AdditionalHeaders = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {githubPat}" }
        }
    });
await using var mcpClient = await McpClient.CreateAsync(clientTransport);
var tools = await mcpClient.ListToolsAsync();

// Tools might include:
// - search_code: Search code in repositories
// - create_issue: Create a new issue
// - list_pull_requests: List open PRs
// - get_file_contents: Read file contents
```

### Example: Hugging Face MCP Server (Working Sample)

The samples folder includes a complete working example with Hugging Face:

```csharp
using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

// Connect to Hugging Face MCP server
var clientTransport = new HttpClientTransport(
    new HttpClientTransportOptions  
    {
        Name = "HF Server",
        Endpoint = new Uri("https://huggingface.co/mcp"),
        AdditionalHeaders = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {hfApiKey}" }
        }
    });
await using var mcpClient = await McpClient.CreateAsync(clientTransport);
var tools = await mcpClient.ListToolsAsync();

// Create chat client with function invocation
IChatClient client = new AzureOpenAIClient(
        new Uri(config["endpoint"]),
        new ApiKeyCredential(config["apikey"]))
        .GetChatClient("gpt-4o-mini")
        .AsIChatClient()
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

// Use MCP tools with the chat client
var chatOptions = new ChatOptions
{
    Tools = [.. tools]
};

// The AI can now use tools from the MCP server
var query = "Create an image of a pixelated puppy.";
var result = await client.GetResponseAsync(query, chatOptions);
Console.WriteLine($"AI response: {result}");
```

---

## Part 4: MCP with Local Models (Ollama)

MCP works with any AI provider. Here's how to use MCP tools with Ollama running locally:

```csharp
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OllamaSharp;

// Connect to any MCP server
var clientTransport = new HttpClientTransport(
    new HttpClientTransportOptions
    {
        Name = "MCP Server",
        Endpoint = new Uri("https://your-mcp-server.com/mcp"),
        AdditionalHeaders = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {apiKey}" }
        }
    });
await using var mcpClient = await McpClient.CreateAsync(clientTransport);
var tools = await mcpClient.ListToolsAsync();

// Create Ollama client with function invocation
var uri = new Uri("http://localhost:11434");
var client = new OllamaApiClient(uri, "llama3.2")
    .AsChatCompletionService()
    .AsChatClient()
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

var chatOptions = new ChatOptions
{
    Tools = [.. tools]
};

// Use MCP tools with local model
var result = await client.GetResponseAsync("Your query here", chatOptions);
```

This pattern lets you run AI locally while still accessing external MCP services.

---

## Part 5: Building Your Own MCP Server

You can create MCP servers to expose your own tools:

```csharp
using ModelContextProtocol.Server;
using System.ComponentModel;

// Define your tools as methods with descriptions
public class InventoryTools
{
    [Description("Check stock levels for a product")]
    public async Task<string> CheckStock(string productId)
    {
        // Query your inventory system
        var stock = await _inventoryService.GetStockAsync(productId);
        return $"Product {productId} has {stock} units in stock.";
    }
    
    [Description("Reserve stock for an order")]
    public async Task<string> ReserveStock(string productId, int quantity)
    {
        var result = await _inventoryService.ReserveAsync(productId, quantity);
        return result.Success 
            ? $"Reserved {quantity} units of {productId}" 
            : $"Failed: {result.Error}";
    }
}

// Expose tools via MCP server
var server = new McpServerBuilder()
    .AddTools<InventoryTools>()
    .Build();

await server.RunAsync();
```

Now any MCP-compatible client can connect and use your inventory tools!

---

## Part 6: MCP Security Considerations

⚠️ **Important:** When using third-party MCP servers:

### Data Privacy
- Review what data is sent to external servers
- Be aware of data retention policies
- Consider data residency requirements

### Authentication
```csharp
// Always use secure token handling - load from environment or user secrets
var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();

var clientTransport = new HttpClientTransport(
    new HttpClientTransportOptions
    {
        Name = "Secure Server",
        Endpoint = new Uri("https://secure-mcp.example.com"),
        AdditionalHeaders = new Dictionary<string, string>
        {
            // Use configuration, never hardcode tokens
            { "Authorization", $"Bearer {config["MCP_TOKEN"]}" }
        }
    });
await using var mcpClient = await McpClient.CreateAsync(clientTransport);
```

### Trust Verification
```csharp
// Only connect to trusted MCP servers
var trustedServers = new[]
{
    "mcp.github.com",
    "mcp.microsoft.com",
    "your-internal-server.company.com"
};

if (!trustedServers.Contains(serverUri.Host))
{
    throw new SecurityException($"Untrusted MCP server: {serverUri.Host}");
}
```

**Learn more:** [MCP Security Best Practices](https://modelcontextprotocol.io/specification/draft/basic/security_best_practices) on the MCP specification site.

---

## Part 7: MCP in Multi-Agent Workflows

Combine MCP with multi-agent patterns:

```csharp
// Research agent with web search MCP
var webSearchMcp = await McpClient.ConnectAsync("https://mcp.websearch.com");
AIAgent researcher = chatClient.CreateAIAgent(
    name: "Researcher",
    instructions: "Research topics using web search.",
    tools: await webSearchMcp.GetToolsAsync());

// Code agent with GitHub MCP
var githubMcp = await McpClient.ConnectAsync("https://mcp.github.com");
AIAgent coder = chatClient.CreateAIAgent(
    name: "Coder",
    instructions: "Write and manage code in the repository.",
    tools: await githubMcp.GetToolsAsync());

// Documentation agent with filesystem MCP
var fsMcp = await McpClient.ConnectAsync("stdio://mcp-filesystem");
AIAgent documenter = chatClient.CreateAIAgent(
    name: "Documenter",
    instructions: "Create and update documentation files.",
    tools: await fsMcp.GetToolsAsync());

// Orchestrate them in a workflow
Workflow devWorkflow = AgentWorkflowBuilder.BuildSequential(
    researcher, coder, documenter);

await devWorkflow.AsAgent().RunAsync(
    "Research best practices for error handling, implement them, and document the changes.");
```

---

## Part 8: When to Use MCP

### Use MCP When:
- Integrating with well-known services (GitHub, Slack, databases)
- Building reusable tool servers
- Need standardized authentication patterns
- Want to leverage community-built servers

### Use Custom Tools When:
- Simple, one-off functions
- Internal business logic
- No need for external sharing
- Performance-critical operations

### Decision Guide

```
Need to integrate with external service?
├── Yes: Is there an existing MCP server?
│   ├── Yes: Use MCP ✅
│   └── No: Worth building MCP server?
│       ├── Multiple consumers: Build MCP server
│       └── Single use: Custom tool is fine
└── No: Custom tool ✅
```

---

## Let's Review: What You Learned

| Concept | Key Takeaway |
|---------|-------------|
| **MCP** | Open standard for tool/data integration |
| **MCP Servers** | Expose tools, resources, prompts in standard format |
| **MCP Clients** | Connect to servers and use their capabilities |
| **Security** | Review data flow, use secure authentication |
| **Ecosystem** | Growing community of pre-built servers |

### Quick Self-Check

1. What problem does MCP solve?
2. What are the three components an MCP server can expose?
3. What security considerations apply when using third-party MCP servers?

---

## Sample Code Reference

| Sample | Description |
|--------|-------------|
| [MCP-01-HuggingFace](../samples/CoreSamples/MCP-01-HuggingFace/) | MCP with Azure OpenAI |
| [MCP-02-HuggingFace-Ollama](../samples/CoreSamples/MCP-02-HuggingFace-Ollama/) | MCP with local Ollama models |
| Aspire MCP Sample | See [04-PracticalSamples](../04-PracticalSamples/) for Aspire + MCP integration |

---

## Additional Resources

- [Model Context Protocol Specification](https://modelcontextprotocol.io/introduction): Official MCP specification and concepts
- [MCP in Agent Framework](https://learn.microsoft.com/agent-framework/user-guide/model-context-protocol/): Using MCP with Microsoft Agent Framework
- [Security Best Practices](https://modelcontextprotocol.io/specification/draft/basic/security_best_practices): Securing MCP server connections
- [Understanding MCP Security Risks (Microsoft Blog)](https://techcommunity.microsoft.com/blog/microsoft-security-blog/understanding-and-mitigating-security-risks-in-mcp-implementations/4404667): Security considerations for production deployments

---

## Lesson Complete

You now understand how to build AI agents that reason and act:

- **First Agent**: Creating agents from chat clients
- **Tools**: Giving agents the ability to take actions
- **Multi-Agent**: Orchestrating multiple specialists together
- **MCP**: Extending capabilities with standardized protocols

---

## What's Next?

Congratulations! You've completed the core curriculum. You now have the skills to:

1. **Build chat applications** with streaming and structured output
2. **Implement RAG** for grounded, accurate responses
3. **Process visual content** with multimodal AI
4. **Create agents** that reason, plan, and act autonomously
5. **Orchestrate multi-agent systems** for complex tasks

### Continue Learning

- **Explore the samples** - The [samples/AgentFx](../samples/AgentFx/) folder has many more patterns
- **Build a real project** - Apply what you've learned to your own use case
- **Join the community** - Contribute to the [Agent Framework repo](https://github.com/microsoft/agent-framework)

---

## Key Takeaway

> **You are not just calling AI anymore. You are building systems that reason and act.**

This is the future of software development. Welcome to it.
