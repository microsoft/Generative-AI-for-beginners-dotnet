# Learner Experience Report

**Perspective:** New user with medium .NET and Azure knowledge, wanting to learn Generative AI  
**Date:** 2026-02-27  
**Scope:** Full course walkthrough — README → Lesson 1 → 2 → 3 → 4 → 5, setup guides, and sample code

---

## Executive Summary

The course is **well-structured and highly practical** for .NET developers. The progressive lesson flow (concepts → techniques → patterns → agents → responsible AI) is logical and each lesson builds on the previous one. The code samples are real and runnable. However, several friction points would confuse or block a learner — particularly around Azure setup, inconsistent configuration keys, a broken link in the root README, and missing guidance on getting a free Azure account.

**Overall Rating: 7.5/10** — Excellent content, needs onboarding polish.

---

## Detailed Findings

### ✅ What's Good

| Area | Assessment |
|------|-----------|
| **Lesson progression** | Excellent. 5 lessons flow naturally: intro → techniques → patterns → agents → responsible AI. Each builds on the last. |
| **"You already know this" framing** | Brilliant. Lesson 1 immediately tells .NET devs they already have the skills (API calls, DI, async). Reduces intimidation. |
| **Code-first approach** | Every concept has a runnable sample. No "theory only" lessons. |
| **IChatClient abstraction** | Well explained. The "one interface, any provider" message is clear and repeated throughout. |
| **Multiple setup paths** | Three clear options (Codespaces, Ollama, Azure). Learner can pick based on their situation. |
| **Video + text combo** | Each lesson has a video thumbnail with a link. Great for different learning styles. |
| **Self-check questions** | End-of-section questions help learners validate their understanding. |
| **Sample code organization** | The `samples/` folder is well-organized with a clear README mapping samples to lessons. |
| **Responsible AI lesson** | Lesson 5 is thorough and practical, not just a checkbox. Covers bias, guardrails, transparency, and agentic risks. |
| **Multi-language support** | 8 translations available — impressive accessibility. |

---

### ❌ What's Broken or Blocking

#### 1. **Broken link in root README** — `./06-MAF/` does not exist
- **Location:** `README.md` line 39
- **Issue:** The "What's New" section links to `./06-MAF/` but this folder does not exist in the repo. The actual samples are at `./samples/MAF/`.
- **Impact:** 🔴 High — First thing a learner sees in "What's New". Clicking it goes to a 404.
- **Fix:** Change `./06-MAF/` to `./samples/MAF/` or `./04-AgentsWithMAF/readme.md`.

#### 2. **Inconsistent user secrets key names across setup guides**
- **Location:** `setup-azure-openai.md` vs `setup-github-codespaces.md` vs actual code
- **Issue:** Three different naming conventions are used:
  | Document | Endpoint Key | API Key | Model Key |
  |----------|-------------|---------|-----------|
  | `setup-azure-openai.md` | `AZURE_OPENAI_ENDPOINT` | `AZURE_OPENAI_APIKEY` | `AZURE_OPENAI_MODEL` |
  | `setup-github-codespaces.md` | `endpoint` | `apikey` | `deploymentName` |
  | Actual code (`BasicChat-01MEAI/Program.cs`) | `endpoint` | `apikey` | `deploymentName` |
- **Impact:** 🔴 High — A learner following `setup-azure-openai.md` will set the WRONG key names and the samples will crash with null reference exceptions. The code reads `config["endpoint"]`, not `config["AZURE_OPENAI_ENDPOINT"]`.
- **Fix:** Standardize all setup docs to use `endpoint`, `apikey`, and `deploymentName` to match the actual code. Or update the code to read both formats.

#### 3. **No guidance on creating a free Azure account**
- **Location:** `setup-azure-openai.md`
- **Issue:** The doc says "Use enterprise-grade models via your Azure subscription" but never tells a learner how to GET an Azure subscription. No mention of:
  - Azure free account ($200 credit): https://azure.microsoft.com/free/
  - Azure for Students: https://azure.microsoft.com/free/students/
  - Whether the free tier includes Azure OpenAI access
  - Estimated costs for running the samples
- **Impact:** 🟡 Medium — A learner without an Azure account hits a wall. They don't know if it costs money or how much.
- **Fix:** Add a "Step 0: Get an Azure Account" section with links to free tier and cost expectations.

#### 4. **Azure AI Foundry Portal setup is too brief**
- **Location:** `setup-azure-openai.md`
- **Issue:** The guide says "Go to ai.azure.com, create a Hub and Project" in 2 bullet points. For a learner with medium Azure knowledge:
  - What is a "Hub"? What is a "Project"? These are Foundry-specific concepts.
  - No screenshots showing where to click.
  - No mention that Azure OpenAI requires a separate approval/registration step.
  - No guidance on which pricing tier to select.
  - Where exactly is the API key on the "Models + endpoints" page?
- **Impact:** 🟡 Medium — Learner can get lost in the Azure portal without visual guidance.
- **Fix:** Add screenshots, expand the step-by-step, and mention any approval requirements.

---

### ⚠️ Warnings and Concerns

#### 5. **Lesson 4 docs may show outdated Agent Framework API**
- **Location:** `04-AgentsWithMAF/01-building-first-agent.md` lines 25, 70
- **Issue:** The docs show `chatClient.CreateAIAgent(...)` and `AgentRunResponse` types. The repo's "What's New" section mentions Agent Framework reached RC (`1.0.0-rc1`) with breaking API changes. The actual sample code (`MAF01/Program.cs`) still uses `CreateAIAgent`, which works with the current packages, but if a learner installs the latest RC packages directly, the API may have changed to `AsAIAgent()`.
- **Impact:** 🟡 Medium — Currently works, but may break as packages update. The docs should note the preview status.
- **Fix:** Add a note that the API is pre-release and may change. Link to the official Agent Framework migration guide.

#### 6. **User secrets must be set per-project, not explained clearly**
- **Location:** `setup-github-codespaces.md` step 2
- **Issue:** The guide shows setting secrets for ONE project (`BasicChat-01MEAI`). A learner will wonder: "Do I need to run `dotnet user-secrets set` for EVERY sample?" The answer is yes (user secrets are per-project), but this isn't explained.
- **Impact:** 🟡 Medium — Learner runs BasicChat-01MEAI successfully, then tries BasicChat-03Ollama and it fails because secrets aren't set.
- **Fix:** Explain that user secrets are per-project. Provide a script or instructions to set secrets for all projects at once (or mention a shared approach).

#### 7. **No "which model do I need?" guidance for Azure**
- **Location:** `setup-azure-openai.md` step 2
- **Issue:** Says "Select gpt-4o or gpt-4o-mini" but doesn't explain:
  - Why choose one over the other (cost vs capability)
  - Whether ALL samples work with gpt-4o-mini or if some require gpt-4o
  - Whether an embedding model is also needed (Lesson 3 uses embeddings)
  - The Ollama guide mentions `all-minilm` for embeddings, but the Azure guide doesn't mention deploying an embedding model
- **Impact:** 🟡 Medium — Learner deploys only a chat model, then gets errors in Lesson 3 RAG samples that need embeddings.
- **Fix:** List all models needed for the full course. Add a note about embedding models for Lesson 3.

#### 8. **Lesson 1 setup-github-codespaces.md skips "where to get credentials"**
- **Location:** `setup-github-codespaces.md` step 2
- **Issue:** It tells you to set user secrets with your endpoint, API key, and deployment name, but says "Replace the values with your Azure OpenAI or Microsoft Foundry endpoint." It doesn't link to `setup-azure-openai.md` to explain HOW to get those values.
- **Impact:** 🟡 Medium — A Codespaces user who doesn't already have Azure credentials is stuck.
- **Fix:** Add a link: "Don't have these credentials yet? Follow the [Azure OpenAI Setup Guide](./setup-azure-openai.md) first."

---

### 💡 Suggestions for Improvement

#### 9. **Add a "Learning Path" visual diagram**
- The course has a clear progression but no visual roadmap. A simple flow diagram (Lesson 1 → 2 → 3 → 4 → 5) with "what you'll build" at each step would help learners see the full journey upfront.

#### 10. **Add estimated time per lesson**
- Learners want to know: "Can I do this in a lunch break?" Add time estimates like "⏱️ ~30 minutes" to each lesson header.

#### 11. **Add a "Troubleshooting" section to each setup guide**
- Common issues like "Ollama not starting," "API key rejected," or "model not found" with solutions would prevent learner frustration. The `docs/FIX-YOUR-ERROR.md` file exists but is not linked from any setup guide.

#### 12. **Clarify the Ollama-first vs Azure-first learning path**
- Lesson 1 recommends Codespaces (which needs Azure). But Ollama is free and requires no account. For a budget-conscious learner, a clearer "FREE path: use Ollama" vs "CLOUD path: use Azure" decision tree would help.

#### 13. **Add a "Prerequisites Checklist" at the start of Lesson 2**
- Before diving into code, confirm: "Before you start: ✅ .NET 9 installed ✅ User secrets configured ✅ Model accessible (run `dotnet run` in BasicChat-01MEAI to verify)."

#### 14. **Link `docs/FIX-YOUR-ERROR.md` from setup guides**
- This file exists but is orphaned — no lesson or setup guide links to it. It should be referenced in all three setup guides as a troubleshooting resource.

---

## Lesson-by-Lesson Assessment

| Lesson | Rating | Notes |
|--------|--------|-------|
| **01 - Introduction** | ⭐⭐⭐⭐ | Excellent conceptual intro. Setup guides need work (see findings 2-4, 6-8). |
| **02 - Techniques** | ⭐⭐⭐⭐⭐ | Outstanding. Clear progression: chat → streaming → structured output → function calling → middleware. Code samples match docs. |
| **03 - Patterns** | ⭐⭐⭐⭐ | Good coverage of embeddings, RAG, vision, local runners. Missing: explicit note about needing an embedding model deployed in Azure. |
| **04 - Agents** | ⭐⭐⭐⭐ | Good agent intro. Warning: API may be pre-release and subject to change. Docs and code currently align. |
| **05 - Responsible AI** | ⭐⭐⭐⭐⭐ | Excellent. No code samples (appropriate for the topic). Thorough coverage of bias, safety, transparency, and agentic risks. |

---

## Priority Fix List

| Priority | Issue | Effort |
|----------|-------|--------|
| 🔴 P0 | Fix broken `./06-MAF/` link in root README | 1 min |
| 🔴 P0 | Standardize user secrets key names across all setup docs | 15 min |
| 🟡 P1 | Add "Step 0: Get an Azure Account" with free tier links | 10 min |
| 🟡 P1 | Cross-link Codespaces guide → Azure setup guide for credentials | 2 min |
| 🟡 P1 | Expand Azure AI Foundry setup with more detail/screenshots | 30 min |
| 🟡 P1 | Document which models to deploy for full course (chat + embedding) | 10 min |
| 🟡 P1 | Explain per-project user secrets + provide bulk setup guidance | 15 min |
| 🟢 P2 | Link FIX-YOUR-ERROR.md from setup guides | 5 min |
| 🟢 P2 | Add Agent Framework pre-release API warning | 5 min |
| 🟢 P2 | Add time estimates to lesson headers | 10 min |
| 🟢 P2 | Add learning path visual diagram | 20 min |
| 🟢 P2 | Add prerequisite checklists to lessons 2-4 | 15 min |

---

## Conclusion

The course content is **excellent** — well-written, practical, and progressively structured. The main friction is in the **onboarding and setup experience**, where inconsistent configuration, missing Azure account guidance, and a broken link could block or confuse learners in their first 30 minutes. Fixing the P0 and P1 items above would significantly improve the first-run experience and reduce drop-off at the setup stage.
