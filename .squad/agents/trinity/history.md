# Trinity — History

## Project Context
- **Project:** Generative AI for Beginners .NET — hands-on .NET course for GenAI
- **Stack:** .NET 9+, C#, MEAI, Semantic Kernel, Azure OpenAI, Ollama
- **User:** Bruno Capuano

## Learnings

### PR #491 Documentation Review (Feb 2026)
- **Ambiguous terminology in beginner docs is high-risk**: "Ollama (deprecated SK samples)" creates confusion for learners—does Ollama deprecate SK, or are SK samples deprecated? Clarifying language is essential in a course that targets beginners.
- **Copy-paste errors in What's New sections reduce trust**: Duplicate lines in high-visibility changelog sections signal carelessness and hurt professional credibility. QA should include a "uniqueness scan" for What's New and changelog entries.
- **Deprecation messaging is well-standardized here**: Strikethrough + "(Deprecated)" + links to deprecated folder + MEAI alternative is a repeatable pattern that works across 8 files. Recommend this pattern for future deprecations.
- **Link integrity is solid**: All 12 verified links functional (deprecated folder, setup guides, alternative samples). The repo's documentation infrastructure is mature and reliable.

### Translation Maintenance Post-Deprecation (Feb 2026)
- **8-language translation updates require a synchronization checklist**: Manual updates across ES, ZH-CN, FR, JA, KO, DE, PT-BR, IT are prone to drift if not tracked methodically. Recommend creating a "translation sync" template for future deprecations.
- **Terminology consistency across languages is more important than literal translation**: "Archived Semantic Kernel RAG examples" works better than locale-specific phrasings because it aligns with English docs and reduces confusion for bilingual learners.
- **Cultural nuance matters for beginners**: Japanese and Korean translations require slightly different deprecation phrasing due to language structure. Always review locale-specific translations with native speakers or established cultural guidelines.
- **15-file updates across 8 languages is a 1-person workload**: Trinity completed this in a single session. Document this scope as a "standard deprecation task size" for planning future translation work.

### Phase 5.2 + Phase 6 — Azure Resource Setup Documentation (Mar 2026)
**Documents Created/Updated:**
1. **`docs/azure-resource-setup.md`** (NEW) — Comprehensive 12,700+ word guide covering:
   - Automated setup via `setup.ps1` with step-by-step walkthrough
   - Manual setup options for existing Azure accounts
   - Configuration details: shared `UserSecretsId = genai-beginners-dotnet`, model deployments (gpt-5-mini, text-embedding-3-small)
   - Specialized secrets for AI Foundry, Azure AI Search (RAG), Azure Speech
   - Cost implications and pricing context
   - Customization (region, model selection, quota increases)
   - Resource cleanup via `cleanup.ps1`
   - Troubleshooting section with 6 common issues and solutions
   - Accessibility: Beginner-friendly, clear copy-paste commands, no jargon

2. **`CONTRIBUTING.MD`** (UPDATED) — Added new "Code Standards for .NET Samples" section:
   - Shared User Secrets ID convention (`genai-beginners-dotnet`)
   - When adding samples: use the shared secrets ID
   - File-based app pattern for simple console samples (.NET 10)
   - Configuration best practices (never hardcode model names)
   - Default deployment references: `config["AzureOpenAI:Deployment"] ?? "gpt-5-mini"`

3. **`README.md`** (UPDATED) — Root README enhancements:
   - Added `azd` CLI to prerequisites
   - New "Quick Start with Azure" section linking to `setup.ps1`
   - New "Manual Setup for Existing Azure Accounts" section with direct dotnet user-secrets example
   - New "Resource Cleanup" section referencing `cleanup.ps1`
   - All sections preserve existing content (no removals)

**Key Decisions Made:**
- **Centralized setup guide:** `docs/azure-resource-setup.md` is single source of truth for Azure setup
- **Beginner focus:** All docs use plain language, concrete examples, copy-paste commands
- **Shared secrets convention:** All samples use `genai-beginners-dotnet` ID (enforced via CONTRIBUTING.MD)
- **Model standardization:** gpt-5-mini for chat, text-embedding-3-small for embeddings (documented in azure-resource-setup.md)
- **Translation scope:** Setup docs reference `docs/azure-resource-setup.md` (English-only guide); existing translation structure for lesson 02 getting-started files preserved for future updates
