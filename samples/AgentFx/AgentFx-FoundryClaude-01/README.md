# AgentFx-FoundryClaude-01: Basic Chat with Claude Agent

This sample demonstrates how to use **Microsoft Agent Framework (AgentFx)** with **Claude models** deployed in **Azure AI Foundry**. It shows the integration of `ChatClientAgent` with Claude via a custom HTTP message handler that bridges OpenAI and Claude API formats.

## Overview

- **Framework**: Microsoft Agent Framework (AgentFx)
- **AI Model**: Claude (Haiku, Sonnet, or Opus) via Azure AI Foundry
- **Pattern**: Basic agent chat with single prompt/response
- **Key Concept**: Using `ClaudeToOpenAIMessageHandler` to enable Claude models with AgentFx

## Prerequisites

### 1. Azure AI Foundry Setup

1. Create an Azure AI Foundry project
2. Deploy a Claude model (e.g., `claude-haiku-4-5`, `claude-sonnet-4-5`)
3. Note your:
   - **Endpoint**: `https://<resource-name>.cognitiveservices.azure.com`
   - **Claude Endpoint**: `https://<resource-name>.services.ai.azure.com/anthropic/v1/messages`
   - **API Key**: From Azure portal
   - **Deployment Name**: Your Claude model deployment name

### 2. .NET Environment

- [.NET 9 SDK](https://dotnet.microsoft.com/download) or later

## Configuration

Set user secrets for the project:

```bash
cd samples/AgentFx/AgentFx-FoundryClaude-01

dotnet user-secrets set "endpoint" "https://<resource-name>.cognitiveservices.azure.com"
dotnet user-secrets set "endpointClaude" "https://<resource-name>.services.ai.azure.com/anthropic/v1/messages"
dotnet user-secrets set "apikey" "<your-api-key>"
dotnet user-secrets set "deploymentName" "claude-haiku-4-5"
```

## How to Run

```bash
# Navigate to the project directory
cd samples/AgentFx/AgentFx-FoundryClaude-01

# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

## What This Sample Demonstrates

### 1. **Claude Integration with AgentFx**

- Uses `ClaudeToOpenAIMessageHandler` to transform API calls between OpenAI and Claude formats
- Enables seamless Claude model usage with Microsoft Agent Framework

### 2. **ChatClientAgent Pattern**

- Creates an `AIAgent` using `ChatClientAgent`
- Configures agent with name and instructions
- Executes single prompt with `RunAsync()`

### 3. **Custom HTTP Transport**

- Wraps custom `HttpMessageHandler` in `HttpClientPipelineTransport`
- Configures `AzureOpenAIClient` with custom transport
- Maintains compatibility with AgentFx APIs

## Code Structure

```
AgentFx-FoundryClaude-01/
├── Program.cs                          # Main application with agent setup
├── ClaudeToOpenAIMessageHandler.cs     # HTTP handler for Claude API translation
├── AgentFx-FoundryClaude-01.csproj     # Project file with dependencies
└── README.md                           # This file
```

## Key Code Snippets

### Creating the IChatClient with Claude

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

### Creating and Running the Agent

```csharp
AIAgent writer = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "Writer",
        Instructions = "You are a creative writer..."
    });

AgentRunResponse response = await writer.RunAsync("Write a short story...");
Console.WriteLine(response.Text);
```

## Expected Output

```
============================================================
AgentFx with Claude via Azure AI Foundry
============================================================
Model: claude-haiku-4-5
Endpoint: https://<resource-name>.cognitiveservices.azure.com
============================================================

Prompt: Write a short story about a haunted house with a character named Lucia.

Response:
------------------------------------------------------------
[Claude's creative story about Lucia and the haunted house]
------------------------------------------------------------
```

## Related Samples

- **AgentFx-FoundryClaude-Persisting-01**: Demonstrates conversation persistence with Claude agents
- **AgentFx-AIWebChatApp-FoundryClaude**: Blazor web chat application with Claude agent

## Additional Resources

- [Microsoft Agent Framework Documentation](https://learn.microsoft.com/agent-framework/)
- [Azure AI Foundry - Claude Models](https://learn.microsoft.com/azure/ai-foundry/foundry-models/how-to/use-foundry-models-claude)
- [Claude API Documentation](https://docs.anthropic.com/claude/reference/messages_post)
- [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/ai-extensions)

## Troubleshooting

### Authentication Errors

- Verify API key is correct in user secrets
- Ensure Claude endpoint URL matches your Azure resource
- Check that deployment name matches your Claude model deployment

### Model Not Found

- Confirm Claude model is deployed in your Azure AI Foundry project
- Verify deployment name in configuration matches actual deployment

### Build Errors

- Ensure .NET 9 SDK is installed
- Run `dotnet restore` to restore NuGet packages
- Check that all package versions are compatible

## License

This sample is part of the Generative AI for Beginners .NET course and follows the repository's MIT license.
