# 03 — Building Blocks → Samples Map

Maps each building block in the session flow to **existing repo samples** (reuse-first),
and flags where a sample is missing (→ see [04-gaps-and-sample-proposals.md](./04-gaps-and-sample-proposals.md)).

## The sample flow (from the brief)

```
Foundations
  └─ MEAI — Microsoft.Extensions.AI
Evolve with intelligence & tools
  ├─ Microsoft.Extensions.VectorData / Microsoft.Extensions.DataIngestion
  └─ MCP — C# MCP SDK
The whole agent package
  └─ MAF — Microsoft Agent Framework
```

## Mapping table

| Building block | Concept to show | Existing sample(s) | Status |
|---|---|---|---|
| **Cold-open / close app** | Zava Support Center: 3 agents (image + NeMo + MAF), grounded RAG, citations, A2A | `MAF-A2A-NVIDIA-NemoAgents` (separate repo) | ✅ **Built** (bookend demo) |
| **MEAI — image** | `IImageGenerator`, GPT-Image-2 | `ElBruno.Text2Image.Foundry` (in Zava); `samples/MAF/MAF-ImageGen-03-Foundry` (MAF agent + GPT-Image-2) | ✅ **Built** — mirrors Zava Pitch Image Agent |
| **MEAI — chat** | `IChatClient`, provider-agnostic | `samples/CoreSamples/BasicChat-01MEAI` | ✅ Reuse |
| **MEAI — Foundry model swap** | one endpoint, many models: `gpt-5.5 → grok-4`; Integrated Security | `samples/CoreSamples/BasicChat-05AIFoundryModels` | ✅ **Block 2 demo** |
| **MEAI — provider swap** | Foundry ↔ Ollama | `samples/CoreSamples/BasicChat-03Ollama` | ✅ Reuse |
| **MEAI — local embeddings** | `IEmbeddingGenerator`, no cloud deployment | `ElBruno.LocalEmbeddings` (ONNX MiniLM, in Zava) | ✅ In Zava |
| **MEAI — functions/middleware** | function calling, `UseFunctionInvocation` | `samples/CoreSamples/MEAIFunctions*` | ✅ Reuse (mention) |
| **MCP — C# SDK** | `McpClient`, tools → `ChatOptions`; *same question before/after MCP* | `samples/CoreSamples/MCP-03-MicrosoftLearn` | ✅ Reuse (keyless Learn MCP) |
| **MCP — local model** | MCP tools + Ollama | `samples/CoreSamples/MCP-02-HuggingFace-Ollama` | ⚠️ Backup (HF MCP deprecated) |
| **VectorData — search** | embeddings + similarity search | `samples/CoreSamples/RAGSimple-02MEAIVectorsMemory` | ✅ **Rewritten** on `InMemoryVectorStore` + `VectorStoreCollection.SearchAsync` (keyless) |
| **VectorData — real store** | Azure AI Search / Qdrant connectors | `RAGSimple-03MEAIVectorsAISearch`, `RAGSimple-04MEAIVectorsQdrant` | ✅ Reuse (mention) |
| **DataIngestion — pipeline** | `IngestionPipeline<T>` read→chunk→enrich→embed→write | `samples/CoreSamples/DataIngestion-01-Simple` | ✅ **Built** |
| **VectorData in the app** | `[VectorStoreKey]`, `[VectorStoreVector]`, grounded search tool | Zava `KnowledgeSearch` + `MafActionAgent` | ✅ In Zava |
| **MAF — first agent** | single agent, instructions, tools | `samples/MAF/MAF01` | ✅ Reuse |
| **MAF — agent owns MCP tools** | `AsAIAgent(name, instructions, tools)` over Learn MCP | `samples/MAF/MAF-MCP-01` | ✅ **Built** |
| **MAF — multi-agent** | orchestration / workflows | `samples/MAF/MAF02`, `samples/MAF/MAF-MultiAgents` | ✅ Reuse (optional) |
| **MAF in the app** | `Microsoft.Agents.AI` action agent + RAG tool | Zava `MafActionAgent` | ✅ In Zava (callback) |
| **A2A — host + call an agent** | `app.MapA2A(agent, path)` + `A2AClient(uri).AsAIAgent()` | `samples/CoreSamples/A2A-01` | ✅ **Built** |
| **A2A — cross-vendor** | JSON-RPC between .NET MAF ↔ Python NVIDIA NeMo | Zava A2A bridge | ✅ In Zava |

## Notes on key reuse targets

### Zava Support Center (cold open + reassemble)

- The fully-implemented **NeMo + MAF A2A** app (repo `elbruno/MAF-A2A-NVIDIA-NemoAgents`,
  local `d:\elbruno\MAF-A2A-NVIDIA-NemoAgents`). `aspire start` → `http://localhost:5000`.
- Showcases the whole stack we deconstruct, **plus** the multi-agent story:
  - **Pitch Image Agent** → `IImageGenerator` (`ElBruno.Text2Image.Foundry`, GPT-Image-2)
  - **MAF Action Agent** → `Microsoft.Agents.AI` on `IChatClient`
  - **Grounded RAG** → DataIngestion + VectorData with **local** embeddings
    (`ElBruno.LocalEmbeddings`, ONNX MiniLM — no cloud embedding deployment)
  - **Cited sources** → structured `Sources` array → clickable citation chips + `/knowledge` viewer
  - **NeMo Data Analysis Agent** (Python / NVIDIA) reached over **A2A** (JSON-RPC)
  - Optional **MCP retrieval** behind `ENABLE_MCP_RETRIEVAL`
- This single app is both the **opening hook** and the **closing payoff**.
- **Demo prompts** (rehearsed, known-good):
  - *"Analyze quarterly revenue trends"* (NeMo)
  - *"Trigger an alert for the payments service error-rate spike and recommend the runbook
    remediation"* (MAF grounded RAG — cites `RB-014` + `ASP-001`)
  - *"Generate an incident report for the payments error-rate spike using the runbook and
    policy"* (MAF grounded RAG)
  - *"Generate an incident-response image"* (Pitch Image Agent, on demand)

> **`ChatApp20` remains a simpler fallback** if the multi-agent app is unavailable: it
> wires `AddChatClient` + `AddEmbeddingGenerator` + `AddSqliteVecCollection` +
> `AddAIAgent("ChatAgent", …)`, the same blocks in one .NET process.

### `BasicChat-01MEAI` (foundations)

- File-based app (`app.cs`) with `#:package` directives — clean, minimal, modern C#.
- Perfect "this is the whole thing" moment for `IChatClient`.

### `BasicChat-05AIFoundryModels` (Block 2 live demo)

- One **Microsoft Foundry** endpoint, many models. Swap `AzureOpenAI:Deployment` to move
  from **`gpt-5.5` → `grok-4`** — same code, same endpoint, same `IChatClient`.
- Toggle `AzureOpenAI:AuthMode` between `apikey` and `integrated` to contrast key auth with
  **Integrated Security** (`AzureCliCredential` / Microsoft Entra ID — the recommended path).
- Then switch to `BasicChat-03Ollama` to run the same `IChatClient` against a local model.

### `MCP-03-MicrosoftLearn` (tools)

- Shows `McpClient.CreateAsync`, `ListToolsAsync`, and tools flowing into `ChatOptions`
  with `UseFunctionInvocation()`. Asks the **same question twice** — once with no tools
  (model knowledge, likely stale) and once with the Learn MCP tools (cites the live docs).
  Runs against the public **Microsoft Learn MCP Server** (`https://learn.microsoft.com/api/mcp`)
  — keyless, so no tokens to configure on stage. Replaces the deprecated `MCP-01-HuggingFace`
  demo.

### `MAF-MCP-01` (agent owns the MCP tools)

- The same Learn MCP tools as Block 4, but handed to a **MAF agent** via
  `chatClient.AsAIAgent(name, instructions, tools: [.. tools])`. The agent calls the docs
  tool itself when asked the "latest MAF version" question. Bridges Block 4 (MEAI wiring
  tools by hand) to Block 5 (the agent owns its tools).

### `A2A-01` (agent-to-agent)

- A single console app that **hosts** a MAF writer-agent over **A2A**
  (`app.MapA2A(agent, "/a2a/writer-agent")`) and then **calls it as a remote agent** from a
  client (`new A2AClient(uri).AsAIAgent(...)` → `RunAsync(...)`). One process, real A2A
  round trip — the local, minimal version of the cross-vendor hop in Zava. Uses prerelease
  `Microsoft.Agents.AI.A2A` + `Microsoft.Agents.AI.Hosting.A2A.AspNetCore` packages.

### `DataIngestion-01-Simple` (intelligence)

- Full `IngestionPipeline<T>`: `MarkdownReader` → `SemanticSimilarityChunker` →
  `SummaryEnricher` → `VectorStoreWriter` over a `SqliteVectorStore`, then
  `VectorStoreCollection.SearchAsync`. The package-based counterpart to the chat app's
  hand-rolled ingestion — read → chunk → enrich → embed → write, fully visible.

## Lesson cross-reference (for the CTA)

| Block | Course lesson |
|---|---|
| MEAI | `02-GenerativeAITechniques/` |
| VectorData / RAG / DataIngestion | `03-AIPatternsAndApplications/` |
| MCP | `04-AgentsWithMAF/04-model-context-protocol.md` |
| MAF | `04-AgentsWithMAF/` |
