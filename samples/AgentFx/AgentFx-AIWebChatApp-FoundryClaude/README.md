# AgentFx-AIWebChatApp-FoundryClaude: Blazor Web Chat with Claude

This sample demonstrates a **Blazor Server web application** using **Microsoft Agent Framework (AgentFx)** with **Claude models** deployed in **Microsoft Foundry**. It provides a modern, interactive chat interface for real-time conversations with Claude.

## Overview

- **Framework**: Blazor Server (.NET 9) + Microsoft Agent Framework
- **AI Model**: Claude (Haiku, Sonnet, or Opus) via Microsoft Foundry
- **Pattern**: Interactive web chat with dependency injection
- **Key Features**: Real-time chat, streaming responses, modern UI

## Prerequisites

### 1. Microsoft Foundry Setup

1. Create an Microsoft Foundry project
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
cd samples/AgentFx/AgentFx-AIWebChatApp-FoundryClaude

dotnet user-secrets set "endpoint" "https://<resource-name>.cognitiveservices.azure.com"
dotnet user-secrets set "endpointClaude" "https://<resource-name>.services.ai.azure.com/anthropic/v1/messages"
dotnet user-secrets set "apikey" "<your-api-key>"
dotnet user-secrets set "deploymentName" "claude-haiku-4-5"
```

## How to Run

```bash
# Navigate to the project directory
cd samples/AgentFx/AgentFx-AIWebChatApp-FoundryClaude

# Restore dependencies
dotnet restore

# Run the application
dotnet run

# Open your browser to https://localhost:5001 (or the URL shown in console)
```

## Features

### 1. **Modern Chat Interface**

- Clean, gradient-themed UI
- Message bubbles for user and assistant
- Smooth animations and transitions
- Responsive design

### 2. **Real-time Interaction**

- Send messages with Enter key (Shift+Enter for new line)
- Loading indicator while Claude thinks
- Auto-scroll to latest messages
- Disabled input during processing

### 3. **Agent Framework Integration**

- Claude agent registered via dependency injection
- Persistent agent instance across requests
- Error handling and logging
- Blazor Server interactive rendering

## Code Structure

```
AgentFx-AIWebChatApp-FoundryClaude/
├── Program.cs                              # App configuration and DI setup
├── ClaudeToOpenAIMessageHandler.cs         # HTTP handler for Claude API
├── Components/
│   ├── App.razor                           # Root component
│   ├── Routes.razor                        # Routing configuration
│   ├── Layout/
│   │   └── MainLayout.razor                # Main layout
│   └── Pages/
│       └── Home.razor                      # Chat UI page
├── wwwroot/
│   └── app.css                             # Application styles
├── appsettings.json                        # App configuration
└── README.md                               # This file
```

## Key Code Snippets

### Agent Registration in Program.cs

```csharp
// Configure Claude IChatClient
builder.Services.AddSingleton<IChatClient>(_ =>
{
    var customHttpMessageHandler = new ClaudeToOpenAIMessageHandler
    {
        AzureClaudeDeploymentUrl = endpointClaude,
        ApiKey = apiKey,
        Model = deploymentName
    };
    HttpClient customHttpClient = new(customHttpMessageHandler);
    var transport = new HttpClientPipelineTransport(customHttpClient);

    return new AzureOpenAIClient(
        endpoint: new Uri(endpoint),
        credential: new ApiKeyCredential(apiKey),
        options: new AzureOpenAIClientOptions { Transport = transport })
    .GetChatClient(deploymentName)
    .AsIChatClient()
    .AsBuilder()
    .Build();
});

// Register AI Agent
builder.Services.AddSingleton<AIAgent>(sp =>
{
    var chatClient = sp.GetRequiredService<IChatClient>();
    return chatClient.CreateAIAgent(
        name: "ClaudeChat",
        instructions: "You are a helpful and friendly AI assistant..."
    );
});
```

### Using Agent in Blazor Component

```csharp
@inject AIAgent Agent

private async Task SendMessage()
{
    _messages.Add(new ChatMessage("user", userMessage));
    _isLoading = true;
    StateHasChanged();

    try
    {
        var response = await Agent.RunAsync(userMessage);
        _messages.Add(new ChatMessage("assistant", response.Text));
    }
    finally
    {
        _isLoading = false;
        StateHasChanged();
    }
}
```

## UI Features

### Chat Interface

- **Header**: Gradient background with app title
- **Message Area**: Scrollable conversation history
- **User Messages**: Blue gradient bubbles, right-aligned
- **Claude Messages**: White bubbles with border, left-aligned
- **Input Area**: Multi-line textarea with send button

### Keyboard Shortcuts

- **Enter**: Send message
- **Shift+Enter**: New line in message

## Deployment Considerations

### Production Deployment

For production use, consider:

1. **Authentication**: Add user authentication (Azure AD, Identity, etc.)
2. **Rate Limiting**: Implement request throttling
3. **Session Management**: Persist conversation history
4. **HTTPS**: Ensure secure connections
5. **Logging**: Enhanced logging and Application Insights
6. **Scalability**: Consider Azure App Service or Azure Container Apps

### Environment Variables

For deployment environments, use environment variables instead of user secrets:

```bash
export endpoint="https://<resource-name>.cognitiveservices.azure.com"
export endpointClaude="https://<resource-name>.services.ai.azure.com/anthropic/v1/messages"
export apikey="<your-api-key>"
export deploymentName="claude-haiku-4-5"
```

## Enhancements

Potential improvements:

- **Streaming Responses**: Display tokens as they arrive
- **Conversation History**: Save and load past conversations
- **Multiple Threads**: Support multiple conversation threads
- **Markdown Rendering**: Render formatted responses
- **File Attachments**: Support document upload
- **Function Calling**: Add tools/functions for the agent

## Related Samples

- **AgentFx-FoundryClaude-01**: Basic console chat with Claude
- **AgentFx-FoundryClaude-Persisting-01**: Console chat with conversation persistence
- **AgentFx-AIWebChatApp-Simple**: Web chat with OpenAI models

## Additional Resources

- [Blazor Documentation](https://learn.microsoft.com/aspnet/core/blazor/)
- [Microsoft Agent Framework](https://learn.microsoft.com/agent-framework/)
- [Microsoft Foundry - Claude Models](https://learn.microsoft.com/azure/ai-foundry/foundry-models/how-to/use-foundry-models-claude)
- [Claude API Documentation](https://docs.anthropic.com/claude/reference/messages_post)
- [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/ai-extensions)

## Troubleshooting

### Application Won't Start

- Verify .NET 9 SDK is installed
- Check user secrets are configured correctly
- Ensure ports 5000/5001 are available

### Claude API Errors

- Verify API key and endpoints in user secrets
- Check Claude model deployment status in Azure
- Review application logs for detailed error messages

### UI Not Updating

- Ensure `@rendermode InteractiveServer` is set on the page
- Check browser console for JavaScript errors
- Verify Blazor SignalR connection is established

## License

This sample is part of the Generative AI for Beginners .NET course and follows the repository's MIT license.
