# AgentFx-FoundryClaude-Persisting-01: Persisting Conversations with Claude

This sample demonstrates **conversation persistence** using Microsoft Agent Framework (AgentFx) with **Claude models** deployed in **Microsoft Foundry**. It shows how to serialize and deserialize agent threads to maintain conversation context across sessions.

## Overview

- **Framework**: Microsoft Agent Framework (AgentFx)
- **AI Model**: Claude (Haiku, Sonnet, or Opus) via Microsoft Foundry
- **Pattern**: Thread serialization and persistence
- **Key Concepts**: `GetNewThread()`, `Serialize()`, `DeserializeThread()`, stateful conversations

## What This Sample Demonstrates

This sample walks through four key steps:

1. **Create and Persist Thread**: Create a new thread, ask a question, and save the thread state to a JSON file
2. **Fresh Thread Without Context**: Start a new thread and ask a question that requires context (demonstrates no memory)
3. **Reload Persisted Thread**: Load the saved thread and ask the same question (demonstrates preserved context)
4. **Continue Conversation**: Continue the conversation in the persisted thread with full history

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
cd samples/AgentFx/AgentFx-FoundryClaude-Persisting-01

dotnet user-secrets set "endpoint" "https://<resource-name>.cognitiveservices.azure.com"
dotnet user-secrets set "endpointClaude" "https://<resource-name>.services.ai.azure.com/anthropic/v1/messages"
dotnet user-secrets set "apikey" "<your-api-key>"
dotnet user-secrets set "deploymentName" "claude-haiku-4-5"
```

## How to Run

```bash
# Navigate to the project directory
cd samples/AgentFx/AgentFx-FoundryClaude-Persisting-01

# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

## Expected Output

The sample will demonstrate four distinct scenarios:

```
============================================================
Step 1: Create new thread, ask a question, and persist the thread
============================================================
Using Claude model: claude-haiku-4-5

--- Start answering your question ---
Question: My name is Bruno and I live in Madrid
[Response from Claude acknowledging the information]

Saved thread to: ...\agent_thread_claude.json

============================================================
Step 2: Create new thread and ask "where do I live?"
============================================================
--- Start answering your question (without context) ---
Question: Where do I live?
[Response indicating the agent doesn't know without context]

============================================================
Step 3: Load persisted thread and ask "where do I live?"
============================================================
--- Start answering your question (with context) ---
Question: Where do I live?
[Response: "You live in Madrid" - context preserved!]

============================================================
Step 4: Continue conversation with more context
============================================================
--- Start answering your question ---
Question: What is my name?
[Response: "Your name is Bruno" - full history available]

Updated thread saved to: ...\agent_thread_claude.json
```

## Key Code Snippets

### Creating and Persisting a Thread

```csharp
// Create new thread
var thread = agent.GetNewThread();

// Run agent with the thread
var response = await agent.RunAsync(question, thread);

// Serialize thread to JSON
var threadRaw = thread.Serialize(JsonSerializerOptions.Web).GetRawText();
await File.WriteAllTextAsync(SavedThreadFilePath, threadRaw);
```

### Reloading a Persisted Thread

```csharp
// Load JSON from file
var loadedJson = await File.ReadAllTextAsync(SavedThreadFilePath);
JsonElement reloaded = JsonSerializer.Deserialize<JsonElement>(loadedJson, JsonSerializerOptions.Web);

// Rehydrate the thread
thread = agent.DeserializeThread(reloaded, JsonSerializerOptions.Web);

// Continue conversation with full context
response = await agent.RunAsync(nextQuestion, thread);
```

## Code Structure

```
AgentFx-FoundryClaude-Persisting-01/
├── Program.cs                                    # Main application with persistence logic
├── ClaudeToOpenAIMessageHandler.cs               # HTTP handler for Claude API translation
├── AgentFx-FoundryClaude-Persisting-01.csproj    # Project file with dependencies
└── README.md                                     # This file

Linked Files:
└── StreamConsoleHelper.cs (from AgentFx-BackgroundResponses-01-Simple)
```

## Use Cases

This pattern is valuable for:

- **Multi-session chatbots**: Resume conversations across user sessions
- **Long-running workflows**: Persist agent state for complex multi-step processes
- **Debugging and testing**: Save and replay conversation threads
- **Context management**: Maintain conversation history without re-sending full history

## Technical Details

### Thread Serialization Format

The thread is serialized to JSON using `JsonSerializerOptions.Web` which includes:

- All conversation messages (user and assistant)
- Message metadata and timestamps
- Thread configuration and state

### File Storage

Threads are saved to `agent_thread_claude.json` in the application directory. In production scenarios, consider:

- Database storage for scalability
- Encrypted storage for sensitive data
- Retention policies for old threads
- User-specific thread isolation

## Related Samples

- **AgentFx-FoundryClaude-01**: Basic chat without persistence
- **AgentFx-AIWebChatApp-FoundryClaude**: Web-based chat with Claude agent
- **AgentFx-Persisting-01-Simple**: Persistence with OpenAI models

## Additional Resources

- [Microsoft Agent Framework Documentation](https://learn.microsoft.com/agent-framework/)
- [AgentFx Persisting Conversations Tutorial](https://learn.microsoft.com/agent-framework/tutorials/agents/persisted-conversation)
- [Microsoft Foundry - Claude Models](https://learn.microsoft.com/azure/ai-foundry/foundry-models/how-to/use-foundry-models-claude)
- [Claude API Documentation](https://docs.anthropic.com/claude/reference/messages_post)

## Troubleshooting

### Thread Serialization Errors

- Ensure `JsonSerializerOptions.Web` is used for both serialization and deserialization
- Verify the thread file is valid JSON
- Check file permissions for read/write access

### Context Not Preserved

- Confirm you're using the correct thread (not creating a new one)
- Verify the thread was successfully deserialized before running
- Check that the file contains the expected conversation history

### Claude API Errors

- Verify API key and endpoints are correct
- Ensure Claude model is deployed and accessible
- Check Microsoft Foundry service health

## License

This sample is part of the Generative AI for Beginners .NET course and follows the repository's MIT license.
