# 06 — Slides (OD805 · AI Building Blocks for .NET)

> Minimal, photographable slides. **One intro slide per section** + **one cumulative recap
> slide per section** that *builds up* the stack block by block. Use `---` as the slide
> break (works in Marp / reveal.js / most Markdown deck tools).
>
> Design intent: the recap slides are the spine of the talk. By the close, the final recap
> slide shows the **entire stack** the audience just watched get assembled — the photo they
> take home.

---

## Block 0 — Cold open

# One chat. Three agents

**Zava Support Center**

- An **image** agent
- An **NVIDIA NeMo** analysis agent
- A **.NET action** agent that cites its runbooks

> *"By the end, you'll name every .NET building block behind this."*

---

## Block 1 — Roadmap

# Deconstruct the magic

| Layer | Building block |
|-------|----------------|
| Foundations | **MEAI** — `IChatClient` |
| Intelligence | **VectorData** + **DataIngestion** |
| Tools | **MCP** (C# SDK) |
| Whole package | **MAF** (`Microsoft.Agents.AI`) |
| Between vendors | **A2A** |

> Every block is **NuGet + C#**.

---

## Block 2 — Intro

# Foundations: Microsoft.Extensions.AI

**One interface. Many providers. Many models.**

- `IChatClient` — provider-agnostic chat
- Microsoft Foundry: **one endpoint, many models**
- Swap model = change **one string**
- Integrated Security with **Entra ID** (no keys)

---

## Block 2 — Recap

# The stack so far

### Microsoft.Extensions.AI (MEAI)

- ✅ **Chat** — `IChatClient`

> *One abstraction over every model — cloud or local.*

---

## Block 3 — Intro

# Intelligence: VectorData + DataIngestion

**RAG isn't a black box. It's two building blocks.**

`Reader → Chunker → (Enrich) → Embeddings → VectorStoreWriter → VectorStore`

- **Microsoft.Extensions.VectorData** — store + search
- **Microsoft.Extensions.DataIngestion** — read → chunk → enrich → embed → write
- Embeddings can run **locally** (ONNX MiniLM) — no cloud deployment

---

## Block 3 — Recap

# The stack so far

### Microsoft.Extensions.AI (MEAI)

- ✅ Chat
- ✅ **Embeddings**
- ✅ **Vector Data**
- ✅ **Data Ingestion**

> *Now the model can reach into our own data.*

---

## Block 4 — Intro

# Tools: MCP with the C# SDK

**Models become useful when they can act and fetch live context.**

- **MCP** = the standard wire for tools + context
- First-class **C# MCP SDK** (`ModelContextProtocol`)
- Public, keyless **Microsoft Learn MCP Server**
- Same question, asked twice — **before** vs **after** the tools

> *Before:* the model guesses. *After:* it cites live docs.

---

## Block 4 — Recap

# The stack so far

### Microsoft.Extensions.AI (MEAI)

- ✅ Chat
- ✅ Embeddings
- ✅ Vector Data
- ✅ Data Ingestion
- ✅ **MCP Tools**

> *Now it can reach out to the world — live.*

---

## Block 5 — Intro

# The whole package: Microsoft Agent Framework

**MAF composes chat + tools + memory into agents.**

- `chatClient.AsAIAgent(name, instructions, tools)`
- An agent can **own** its MCP tools
- Agents → **workflows**
- **A2A** when an agent isn't even .NET

---

## Block 5 — Recap

# The stack so far

### Microsoft.Extensions.AI (MEAI)

- ✅ Chat
- ✅ Embeddings
- ✅ Vector Data
- ✅ Data Ingestion
- ✅ MCP Tools

### Microsoft Agent Framework (MAF)

- ✅ **Agents** (`Microsoft.Agents.AI`)

---

## Block 6 — Reassemble

# No black boxes

**Zava Support Center, labeled:**

| What you saw | Building block |
|--------------|----------------|
| Hero image | `IImageGenerator` (MEAI) |
| Chat answer | `IChatClient` (MEAI) |
| Grounded retrieval | VectorData + DataIngestion |
| Action agent | MAF (`Microsoft.Agents.AI`) |
| NeMo hop | **A2A** (cross-vendor) |

---

## Block 7 — Close · The full stack

# The whole stack — assembled

### Microsoft.Extensions.AI (MEAI)

- ✅ Chat
- ✅ Embeddings
- ✅ Vector Data
- ✅ Data Ingestion
- ✅ MCP Tools

### Microsoft Agent Framework (MAF)

- ✅ Agents
- ✅ **A2A** (agent-to-agent, cross-vendor)

> **Generative AI for Beginners .NET** — lessons 02–04 cover every block.
> Scaffold the AI Chat template tonight, then rebuild its brain.

---

## Speaker notes — recap-slide build order

The recap slides are **cumulative**. Keep the list identical line-for-line between blocks and
only **add** the new bold item, so the audience sees the stack *grow*:

| After block | New line added | Cumulative list |
|-------------|----------------|-----------------|
| 2 | Chat | MEAI: Chat |
| 3 | Embeddings, Vector Data, Data Ingestion | MEAI: Chat · Embeddings · Vector Data · Data Ingestion |
| 4 | MCP Tools | + MCP Tools |
| 5 | Agents (MAF) | + MAF: Agents |
| 7 (close) | A2A | + MAF: A2A |

> **Learn more:** [.NET + AI docs](https://learn.microsoft.com/dotnet/ai/?wt.mc_id=) ·
> [Microsoft Agent Framework](https://learn.microsoft.com/agent-framework/?wt.mc_id=)
