# AgentFx-BackgroundResponses-01-Simple

This simple sample demonstrates background responses (continuation tokens) using a minimal console app. It shows how to start streaming a response, capture a continuation token, stop the stream, and later resume the generation using that token.

What it shows

- Enabling background responses via `AgentRunOptions.AllowBackgroundResponses`.
- Streaming partial responses using `RunStreamingAsync`.
- Capturing a continuation token and stopping the stream.
- Resuming generation with the previously captured token and combining streamed fragments into readable output.

Files of interest

- `Program.cs` – demo flow and streaming logic
- `StreamConsoleHelper.cs` – helper for formatting and accumulating token fragments
- `ResponseClientProvider.cs` – obtains the response client configured for your environment

Run

1. Configure your provider credentials (user-secrets or environment variables) as described in the top-level lesson `06-AgentFx/readme.md`.

2. Build and run:

```bash
cd samples/AgentFx/AgentFx-BackgroundResponses-01-Simple
dotnet build
dotnet run
```

Related

- Lesson guide: `06-AgentFx/README-BackgroundResponses.md`
- Persisted conversation samples: `../AgentFx-Persisting-01-Simple/` and `../AgentFx-Persisting-02-Menu/`

License: MIT
