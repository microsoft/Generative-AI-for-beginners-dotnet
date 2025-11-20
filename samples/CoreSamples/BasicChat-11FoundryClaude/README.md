# BasicChat-11FoundryClaude

This sample demonstrates how to use Claude models deployed in Microsoft Foundry with the Microsoft.Extensions.AI framework.

## Overview

This sample uses a custom `ClaudeToOpenAIMessageHandler` to transform Azure OpenAI API requests to the Claude Messages API format required by Microsoft Foundry Claude deployments.

## Prerequisites

1. A Microsoft Foundry resource with a deployed Claude model (e.g., Claude Haiku 4.5, Claude Sonnet 4.5, or Claude Opus 4.1)
2. .NET 10.0 SDK or later
3. API key from your Microsoft Foundry Claude deployment

## Setup

### 1. Set User Secrets

Configure your Microsoft Foundry credentials using .NET user secrets:

```bash
cd samples/CoreSamples/BasicChat-11FoundryClaude
dotnet user-secrets set "endpoint" "https://<your-resource-name>.cognitiveservices.azure.com"
dotnet user-secrets set "endpointClaude" "https://<your-resource-name>.services.ai.azure.com/anthropic/v1/messages"
dotnet user-secrets set "apikey" "<your-api-key>"
dotnet user-secrets set "deploymentName" "claude-haiku-4-5"
```

Replace:

- `<your-resource-name>` with your Microsoft Foundry resource name
- `<your-api-key>` with your API key from the Microsoft Foundry portal
- `claude-haiku-4-5` with your deployment name if different

### 2. Run the Sample

```bash
dotnet run
```

## Authentication

Claude models in Microsoft Foundry use **API key authentication** with the `x-api-key` header (not `Authorization: Bearer`). The custom `ClaudeToOpenAIMessageHandler` handles this transformation automatically.

According to the [Microsoft documentation](https://learn.microsoft.com/en-us/azure/ai-foundry/foundry-models/how-to/use-foundry-models-claude?view=foundry-classic), Claude API endpoints use:

- **Header**: `x-api-key: <your-api-key>`
- **Endpoint**: `https://<resource-name>.services.ai.azure.com/anthropic/v1/messages`
- **Required header**: `anthropic-version: 2023-06-01`

## How It Works

1. The application creates an `AzureOpenAIClient` with a custom HTTP transport
2. The `ClaudeToOpenAIMessageHandler` intercepts requests to Claude deployments
3. It transforms OpenAI-style requests to Claude Messages API format:
   - Converts message format and extracts system messages
   - Skips empty messages (Claude requires non-empty content)
   - Adds required Claude headers (`x-api-key`, `anthropic-version`)
   - Transforms endpoint URL to Claude's format
4. Streaming responses are transformed back to OpenAI format using SSE (Server-Sent Events)
5. All responses maintain OpenAI API compatibility

## Code Architecture

The handler is organized into clear regions:

- **Request Transformation**: Converts OpenAI format to Claude format
- **Response Transformation**: Converts Claude responses back to OpenAI format  
- **Streaming Response Transformation**: Handles Server-Sent Events (SSE) streaming

Key features:

- Constants for maintainable configuration
- Small, focused methods with single responsibilities
- Clear separation of concerns
- XML documentation for IntelliSense support

## Supported Models

This sample works with all Claude models available in Microsoft Foundry:

- Claude Haiku 4.5
- Claude Sonnet 4.5
- Claude Opus 4.1

## References

- [Deploy and use Claude models in Microsoft Foundry](https://learn.microsoft.com/en-us/azure/ai-foundry/foundry-models/how-to/use-foundry-models-claude?view=foundry-classic)
- [Claude API Documentation](https://docs.claude.com/en/api/messages)
- [Microsoft.Extensions.AI](https://learn.microsoft.com/en-us/dotnet/ai/ai-extensions)
