# MAF v1 Upgrade Plan — Generative AI for Beginners .NET

**Branch:** `squad/dozer-maf-upgrades-2026-04-08`
**Requested by:** Bruno Capuano
**Date:** 2026-04-08

---

## 1. Context & Motivation

Microsoft Agent Framework v1.0 GA was released on **2026-04-02** ([GitHub release](https://github.com/microsoft/agent-framework/releases/tag/dotnet-1.0.0)). All 26 MAF samples in this repo currently reference **preview** packages (`1.0.0-preview.260108.1` / `1.0.0-preview.251219.1`). This plan covers upgrading to stable v1, updating docs, adding new Hosted Agent scenarios, and improving the overall learning experience.

### Key References
- [Introducing Microsoft Agent Framework (blog)](https://devblogs.microsoft.com/foundry/introducing-microsoft-agent-framework-the-open-source-engine-for-agentic-ai-apps/) — 2025-10-01
- [MAF Reaches Release Candidate (blog)](https://devblogs.microsoft.com/foundry/microsoft-agent-framework-reaches-release-candidate/) — 2026-02-20
- [Foundry Agent Service GA (blog)](https://devblogs.microsoft.com/foundry/foundry-agent-service-ga/) — 2026-03-16
- [MAF v1.0 GitHub Release](https://github.com/microsoft/agent-framework/releases/tag/dotnet-1.0.0) — 2026-04-02
- [Hosted Agents Docs](https://learn.microsoft.com/en-us/azure/foundry/agents/concepts/hosted-agents)

---

## 2. Package Upgrade Matrix

### v1 Stable NuGet Packages (targets)
| Package | v1 Version |
|---------|-----------|
| `Microsoft.Agents.AI` | 1.0.0 |
| `Microsoft.Agents.AI.Abstractions` | 1.0.0 |
| `Microsoft.Agents.AI.Foundry` | 1.0.0 (replaces `Microsoft.Agents.AI.AzureAI`) |
| `Microsoft.Agents.AI.OpenAI` | 1.0.0 |
| `Microsoft.Agents.AI.Workflows` | 1.0.0 |
| `Microsoft.Agents.AI.Workflows.Generators` | 1.0.0 |

### Breaking Changes to Handle
1. **`Microsoft.Agents.AI.AzureAI` → `Microsoft.Agents.AI.Foundry`** — package rename, likely namespace changes
2. **`OpenAIAssistantClientExtensions` removed** — any samples using this need alternative
3. **`ServiceStoredSimulatingChatClient` → `PerServiceCallChatHistoryPersistingChatClient`** — rename in persisting samples
4. **RC5 cleanup:** kwargs across agents/chat clients/tools/sessions changed
5. **`Azure.AI.Agents.Persistent`** — check if still beta or if there's a v1 equivalent

### Additional Packages to Review
| Package | Current | Action |
|---------|---------|--------|
| `Azure.AI.OpenAI` | 2.8.0-beta.1 | Check for stable release |
| `OpenAI` | 2.8.0 | Check for updates |
| `Aspire.Azure.AI.OpenAI` | 9.5.2-preview / 13.1.0-preview | Check for stable |
| `OllamaSharp` | 5.4.18 | Check for updates |
| `ModelContextProtocol` | 1.0.0 | Already stable ✅ |
| `elbruno.Extensions.AI.Claude` | 0.1.0-preview.6 | Check for updates |
| `Microsoft.Extensions.AI` | 10.3.0 | Already stable ✅ |

---

## 3. Sample-by-Sample Upgrade Analysis

### Tier 1 — Simple upgrades (package bump only, no breaking changes expected)
| # | Sample | Key Change |
|---|--------|------------|
| 1 | MAF01 | `Microsoft.Agents.AI` preview → 1.0.0 |
| 2 | MAF02 | `Microsoft.Agents.AI` + `Workflows` preview → 1.0.0 |
| 3 | MAF-Ollama-01 | `Microsoft.Agents.AI` preview → 1.0.0 |
| 4 | MAF-FoundryClaude-01 | `Microsoft.Agents.AI` preview → 1.0.0 |
| 5 | MAF-FoundryClaude-Persisting-01 | `Microsoft.Agents.AI` preview → 1.0.0 |
| 6 | MAF-MultiModel | `Microsoft.Agents.AI.Workflows` preview → 1.0.0 |

### Tier 2 — Package rename required (`AzureAI` → `Foundry`)
| # | Sample | Key Change |
|---|--------|------------|
| 7 | MAF-AIFoundry-01 | `AI.OpenAI` preview → 1.0.0 |
| 8 | MAF-AIFoundry-02 | `AI.OpenAI` preview → 1.0.0 |
| 9 | MAF-BackgroundResponses-01-Simple | `Abstractions` + `AzureAI` + `OpenAI` → v1 (AzureAI→Foundry) |
| 10 | MAF-BackgroundResponses-02-Tools | Same as above |
| 11 | MAF-BackgroundResponses-03-Complex | Same as above |
| 12 | MAF-Persisting-01-Simple | `Abstractions` + `AzureAI` + `OpenAI` → v1 |
| 13 | MAF-Persisting-02-Menu | `Abstractions` + `AzureAI` + `OpenAI` → v1 |
| 14 | MAF-ImageGen-01 | `AI.OpenAI` preview → 1.0.0 |
| 15 | MAF-ImageGen-02 | `AI.OpenAI` preview → 1.0.0 |

### Tier 3 — Complex upgrades (Foundry persistent agents, hosting, workflows)
| # | Sample | Key Change |
|---|--------|------------|
| 16 | MAF-AIFoundryAgents-01 | `AzureAI.Persistent` + `Workflows` → v1; check `Azure.AI.Agents.Persistent` beta compat |
| 17 | MAF-MicrosoftFoundryAgents-01 | Same as above |
| 18 | MAF-MicrosoftFoundryAgents-02 | Same as above |
| 19 | MAF-MultiAgents | `AzureAI` + `AzureAI.Persistent` + `Workflows` → v1; 3-agent orchestration |
| 20 | MAF-MultiAgents-Factory-01 | `Declarative` + `Workflows` → v1; factory pattern |

### Tier 4 — Web/Hosting apps (Aspire + Hosting packages)
| # | Sample | Key Change |
|---|--------|------------|
| 21 | MAF-AIWebChatApp-Simple | `Hosting` + `OpenAI` alpha/preview → v1; Aspire update |
| 22 | MAF-AIWebChatApp-Middleware | `Hosting` + `Abstractions` → v1; middleware pipeline |
| 23 | MAF-AIWebChatApp-Persisting | `Hosting` + `Abstractions` → v1 |
| 24 | MAF-AIWebChatApp-MutliAgent | `DevUI` + `Declarative` + `Hosting` → v1; multi-agent web |
| 25 | MAF-AIWebChatApp-FoundryClaude | `Hosting` → v1; Claude integration |
| 26 | MAF-AIWebChatApp-AG-UI | `AGUI` + `Hosting.AGUI.AspNetCore` → v1; AG-UI integration |

---

## 4. New Hosted Agent Scenarios

Based on the [Hosted Agents documentation](https://learn.microsoft.com/en-us/azure/foundry/agents/concepts/hosted-agents), create 2 new sample projects:

### Scenario A: `MAF-HostedAgent-01-TimeZone` — Basic Hosted Agent
**What it does:** A containerized .NET agent deployed to Foundry Agent Service that answers time/date questions using a local tool function (inspired by the official Python sample).

**Key concepts demonstrated:**
- Using `Azure.AI.AgentServer.Core` and `Azure.AI.AgentServer.AgentFramework` NuGet packages
- Wrapping a MAF agent with the hosting adapter
- Local testing on `localhost:8088`
- Dockerfile for `linux/amd64` containerization
- `agent.yaml` manifest for Foundry deployment
- Deployment via `azd ai agent init` + `azd up`
- Managed identity + Azure Container Registry integration

**Structure:**
```
samples/MAF/MAF-HostedAgent-01-TimeZone/
├── Program.cs              # Agent with timezone tool
├── Dockerfile
├── agent.yaml              # Foundry agent manifest
├── MAF-HostedAgent-01.csproj
├── README.md               # Step-by-step guide
└── test.http               # REST test file for local testing
```

### Scenario B: `MAF-HostedAgent-02-MultiAgent` — Hosted Multi-Agent Workflow
**What it does:** A containerized multi-agent workflow (researcher → writer → reviewer) deployed as a hosted agent on Foundry. Demonstrates enterprise-grade agent hosting with observability.

**Key concepts demonstrated:**
- Multi-agent workflow packaged as a single hosted agent
- OpenTelemetry tracing integration
- Scaling configuration (min/max replicas, scale-to-zero)
- Agent versioning and non-versioned updates
- Log streaming for debugging
- Publishing to channels

**Structure:**
```
samples/MAF/MAF-HostedAgent-02-MultiAgent/
├── Program.cs              # Multi-agent workflow with hosting adapter
├── Agents/
│   ├── ResearchAgent.cs
│   ├── WriterAgent.cs
│   └── ReviewerAgent.cs
├── Dockerfile
├── agent.yaml
├── MAF-HostedAgent-02.csproj
├── README.md
└── test.http
```

---

## 5. Documentation Updates

### 5.1 What's New Section (root README.md)
Add entry for MAF v1 GA release:
```markdown
### 🚀 Microsoft Agent Framework v1.0 GA (April 2026)
- All 26+ MAF samples upgraded from preview to **stable v1.0** packages
- **Breaking change:** `Microsoft.Agents.AI.AzureAI` renamed to `Microsoft.Agents.AI.Foundry`
- **2 new Hosted Agent scenarios** — deploy containerized agents to Foundry Agent Service
- Multi-agent workflows, streaming, persistence, and MCP all production-ready
- See: [Official GA Release](https://github.com/microsoft/agent-framework/releases/tag/dotnet-1.0.0) | [RC Blog Post](https://devblogs.microsoft.com/foundry/microsoft-agent-framework-reaches-release-candidate/) | [Foundry Agent Service GA](https://devblogs.microsoft.com/foundry/foundry-agent-service-ga/)
```

### 5.2 Lesson 04 — Agents with MAF (`04-AgentsWithMAF/readme.md`)
- [ ] Replace all `--prerelease` install instructions with stable v1 package references
- [ ] Add new section: **"Hosted Agents"** covering Foundry Agent Service deployment
- [ ] Add new section: **"Observability & Tracing"** covering OpenTelemetry integration
- [ ] Reference the 2 new hosted agent samples
- [ ] Update the sample reference table with all 28 samples (26 existing + 2 new)

### 5.3 Lesson 04 Sub-Pages
- [ ] `01-building-first-agent.md` — Remove `--prerelease` flag from install commands (line ~47-51)
- [ ] `02-agents-with-tools.md` — Update package references
- [ ] `03-multi-agent-workflows.md` — Update package references
- [ ] `04-model-context-protocol.md` — Update package references

### 5.4 Lesson 01 — Introduction (`01-IntroductionToGenerativeAI/readme.md`)
- [ ] Remove "Microsoft Agent Framework (Coming Later)" text (line ~186-190) — MAF is here now!
- [ ] Add brief MAF mention with link to Lesson 04

### 5.5 Samples Documentation
- [ ] Create `samples/MAF/README.md` — unified index of all MAF samples organized by category:
  - Getting Started (MAF01, MAF02, MAF-Ollama-01)
  - Azure AI Foundry (MAF-AIFoundry-01/02, MAF-AIFoundryAgents-01)
  - Multi-Agent Workflows (MAF-MultiAgents, MAF-MultiAgents-Factory-01, MAF-MultiModel)
  - Background Responses (01-Simple, 02-Tools, 03-Complex)
  - Persistence (MAF-Persisting-01/02, MAF-FoundryClaude-Persisting-01)
  - Web Apps (all MAF-AIWebChatApp-* samples)
  - Image Generation + MCP (MAF-ImageGen-01/02)
  - Claude Integration (MAF-FoundryClaude-01, MAF-AIWebChatApp-FoundryClaude)
  - **NEW: Hosted Agents** (MAF-HostedAgent-01/02)
- [ ] Update `samples/MAF/CLAUDE-SAMPLES-README.md` — update elbruno.Extensions.AI.Claude version

### 5.6 What's New Archive (`10-WhatsNew/readme.md`)
- [ ] Add detailed MAF v1 entry with migration notes

### 5.7 Translation Impact
Languages requiring updates: zh, tw, fr, ja, ko, pt, es, de
- Root README.md What's New section
- Lesson 04 equivalents (agent docs exist in several locales)
- Any MAF-referencing docs in translations

---

## 6. Improvement Suggestions for GenAI .NET Experience

### 6.1 Learning Path Clarity
- **Add a visual learning path** in root README: Setup → Lesson 1 (Intro) → Lesson 2 (Techniques) → Lesson 3 (Patterns) → Lesson 4 (Agents) → Lesson 5 (Responsible AI)
- **Add a "Start Here" guide** for absolute beginners with 3 tracks:
  - 🏃 Quick Start (30 min): Run MAF01, see an agent work
  - 🚶 Full Course (half day): All 5 lessons in order
  - 🎯 Agent Deep Dive: Skip to Lesson 4, build hosted agents

### 6.2 New Lesson Content Ideas
- **Lesson 4.5 — Advanced Agents:** Hosted agents, persistent agents, telemetry/tracing, prompt injection defense, human-in-the-loop patterns
- **Lesson 6 — Production Deployment:** CI/CD for agents, Azure Container Registry, Foundry Agent Service, monitoring with Application Insights
- **Lesson on A2A (Agent-to-Agent) Protocol:** Cross-runtime agent collaboration, the emerging standard

### 6.3 Sample Improvements
- **Capstone project:** A complete end-to-end sample (e.g., "AI Research Assistant") that combines RAG + multi-agent + MCP + hosted deployment — the culmination of all lessons
- **Hands-on checkpoints** per lesson: "Run this sample → Change this prompt → Swap provider → See the difference"
- **Interactive code tasks:** Add `// TODO:` comments in samples for learners to complete
- **"Diff" view:** Show before/after for each concept (e.g., "chat without tools" → "chat with tools")

### 6.4 Developer Experience
- **Unified setup script:** `setup.ps1` could detect and configure both Ollama and Azure OpenAI, validate prerequisites, and report readiness
- **DevContainer improvements:** Pre-pull Ollama models, pre-install MAF packages, include azd CLI for hosted agent deployment
- **Sample runner:** A top-level script that lists all samples and lets you pick one to run
- **Package status badges:** Add NuGet version badges to sample READMEs so students see if they're on the latest

### 6.5 Content Gaps to Fill
- **Observability:** No samples currently show OpenTelemetry tracing for agents — critical for production
- **Safety:** No prompt injection defense or human-in-the-loop patterns in samples
- **A2A Protocol:** The Agent-to-Agent protocol is a key v1 feature but has no sample
- **Declarative agents:** `MAF-MultiAgents-Factory-01` uses declarative config but it's not well-documented as a pattern
- **Agent memory strategies:** v1 supports various memory patterns — no dedicated sample

### 6.6 Community & Engagement
- **Add a "Contributing Samples" guide** — encourage community to submit MAF samples
- **GitHub Discussions:** Enable for Q&A about specific lessons/samples
- **Monthly "What's New" cadence:** Keep the What's New section fresh with package updates, new samples, community contributions

---

## 7. Execution Order (Recommended)

### Phase 1: Foundation (do first)
1. Upgrade Tier 1 samples (simple package bumps) — build & verify
2. Upgrade Tier 2 samples (AzureAI → Foundry rename) — build & verify
3. Update Lesson 04 install instructions (remove `--prerelease`)
4. Update Lesson 01 "Coming Later" text

### Phase 2: Complex Upgrades
5. Upgrade Tier 3 samples (persistent agents, workflows) — build & verify
6. Upgrade Tier 4 samples (web/hosting apps) — build & verify
7. Run `dotnet format` on all changed projects
8. Full solution build verification: `dotnet build samples/MAF/MAF-Demos.slnx`

### Phase 3: New Content
9. Create `MAF-HostedAgent-01-TimeZone` sample
10. Create `MAF-HostedAgent-02-MultiAgent` sample
11. Create `samples/MAF/README.md` unified index
12. Add Hosted Agents section to Lesson 04

### Phase 4: What's New & Docs
13. Update root README.md What's New section
14. Update `10-WhatsNew/readme.md` archive
15. Update CLAUDE-SAMPLES-README.md
16. Update Lesson 04 sub-pages (01-04)

### Phase 5: Polish
17. Update translations (8 languages)
18. Commit, push, create PR with comprehensive description
19. Tag Oracle to draft a changelog entry

---

## 8. Risk Assessment

| Risk | Impact | Mitigation |
|------|--------|------------|
| v1 API breaking changes beyond documented ones | High | Build each sample individually, fix as discovered |
| `Azure.AI.Agents.Persistent` still beta | Medium | Check NuGet for v1 equivalent; may need to keep beta with note |
| Aspire packages still preview | Low | Note in docs; Aspire follows its own release cycle |
| `elbruno.Extensions.AI.Claude` still preview | Low | Community package; note in docs |
| Hosted Agent samples need Azure subscription | Medium | Provide clear setup guide; include local-testing-only path |
| Translation sync across 8 languages | Medium | Prioritize root README; lesson translations can follow |
