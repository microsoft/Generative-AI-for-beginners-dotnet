# Squad Decisions

## Active Decisions

### 1. Azure Infrastructure via azd + Bicep

**Author:** Neo  
**Date:** Phase 1  
**Status:** Implemented

**Decision:** Use `azd` with Bicep templates for Azure OpenAI provisioning. Secrets ID: `genai-beginners-dotnet` (shared across all samples). Three user secrets: `AzureOpenAI:Endpoint`, `AzureOpenAI:Deployment` (`gpt-5-mini`), `AzureOpenAI:EmbeddingDeployment` (`text-embedding-3-small`). `setup.ps1` and `cleanup.ps1` at repo root for one-command provisioning/teardown.

**Impact:** All samples should read from `--id genai-beginners-dotnet` user secrets. New samples needing Azure OpenAI can use the provisioned endpoint and deployment names directly.

---

### 2. CoreSamples Standardized Secrets & Model Config

**Author:** Neo  
**Date:** 2025-07-23  
**Status:** Implemented

**Decision:** Standardized config key: `AzureOpenAI:Deployment` replaces `deploymentName`, `AZURE_OPENAI_MODEL`, and hardcoded model strings across 13 Program.cs files. Default fallback: `gpt-5-mini` for all Azure OpenAI chat samples. Exceptions preserved: Ollama models, dall-e-3 (image gen), sora/sora-2 (video gen), Claude (different config pattern), gpt-realtime (Audio-02), and library enums (LLMTornado) left unchanged.

**Impact:** Any future CoreSamples project should use `genai-beginners-dotnet` as UserSecretsId and `config["AzureOpenAI:Deployment"]` for the model name. The `setup.ps1` script writes secrets with these exact keys, so all samples work out of the box after running setup.

---

### 3. MAF samples migrated to shared secrets & config pattern

**Author:** Neo  
**Date:** Phase 3 migration  
**Status:** Implemented  
**Scope:** All samples/MAF/ projects

**Decision:** 29 .csproj files now use `UserSecretsId = genai-beginners-dotnet` (shared across all course samples). 13 .cs files migrated from `config["deploymentName"]` to `config["AzureOpenAI:Deployment"]` config key. Default Azure OpenAI model standardized to `gpt-5-mini` across all MAF samples. Aspire AppHost deployments updated from `gpt-4o-mini` to `gpt-5-mini`.

**Rationale:** Single `dotnet user-secrets` store for the entire course eliminates per-project secret setup. Consistent config key (`AzureOpenAI:Deployment`) matches the `setup.ps1` provisioning pattern from Phase 1. Students run `setup.ps1` once and all samples work.

**Preserved:** Claude model references (`claude-haiku-4-5`) kept with `config["deploymentName"]` key — different provider pattern. Ollama model references (`llama3.2`) untouched. Image generation models (`dall-e-3`) untouched. MAF-Ollama-01 and MAF-MultiAgents-Factory-01 skipped (Ollama-only, no secrets needed).

**Team Impact:** Doc team update MAF setup instructions that reference `deploymentName` to use `AzureOpenAI:Deployment`. Claude-based samples still use the old `deploymentName` key — intentional divergence since Claude uses different provider config.

---

### 4. .NET 10 File-Based App Migration Pattern

**Author:** Neo (Phase 4)  
**Date:** 2026-03-04  
**Status:** Implemented  
**Scope:** 13 Tier 1 CoreSamples

**Decision:** Use file-based app format (`app.cs` with `#:package` and `#:property` directives) for simple console samples. Multi-file projects merge all .cs types into single `app.cs`. Ollama-only projects omit `#:property UserSecretsId`. All package versions preserved from original .csproj files.

**Rationale:** .NET 10 supports `dotnet run app.cs` without .csproj files, simplifying the developer experience for basic samples. Complex projects (Aspire, web, multi-project) remain as traditional .csproj projects.

**Implications:** Future simple samples should follow file-based pattern. Multi-file projects require merging all types into single `app.cs`. Aspire, web, and complex multi-project samples continue as traditional .csproj.

---

### 5. Centralized Azure Resource Setup Documentation

**Author:** Trinity (Phases 5–6)  
**Date:** 2026-03-04  
**Status:** Superseded

**Decision:** Create single comprehensive setup guide at `docs/azure-resource-setup.md` rather than scattering docs. Consolidates all paths (automated via `setup.ps1`, manual, existing accounts) in one place. Enforce `UserSecretsId = genai-beginners-dotnet` in CONTRIBUTING.MD. Default models: `gpt-5-mini` (chat), `text-embedding-3-small` (embeddings).

**Rationale:** Setup is prerequisite for all lessons, not specific to any single lesson. Single source of truth reduces documentation drift. Students configure secrets once, reuse across all samples.

**Outcome:** `docs/azure-resource-setup.md` (12,700+ words), CONTRIBUTING.MD updated with "Code Standards for .NET Samples," root README enhanced with setup guidance. Existing translation files remain independent; translators update at own pace.

**Team Impact:** Translation volunteers can update language-specific setup docs independently using English guide as source. New sample contributors must follow CONTRIBUTING.MD standards.

**Superseded by Decision 6.**

---

### 6. Lesson-Aligned Azure Setup Guide Expansion & Relocation

**Author:** Trinity (Phases 7–8)  
**Date:** 2026-03-04  
**Status:** Implemented

**Decision:** Expand Azure setup documentation from 32-line stub into comprehensive 302-line canonical guide at `01-IntroductionToGenerativeAI/setup-azure-openai.md`. Document three distinct setup paths: automated (setup.ps1), existing resources, and manual portal. Include "Want to Know More" section for script internals, cleanup procedures, and troubleshooting. Supersedes `docs/azure-resource-setup.md`; all repo references updated to new location.

**Rationale:** 
- Lesson-aligned structure: Setup guide colocated with Lesson 01 where students first configure Azure
- Single canonical reference: Reduces documentation fragmentation and drift
- Progressive disclosure: Beginners follow Path 1 (automated); experienced users use Paths 2–3
- Transparency: Documenting script internals builds student understanding

**Outcome:** 
- **New guide:** `01-IntroductionToGenerativeAI/setup-azure-openai.md` (302 lines)
- **Path 1:** Automated setup via `setup.ps1` (one command, creates resources and configures secrets)
- **Path 2:** Existing Azure resources (bring-your-own account workflow)
- **Path 3:** Manual Azure Foundry portal navigation (step-by-step screenshots/instructions)
- **Script transparency:** "Want to Know More" section documents `setup.ps1`, `cleanup.ps1`, and `setup-secrets.ps1` internals
- **References updated:** 7 locations updated (README.md × 3, lessons × 1, .github × 2, CONTRIBUTING.MD × 1)
- **Stale doc deleted:** `docs/azure-resource-setup.md` removed; no longer canonical

**Team Impact:** 
- All student-facing setup instructions now route through the lesson-based guide
- Translation team: Update `translations/<lang>/` setup sections independently
- Contributors: Reference new guide in PRs and issues

---

### 7. Default Model Consistency (Copilot User Directive)

**Author:** Bruno Capuano (via Copilot)  
**Date:** 2026-03-04  
**Status:** Documented

**Decision:** Use `gpt-5-mini` as the default LLM for all Microsoft Foundry samples across the repo, unless the sample specifically needs a different model (e.g., `dall-e-3` for image generation, `sora`/`sora-2` for video, Claude-specific samples, Ollama models for local-only samples).

**Rationale:** 
- User request from Bruno Capuano
- Ensures consistency across the entire repo
- Clear exceptions documented for special-case models

**Scope:** All lesson documentation (01–04), code samples (CoreSamples, AppsWithGenAI), and MAF READMEs.

**Preserved Exceptions:**
- Image generation: `dall-e-3`
- Video generation: `sora`, `sora-2`
- Claude-specific samples: Claude models with `config["deploymentName"]` key
- Ollama-only samples: Ollama model references (e.g., `llama3.2`, `phi4-mini`)
- Audio/Speech: `gpt-realtime`, speech models
- Other providers: No changes to non-Azure OpenAI models

**Team Impact:** 
- Any future docs or samples should use `gpt-5-mini` as the default Azure OpenAI model
- Exceptions list is final; new special-case models must be explicitly approved
- Translation team should update their localized docs (many `gpt-4o-mini` references remain in all 8 language translations)

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
