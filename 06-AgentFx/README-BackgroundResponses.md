# AgentFx Background Responses - Simple Sample

This sample demonstrates how to use the Agent Framework "Background Responses" feature with a minimal console app.

Background responses let an agent continue generating output in the background after you stop listening to streaming updates. You can capture a continuation token during streaming, stop the stream (for example to process partial content or simulate interruption), and later resume the generation using that token to receive the remaining text.

Documentation: [Agent Background Responses](https://learn.microsoft.com/agent-framework/user-guide/agents/agent-background-responses)

What this sample shows

- How to enable background responses via `AgentRunOptions.AllowBackgroundResponses`.
- How to stream partial responses from an agent using `RunStreamingAsync`.
- How to capture a continuation token from a streaming update and stop the stream.
- How to resume generation from the continuation token and receive the remainder of the response.
- A small helper that formats console output and collects token fragments into readable sentences during continuation so resumed output is printed as human-friendly paragraphs rather than one token per line.

Project files of interest

- `Program.cs` – contains the demo flow in `BackgroundResponsesDemo.RunAsync()`.
- `StreamConsoleHelper.cs` – moved from the top of `Program.cs` into its own file. Contains printing helpers and the token-accumulation logic used during continuation.
- `ResponseClientProvider` – (project-local) abstraction used to obtain the response client configured for your environment.

Recent changes / notes

- The console helper that formats streaming updates was moved to its own file (`StreamConsoleHelper.cs`) to keep `Program.cs` focused on the demo flow.
- Continuation output is now accumulated and flushed as full sentences or phrases (on sentence terminators, newlines, or when the buffer grows) so the resumed response is human-readable instead of token-per-line.

## Complex sample (03-Complex)

A companion sample demonstrates a slightly more complex flow that combines short interactive queries with a long-running generation that you interrupt and later resume. See:

`../samples/MAF/AgentFx-BackgroundResponses-03-Complex/Program.cs`

What the complex sample demonstrates

- The same background-response pattern (stream → interrupt → resume) used in the simple sample.
- How to interleave short synchronous/question-style prompts on the same agent thread between streaming and continuation. The sample asks two short questions (for example arithmetic and a capital city question) and prints those answers using the accumulation helper so token fragments are joined into readable text.
- How to resume a previously interrupted long response after handling other short queries – useful for workflows that need to process quick follow-ups or tool calls before continuing a larger generated result.
- Shows the same `StreamConsoleHelper` usage (StartAccumulatedStream, AccumulateAndPrint, FlushAccumulated) to produce readable continuation output.

Why this is useful

- Real-world agents often need to juggle longer background work with short immediate queries, or they may call tools/supplementary prompts while a long response is being generated. The complex sample shows how to structure your app to support these scenarios while keeping console output intelligible.

Run the complex sample

1. Change to the complex sample folder:

   Windows (CMD/PowerShell):
   ```bash
   cd samples\MAF\AgentFx-BackgroundResponses-03-Complex
   ```
   
   Linux/macOS/Git Bash/WSL/Codespaces:
   ```bash
   cd samples/MAF/AgentFx-BackgroundResponses-03-Complex
   ```

   > **Note**: The paths above assume you're in the repository root directory. If you're currently in the `06-AgentFx` directory:
   > - Windows (CMD/PowerShell): `cd ..\samples\MAF\AgentFx-BackgroundResponses-03-Complex`
   > - Linux/macOS/Git Bash/WSL/Codespaces: `cd ../samples/MAF/AgentFx-BackgroundResponses-03-Complex`

2. Build and run:

   ```bash
   dotnet build
   dotnet run
   ```

The same prerequisites apply (see above).

What to expect in the complex sample console

- A header and the initial long prompt. The app will stream initial updates and simulate an interruption after receiving some content.
- The sample then runs two short questions on the same thread; their outputs are printed as readable sentences using the accumulation helper.
- Finally the demo resumes the original long response using the earlier captured continuation token and prints the remainder as assembled sentences.

How to experiment

- Try adding calls to tools or other prompts between the interruption and the continuation to see how the agent and thread behave.
- Adjust the interruption condition in `Program.cs` to control when the sample stops streaming the initial response.

Notes

- Secrets and API keys are not stored in code. Use environment variables or user-secrets as described in the main repository docs.
- This sample focuses on clarity and learning, not production patterns. In production code, handle errors and retries, and avoid depending on Console for UX.

## Persisting conversations (how this differs from background responses)

Background responses and persisted conversations are complementary features of AgentFx and solve different needs:

- Background responses (continuation tokens) let a running generation be interrupted and resumed within the same agent thread and runtime — useful for streaming scenarios where you capture a continuation token during streaming and later continue the same generation.
- Persisted conversations (AgentThread serialization) let you store the entire conversation state to durable storage (file, blob, database) and reload it later, potentially in a new process or machine, preserving the conversation history and context across application restarts.

Key differences:

- Scope: continuation tokens resume a single in-flight generation; persisted threads restore full conversation history and metadata.
- Lifetime: continuation tokens are short-lived by design (for the same running generation). Persisted thread JSON is intended for long-term storage and retrieval.
- Usage patterns: use background responses when you need to pause/resume a long-running generation inside the same session; use persisted conversations when you need to save chat history and resume later (e.g., chat apps, long-running agents, user sessions).

See the new lesson-level guide on persisting conversations for step-by-step serialization/deserialization examples and run instructions: [README-PersistingConversations.md](./README-PersistingConversations.md).

License
MIT
