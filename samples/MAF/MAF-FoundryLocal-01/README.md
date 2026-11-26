# Agent Framework with Foundry Local

These are actual `AIAgent` instances created from MEAI `IChatClient`, not just a manually implemented function loop. Follow [setup](../../../01-IntroductionToGenerativeAI/setup-foundry-local.md); from repository root:

```powershell
dotnet run --project .\samples\CoreSamples\FoundryLocal-01Techniques -- prepare --prepare
dotnet run --project .\samples\MAF\MAF-FoundryLocal-01 -- story
dotnet run --project .\samples\MAF\MAF-FoundryLocal-01 -- tools
dotnet run --project .\samples\MAF\MAF-FoundryLocal-01 -- workflow
```

`story` generates a short Spanish story. `tools` uses one observable read-only library-hours function and checks the answer contains the opening time. `workflow` explicitly passes the writer's output to a separate editor agent sequentially; it does not pretend to be a hosted multi-agent service or a persisted workflow engine.

The default model is `phi-4-mini`. The SDK owns a private CPU-first runtime and loopback endpoint, adapted through official OpenAI/MEAI clients. This is **generic Agent Framework integration**, not a claim that the dedicated Foundry Local provider supports .NET.

The tools agent uses an explicitly bounded MEAI function stack and `UseProvidedChatClientAsIs` so another automatic stack does not hide the iteration policy. Requests have a four-minute cooperative cancellation budget; tools and outputs remain untrusted. No Azure keys, remote tools or community adapter are required.

Read [Program.cs](./Program.cs), then use [local MCP](../../CoreSamples/MCP-FoundryLocal-LocalTools/README.md) and the [full route](../../../docs/foundry-local-learning-path.md).
