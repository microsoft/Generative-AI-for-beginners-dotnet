# AgentFx-Persisting-02-Menu

This interactive console sample demonstrates creating, saving, loading, and resuming `AgentThread` instances via a simple menu-driven UI. It shows how you can persist conversation state to disk (temporary files by default) and resume later.

Features

- Start a new persisted session and save the thread to a file
- Start a non-persisted (in-memory) session for quick testing
- Load an existing saved thread and continue the conversation
- Uses `PersistingUI.cs` for console interactions and `StreamConsoleHelper` for formatted streaming output

Setup and run

1. Configure provider credentials (example using user-secrets):

 ```bash

cd samples/AgentFx/AgentFx-Persisting-02-Menu
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

- By default the sample writes saved thread files to the system temporary folder (`Path.GetTempPath()`), and prints the saved filename to console after persisting.
- Shared helper files are linked into the project (`ChatClientProvider.cs`, `StreamConsoleHelper.cs`, `ResponseClientProvider.cs`).
- Treat persisted thread files as sensitive — they contain conversational history.

Troubleshooting

- If you see authentication or authorization errors ensure user-secrets or environment variables are set for your chosen provider.
- If you cannot load a saved thread, confirm the file exists and you are using the same agent configuration as when it was saved.

Related docs

- Lesson guide: `06-AgentFx/README-PersistingConversations.md`
- Microsoft Learn: [Persisting and Resuming Agent Conversations](https://learn.microsoft.com/agent-framework/tutorials/agents/persisted-conversation?pivots=programming-language-csharp)

License: MIT
