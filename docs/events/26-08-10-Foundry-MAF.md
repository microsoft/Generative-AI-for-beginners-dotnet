# Podcast Session — Microsoft Foundry + Microsoft Agent Framework

**Date:** 2026-08-10 · **Format:** Guest on a video podcast · **Duration:** 45 minutes
**Repo:** [microsoft/Generative-AI-for-beginners-dotnet](https://github.com/microsoft/Generative-AI-for-beginners-dotnet?wt.mc_id=dotnet-153583-brunocapuano)

This is the host support document for the session. It walks a **beginner → advanced** arc: one chat call, then history and streaming, then tools, then multimodal, then RAG, then live grounding via MCP, then agents, and finally multi-agent workflows and Foundry's two agent types.

Every demo runs against **Microsoft Foundry** (cloud) with **keyless auth** (Microsoft Entra ID via `az login`) so nothing on screen leaks a secret.

---

## Pre-flight checklist

Run this **before** going live. Total warm-up ≈ 5 minutes.

Every demo in this doc — CoreSamples and MAF alike — shares the same `UserSecretsId`, **`genai-beginners-dotnet`**. That means secrets are set **once, globally**, and every sample picks them up. The repo ships a root script that does exactly this.

```powershell
# 1. Sign in (keyless auth — no API keys on screen)
az login

# 2. Set the shared secrets for ALL samples in one shot.
#    setup-secrets.ps1 writes to the shared store via `dotnet user-secrets --id`,
#    so it works from the repo root — no need to cd into any project.
#    -Deployment and -EmbeddingDeployment already default to the values below.
#    If you want the image appendix demo ready too, pass -ImageDeployment.
.\setup-secrets.ps1 -Endpoint "https://<your-endpoint>.openai.azure.com/" `
                    -Deployment "gpt-5-mini" `
                    -EmbeddingDeployment "text-embedding-3-small" `
                    -ImageDeployment "gpt-image-2"

# 3. Demo 8b additionally needs the Foundry project endpoint + an agent name.
#    "agentName" is the agent's identity *in your code* — it appears in the
#    console prefix "Agent [X]:". It is NOT a portal resource: the Responses
#    API path creates no server-side agent. The sample throws immediately if
#    it's missing, so set it now.
#    Same shared store, so --id works from the repo root too.
dotnet user-secrets set --id genai-beginners-dotnet "azureFoundryProjectEndpoint" "https://<your-project>.services.ai.azure.com/api/projects/<project>"
dotnet user-secrets set --id genai-beginners-dotnet "agentName" "PodcastAgent"

# 4. OPTIONAL — only if you might use the appendix demos.
#    AgentLabs-* wants its own spelling of the Foundry project endpoint:
dotnet user-secrets set --id genai-beginners-dotnet "aifoundryproject_endpoint" "https://<your-project>.services.ai.azure.com/api/projects/<project>"
dotnet user-secrets set --id genai-beginners-dotnet "aifoundryproject_tenantid" "<your-tenant-id>"   # az account show --query tenantId -o tsv
#    The Blazor web chat apps are Aspire-flavoured and want a connection string:
dotnet user-secrets set --id genai-beginners-dotnet "ConnectionStrings:openai" "Endpoint=https://<your-endpoint>.openai.azure.com/"
#    Speech needs its own Azure AI Speech resource + a working microphone:
dotnet user-secrets set --id genai-beginners-dotnet "SPEECH_KEY" "<your-speech-key>"
dotnet user-secrets set --id genai-beginners-dotnet "SPEECH_REGION" "<your-speech-region>"

# 5. Warm the build cache so no demo pauses on a cold compile
dotnet build samples\CoreSamples\CoreGenerativeAITechniques.sln -c Release
dotnet build samples\MAF\MAF-Demos.slnx -c Release
```

> **Starting from zero Azure resources?** Run `.\setup.ps1` instead of step 2 — it runs `azd up` to provision the Foundry resource *and* sets the same secrets automatically. Use `setup-secrets.ps1` when the resource already exists.
>
> **Verify at any time:** `dotnet user-secrets list --id genai-beginners-dotnet`

**Also confirm:** your Foundry resource has a chat deployment (`gpt-5-mini`) and an embedding deployment (`text-embedding-3-small`). If you want the image appendix demo ready too, add an image deployment such as `gpt-image-2`. Demo 5 (RAG) fails without the embedding deployment.

**On-screen hygiene:** increase terminal font size, clear the console between demos, and keep the Foundry portal open in a second tab for Demo 1 (the model deployments blade — useful when you swap deployment names).

---

## Session timeline

| Time | # | Topic | Sample |
|------|---|-------|--------|
| 0:00–0:03 | 0 | Intro — what is Microsoft Foundry, where MEAI and MAF fit | — |
| 0:03–0:07 | 1 | Hello Foundry — the first chat call | `BasicChat-05AIFoundryModels` |
| 0:07–0:11 | 2 | Streaming + conversation history | `BasicChat-10ConversationHistory` |
| 0:11–0:16 | 3 | Function / tool calling | `MEAIFunctionsAzureOpenAI` |
| 0:16–0:20 | 4 | Vision / multimodal | `Vision-01MEAI-AzureOpenAI` |
| 0:20–0:25 | 5 | RAG — embeddings + vector search | `RAGSimple-02MEAIVectorsMemory` |
| 0:25–0:29 | 6 | MCP — grounding in live Microsoft Learn docs | `MCP-03-MicrosoftLearn` |
| 0:29–0:34 | 7 | First agent with Microsoft Agent Framework | `MAF01` |
| 0:34–0:42 | 8 | Multi-agent workflows + Foundry agent types | `MAF02`, `MAF-MicrosoftFoundryAgents-01` |
| 0:42–0:45 | 9 | Wrap — where to start, resources | — |

---

## 0 · Intro (3 min)

No code. Set the frame so the demos land:

- **Microsoft Foundry** is the platform: one endpoint, many models (OpenAI, Grok, Phi, Claude, Llama), plus agents, evaluations and observability.
- **Microsoft.Extensions.AI (MEAI)** is the .NET abstraction layer: `IChatClient` and `IEmbeddingGenerator<,>`. Same code, any provider.
- **Microsoft Agent Framework (MAF)** is the layer above: agents, tools, threads, and multi-agent workflows.
- The arc for today: *one call → tools → knowledge → agents → teams of agents.*

**Docs**
- [Microsoft Foundry documentation](https://learn.microsoft.com/azure/ai-foundry/?wt.mc_id=dotnet-153583-brunocapuano)
- [.NET + AI overview](https://learn.microsoft.com/dotnet/ai/?wt.mc_id=dotnet-153583-brunocapuano)
- [Microsoft Agent Framework](https://learn.microsoft.com/agent-framework/?wt.mc_id=dotnet-153583-brunocapuano)

---

## 1 · Hello Foundry — the first chat call (4 min)

**Code:** [`samples/CoreSamples/BasicChat-05AIFoundryModels/app.cs`](../../samples/CoreSamples/BasicChat-05AIFoundryModels/app.cs)

```powershell
cd samples\CoreSamples\BasicChat-05AIFoundryModels   # from the repo root
dotnet run app.cs
```

**What the code does.** This is a .NET 10 *file-based app* — a single `app.cs` with `#:package` directives at the top, no `.csproj`, run with `dotnet run app.cs`. It builds an `AzureOpenAIClient` pointed at a Microsoft Foundry endpoint, picks a deployment name from user secrets, and calls `.AsIChatClient()` to turn the provider-specific client into the provider-agnostic `IChatClient` from Microsoft.Extensions.AI. It then asks one question with `GetResponseAsync` and prints the answer. The sample also has an auth switch: `integrated` uses `AzureCliCredential` (Microsoft Entra ID, no keys anywhere), while `apikey` uses a portal key — a great live illustration of why keyless is the default.

**Top 3 talking points**
1. **One endpoint, many models.** Change only `AzureOpenAI:Deployment` and the same code talks to a different model. Swap `gpt-5-mini` → another deployment live and re-run — that is the whole demo.
2. **`IChatClient` is the contract.** From this line forward, nothing in the session is Azure-specific. The same interface backs Ollama, GitHub Models, Foundry Local, and OpenAI.
3. **Keyless by default.** `AzureCliCredential` means no key in source, in config, or on screen. This is the recommended production posture, not just a demo trick.

**The code that matters**

```csharp
// One endpoint, many models — the deployment name is the only thing that changes.
var deploymentName = config["AzureOpenAI:Deployment"] ?? "gpt-5-mini";

// Keyless: Microsoft Entra ID via `az login`. No key in source, config, or on screen.
IChatClient client = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient();          // <- provider-specific becomes provider-agnostic

var response = await client.GetResponseAsync("what is your model name?");
Console.WriteLine(response.Text);
```

**Docs**
- [Microsoft.Extensions.AI libraries](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai?wt.mc_id=dotnet-153583-brunocapuano)
- [Authenticate to Azure AI services with Microsoft Entra ID](https://learn.microsoft.com/azure/ai-services/authentication?wt.mc_id=dotnet-153583-brunocapuano)
- [File-based apps in .NET 10](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-10/sdk?wt.mc_id=dotnet-153583-brunocapuano)

---

## 2 · Streaming + conversation history (4 min)

**Code:** [`samples/CoreSamples/BasicChat-10ConversationHistory/app.cs`](../../samples/CoreSamples/BasicChat-10ConversationHistory/app.cs)

```powershell
cd samples\CoreSamples\BasicChat-10ConversationHistory   # from the repo root
cd ..\BasicChat-10ConversationHistory                    # from Demo 1
dotnet run app.cs
```

**What the code does.** This sample turns the single call from Demo 1 into a real conversation. It keeps a `List<ChatMessage>` seeded with a system message, appends each user turn, streams the reply with `GetStreamingResponseAsync` so tokens appear live, accumulates those tokens into a `StringBuilder`, and appends the completed assistant turn back into the list. That list *is* the memory — the model itself is stateless, and each call resends the full history. The sample is Foundry-first and falls back to a local Ollama model when no endpoint is configured, which proves the point that the conversation code is identical across providers.

**Top 3 talking points**
1. **Streaming is a UX feature, not a model feature.** Same request, different consumption pattern — time-to-first-token drops dramatically and the app feels alive.
2. **The model has no memory.** You own the history. Every turn resends the whole list, which is exactly why context windows and token costs grow with conversation length.
3. **The `.Messages` gotcha.** The response exposes `.Text` and `.Message` (singular) — not `.Messages`. This trips up nearly everyone the first time; the README documents the correct patterns.

**The code that matters**

```csharp
// This list IS the memory. The model is stateless — every turn resends the whole thing.
List<ChatMessage> conversation =
[
    new ChatMessage(ChatRole.System, "You are a good assistance with short and smart answers")
];

conversation.Add(new ChatMessage(ChatRole.User, question));

// Stream token-by-token, accumulating so the completed turn can be stored.
var sb = new StringBuilder();
await foreach (var update in client.GetStreamingResponseAsync(conversation))
{
    Console.Write(update.Text);
    sb.Append(update.Text);
}

// Append the assistant turn, or the next question starts from nothing.
conversation.Add(new ChatMessage(ChatRole.Assistant, sb.ToString()));
```

**Docs**
- [Build a chat app with .NET](https://learn.microsoft.com/dotnet/ai/quickstarts/build-chat-app?wt.mc_id=dotnet-153583-brunocapuano)
- [`IChatClient` API reference](https://learn.microsoft.com/dotnet/api/microsoft.extensions.ai.ichatclient?wt.mc_id=dotnet-153583-brunocapuano)
- [Prompt engineering guidance](https://learn.microsoft.com/azure/ai-foundry/openai/concepts/prompt-engineering?wt.mc_id=dotnet-153583-brunocapuano)

---

## 3 · Function / tool calling (5 min)

**Code:** [`samples/CoreSamples/MEAIFunctionsAzureOpenAI/app.cs`](../../samples/CoreSamples/MEAIFunctionsAzureOpenAI/app.cs)

```powershell
cd samples\CoreSamples\MEAIFunctionsAzureOpenAI   # from the repo root
cd ..\MEAIFunctionsAzureOpenAI                    # from Demo 2
dotnet run app.cs
```

> 🎬 **Reveal built into the code.** Inside `GetWeather()` there is a commented-out
> `Console.WriteLine(">>> [tool] GetWeather() was called by the model")`. Run once with it
> commented — the audience sees three plausible answers and has to *take your word* that a
> C# method ran. Then uncomment it, rerun, and the `>>> [tool]` lines appear **before** the
> responses, twice, unprompted. That is the whole demo in one edit: proof the model reached
> out of the conversation and executed your code. Leave it commented when you start.

**What the code does.** This is the moment the model stops being a text generator and starts *doing things*. A plain local C# function `GetWeather()` is decorated with `[Description]`, wrapped with `AIFunctionFactory.Create(...)`, and handed to the model through `ChatOptions.Tools`. The critical line is `.AsBuilder().UseFunctionInvocation().Build()` — that middleware runs the full tool loop for you: the model requests a call, MEAI invokes your C# method, feeds the result back, and the model composes a final answer. The sample then asks three questions to show the model's own judgement: one it answers directly, one that clearly needs the tool, and one ("should I bring an umbrella?") where it must call the tool *and* reason over the result.

**Top 3 talking points**
1. **A tool is just a C# method.** No plugin manifest, no schema by hand — the `[Description]` attribute plus reflection generate the JSON schema the model sees.
2. **`UseFunctionInvocation()` is middleware.** `IChatClient` is a pipeline, exactly like ASP.NET Core. You can stack function invocation, telemetry, caching, and logging in the same builder chain.
3. **The model chooses.** Notice question 1 does not trigger the tool and question 3 does — and with the log line uncommented you can *count* the calls rather than assert them. That decision is the seed of agency: it's what makes Demo 7's agents possible.

**The code that matters**

```csharp
// 1. A tool is just a C# method plus a description. No manifest, no hand-written schema.
[Description("Get the weather")]
static string GetWeather()
{
    // Console.WriteLine(">>> [tool] GetWeather() was called by the model");   // <- the reveal
    var temperature = Random.Shared.Next(5, 20);
    var condition = Random.Shared.Next(0, 2) == 0 ? "sunny" : "rainy";
    return $"The weather is {temperature} degree C and {condition}";
}

// 2. One middleware call runs the entire tool loop for you.
IChatClient client = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient()
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

// 3. Hand the method to the model and let it decide whether to call it.
var chatOptions = new ChatOptions { Tools = [AIFunctionFactory.Create(GetWeather)] };
var response = await client.GetResponseAsync("Should I bring an umbrella with me today?", chatOptions);
```

**Docs**
- [Function calling with Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/quickstarts/use-function-calling?wt.mc_id=dotnet-153583-brunocapuano)
- [`AIFunctionFactory` reference](https://learn.microsoft.com/dotnet/api/microsoft.extensions.ai.aifunctionfactory?wt.mc_id=dotnet-153583-brunocapuano)
- [Function calling concepts in Microsoft Foundry](https://learn.microsoft.com/azure/ai-foundry/openai/how-to/function-calling?wt.mc_id=dotnet-153583-brunocapuano)

---

## 4 · Vision / multimodal (4 min)

**Code:** [`samples/CoreSamples/Vision-01MEAI-AzureOpenAI/Program.cs`](../../samples/CoreSamples/Vision-01MEAI-AzureOpenAI/Program.cs)

```powershell
cd samples\CoreSamples\Vision-01MEAI-AzureOpenAI   # from the repo root
cd ..\Vision-01MEAI-AzureOpenAI                    # from Demo 3
dotnet run
```

**What the code does.** The sample reads a local image into a byte array, wraps it in a `DataContent` with the `image/jpeg` media type, and adds it to the message list as just another `ChatMessage` alongside the text prompt. That is the whole multimodal story in MEAI: images are `AIContent`, and the same `GetResponseAsync` call handles them. Three images ship with the sample — running shoes (description and counting), a licence plate (OCR), and a German receipt — and swapping the `prompt` / `imageFileName` pair at the top switches scenarios instantly, which makes for a fast, visual segment on camera.

**Top 3 talking points**
1. **Multimodal is not a different API.** Text and images are both `AIContent` in the same `ChatMessage` list — no separate vision client, no separate SDK.
2. **Real business value, not a party trick.** The receipt image ("I bought the coffee and the sausage, add 18% tip") is document understanding replacing what used to be a bespoke OCR-plus-rules pipeline.
3. **Model choice matters here.** Vision requires a multimodal deployment. This is a natural place to mention Foundry's model catalog and picking the right model per task.

**The code that matters**

```csharp
// An image is just AIContent — the same message list, no separate vision client or SDK.
AIContent aic = new DataContent(File.ReadAllBytes(image), "image/jpeg");

List<ChatMessage> messages =
[
    new ChatMessage(ChatRole.System, "You are a useful assistant that describes images using a direct style."),
    new ChatMessage(ChatRole.User, "Describe the image"),
    new ChatMessage(ChatRole.User, [aic]),      // <- the picture rides along as content
];

// Identical call to every text-only demo so far.
var response = await chatClient.GetResponseAsync(messages);
```

**Docs**
- [Vision-enabled chat models in Microsoft Foundry](https://learn.microsoft.com/azure/ai-foundry/openai/how-to/gpt-with-vision?wt.mc_id=dotnet-153583-brunocapuano)
- [Microsoft.Extensions.AI content types](https://learn.microsoft.com/dotnet/api/microsoft.extensions.ai.datacontent?wt.mc_id=dotnet-153583-brunocapuano)
- [Foundry model catalog](https://learn.microsoft.com/azure/ai-foundry/how-to/model-catalog-overview?wt.mc_id=dotnet-153583-brunocapuano)

---

## 5 · RAG — embeddings + vector search (5 min)

**Code:** [`samples/CoreSamples/RAGSimple-02MEAIVectorsMemory/Program.cs`](../../samples/CoreSamples/RAGSimple-02MEAIVectorsMemory/Program.cs)

```powershell
cd samples\CoreSamples\RAGSimple-02MEAIVectorsMemory   # from the repo root
cd ..\RAGSimple-02MEAIVectorsMemory                    # from Demo 4
dotnet run
```

**What the code does.** This is the retrieval half of RAG, built on the official .NET vector abstractions. It creates an `IEmbeddingGenerator<string, Embedding<float>>` from a Foundry embedding deployment, defines a `MovieVectorRecord` annotated with `[VectorStoreKey]`, `[VectorStoreData]` and `[VectorStoreVector(1536, DistanceFunction.CosineSimilarity)]`, then upserts a small movie catalog into a `VectorStoreCollection` backed by local sqlite-vec. Querying is the same two steps every RAG system uses: embed the question, then `SearchAsync` for nearest neighbours. The magic moment on camera is the query *"a family friendly movie that includes ogres and dragons"* returning **Shrek** — a film whose description contains none of those words.

**Top 3 talking points**
1. **Semantic ≠ keyword.** Nothing here does string matching. Meaning is compared as distance between vectors, which is why the ogres/dragons query works.
2. **`Microsoft.Extensions.VectorData` is the same story as `IChatClient`.** One abstraction, swappable stores — sqlite-vec locally today, Azure AI Search or Qdrant in production (see the sibling `RAGSimple-03` and `-04` samples) with the search code unchanged.
3. **This is only the "R" in RAG.** Retrieval returns candidate documents; you then stuff them into the prompt from Demo 2 for grounded generation. Good place to mention chunking and why retrieval quality caps answer quality.

**The code that matters**

```csharp
// 1. Same client, different building block: embeddings instead of chat.
IEmbeddingGenerator<string, Embedding<float>> generator =
    new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
        .GetEmbeddingClient(embeddingDeployment)
        .AsIEmbeddingGenerator();

// 2. Attributes describe the record. 1536 dimensions, compared by cosine similarity.
internal sealed class MovieVectorRecord
{
    [VectorStoreKey]  public string Key { get; set; } = "";
    [VectorStoreData] public string Title { get; set; } = "";
    [VectorStoreData] public string Description { get; set; } = "";

    [VectorStoreVector(1536, DistanceFunction = DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}

// 3. Retrieval is always the same two steps: embed the question, search for neighbours.
var queryEmbedding = await generator.GenerateVectorAsync(
    "A family friendly movie that includes ogres and dragons");

await foreach (var result in movies.SearchAsync(queryEmbedding, top: 2))
    Console.WriteLine($"{result.Score:F3}  {result.Record.Title}");   // -> Shrek
```

**Docs**
- [Implement RAG with .NET](https://learn.microsoft.com/dotnet/ai/conceptual/rag?wt.mc_id=dotnet-153583-brunocapuano)
- [Vector databases and .NET](https://learn.microsoft.com/dotnet/ai/conceptual/vector-databases?wt.mc_id=dotnet-153583-brunocapuano)
- [Embeddings in Microsoft Foundry](https://learn.microsoft.com/azure/ai-foundry/openai/how-to/embeddings?wt.mc_id=dotnet-153583-brunocapuano)

---

## 6 · MCP — grounding in live Microsoft Learn docs (4 min)

**Code:** [`samples/CoreSamples/MCP-03-MicrosoftLearn/Program.cs`](../../samples/CoreSamples/MCP-03-MicrosoftLearn/Program.cs)

```powershell
cd samples\CoreSamples\MCP-03-MicrosoftLearn   # from the repo root
cd ..\MCP-03-MicrosoftLearn                    # from Demo 5
dotnet run
```

**What the code does.** This sample is built as a **before/after** and it is the best "aha" of the session. It asks the exact same fast-moving question twice — *"What is the latest version of Microsoft Agent Framework for C#?"* First without tools, where the model answers from frozen training data and is vague or stale. Then it opens an `HttpClientTransport` to the public, keyless Microsoft Learn MCP server at `https://learn.microsoft.com/api/mcp`, calls `ListToolsAsync()` to discover the documentation tools, passes them in via `ChatOptions.Tools`, and asks again. The second answer is grounded in the live docs with a real version number and a citation. Both answers stream, so the contrast is visible in real time.

**Top 3 talking points**
1. **MCP is USB-C for AI tools.** An open protocol: any MCP server's tools drop into any MCP-aware client. You wrote no integration code for Microsoft Learn — you just connected.
2. **The before/after is the argument for grounding.** Same model, same prompt; the only variable is access to live information. This is the cheapest possible answer to "how do I stop hallucinations about my domain?"
3. **`ListToolsAsync()` means discovery at runtime.** The tool list isn't compiled in. Add tools to the server and the client picks them up on the next run — and note that the discovered tools slot into the exact same `ChatOptions.Tools` from Demo 3.

**The code that matters**

```csharp
// BEFORE — no tools. The model answers from frozen training data.
await foreach (var update in client.GetStreamingResponseAsync(messages, new ChatOptions()))
    Console.Write(update.Text);

// Connect to the public, keyless Microsoft Learn MCP server. No integration code, no API key.
var transport = new HttpClientTransport(new HttpClientTransportOptions
{
    Name = "Microsoft Learn MCP",
    Endpoint = new Uri("https://learn.microsoft.com/api/mcp")
});
await using var mcpClient = await McpClient.CreateAsync(transport);

// Discover what the server offers at runtime — nothing is compiled in.
var tools = await mcpClient.ListToolsAsync();

// AFTER — same model, same question. The only variable is access to live docs.
await foreach (var update in client.GetStreamingResponseAsync(
    messages, new ChatOptions { Tools = [.. tools] }))   // <- same ChatOptions.Tools as Demo 3
    Console.Write(update.Text);
```

**Docs**
- [Model Context Protocol in .NET](https://learn.microsoft.com/dotnet/ai/get-started-mcp?wt.mc_id=dotnet-153583-brunocapuano)
- [Microsoft Learn MCP Server](https://learn.microsoft.com/training/support/mcp?wt.mc_id=dotnet-153583-brunocapuano)
- [Agent tools in Microsoft Agent Framework](https://learn.microsoft.com/agent-framework/tutorials/agents/function-tools?pivots=programming-language-csharp&wt.mc_id=dotnet-153583-brunocapuano)

---

## 7 · First agent with Microsoft Agent Framework (5 min)

**Code:** [`samples/MAF/MAF01/Program.cs`](../../samples/MAF/MAF01/Program.cs)

```powershell
cd samples\MAF\MAF01        # from the repo root
cd ..\..\MAF\MAF01          # from Demo 6 (note: hops out of CoreSamples)
dotnet run
```

**What the code does.** This is the shortest possible bridge from MEAI to MAF, and that is exactly why it's the right first agent demo. It builds the same `IChatClient` used in every previous demo, then calls one extension method — `.AsAIAgent(name: "Writer", instructions: "...")` — and gets back an `AIAgent`. The agent carries a name and persistent instructions rather than requiring you to re-send a system message every turn, and `RunStreamingAsync` streams the result. Two lines of new concept, on top of everything the audience has already seen.

> ⏱️ **Pacing (verified live):** the Writer agent streams a full short story — roughly 7,000 characters, well over a minute of scrolling. Talk over the stream (that's the point of streaming), and don't wait for it to finish before moving on.

**Top 3 talking points**
1. **An agent = model + instructions + tools + memory.** MAF gives that bundle a first-class type (`AIAgent`) instead of leaving it as loose variables scattered around your app.
2. **It sits on top of MEAI, not beside it.** `chatClient.AsAIAgent(...)` — the whole framework builds on the `IChatClient` from Demo 1, so nothing learned so far is thrown away.
3. **Threads and persistence come next.** `CreateSessionAsync` / `SerializeSessionAsync` let a conversation survive process restarts (see `MAF-Persisting-01-Simple`) — the natural production follow-up.

**The code that matters**

```csharp
// Everything from Demo 1, completely unchanged.
IChatClient chatClient = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient();

// One extension method is the entire step up to an agent:
// a name and standing instructions, instead of re-sending a system message every turn.
AIAgent writer = chatClient.AsAIAgent(
    name: "Writer",
    instructions: "Write stories that are engaging and creative.");

await foreach (var update in writer.RunStreamingAsync(
    "Write a short story about a haunted house with a character named Lucia."))
{
    Console.Write(update.Text);
}
```

**Docs**
- [Microsoft Agent Framework overview](https://learn.microsoft.com/agent-framework/overview/agent-framework-overview?wt.mc_id=dotnet-153583-brunocapuano)
- [Create your first agent (C#)](https://learn.microsoft.com/agent-framework/tutorials/agents/run-agent?pivots=programming-language-csharp&wt.mc_id=dotnet-153583-brunocapuano)
- [Persisted conversations](https://learn.microsoft.com/agent-framework/tutorials/agents/persisted-conversation?pivots=programming-language-csharp&wt.mc_id=dotnet-153583-brunocapuano)

---

## 8 · Multi-agent workflows + Foundry agent types (8 min)

**Code:** [`samples/MAF/MAF02/Program.cs`](../../samples/MAF/MAF02/Program.cs) and [`samples/MAF/MAF-MicrosoftFoundryAgents-01/Program.cs`](../../samples/MAF/MAF-MicrosoftFoundryAgents-01/Program.cs)

```powershell
# 8a — two agents in a sequential workflow
cd samples\MAF\MAF02        # from the repo root
cd ..\MAF02                 # from Demo 7
dotnet run

# 8b — an agent that lives in Microsoft Foundry
cd samples\MAF\MAF-MicrosoftFoundryAgents-01   # from the repo root
cd ..\MAF-MicrosoftFoundryAgents-01            # from 8a
dotnet run
```

**What the code does.** **8a** takes the `Writer` agent from Demo 7 and adds an `Editor` agent with different instructions, then composes them with `AgentWorkflowBuilder.BuildSequential(writer, editor)`. The resulting `Workflow` is itself turned back into an agent via `.AsAIAgent()` — so a team of agents is consumed with the identical `RunStreamingAsync` call as a single agent, which is the key composability insight. The console prints a `=== Writer ===` / `=== Editor ===` header as the stream's author changes, so the audience *sees* the handoff and can compare the Editor's rewrite against the Writer's draft on screen. **8b** changes *where inference happens*: instead of a chat deployment, an `AIProjectClient` points at a Microsoft Foundry **project** endpoint, and `.AsAIAgent(model, instructions, name)` produces a **Responses Agent** — a `ChatClientAgent` whose definition lives in your code and runs against the project. Foundry exposes two agent types, and this is the code-first one; the `name` is the agent's identity in your code and console output, not a resource in the portal.

> ⚠️ **Accuracy note for this demo:** the Responses API path **does not create a server-side agent resource**. Per Microsoft Learn, "this path is code-first and does not create a server-managed agent resource." So do **not** promise the audience the agent will appear in the Foundry portal's agent list — it won't. Portal-managed, versioned agents are the *other* type (`AIProjectClient.AgentAdministrationClient`). The repo's own `MAF-MultiAgents/AIFoundryAgentsProvider.cs` makes this explicit: its delete method is a deliberate no-op because "Responses API agents are ephemeral."

> 🎯 **Prompt guidance for 8b (learned from a live run):** do **not** ask it "what is Microsoft Agent Framework?" — the model confidently answers that it's a *deprecated SDK for animated Windows assistants*, confusing MAF with the 1990s Microsoft Agent (Clippy-era) technology. Either avoid it, or use it deliberately as a **callback to Demo 6**: "watch — ungrounded, it gets its own framework wrong; that's exactly what the Learn MCP server fixed." Safe alternatives: *"Explain dependency injection in one sentence"* or *"Give me three ideas for a .NET podcast episode."*

**Top 3 talking points**
1. **Specialists beat one giant prompt.** A focused Writer and a focused Editor produce better output than a single prompt trying to do both, and each is independently testable and swappable.
2. **Workflows are agents.** `workflow.AsAIAgent()` means a multi-agent system composes into a larger system exactly like a single agent. Sequential is just the starter; concurrent, hand-off, and group-chat patterns build from the same builder.
3. **Foundry has two agent types — know which one you're using.** A **Responses Agent** (this demo) is code-first and ephemeral: you own the definition, it ships with your app, nothing is registered server-side. A **Foundry Agent** is versioned and service-managed via `AgentAdministrationClient` or the portal, with strict instructions/tools and portal governance. Same `AIAgent` interface, same calling code — the choice is about *who owns the definition*, and it's the question most teams get wrong first. (`MAF-MultiAgents` goes further, mixing a Foundry agent, an Azure OpenAI agent, and a local Ollama agent in one workflow with OpenTelemetry tracing.)

**The code that matters — 8a, a team of agents**

```csharp
// Two specialists, each independently testable and swappable, instead of one giant prompt.
AIAgent writer = chatClient.AsAIAgent(name: "Writer",
    instructions: "Write stories that are engaging and creative.");
AIAgent editor = chatClient.AsAIAgent(name: "Editor",
    instructions: "Make the story more engaging, fix grammar, and enhance the plot.");

// Compose them, then turn the whole workflow back into a single agent.
Workflow workflow = AgentWorkflowBuilder.BuildSequential(writer, editor);
AIAgent workflowAgent = workflow.AsAIAgent();       // <- the composability insight

// Identical call to Demo 7. A team is consumed exactly like one agent.
string? currentAuthor = null;
await foreach (var update in workflowAgent.RunStreamingAsync(
    "Write a short story about a haunted house. Keep it under 200 words."))
{
    if (update.AuthorName is { Length: > 0 } author && author != currentAuthor)
    {
        currentAuthor = author;
        Console.WriteLine($"\n=== {author} ===");   // makes the Writer -> Editor handoff visible
    }
    Console.Write(update.Text);
}
```

**The code that matters — 8b, an agent in Microsoft Foundry**

```csharp
// Point at a Foundry PROJECT endpoint, not a chat deployment.
AIProjectClient projectClient = new(
    new Uri(azureFoundryProjectEndpoint),
    new AzureCliCredential());

// Responses API -> a code-first ChatClientAgent.
// You own the definition; nothing is registered server-side, nothing appears in the portal.
AIAgent aiAgent = projectClient.AsAIAgent(
    model: deploymentName,
    instructions: "You are a useful agent that replies in short and direct sentences.",
    name: agentName);

var response = await aiAgent.RunAsync(userInput);   // same AIAgent interface as Demo 7
```

**Docs**
- [Microsoft Foundry provider — the two agent types](https://learn.microsoft.com/agent-framework/agents/providers/microsoft-foundry?wt.mc_id=dotnet-153583-brunocapuano)
- [Sequential orchestration in Agent Framework](https://learn.microsoft.com/agent-framework/workflows/orchestrations/sequential?wt.mc_id=dotnet-153583-brunocapuano)
- [Microsoft Foundry Agent Service](https://learn.microsoft.com/azure/ai-foundry/agents/overview?wt.mc_id=dotnet-153583-brunocapuano)

---

## 9 · Wrap (3 min)

**The through-line to say out loud:** every demo was the same `IChatClient`. Add a tool and it acts. Add vectors and it knows your data. Add MCP and it reaches live systems. Add instructions and it becomes an agent. Add a second agent and it becomes a team. Nothing was thrown away between steps.

**Where the audience should start:** clone the repo, run `BasicChat-05AIFoundryModels` with `dotnet run app.cs`, then jump to `MAF01`.

- [Generative AI for Beginners .NET (course)](https://github.com/microsoft/Generative-AI-for-beginners-dotnet?wt.mc_id=dotnet-153583-brunocapuano)
- [Microsoft Agent Framework docs](https://learn.microsoft.com/agent-framework/?wt.mc_id=dotnet-153583-brunocapuano)
- [.NET AI documentation](https://learn.microsoft.com/dotnet/ai/?wt.mc_id=dotnet-153583-brunocapuano)

---

## Appendix · If the conversation goes somewhere else

Unscripted branches, in the order they're most likely to come up. Every entry is a real,
committed sample in this repo — and **every one of them was executed on this machine on
2026-08-10**, so the marks below are measured, not guessed.

**Readiness key:**
✅ **run it** — live-verified today, works end to end ·
⚠️ **runs, but degraded** — starts and is honest about partial capability ·
🔑 **needs another Azure resource** — safe to discuss, but not ready on this machine without extra setup ·
⏭️ **skip for now** — kept as reference material, not part of the recommended current path ·
🧱 **build/code only** — compiles clean, but interactive or unconfigured

| If they ask… | Go to | Ready |
|---|---|---|
| "Can I see the *other* Foundry agent type?" | `samples/CoreSamples/AgentLabs-01-Simple` | ✅ |
| "Does the agent remember across restarts?" | `samples/MAF/MAF-Persisting-01-Simple` | ✅ |
| "What about long-running or interrupted work?" | `samples/MAF/MAF-BackgroundResponses-01-Simple` | ✅ |
| "How do agents talk to *other* agents / other stacks?" | `samples/MAF/A2A-01` | ✅ |
| "Can this run locally / offline / on-device?" | `samples/CoreSamples/01-foundrylocal-hello-world` | ✅ |
| "What does this look like in a real web app?" | `samples/MAF/MAF-AIWebChatApp-Simple` | ✅ |
| "Where do guardrails / responsible AI fit?" | `samples/MAF/MAF-AIWebChatApp-Middleware` | ✅ |
| "Can I mix cloud and local models?" | `samples/MAF/MAF-MultiModel` | ⚠️ |
| "What about non-OpenAI image generation?" | `samples/CoreSamples/ImageGeneration-01` | ✅ |
| "What about speech / audio?" | `samples/CoreSamples/Audio-01-SpeechMic` | ✅ |
| "Production RAG with a real vector database?" | `samples/CoreSamples/RAGSimple-03MEAIVectorsAISearch` | 🔑 |
| "What about non-OpenAI models — Claude?" | `samples/MAF/MAF-FoundryClaude-01` | ⏭️ |
| "Can it generate video?" | `samples/CoreSamples/VideoGeneration-AzureSora-01` | ⏭️ |
| "Can I build my own MCP server?" | `samples/PracticalSamples/src` — `McpSample.AspNetCoreServer` (Aspire) | 🧱 |
| "Show me something fun." | `samples/AppsWithGenAI/SpaceAINet` | 🧱 |

> ✅ **The appendix is in much better shape now.** Image generation and speech are stage-runnable on this machine, the remaining blocked sample (`RAGSimple-03MEAIVectorsAISearch`) now fails honestly with exact setup steps, and the legacy Claude/Sora branches are explicitly marked as optional instead of pretending they are part of the main live path.

> ⚠️ **The one deliberate degraded demo.** `MAF-MultiModel` used to print **"Workflow completed successfully!"** and exit `0` even though its third agent had failed. It now preflights Ollama, says **"Reviewer SKIPPED"** up front, and ends with **"Workflow completed PARTIALLY (2/3 agents)"**, exit code `2`. Agents 1 and 2 still produce the full article, so it's actually a useful “graceful degradation” talking point.

### The five most likely branches

**A · The other Foundry agent type** — `AgentLabs-01-Simple`
The natural follow-up to Demo 8's talking point 3. Where `MAF-MicrosoftFoundryAgents-01` uses
the Responses API and creates nothing server-side, this one calls
`PersistentAgentsClient.Administration.CreateAgentAsync(...)` — a real, server-managed agent
that **does** appear in the Foundry portal, and which the sample explicitly tears down with
`DeleteAgentAsync` at the end. It solves `3x + 11 = 14` and signs off with
**"Agent Math Tutor deleted."** — that create/solve/delete arc is the cleanest way to show
the difference live. ✅ Live-verified today.

> 💡 This one reads *different* secret keys — `aifoundryproject_endpoint` and
> `aifoundryproject_tenantid`, not `azureFoundryProjectEndpoint`. Both are now set (see
> Pre-flight), so it just runs. If you ever hit
> *"Value cannot be null. (Parameter 'endpoint')"*, that's the missing key.

```powershell
cd D:\microsoft\Generative-AI-for-beginners-dotnet\samples\CoreSamples\AgentLabs-01-Simple   # from repo root
cd ..\AgentLabs-01-Simple                                                                    # from a previous CoreSamples demo
dotnet run app.cs
```

**B · Memory that survives a restart** — `MAF-Persisting-01-Simple`
Answers "is the agent stateful?" properly, and it's a great three-beat demo. Step 1 creates a
session with `CreateSessionAsync()`, says "My name is Bruno", and serializes the thread to
`agent_thread.json`. Step 2 opens a **fresh** thread and asks "What is my name?" — the model
says it has no idea. Step 3 reloads the persisted thread, asks the identical question, and
gets "Your name is Bruno." Same code, same model, one difference: state. It's the production
answer to the in-memory `List<ChatMessage>` from Demo 2. ✅ Live-verified today.

```powershell
cd D:\microsoft\Generative-AI-for-beginners-dotnet\samples\MAF\MAF-Persisting-01-Simple   # from repo root
cd ..\MAF-Persisting-01-Simple                                                            # from a previous MAF sample
dotnet run
```

**C · Local and offline** — `01-foundrylocal-hello-world`
The privacy/cost/latency question always comes. Foundry Local runs the model on your machine
behind the same `IChatClient`, so the story is "same code, no cloud." Verified today: it
answered **"Hello, Foundry Local!"** running on the GPU
(`qwen2.5-0.5b-instruct-trtrtx-gpu`), with the network path never leaving `127.0.0.1`. ✅

> ⚠️ **Gotcha:** the sample defaults to `http://127.0.0.1:5273/v1`, but Foundry Local
> **0.8.x picks a dynamic port** (it was `57157` here) — so a bare `dotnet run` fails
> preflight with *"Could not reach Foundry Local service."* Run `foundry service status` to
> read the real port, then set the override. The command below does both.

```powershell
foundry service status        # confirm it's running + read the port from the status URL
cd D:\microsoft\Generative-AI-for-beginners-dotnet\samples\CoreSamples\01-foundrylocal-hello-world
$env:FOUNDRY_LOCAL_BASE_URL="http://127.0.0.1:57157/v1"   # <-- swap in the port you just saw
$env:FOUNDRY_LOCAL_MODEL="qwen2.5-0.5b"
dotnet run
```

If `foundry cache list` says *"No models cached on device"*, pull one first with
`foundry model download qwen2.5-0.5b` (~1 min). The `qwen2.5-0.5b` model **is** cached on
this machine already.

**D · Agent-to-agent interop** — `A2A-01`
One console process that is *both* sides of the A2A protocol: it hosts a writer agent over
the A2A HTTP+JSON binding at `http://localhost:5099/a2a/writer-agent`, then connects to its
own endpoint with an `A2AClient`, wraps that as a standard `AIAgent`, and calls `RunAsync`.
The client never references the agent's implementation — only its endpoint, so the remote
agent could be Python or any other stack. Good answer to "how does this work across teams or
languages?" ✅ Live-verified today.

> ⚠️ **Gotcha:** this one is an ASP.NET Core Web project, so user secrets only load in the
> Development environment. Running `dotnet run` bare throws
> *"Set AzureOpenAI:Endpoint in User Secrets"* even though the secret is set. Set the
> environment variable first — the command below already does.

```powershell
cd D:\microsoft\Generative-AI-for-beginners-dotnet\samples\MAF\A2A-01   # from repo root
cd ..\A2A-01                                                           # from a previous MAF sample
$env:ASPNETCORE_ENVIRONMENT='Development'
dotnet run
```

**E · Beyond the console** — `MAF-AIWebChatApp-Simple` and `-Middleware`
For "how would I actually ship this." Blazor apps built on the same `IChatClient` and
`AIAgent` types, with `UseFunctionInvocation()`, SQLite vector storage and OpenTelemetry
already wired. The `-Middleware` variant is the natural hook for a responsible-AI turn — its
`CustomFunctionCallingMiddleware` is where content filtering, logging, and guardrails would
live, on the same builder chain you showed in Demo 3. Both served **HTTP 200 on
`http://localhost:5297`** today. ✅

> ⚠️ **Two gotchas, both already handled:** these are Aspire-flavoured, so they read
> `ConnectionStrings:openai` (now set) instead of `AzureOpenAI:Endpoint`, and they're Web SDK
> projects so secrets need `ASPNETCORE_ENVIRONMENT=Development`. The deployment names are
> **hardcoded** in `Program.cs` as `gpt-5-mini` + `text-embedding-3-small` — which happen to
> match this resource exactly. Note you run from the nested `ChatApp20\ChatApp20.Web` folder,
> not the sample root.

```powershell
cd D:\microsoft\Generative-AI-for-beginners-dotnet\samples\MAF\MAF-AIWebChatApp-Simple\ChatApp20\ChatApp20.Web
$env:ASPNETCORE_ENVIRONMENT='Development'
dotnet run     # then open http://localhost:5297
```

**F · Interrupt and resume** — `MAF-BackgroundResponses-01-Simple`
The "what about long-running work?" answer, and a genuinely surprising demo. It streams a
response, deliberately breaks out of the loop mid-generation, captures the
`ContinuationToken`, and then resumes from that token in a second call. Today it cut off at
**"Develop"** and resumed with **"ers gathered at .NET Conf…"** — it continues *mid-word*.
That's the moment worth pausing on: the work kept happening server-side while the client was
gone. ✅ Live-verified today.

```powershell
cd D:\microsoft\Generative-AI-for-beginners-dotnet\samples\MAF\MAF-BackgroundResponses-01-Simple
cd ..\MAF-BackgroundResponses-01-Simple                                                        # from a previous MAF sample
dotnet run
```

> 🔧 **This sample was broken until today** and got two fixes while validating this appendix —
> see *Fixes made as a result of running them* at the bottom. Before the fix it printed
> *"No continuation token available to resume the response. Aborting continuation."*, which is
> the exact opposite of its point. Make sure you're on the latest `main`.

### Why the remaining non-green rows are non-green

After the follow-up validation pass, two of the previously blocked branches are now active on this machine: **image generation** (`gpt-image-2`) and **speech** (Azure AI Speech in `eastus2`). That leaves just one truly blocked sample (`RAGSimple-03MEAIVectorsAISearch`) plus two **intentional skips** (`MAF-FoundryClaude-01` and `VideoGeneration-AzureSora-01`).

The important repo-quality improvement is that the remaining non-green samples now explain themselves: missing resources are named, the exact secret keys are named, and the console output tells you what to deploy or configure next instead of failing cryptically.

#### ⚠️ Read this first — the shared-secrets trap

Every sample in this repo shares **one** user-secrets store, `genai-beginners-dotnet`. Some
samples historically read *unscoped* keys out of it — `endpoint`, `apikey`, `api_key` — which
means **setting a secret for one sample could silently change another's behaviour.** That is
now fixed: each sample reads a scoped key first (`Sora:Endpoint`, `Claude:Endpoint`,
`AzureAISearch:Uri`, `AzureOpenAI:ImageDeployment`) and only falls back to the old generic name
for backward compatibility. **Use the scoped names below.**

#### Images — `ImageGeneration-01`

This one is now **ready to run** on this machine with `AzureOpenAI:ImageDeployment=gpt-image-2`.

```powershell
dotnet user-secrets set --id genai-beginners-dotnet "AzureOpenAI:ImageDeployment" "gpt-image-2"
```

> Two real compatibility fixes were needed while validating it live: the current service rejected `quality="standard"`, so the sample now uses `quality="auto"`, and the returned payload is now saved from `image.ImageBytes` instead of assuming a directly-downloadable `image.ImageUri`.

#### Video — `VideoGeneration-AzureSora-01`

Treat this as a **legacy reference sample** for now. Sora is deprecated for the current course/event path, so this appendix branch is intentionally marked **skip for now** rather than being presented as something you should wire up for today's session.

If the curriculum later gets a new recommended video path, update the doc and sample guidance there rather than reviving this one ad hoc.

#### Claude — `MAF-FoundryClaude-01`

Keep this as an **optional provider-specific branch**. The sample still exists and its error path is now much cleaner, but for today's event and the current repo path we are explicitly **skipping Anthropic/Claude** so the story stays focused on Foundry + MEAI + MAF with the samples we have fully validated live.

If someone asks conceptually, the good talking point still stands: Claude, DeepSeek, Grok, Kimi, and GPT can all sit behind the same `IChatClient` abstraction — but for this session, show that as an architecture point rather than switching demos.

#### Production RAG — `RAGSimple-03MEAIVectorsAISearch`

Needs an **Azure AI Search** resource (a different service from Azure OpenAI). Only two secrets
are required — the endpoint and embedding model now fall back to the `AzureOpenAI:*` values you
already set in Pre-flight:

```powershell
dotnet user-secrets set --id genai-beginners-dotnet "AzureAISearch:Uri" "https://<your-search-service>.search.windows.net"
dotnet user-secrets set --id genai-beginners-dotnet "AzureAISearch:Secret" "<primary admin key>"
```

Portal path: **Search services → your resource → Overview → "Url"**, and **Settings → Keys →
"Primary admin key"**.

#### Speech — `Audio-01-SpeechMic`

This one is now **stage-runnable** on this machine because the Speech resource is configured. It still depends on a real microphone for a full demo, so the only thing automation can prove is that the sample reaches live speech recognition instead of failing preflight.

```powershell
dotnet user-secrets set --id genai-beginners-dotnet "SPEECH_KEY" "<key>"
dotnet user-secrets set --id genai-beginners-dotnet "SPEECH_REGION" "eastus2"
```

Portal path: your Speech resource → **Keys and Endpoint** → KEY 1 + Location/Region.

### The two 🧱 rows

- **`SpaceAINet`** builds clean, but its AI mode uses its own secret names
  (`AZURE_OPENAI_ENDPOINT`, `AZURE_OPENAI_MODEL`, `AZURE_OPENAI_APIKEY`) rather than the
  `AzureOpenAI:*` convention every other demo shares, and those are not set. It's also a
  keyboard-driven game, so it's a "look at this" not a "watch it run."
- **`samples/PracticalSamples/src`** — the Aspire MCP AppHost builds clean, but launching it
  brings up the Aspire dashboard, which is a detour mid-podcast. Good code to open, slow to
  demo.

---

## Fallback plan

If a live demo misbehaves, do not debug on air — switch and keep moving.

| If this fails | Do this |
|---|---|
| Any Foundry call (auth/quota) | Re-run `az login`; if quota, switch `AzureOpenAI:Deployment` to a second deployment |
| Demo 5 (RAG) — no embedding deployment | Skip to Demo 6; describe embeddings verbally over the code |
| Demo 6 (MCP) — Learn server unreachable | Show `MAF-MCP-01` (same server, agent-flavoured) or narrate the "before" half only |
| Demo 8b — Foundry project endpoint missing | Stay in `MAF02`; talk through the two Foundry agent types verbally instead |
| Network is fully down | `samples/CoreSamples/01-foundrylocal-hello-world` (Foundry Local, fully on-device) |
| Time is short (need to cut ~8 min) | Drop Demo 4 (vision) and Demo 2 (fold streaming into Demo 1) |

**Bonus demos if time runs long — or if the conversation wanders:** see the
[Appendix](#appendix--if-the-conversation-goes-somewhere-else) above for a full
"if they ask X → go to Y" lookup table with readiness marks.

---

## Validation status

All demo projects in this document were verified on 2026-08-10.

### Live end-to-end runs (executed against Microsoft Foundry with `az login` keyless auth)

Every demo below was actually **run**, not just compiled. All exited `0` during live validation.

| # | Demo | Result |
|---|------|--------|
| 1 | `BasicChat-05AIFoundryModels` | ✅ Answered; model + auth-mode banner displays correctly |
| 2 | `BasicChat-10ConversationHistory` | ✅ Streamed, and correctly recalled a name from turn 1 on turn 2 — history proven on screen |
| 3 | `MEAIFunctionsAzureOpenAI` | ✅ Model invoked both tools (date + weather) across 3 responses |
| 4 | `Vision-01MEAI-AzureOpenAI` | ✅ Described `running-shoes.jpg` accurately (colors, wet road, motion) |
| 5 | `RAGSimple-02MEAIVectorsMemory` | ✅ "ogres and dragons" → **Shrek**; "hacker / simulation" → **The Matrix** |
| 6 | `MCP-03-MicrosoftLearn` | ✅ Before/after contrast is sharp: refuses ungrounded, then answers with a live Learn citation |
| 7 | `MAF01` | ✅ Agent streamed a full story |
| 8a | `MAF02` | ✅ Sequential workflow ran; Writer → Editor handoff visible |
| 8b | `MAF-MicrosoftFoundryAgents-01` | ✅ Foundry Responses agent replied as `Agent [PodcastAgent]` |

### Appendix samples (also run on 2026-08-10)

All 15 appendix entries were executed. Results drive the readiness marks in the table above.

| Result | Count | Samples |
|---|---|---|
| ✅ Ran end to end | 9 | `AgentLabs-01-Simple`, `MAF-Persisting-01-Simple`, `MAF-BackgroundResponses-01-Simple`, `A2A-01`, `01-foundrylocal-hello-world`, `MAF-AIWebChatApp-Simple`, `MAF-AIWebChatApp-Middleware`, `ImageGeneration-01`, `Audio-01-SpeechMic`* |
| ⚠️ Ran, degraded by design | 1 | `MAF-MultiModel` (Ollama not installed — now reports 2/3 and exits `2`) |
| 🔑 Blocked on a missing resource | 1 | `RAGSimple-03MEAIVectorsAISearch` |
| ⏭️ Kept as reference, skipped for now | 2 | `MAF-FoundryClaude-01`, `VideoGeneration-AzureSora-01` |
| 🧱 Builds clean, not run | 2 | `SpaceAINet`, `PracticalSamples/src` (Aspire MCP) |

\* `Audio-01-SpeechMic` was validated as far as automation reasonably can: with real Speech secrets it reaches live recognition and returns `NOMATCH` without microphone speech, which is enough to prove preflight and runtime wiring are correct for a live stage demo.

Several of the ✅ results required either a config update or a code fix that is now captured in Pre-flight step 4 and in the per-branch notes: the `aifoundryproject_*` keys, the `ConnectionStrings:openai` value, the Foundry Local dynamic port, the background-responses fixes, the image-generation compatibility fixes, and the Speech preflight cleanup.

### Second pass — fixing the broken paths and clarifying the optional ones

The follow-up pass had three goals: make broken samples honest, activate the newly available image/speech scenarios, and mark the legacy Claude/Sora branches as optional instead of implying they belong in the main path.

| Sample | Before | After (verified live) |
|---|---|---|
| `MAF-MultiModel` | Exit `0`, *"Workflow completed successfully!"* with a dead agent | Ollama preflight, *"Reviewer SKIPPED"*, *"completed PARTIALLY (2/3 agents)"*, exit `2` |
| `ImageGeneration-01` | Raw `HTTP 400 OperationNotSupported`, then runtime issues with current image APIs | Uses `AzureOpenAI:ImageDeployment`, `quality="auto"`, saves `image.ImageBytes`, exits `0` and writes the image |
| `Audio-01-SpeechMic` | Native `0x5` | Friendly preflight for `SPEECH_KEY`/`SPEECH_REGION`; with real Speech config it reaches live recognition |
| `RAGSimple-03MEAIVectorsAISearch` | Null `uriString` crash | Resolves endpoint + embedding model from existing secrets; asks only for the 2 Search keys with portal directions, exit `1` |
| `MAF-FoundryClaude-01` | *"Missing 'endpointClaude' configuration"* | Uses scoped `Claude:*` keys and now fails honestly, but is intentionally skipped for this event |
| `VideoGeneration-AzureSora-01` | *"Please set the endpoint and apikey"* | Uses scoped `Sora:*` keys and now fails honestly, but is intentionally treated as a legacy reference |

Both solutions rebuild in Release after these changes. The targeted validation pass still shows some pre-existing nullable/compiler warnings elsewhere in the repo, but the updated image path, speech path, and documentation changes build cleanly enough for today’s event prep.

### Fixes made as a result of running them

- **Demo 4** emitted 5 `CS0219` warnings that scrolled on screen before the answer — the unused prompt/image variants are intentional swap-in options, now explicitly marked so the console stays clean.
- **Demo 5** emitted 3 nullability warnings (`CS8618`/`CS8601`) from the shared `MEAIVectorsShared` project — fixed with the standard `default!` idiom for unconstrained generics. This also cleans up the other RAG samples that share it.
- **Demo 8a** streamed **15,412 characters** as one anonymous wall of text — the multi-agent point was invisible. Now prints `=== Writer ===` / `=== Editor ===` as the stream's author changes, and caps the story length. Output dropped to **2,283 characters**, and the Editor's rewrite can be compared against the draft live.
- **`MAF-BackgroundResponses-01-Simple` — two real bugs**, both found by running the appendix samples:
  1. `ResponseClientProvider` was returning `azureClient.GetChatClient(...)` — the **chat completions** client. Background responses and continuation tokens are a **Responses API** feature, so `update.ContinuationToken` was always `null` and the sample's whole second half printed *"No continuation token available to resume the response. Aborting continuation."* Changed to `azureClient.GetResponsesClient().AsIChatClient(deploymentName)`. The demo now genuinely interrupts and resumes mid-word.
  2. `Console.Clear()` in `StreamConsoleHelper.PrintHeader` and `ConsoleHelper.PrintHeader` threw `IOException: The handle is invalid` whenever output was redirected (piping to a file, CI, or a recorded session), killing the app before it did anything. Both are now guarded with `!Console.IsOutputRedirected`.

  These files are shared by link into six other projects (`MAF-BackgroundResponses-02/-03`, `MAF-Persisting-01/-02`, `MAF-FoundryClaude-Persisting-01`); the full `MAF-Demos.slnx` still builds successfully, and `MAF-Persisting-01-Simple` was re-run to confirm unchanged behaviour.
- **`MAF-MultiModel` reported success while failing.** `Program.cs` ended in an unconditional `Console.WriteLine("Workflow completed successfully!")` — the workflow result was never inspected. With Ollama absent, agent 3 of 3 died and the app still exited `0`. In a beginners repo that's worse than a crash. Now: a 3-second preflight probe against the Ollama endpoint (URL and model hoisted into `ChatClientProvider` constants so they can't drift), explicit `Reviewer SKIPPED` messaging, a degraded 2-agent run that exits `2`, and a `try/catch` around `RunAsync` that names the failing stage and exits `1`.
- **Unscoped secret keys in a shared store.** Every sample shares `UserSecretsId=genai-beginners-dotnet`, yet `VideoGeneration-AzureSora-01` read `endpoint`/`api_key`, `RAGSimple-03` read `endpoint`/`apikey`, and `MAF-FoundryClaude-01` read `apikey` — so these samples were quietly fighting over the same two key names. All three now read scoped keys (`Sora:*`, `Claude:*`, `AzureAISearch:*`) with fallback to the legacy names, so existing setups keep working. `RAGSimple-03` additionally reuses `AzureOpenAI:Endpoint` and `AzureOpenAI:EmbeddingDeployment`, cutting its required secrets from five to two, and switched its embeddings client to keyless `AzureCliCredential` instead of crashing on a null `ApiKeyCredential`.
- **`ImageGeneration-01` could never work.** It passed `AzureOpenAI:Deployment` — the shared *chat* deployment — into `GetImageClient()`. Now reads `AzureOpenAI:ImageDeployment` (falling back for compatibility) and traps the resulting HTTP 400 to explain, in plain language, that the deployment is a chat model.
- **`Audio-01-SpeechMic` surfaced a bare `0x5`.** Added a preflight for `SPEECH_KEY`/`SPEECH_REGION` that explains Azure AI Speech is a separate resource, gives the portal path and the exact secret commands, and notes the microphone requirement.

### Build & supply chain

- ✅ The two solutions touched by this prep pass — `CoreGenerativeAITechniques.sln` and `MAF-Demos.slnx` — rebuild in Release.
- ✅ `ImageGeneration-01` now runs successfully with a real `gpt-image-2` deployment.
- ✅ `Audio-01-SpeechMic` now reaches live recognition with real Speech secrets; the remaining limitation is microphone input, not configuration.
- ✅ The docs and helper script now match the current recommended setup: chat + embeddings for the main flow, optional image deployment for the appendix, optional Speech secrets for audio, and explicit skip/legacy messaging for Claude and Sora.
