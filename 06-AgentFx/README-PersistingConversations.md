# Persisting conversations with AgentFx

This guide explains how to persist (serialize) and resume (deserialize) AgentFx conversation threads so you can continue a conversation across application restarts or on different hosts.

Conceptual overview

- AgentThread: represents the conversation history and state for an agent. It is created by calling `agent.GetNewThread()` or returned by `agent.RunAsync(...)`.
- Serialize: call `thread.Serialize(JsonSerializerOptions.Web).GetRawText()` to obtain a JSON string you can store in a file, blob, or database.
- Deserialize: load the JSON back, parse it to a `JsonElement`, and call `agent.DeserializeThread(jsonElement, JsonSerializerOptions.Web)` on an agent instance of the same type to rehydrate the thread.

Minimal C# flow (pseudo-code)

```csharp
// create or obtain your agent (same type used for deserialization)
var agent = new ChatClientAgent(chatClient, options);

// create a thread and run a prompt
var thread = agent.GetNewThread();
var response = await agent.RunAsync("Hello world", thread);

// serialize the thread
var json = thread.Serialize(JsonSerializerOptions.Web).GetRawText();
File.WriteAllText("agent_thread.json", json);

// later - deserialize and resume
var rehydratedJson = JsonDocument.Parse(File.ReadAllText("agent_thread.json")).RootElement;
var resumedThread = agent.DeserializeThread(rehydratedJson, JsonSerializerOptions.Web);
var next = await agent.RunAsync("Continue the conversation", resumedThread);
```

Important notes and pitfalls

- Use the same agent type for deserialization as the one that created the thread. Deserializing into a different agent class may fail or produce unexpected behavior.
- Persisted thread JSON contains conversation history and metadata — treat it as sensitive data and store securely.
- The serialization format is portable JSON; you can store it in files, databases, or object storage.
- If you rely on external tools or environment-specific resources during run (for example, tool handles or local paths), ensure those are available or reattached when resuming.

How the persisting samples in this repo work

- `samples/MAF/AgentFx-Persisting-01-Simple`: demonstrates creating a thread, running a prompt, saving the `agent_thread.json` file in the app folder, and reloading it to continue the conversation.
- `samples/MAF/AgentFx-Persisting-02-Menu`: interactive console that lets you create new persisting sessions, save to a temp file, and load previously saved threads from disk for resumption.

- `samples/MAF/AgentFx-AIWebChatApp-Persisting`: a Blazor web chat application that demonstrates per-user `AgentThread` persistence. Each user session maps to a durable thread which is serialized after each interaction and stored; returning users can resume their conversations with full context. This sample shows session mapping, storage abstraction, secure JSON handling, and thread rehydration in a web-hosted environment.

Running the samples

1. Configure provider secrets (example for Azure/OpenAI or GitHub models):

```bash
cd samples/MAF/AgentFx-Persisting-01-Simple
dotnet user-secrets set "endpoint" "https://<your-endpoint>.services.ai.azure.com/"
dotnet user-secrets set "deploymentName" "gpt-5-mini"
dotnet user-secrets set "apikey" "<your-api-key>"
dotnet run
```

or for the menu sample:

```bash
cd samples/MAF/AgentFx-Persisting-02-Menu
dotnet run
```

Further reading

- Microsoft Learn: [Persisting and Resuming Agent Conversations](https://learn.microsoft.com/agent-framework/tutorials/agents/persisted-conversation?pivots=programming-language-csharp)
- AgentFx Background Responses (continuations): `README-BackgroundResponses.md` (this directory)

License: MIT
