# AgentFx-FoundryClaude-01: Basic Chat with Claude Agent

This sample demonstrates how to use **Microsoft Agent Framework (AgentFx)** with **Claude models** deployed in **Microsoft Foundry**. It shows the integration of `ChatClientAgent` with Claude using the **elbruno.Extensions.AI.Claude** NuGet package.

## Overview

- **Framework**: Microsoft Agent Framework (AgentFx)
- **AI Model**: Claude (Haiku, Sonnet, or Opus) via Microsoft Foundry
- **Pattern**: Basic agent chat with single prompt/response
- **Key Package**: [elbruno.Extensions.AI.Claude](https://www.nuget.org/packages/elbruno.Extensions.AI.Claude/) - provides seamless Claude integration

## Prerequisites

### 1. Microsoft Foundry Setup

1. Create a Microsoft Foundry project
2. Deploy a Claude model (e.g., `claude-haiku-4-5`, `claude-sonnet-4-5`)
3. Note your:
   - **Claude Endpoint**: `https://<resource-name>.services.ai.azure.com/anthropic/v1/messages`
   - **API Key**: From Azure portal
   - **Deployment Name**: Your Claude model deployment name

### 2. .NET Environment

- [.NET 9 SDK](https://dotnet.microsoft.com/download) or later

## Configuration

Set user secrets for the project:

```bash
cd samples/AgentFx/AgentFx-FoundryClaude-01

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

- Uses **elbruno.Extensions.AI.Claude** NuGet package for seamless Claude integration
- Implements `IChatClient` interface from Microsoft.Extensions.AI
- Enables native Claude model usage with Microsoft Agent Framework

### 2. **ChatClientAgent Pattern**

- Creates an `AIAgent` using `ChatClientAgent`
- Configures agent with name and instructions
- Executes single prompt with `RunAsync()`

### 3. **Simple Configuration**

- Direct instantiation of `AzureClaudeClient` with endpoint, model ID, and API key
- No custom HTTP handlers or transformations needed
- Clean, maintainable code following Microsoft.Extensions.AI patterns

## Code Structure

```
AgentFx-FoundryClaude-01/
├── Program.cs                          # Main application with agent setup
├── AgentFx-FoundryClaude-01.csproj     # Project file with dependencies
└── README.md                           # This file
```

## Key Code Snippets

### Installing the Package

```bash
dotnet add package elbruno.Extensions.AI.Claude --version 0.1.0-preview.2
```

### Creating the IChatClient with Claude

```csharp
using elbruno.Extensions.AI.Claude;
using Microsoft.Extensions.AI;

// Create IChatClient using AzureClaudeClient from the NuGet package
IChatClient chatClient = new AzureClaudeClient(
    endpoint: new Uri(endpointClaude),
    modelId: deploymentName,
    apiKey: apiKey);
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
AgentFx with Claude via Microsoft Foundry
============================================================
Model: claude-haiku-4-5
Endpoint: https://<resource-name>.services.ai.azure.com/anthropic/v1/messages
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

- [elbruno.Extensions.AI.Claude NuGet Package](https://www.nuget.org/packages/elbruno.Extensions.AI.Claude/)
- [Microsoft Agent Framework Documentation](https://learn.microsoft.com/agent-framework/)
- [Microsoft Foundry - Claude Models](https://learn.microsoft.com/azure/ai-foundry/foundry-models/how-to/use-foundry-models-claude)
- [Claude API Documentation](https://docs.anthropic.com/claude/reference/messages_post)
- [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/ai-extensions)

## Troubleshooting

### Authentication Errors

- Verify API key is correct in user secrets
- Ensure Claude endpoint URL matches your Azure resource
- Check that deployment name matches your Claude model deployment

### Model Not Found

- Confirm Claude model is deployed in your Microsoft Foundry project
- Verify deployment name in configuration matches actual deployment

### Build Errors

- Ensure .NET 9 SDK is installed
- Run `dotnet restore` to restore NuGet packages
- Check that all package versions are compatible

## License

This sample is part of the Generative AI for Beginners .NET course and follows the repository's MIT license.
