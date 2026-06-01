# 02 — Session Script (40 minutes)

> OD805 · **AI Building Blocks for .NET** · run-of-show with timings, demos, and talking points.
> Times are cumulative. Buffer of ~3 min is built in for room/Q&A overflow.

## Timing overview

| Block | Topic | Time | Cumulative |
|-------|-------|------|-----------|
| 0 | Cold open: the magic | 4 min | 0:00 → 0:04 |
| 1 | The hook + roadmap | 2 min | 0:04 → 0:06 |
| 2 | Foundations — MEAI (chat) | 7 min | 0:06 → 0:13 |
| 3 | Intelligence — VectorData + DataIngestion | 9 min | 0:13 → 0:22 |
| 4 | Tools — MCP (C# SDK) | 7 min | 0:22 → 0:29 |
| 5 | The whole package — MAF | 7 min | 0:29 → 0:36 |
| 6 | Reassemble + takeaways | 3 min | 0:36 → 0:39 |
| 7 | Close / CTA | 1 min | 0:39 → 0:40 |

---

## Block 0 — Cold open: the magic (0:00–0:04)

**Goal:** earn attention with a working, relatable multi-agent app before any slides.

- **Demo:** **Zava Support Center** — the NeMo + MAF **A2A** app
  (repo: [`elbruno/MAF-A2A-NVIDIA-NemoAgents`](https://github.com/elbruno/MAF-A2A-NVIDIA-NemoAgents),
  local: `d:\elbruno\MAF-A2A-NVIDIA-NemoAgents`). `aspire start` → `http://localhost:5000`.
- **One incident, three agents** (run the steps in order so the story rhymes):
  1. **Cold-open image (already there):** the chat opens with a pre-rendered "incident
     hero" image produced by the **Pitch Image Agent** (`IImageGenerator` →
     **GPT-Image-2 / Azure OpenAI**). Optionally re-trigger it live with the
     *"Generate an incident-response image"* prompt — but the pre-rendered one means
     **zero waiting** on stage.
  2. **Ask NeMo (NVIDIA):** *"Analyze quarterly revenue trends"* — Python NeMo agent
     returns trend + anomaly analysis. (Different stack, different vendor — reached over A2A.)
  3. **Ask MAF (grounded RAG):** *"Trigger an alert for the payments service error-rate
     spike and recommend the runbook remediation"* — the MAF agent grounds its action in
     the runbook KB and answers citing **`RB-014`** + **`ASP-001`** as **clickable
     "Grounded in" chips**. Click a chip → the exact runbook opens in the knowledge viewer.
- **Say:** "One chat, three agents — an image agent, an NVIDIA NeMo analysis agent, and a
  .NET action agent that grounds every recommendation in a runbook, with citations. By the
  end of this talk you'll be able to name every .NET building block behind it."

> Keep it short. Do **not** explain how it works yet. The mystery is the point.
> **Fallback:** recorded video of this exact flow (Wi-Fi / NVIDIA / GPT-Image-2 risk).

## Block 1 — The hook + roadmap (0:04–0:06)

- **Deliver the hook line** (see concept doc §4, recommended #1).
- **Slide — the roadmap** (the deconstruct visual):
  - Foundations → **MEAI**
  - Intelligence & tools → **VectorData / DataIngestion** + **MCP**
  - Whole package → **MAF**
- **Say:** "Every block is NuGet + C#. By the end you'll be able to point at any part of
  that app and name the building block behind it."

## Block 2 — Foundations: Microsoft.Extensions.AI (0:06–0:13)

**Message:** one abstraction, many providers — and in **Microsoft Foundry**, one endpoint,
many models. The magic is *an interface*.

- **Demo A — Foundry chat (one live demo):** `samples/CoreSamples/BasicChat-05AIFoundryModels`
  (`app.cs`, file-based app). Run it once with **deployment name + endpoint + apikey**.
  - Highlight `IChatClient` and `client.GetStreamingResponseAsync(...)`.
  - **Ask:** `Q: hi, what is your model name?`
  - **Say:** "No SDK-specific types in my logic — just `IChatClient` over a Foundry model."
- **Demo B — swap the model (Foundry):** change **only** `AzureOpenAI:Deployment` to switch
  **`gpt-5.5` → `grok-4`**. Same code, same endpoint, same `IChatClient`.
  - Ask the **same question again** so the audience sees the model identity/card answer change while the streaming output stays the same UX.
  - **Say:** "One endpoint hosts many models in Foundry. To move from GPT-5.5 to Grok-4 I
    change a single string — the deployment name — nothing else."
- **Demo C — Integrated Security (the right way):** flip `AzureOpenAI:AuthMode` to
  `integrated` (`AzureCliCredential` / Microsoft Entra ID), drop the key, `az login`, rerun.
  - **Say:** "No keys in config. Integrated Security with Entra ID is how you *should* work
    in Foundry — the credential, not a secret string."
- **Demo D — swap the provider to local:** switch to `BasicChat-03Ollama`.
  - Same `IChatClient`, different registration (Foundry → local Ollama).
  - **Say:** "Cloud model to a local model on my laptop — zero changes to app logic.
    That's the foundation everything else sits on."
- **Optional 20s:** mention structured output exists too (lesson 02); streaming is already visible in the demo.

> **Learn more:** [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai?wt.mc_id=) ·
> [Build a chat app](https://learn.microsoft.com/dotnet/ai/quickstarts/build-chat-app?wt.mc_id=) ·
> [Microsoft Foundry](https://learn.microsoft.com/dotnet/ai/?wt.mc_id=)

## Block 3 — Intelligence: VectorData + DataIngestion (0:13–0:22)

**Message:** RAG isn't a black box. It's two official building blocks:
**Microsoft.Extensions.VectorData** (store/search) and **Microsoft.Extensions.DataIngestion**
(read → chunk → enrich → embed → write).

- **Concept slide (60s):** the pipeline diagram —
  `Reader → Chunker → (Enrich) → Embeddings → VectorStoreWriter → VectorStore`.
- **Demo A — VectorData search (official abstraction):**
  `samples/CoreSamples/RAGSimple-02MEAIVectorsMemory` — now built on
  **`Microsoft.Extensions.VectorData`**: `InMemoryVectorStore` → `GetCollection` →
  `UpsertAsync` → `SearchAsync`, over the movie data. Keyless (`az login`), reuses the
  standard `AzureOpenAI:Endpoint` + `AzureOpenAI:EmbeddingDeployment` secrets.
  - **Say:** "This is the *official* VectorData block — not a hand-rolled cosine loop. The
    store is swappable: in-memory today, SQLite/Qdrant/Azure AI Search in prod, same
    `VectorStoreCollection.SearchAsync` code."
- **Demo B — DataIngestion pipeline (NEW — built):**
  `samples/CoreSamples/DataIngestion-01-Simple` console app using
  `IngestionPipeline<T>` (`MarkdownReader` → `SemanticSimilarityChunker` →
  `SummaryEnricher` → `VectorStoreWriter` over `SqliteVectorStore` → query).
  - **Say:** "This *is* what the chat app does at startup with the files in `wwwroot/Data`
    — now you can see every step: read → chunk → enrich → embed → write, then search."
- **Callback to the cold open:** "In Zava, this exact pipeline grounds the MAF agent — and
  the embeddings run **locally** (`ElBruno.LocalEmbeddings`, ONNX MiniLM), so there's *no
  cloud embedding deployment* at all. Same `IEmbeddingGenerator` interface, zero cloud calls."
- **Bridge:** "Now the model can reach *into our own data*. Next: how it reaches *out* to
  the world — live tools and context over MCP."

> If the new sample isn't ready, fall back to walking the chat app's `Services/Ingestion`
>
> - `IngestedChunk.cs` (`[VectorStoreKey]` / `[VectorStoreVector]`) as the live example.

> **Learn more:** [Embeddings in .NET](https://learn.microsoft.com/dotnet/ai/conceptual/embeddings?wt.mc_id=) ·
> [RAG with .NET](https://learn.microsoft.com/dotnet/ai/tutorials/tutorial-ai-vector-search?wt.mc_id=) ·
> [Microsoft.Extensions.VectorData](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai?wt.mc_id=)

## Block 4 — Tools: MCP with the C# SDK (0:22–0:29)

**Message:** models become useful when they can *act* and *fetch live context*. MCP is
the standard wire for that, and there's a first-class **C# MCP SDK**.

- **Demo:** `samples/CoreSamples/MCP-03-MicrosoftLearn` — the *same question, asked twice*.
  - The question: *"What is the latest version of Microsoft Agent Framework for C#? Answer
    with the version number and a Microsoft Learn docs link."*
  - **BEFORE (no MCP):** call `client.GetResponseAsync(question, ...)` with no tools. The
    model answers from **training knowledge** — likely stale, vague, or a guessed version.
    - **Say:** "This is the model on its own. It's confident — but it can't know what shipped
      after its training cutoff."
  - **AFTER (with MCP):** connect an `McpClient` to the public **Microsoft Learn MCP Server**
    (`https://learn.microsoft.com/api/mcp` — keyless, streamable HTTP), `ListToolsAsync()`
    (`microsoft_docs_search`, `microsoft_docs_fetch`, …), pass `tools` into `ChatOptions`
    with `UseFunctionInvocation()`, and ask **the exact same question**.
    - **Say:** "Same question, same model — but now it reaches into live Microsoft Learn docs,
      cites the current version, and gives me a real link. I didn't hardcode the tool call;
      I handed it the tools and MEAI's function-invocation middleware did the orchestration."
  - **Note:** the older HuggingFace MCP demo (`MCP-01-HuggingFace`) is no longer supported;
    the Learn MCP server is public and keyless, so there's nothing to configure.
- **Bridge:** "Now we have a model, our own knowledge, and live tools. Let's package it like
  the template did — as an agent."

> **Learn more:** [MCP in .NET](https://learn.microsoft.com/dotnet/ai/get-started-mcp?wt.mc_id=) ·
> [Build an MCP client](https://learn.microsoft.com/dotnet/ai/quickstarts/build-mcp-client?wt.mc_id=) ·
> [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai?wt.mc_id=)

## Block 5 — The whole package: Microsoft Agent Framework (0:29–0:36)

**Message:** MAF composes chat + tools + memory into **agents**, and agents into
**workflows**. The template's `ChatAgent` *is* a MAF agent.

- **Demo A — a first agent:** `samples/MAF/MAF01` (single agent with instructions/tools).
- **Demo B — an agent that owns MCP tools (NEW — built):** `samples/MAF/MAF-MCP-01` —
  the same Learn MCP tools from Block 4, but now handed to a **MAF agent**
  (`chatClient.AsAIAgent(name, instructions, tools: [.. tools])`). Ask the agent the same
  "latest MAF version" question; it calls the docs tool itself.
  - **Say:** "Block 4 was MEAI wiring the tools by hand. Here the *agent* owns the tools —
    same MCP, same `IChatClient`, now wrapped as a reusable agent with its own instructions."
- **Demo C — multi-agent (optional, time-permitting):** `samples/MAF/MAF02` or
  `MAF-MultiAgents` to show orchestration.
- **Callback to the cold open (Zava):** the **MAF Action Agent** is a real
  `Microsoft.Agents.AI` agent built on `IChatClient` + a VectorData-backed
  `KnowledgeSearch` tool (`AIFunctionFactory.Create(...)`) over the local-embeddings KB.
  Every `ActionResult` returns a structured `Sources` array (doc id/title/snippet/score)
  → the **clickable citation chips** from Block 0.
  - **Say:** "Same MAF agent, same `IChatClient`, same VectorData search tool — the action
    agent you saw cite `RB-014` is just these blocks composed."
- **Demo D — A2A, the agent-to-agent wire (NEW — built):** `samples/CoreSamples/A2A-01` —
  a single console app that **hosts** a MAF writer-agent over **A2A** (`app.MapA2A(agent,
  "/a2a/writer-agent")`) and then **calls it as a remote agent** from a client
  (`new A2AClient(uri).AsAIAgent(...)` → `RunAsync(...)`). One process, real A2A round trip.
  - **Say:** "A2A is the HTTP of agent communication. The client never imports the agent's
    code — it just talks to it over the wire. That's exactly how the .NET agents reach the
    Python/NVIDIA NeMo agent in the cold open."
- **Optional — tie it back:** "MAF for the .NET agents, A2A as the protocol between vendors —
  the same JSON-RPC plumbing you just saw locally is what crossed to NeMo in Zava."
- **Demo E — an agent that paints (NEW — built, optional):**
  `samples/MAF/MAF-ImageGen-03-Foundry` — a MAF agent whose **tool** wraps
  `GptImage2Generator` (`ElBruno.Text2Image.Foundry`) to create an image with
  **GPT-Image-2** on Foundry. `dotnet run` → the agent infers a prompt, calls the tool, and
  saves the PNG. This is the **same Pitch Image Agent pattern** behind the Zava cold-open hero image.
  - **Say:** "Same `AsAIAgent` shape as the docs agent — but this tool returns pixels. The
    incident-hero image from the cold open? It's just a MAF agent with an `IImageGenerator`
    tool. No black box."
  - **Note:** GPT-Image-2 can take a while; for the cold open the image is **pre-rendered**.
    Run this only if time allows, or show a pre-generated PNG.

> **Learn more:** [Agents in .NET](https://learn.microsoft.com/dotnet/ai/get-started-app-chat-template?wt.mc_id=) ·
> [Microsoft Agent Framework](https://learn.microsoft.com/agent-framework/?wt.mc_id=) ·
> [Agent2Agent (A2A) protocol](https://learn.microsoft.com/agent-framework/user-guide/agents/agent-types/a2a-agent?wt.mc_id=)

## Block 6 — Reassemble + takeaways (0:36–0:39)

- **Slide:** the **Zava Support Center** screenshot with **block labels overlaid**:
  - the hero image → **`IImageGenerator`** (MEAI · GPT-Image-2)
  - the chat answer → **`IChatClient`** (MEAI)
  - the grounded retrieval → **VectorData + DataIngestion** (local `IEmbeddingGenerator`)
  - the action agent → **MAF** (`Microsoft.Agents.AI`)
  - the NeMo hop → **A2A** (JSON-RPC, cross-vendor)
- **Takeaways slide** (concept doc §5) — the one they photograph.
- **Say:** "No black boxes. The image, the chat, the citations, the action — every part of
  Zava is just `Microsoft.Extensions.*` composed, with A2A connecting the agents that
  aren't even .NET."

## Block 7 — Close / CTA (0:39–0:40)

- Point to the course repo: **Generative AI for Beginners .NET** (lessons 02–04 cover
  every block shown).
- CTA: "Scaffold the AI Chat template tonight, then open lesson 04 and rebuild its brain —
  and if you want the full multi-agent version, the **Zava Support Center** repo is open
  source."
- Thanks + where to find Bruno.

---

## Demo asset checklist (per block)

| Block | Sample / asset | Pre-flight |
|-------|----------------|-----------|
| 0 / 6 | **Zava Support Center** (`MAF-A2A-NVIDIA-NemoAgents`) | `aspire start` OK; Python venv + deps; NVIDIA + AOAI + GPT-Image-2 secrets; hero image pre-rendered; 3 prompts rehearsed; citations clickable; **recorded fallback** |
| 2 | `BasicChat-05AIFoundryModels`, `BasicChat-03Ollama` | Foundry endpoint set; `gpt-5.5` + `grok-4` deployments exist; apikey set; `az login` for Integrated Security; Ollama model pulled |
| 3A | `RAGSimple-02MEAIVectorsMemory` | rewritten on official VectorData (`InMemoryVectorStore`); **keyless** — reuses `AzureOpenAI:Endpoint` + `AzureOpenAI:EmbeddingDeployment`; `az login`; built & verified |
| 3B | `DataIngestion-01-Simple` | built & verified; chat + embedding deployments set |
| 4 | `MCP-03-MicrosoftLearn` | AOAI secret set; `learn.microsoft.com/api/mcp` reachable (keyless); both before/after paths rehearsed |
| 5 | `MAF01` / `MAF-MCP-01` / `A2A-01` | MAF packages restored; A2A-01 builds (prerelease pkgs); AOAI secret set for all three |
| 5 (opt) | `MAF-ImageGen-03-Foundry` | built & verified; needs `AzureOpenAI:ApiKey` (GPT-Image-2 key auth) + `AzureOpenAI:ImageDeployment=gpt-image-2`; GPT-Image-2 is slow — prefer a pre-rendered PNG on stage |

> Full setup/secrets/fallbacks in [05-constraints-and-demo-notes.md](./05-constraints-and-demo-notes.md).
