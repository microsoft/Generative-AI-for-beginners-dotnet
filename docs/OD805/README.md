# OD805 — Session Planning Hub

> **Session:** AI Building Blocks for .NET: Add Intelligence to your C# Apps
> **Code:** OD805 · **Event:** Microsoft Build 2026 · **Duration:** 40 minutes
> **Branch:** `bruno-Build2026-OD805`

This folder holds **all planning artifacts** for the OD805 session: concept, script,
sample mapping, gaps, and demo notes. Nothing here ships in the course — it's the
backstage for the talk.

## Documents

| # | Doc | Purpose |
|---|-----|---------|
| 01 | [Session Concept](./01-session-concept.md) | The narrative, the "hook" line, the arc, and the takeaways. |
| 02 | [Session Script (40 min)](./02-session-script.md) | Minute-by-minute run of show with demos and talking points. |
| 03 | [Building Blocks → Samples Map](./03-building-blocks-and-samples-map.md) | Which repo sample backs each building block. |
| 04 | [Gaps & Sample Proposals](./04-gaps-and-sample-proposals.md) | Missing scenarios (VectorData / DataIngestion) + proposed minimal samples. |
| 05 | [Constraints & Demo Notes](./05-constraints-and-demo-notes.md) | Setup, secrets, models, risks, fallbacks. |
| 06 | [Slides](./06-slides-v02.md) | Intro + cumulative recap slides that build the stack block by block. |

## TL;DR of the concept

1. **Open with the magic** — show the **Zava Support Center**, a polished multi-agent
   support console: an incident-hero image (`IImageGenerator` / GPT-Image-2) is already on
   screen, an **NVIDIA NeMo** agent analyzes data, and a **MAF** action agent answers with
   **grounded, cited** runbook remediation. Audience sees a real, impressive AI app.
2. **The hook** — *"We've all seen the magic. But do you actually know what's under the
   hood — and how far you can take it with just the .NET you already know?"*
3. **Take it apart** — reveal the building blocks that made the magic, from the ground up:
   - **Foundations** → Microsoft.Extensions.AI (MEAI) — chat (Foundry model swap +
     Integrated Security), embeddings, images
   - **Intelligence & tools** → Microsoft.Extensions.VectorData / DataIngestion + MCP (C# SDK)
   - **The whole package** → Microsoft Agent Framework (MAF) + **A2A** (cross-vendor)
4. **Close the loop** — return to Zava: now the audience can name every block.

## Status

- [x] Research existing repo samples and the AI Chat Template
- [x] Identify gaps (no standalone `Microsoft.Extensions.DataIngestion` sample)
- [x] Draft high-level concept + 40-min script
- [x] Build the bookend demo — **Zava Support Center** (NeMo + MAF A2A, grounded RAG,
      local embeddings, GPT-Image-2 pitch agent, clickable citations)
- [x] Build/verify supporting samples (`DataIngestion-01-Simple`, `MCP-03-MicrosoftLearn`)
- [x] Build/verify new agent samples (`MAF-MCP-01` agent-owned MCP tools, `A2A-01` agent-to-agent)
- [x] Rewrite `RAGSimple-02MEAIVectorsMemory` on the official `VectorStoreCollection`
   abstraction with `ElBruno.Connectors.SqliteVec` (keyless) — closes the VectorData gap
- [x] Build `MAF-ImageGen-03-Foundry` — GPT-Image-2 image building block as a MAF agent tool
- [x] Author slides (`06-slides-v02.md`) with cumulative recap build
- [ ] Review / approve final concept + hook line (Bruno)
- [ ] Capture backup recordings (Zava cold open + one cloud demo)
- [ ] Full dry run within 40 min on the presenting machine
