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

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
