# AgentFx Claude Samples - Azure AI Foundry Integration

This collection demonstrates how to use **Microsoft Agent Framework (AgentFx)** with **Claude models** deployed in **Azure AI Foundry**. These samples showcase the integration of Claude's advanced language capabilities with AgentFx's powerful agent orchestration features.

## Overview

All samples in this collection use a custom `ClaudeToOpenAIMessageHandler` to bridge the Claude API format with the OpenAI-compatible APIs used by Microsoft.Extensions.AI and Agent Framework. This allows seamless integration of Claude models while maintaining compatibility with the AgentFx ecosystem.

## Samples Included

### 1. AgentFx-FoundryClaude-01 - Basic Chat

**Path**: `samples/AgentFx/AgentFx-FoundryClaude-01/`

A console application demonstrating basic agent chat with Claude.

**Key Features**:

- Single prompt/response interaction
- `ChatClientAgent` pattern
- Custom HTTP message handler for Claude API translation
- Azure AI Foundry deployment integration

**Use Cases**: Quick prototyping, testing Claude integration, simple Q&A applications

[Full Documentation →](./AgentFx-FoundryClaude-01/README.md)

---

### 2. AgentFx-FoundryClaude-Persisting-01 - Conversation Persistence

**Path**: `samples/AgentFx/AgentFx-FoundryClaude-Persisting-01/`

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

**Path**: `samples/AgentFx/AgentFx-AIWebChatApp-FoundryClaude/`

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

### 1. Azure AI Foundry Setup

1. **Azure Subscription**: Active Azure subscription
2. **Azure AI Foundry Project**: Create a project in [Azure AI Foundry](https://ai.azure.com)
3. **Claude Model Deployment**: Deploy a Claude model (options below)
4. **API Credentials**: Obtain endpoint URLs and API key

### 2. Development Environment

- **.NET 9 SDK** or later: [Download](https://dotnet.microsoft.com/download)
- **IDE**: Visual Studio 2022, VS Code, or Rider (optional but recommended)

### 3. Claude Model Options

Available Claude models in Azure AI Foundry:

| Model | Description | Best For |
|-------|-------------|----------|
| **claude-haiku-4-5** | Fast, cost-effective | High-volume applications, quick responses |
| **claude-sonnet-4-5** | Balanced performance | General-purpose tasks, moderate complexity |
| **claude-opus-4-5** | Highest capability | Complex reasoning, detailed analysis |

## Configuration

Each sample requires the following user secrets:

```bash
cd samples/AgentFx/<sample-name>

dotnet user-secrets set "endpoint" "https://<resource-name>.cognitiveservices.azure.com"
dotnet user-secrets set "endpointClaude" "https://<resource-name>.services.ai.azure.com/anthropic/v1/messages"
dotnet user-secrets set "apikey" "<your-api-key>"
dotnet user-secrets set "deploymentName" "claude-haiku-4-5"
```

### Finding Your Configuration Values

1. **endpoint**: Azure Cognitive Services endpoint
   - Azure Portal → Your AI resource → Keys and Endpoint → Endpoint

2. **endpointClaude**: Claude-specific endpoint
   - Format: `https://<resource-name>.services.ai.azure.com/anthropic/v1/messages`
   - Replace `<resource-name>` with your Azure AI Foundry resource name

3. **apikey**: API key for authentication
   - Azure Portal → Your AI resource → Keys and Endpoint → Key 1 or Key 2

4. **deploymentName**: Your Claude model deployment name
   - Azure AI Foundry → Deployments → Your Claude deployment name

## Quick Start

### Running Console Samples

```bash
# Basic chat example
cd samples/AgentFx/AgentFx-FoundryClaude-01
dotnet run

# Persisting conversations example
cd samples/AgentFx/AgentFx-FoundryClaude-Persisting-01
dotnet run
```

### Running Web Sample

```bash
cd samples/AgentFx/AgentFx-AIWebChatApp-FoundryClaude
dotnet run

# Open browser to: https://localhost:5001
```

## Architecture

### ClaudeToOpenAIMessageHandler

All samples use a custom `ClaudeToOpenAIMessageHandler` that:

1. **Intercepts HTTP requests** to Claude deployments
2. **Transforms OpenAI format** requests to Claude Messages API format
3. **Converts Claude responses** back to OpenAI-compatible format
4. **Handles streaming** for real-time responses (SSE to OpenAI format)
5. **Manages authentication** using x-api-key header (Claude-specific)

This approach allows:

- ✅ Seamless Claude integration with Microsoft.Extensions.AI
- ✅ Full compatibility with Agent Framework APIs
- ✅ Easy switching between AI providers
- ✅ Support for both streaming and non-streaming responses

### Integration Flow

```
AgentFx Application
      ↓
IChatClient (Microsoft.Extensions.AI)
      ↓
AzureOpenAIClient with custom HttpMessageHandler
      ↓
ClaudeToOpenAIMessageHandler
      ↓
Claude API (Azure AI Foundry)
```

## Common Patterns

### Creating an IChatClient with Claude

```csharp
var customHttpMessageHandler = new ClaudeToOpenAIMessageHandler
{
    AzureClaudeDeploymentUrl = endpointClaude,
    ApiKey = apiKey,
    Model = deploymentName
};
HttpClient customHttpClient = new(customHttpMessageHandler);
var transport = new HttpClientPipelineTransport(customHttpClient);

IChatClient chatClient = new AzureOpenAIClient(
    endpoint: new Uri(endpoint),
    credential: new ApiKeyCredential(apiKey),
    options: new AzureOpenAIClientOptions { Transport = transport })
    .GetChatClient(deploymentName)
    .AsIChatClient()
    .AsBuilder()
    .Build();
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

| Feature | Claude (via Azure AI Foundry) | OpenAI (via Azure OpenAI) |
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
- **Solution**: Verify `deploymentName` matches Azure AI Foundry deployment
- **Check**: Go to Azure AI Foundry → Deployments → Verify name

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
- [Azure AI Foundry Documentation](https://learn.microsoft.com/azure/ai-foundry/)
- [Claude in Azure AI Foundry](https://learn.microsoft.com/azure/ai-foundry/foundry-models/how-to/use-foundry-models-claude)
- [Claude API Documentation](https://docs.anthropic.com/claude/reference/messages_post)
- [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/ai-extensions)

### Related Samples

- **AgentFx-AIFoundry-01**: Azure AI Foundry with OpenAI models
- **AgentFx-Ollama-01**: Local AI models with Ollama
- **AgentFx-MultiAgents**: Multi-agent orchestration patterns

### Learning Resources

- [Generative AI for Beginners .NET Course](https://github.com/microsoft/Generative-AI-for-beginners-dotnet)
- [Agent Framework Tutorials](https://learn.microsoft.com/agent-framework/tutorials/)
- [Azure AI Foundry Quickstarts](https://learn.microsoft.com/azure/ai-foundry/quickstarts/)

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
