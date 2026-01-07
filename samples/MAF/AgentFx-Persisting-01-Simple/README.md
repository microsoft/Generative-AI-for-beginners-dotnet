# AgentFx-Persisting-01-Simple

This sample demonstrates how to persist (serialize) an `AgentThread` to disk and later reload it to resume a conversation with the same agent type.

What it shows

- Creating an `AgentThread` and running a prompt
- Serializing the thread JSON to `agent_thread.json` in the application folder
- Rehydrating the thread by deserializing the JSON and calling `agent.DeserializeThread(...)`
- Continuing the conversation with the resumed thread

Setup

1. Configure your AI provider credentials (example using dotnet user-secrets):

 ```bash

cd samples/MAF/AgentFx-Persisting-01-Simple
dotnet user-secrets set "endpoint" "https://<your-endpoint>.services.ai.azure.com/"
dotnet user-secrets set "deploymentName" "gpt-5-mini"
dotnet user-secrets set "apikey" "<your-api-key>"
 ```

1. Build and run:

 ```bash

dotnet build
dotnet run
 ```

Notes

- The sample relies on shared helper files (ChatClientProvider.cs, StreamConsoleHelper.cs, ResponseClientProvider.cs) that are linked into the project via `csproj` compile includes.
- The saved thread file `agent_thread.json` will be created in the app working directory. Treat this file as sensitive — it contains conversation history.

Troubleshooting

- If you see authentication errors, verify your user-secrets or environment variables are set correctly for your chosen provider.
- If deserialization fails, ensure you are using the same agent type to deserialize the thread.

Related docs

- Lesson guide: `06-AgentFx/README-PersistingConversations.md`
- Microsoft Learn: [Persisting and Resuming Agent Conversations](https://learn.microsoft.com/agent-framework/tutorials/agents/persisted-conversation?pivots=programming-language-csharp)

License: MIT
