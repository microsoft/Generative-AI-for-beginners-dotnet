# AgentFx Claude Samples - Microsoft Foundry Integration

This collection demonstrates how to use **Microsoft Agent Framework (AgentFx)** with **Claude models** deployed in **Microsoft Foundry**. These samples showcase the integration of Claude's advanced language capabilities with AgentFx's powerful agent orchestration features using the **[elbruno.Extensions.AI.Claude](https://www.nuget.org/packages/elbruno.Extensions.AI.Claude/)** NuGet package.

## Overview

All samples in this collection use the **elbruno.Extensions.AI.Claude** NuGet package, which provides a clean, native implementation of `IChatClient` for Claude models deployed in Microsoft Foundry. This package simplifies Claude integration by eliminating the need for custom HTTP handlers or API transformations, while maintaining full compatibility with Microsoft.Extensions.AI and the Agent Framework ecosystem.

## Samples Included

### 1. AgentFx-FoundryClaude-01 - Basic Chat

**Path**: `samples/MAF/AgentFx-FoundryClaude-01/`

A console application demonstrating basic agent chat with Claude.

**Key Features**:

- Single prompt/response interaction
- `ChatClientAgent` pattern
- Direct `AzureClaudeClient` instantiation from NuGet package
- Microsoft Foundry deployment integration

**Use Cases**: Quick prototyping, testing Claude integration, simple Q&A applications

[Full Documentation →](./AgentFx-FoundryClaude-01/README.md)

---

### 2. AgentFx-FoundryClaude-Persisting-01 - Conversation Persistence

**Path**: `samples/MAF/AgentFx-FoundryClaude-Persisting-01/`

A console application showing how to persist and resume agent conversations with Claude.

**Key Features**:

- Thread serialization and deserialization
- Conversation state persistence to JSON files
- Multi-turn conversations with preserved context
- Step-by-step demonstration of context retention

**Use Cases**: Multi-session chatbots, long-running workflows, debugging conversation flows

[Full Documentation →](./AgentFx-FoundryClaude-Persisting-01/README.md)

---

### 3. AgentFx-AIWebChatApp-FoundryClaude - Blazor Web Chat

**Path**: `samples/MAF/AgentFx-AIWebChatApp-FoundryClaude/`

A Blazor Server web application with a modern chat interface powered by Claude.

**Key Features**:

- Interactive web-based chat UI
- Real-time messaging with Claude
- Dependency injection for agent configuration
- Modern gradient-themed interface
- Blazor Server with interactive rendering

**Use Cases**: Web-based chatbots, customer support interfaces, interactive AI applications

[Full Documentation →](./AgentFx-AIWebChatApp-FoundryClaude/README.md)

---

## Prerequisites

All samples require:

### 1. Microsoft Foundry Setup

1. **Azure Subscription**: Active Azure subscription
2. **Microsoft Foundry Project**: Create a project in [Microsoft Foundry](https://ai.azure.com)
3. **Claude Model Deployment**: Deploy a Claude model (options below)
4. **API Credentials**: Obtain endpoint URLs and API key

### 2. Development Environment

- **.NET 9 SDK** or later: [Download](https://dotnet.microsoft.com/download)
- **IDE**: Visual Studio 2022, VS Code, or Rider (optional but recommended)

### 3. Claude Model Options

Available Claude models in Microsoft Foundry:

| Model | Description | Best For |
|-------|-------------|----------|
| **claude-haiku-4-5** | Fast, cost-effective | High-volume applications, quick responses |
| **claude-sonnet-4-5** | Balanced performance | General-purpose tasks, moderate complexity |
| **claude-opus-4-5** | Highest capability | Complex reasoning, detailed analysis |

## Configuration

Each sample requires the following user secrets:

```bash
cd samples/MAF/<sample-name>

dotnet user-secrets set "endpointClaude" "https://<resource-name>.services.ai.azure.com/anthropic/v1/messages"
dotnet user-secrets set "apikey" "<your-api-key>"
dotnet user-secrets set "deploymentName" "claude-haiku-4-5"
```

### Finding Your Configuration Values

1. **endpointClaude**: Claude-specific endpoint
   - Format: `https://<resource-name>.services.ai.azure.com/anthropic/v1/messages`
   - Replace `<resource-name>` with your Microsoft Foundry resource name

2. **apikey**: API key for authentication
   - Azure Portal → Your AI resource → Keys and Endpoint → Key 1 or Key 2

3. **deploymentName**: Your Claude model deployment name
   - Microsoft Foundry → Deployments → Your Claude deployment name

## Quick Start

### Installing the Package

All samples use the elbruno.Extensions.AI.Claude NuGet package:

```bash
dotnet add package elbruno.Extensions.AI.Claude --version 0.1.0-preview.2
```

### Running Console Samples

```bash
# Basic chat example
cd samples/MAF/AgentFx-FoundryClaude-01
dotnet run

# Persisting conversations example
cd samples/MAF/AgentFx-FoundryClaude-Persisting-01
dotnet run
```

### Running Web Sample

```bash
cd samples/MAF/AgentFx-AIWebChatApp-FoundryClaude
dotnet run

# Open browser to: https://localhost:5001
```

## Architecture

### AzureClaudeClient from elbruno.Extensions.AI.Claude

All samples use `AzureClaudeClient` from the **elbruno.Extensions.AI.Claude** NuGet package:

**Key Features**:

1. **Native IChatClient implementation** - Direct implementation of Microsoft.Extensions.AI abstractions
2. **Simple configuration** - Just endpoint, model ID, and API key
3. **No custom handlers needed** - All API transformation handled internally
4. **Full streaming support** - Real-time responses with Server-Sent Events
5. **Automatic header management** - Includes required `anthropic-version` header

This approach provides:

- ✅ Clean, maintainable code without custom HTTP handlers
- ✅ Native Claude integration with Microsoft.Extensions.AI
- ✅ Full compatibility with Agent Framework APIs
- ✅ Easy switching between AI providers
- ✅ Support for both streaming and non-streaming responses

### Integration Flow

```
AgentFx Application
      ↓
IChatClient (Microsoft.Extensions.AI)
      ↓
AzureClaudeClient (elbruno.Extensions.AI.Claude)
      ↓
Claude API (Microsoft Foundry)
```

## Common Patterns

### Creating an IChatClient with Claude

```csharp
using elbruno.Extensions.AI.Claude;
using Microsoft.Extensions.AI;

// Create IChatClient using AzureClaudeClient
IChatClient chatClient = new AzureClaudeClient(
    endpoint: new Uri(endpointClaude),
    modelId: deploymentName,
    apiKey: apiKey);
```

### Creating an Agent

```csharp
var agent = chatClient.CreateAIAgent(
    name: "MyAgent",
    instructions: "You are a helpful assistant"
);

// Run the agent
var response = await agent.RunAsync("Your question here");
Console.WriteLine(response.Text);
```

### Persisting Conversations

```csharp
// Create thread
var thread = agent.GetNewThread();
var response = await agent.RunAsync(question, thread);

// Save thread
var threadJson = thread.Serialize(JsonSerializerOptions.Web).GetRawText();
await File.WriteAllTextAsync("thread.json", threadJson);

// Load thread
var loadedJson = await File.ReadAllTextAsync("thread.json");
var reloaded = JsonSerializer.Deserialize<JsonElement>(loadedJson, JsonSerializerOptions.Web);
thread = agent.DeserializeThread(reloaded, JsonSerializerOptions.Web);

// Continue conversation
response = await agent.RunAsync(nextQuestion, thread);
```

## Comparison: Claude Models vs OpenAI Models

| Feature | Claude (via Microsoft Foundry) | OpenAI (via Azure OpenAI) |
|---------|-------------------------------|---------------------------|
| **Authentication** | x-api-key header | api-key header or Bearer token |
| **Message Format** | Anthropic Messages API | OpenAI Chat Completions API |
| **System Messages** | Separate `system` parameter | Part of `messages` array |
| **Streaming** | Server-Sent Events (SSE) | Server-Sent Events (SSE) |
| **Context Length** | Up to 200K tokens (Claude 3+) | Varies by model (4K-128K) |
| **Tool Calling** | Native function calling | Native function calling |

The `ClaudeToOpenAIMessageHandler` abstracts these differences, allowing you to use the same AgentFx APIs regardless of the underlying model.

## Best Practices

### 1. **Model Selection**

- Use **claude-haiku-4-5** for high-volume, cost-sensitive applications
- Use **claude-sonnet-4-5** for balanced performance and capability
- Use **claude-opus-4-5** for complex reasoning tasks

### 2. **Error Handling**

- Always wrap agent calls in try-catch blocks
- Log errors for debugging
- Provide user-friendly error messages

### 3. **Security**

- Never hardcode API keys
- Use user secrets for local development
- Use managed identities or Key Vault in production

### 4. **Performance**

- Enable streaming for better UX
- Cache frequent responses when appropriate
- Monitor token usage and costs

### 5. **Testing**

- Test with different prompt lengths
- Validate conversation persistence
- Check error scenarios (network issues, rate limits)

## Troubleshooting

### Common Issues

#### Authentication Errors (401/403)

- **Cause**: Invalid API key or incorrect endpoint
- **Solution**: Verify `apikey` and `endpointClaude` in user secrets
- **Check**: Ensure API key is from the correct Azure resource

#### Model Not Found (404)

- **Cause**: Deployment name doesn't match actual deployment
- **Solution**: Verify `deploymentName` matches Microsoft Foundry deployment
- **Check**: Go to Microsoft Foundry → Deployments → Verify name

#### Streaming Not Working

- **Cause**: Handler not properly transforming SSE events
- **Solution**: Ensure `ClaudeToOpenAIMessageHandler` is correctly configured
- **Check**: Review console logs for handler errors

#### Context Not Preserved (Persisting Samples)

- **Cause**: Thread not properly serialized or deserialized
- **Solution**: Use `JsonSerializerOptions.Web` for both operations
- **Check**: Validate saved JSON file is not corrupted

### Getting Help

1. **Check Logs**: Review application logs for detailed error messages
2. **Azure Portal**: Verify Claude deployment status and quota
3. **Documentation**: Refer to sample-specific README files
4. **GitHub Issues**: Report bugs or request features in the repo

## Additional Resources

### Documentation

- [Microsoft Agent Framework](https://learn.microsoft.com/agent-framework/)
- [Microsoft Foundry Documentation](https://learn.microsoft.com/azure/ai-foundry/)
- [Claude in Microsoft Foundry](https://learn.microsoft.com/azure/ai-foundry/foundry-models/how-to/use-foundry-models-claude)
- [Claude API Documentation](https://docs.anthropic.com/claude/reference/messages_post)
- [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/ai-extensions)

### Related Samples

- **AgentFx-AIFoundry-01**: Microsoft Foundry with OpenAI models
- **AgentFx-Ollama-01**: Local AI models with Ollama
- **AgentFx-MultiAgents**: Multi-agent orchestration patterns

### Learning Resources

- [Generative AI for Beginners .NET Course](https://github.com/microsoft/Generative-AI-for-beginners-dotnet)
- [Agent Framework Tutorials](https://learn.microsoft.com/agent-framework/tutorials/)
- [Microsoft Foundry Quickstarts](https://learn.microsoft.com/azure/ai-foundry/quickstarts/)

## Contributing

Contributions are welcome! Please follow the repository's contribution guidelines:

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This sample collection is part of the Generative AI for Beginners .NET course and follows the repository's MIT license.

---

## Sample Comparison Table

| Sample | Type | Complexity | Key Learning |
|--------|------|------------|--------------|
| **AgentFx-FoundryClaude-01** | Console | Beginner | Basic Claude integration |
| **AgentFx-FoundryClaude-Persisting-01** | Console | Intermediate | Conversation persistence |
| **AgentFx-AIWebChatApp-FoundryClaude** | Web | Intermediate | Web UI with DI |

Choose the sample that best fits your learning goals or project requirements!
