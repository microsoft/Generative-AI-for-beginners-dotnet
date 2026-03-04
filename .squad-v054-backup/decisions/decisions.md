# Decisions Log

## Decision: SK Connector Packages Cannot Be Replaced Yet

**Author:** Neo | **Date:** 2025-07-17

### Context
Investigated replacing all Semantic Kernel connector NuGet packages with MEAI (Microsoft.Extensions.VectorData) equivalents across 8 projects.

### Decision
**Keep all SK connector packages** and document them with TODO comments. No MEAI-native replacement packages exist on NuGet yet:

| SK Package | MEAI Replacement | Status |
|---|---|---|
| `Microsoft.SemanticKernel.Connectors.InMemory` | `Microsoft.Extensions.VectorData.InMemory` | ❌ Does not exist |
| `Microsoft.SemanticKernel.Connectors.AzureAISearch` | `Microsoft.Extensions.VectorData.AzureAISearch` | ❌ Does not exist |
| `Microsoft.SemanticKernel.Connectors.Qdrant` | `Microsoft.Extensions.VectorData.Qdrant` | ❌ Does not exist |
| `Microsoft.SemanticKernel.Connectors.SqliteVec` | `Microsoft.Extensions.VectorData.SqliteVec` | ❌ Does not exist |
| `Microsoft.SemanticKernel.Core` (TextChunker) | N/A | ❌ No MEAI text chunker |

### What Was Done
- Added XML/code comments to all 8 csproj files and 8 source files marking the SK dependencies with TODOs
- Verified all projects still build (except pre-existing NU1605 in Middleware project)

### Action Items
- Revisit when MEAI vector store provider packages are published to NuGet
- Monitor https://devblogs.microsoft.com/dotnet/ for VectorData provider announcements

---

## Decision: NuGet Package Upgrades (Round 2)

**Author:** Dozer  
**Date:** 2025-07-18

### Packages Upgraded

| Package | Old Version | New Version | Files Updated |
|---------|------------|-------------|---------------|
| Microsoft.Extensions.AI | 10.2.0 | 10.3.0 | 41 |
| Microsoft.Extensions.AI.OpenAI | 10.2.0-preview.1.26063.2 | 10.3.0 | 29 |
| Microsoft.Extensions.Configuration | 10.0.1 | 10.0.3 | 9 |
| Microsoft.Extensions.Configuration.Json | 10.0.1 | 10.0.3 | 7 |
| Microsoft.Extensions.Configuration.UserSecrets | 10.0.1 | 10.0.3 | 49 |

**Total:** 79 files changed across 5 packages.

### Packages Left Unchanged (by design)

- **Microsoft.Extensions.AI.AzureAIInference** (10.0.0-preview.1.25559.3) — no GA version available
- **Microsoft.SemanticKernel.Connectors.InMemory** (1.66.0-preview) — SK package in RAGSimple, left as-is per constraints
- **Microsoft.Extensions.VectorData.Abstractions** (9.7.0 → 10.0.0 available) — major version bump in RAGSimple project, left as-is per constraints
- **Microsoft.Agents.AI / Microsoft.Agents.AI.OpenAI** (preview) — no GA available

### Notable

- Microsoft.Extensions.AI.OpenAI moved from preview to GA (10.3.0) — a significant improvement.

### Build Verification

All 5 representative projects built successfully with 0 errors.

---

## PR #491 Review Cycle: SK Deprecation & NuGet Upgrades

**Status:** ✅ FIXES APPLIED (All blockers resolved)

### Phase 1 Findings Summary

**Oracle (Changelog Review):** ✅ 95% accurate changelog. Root README.md "What's New" gap accepted as non-blocking.

**Tank (Build Verification):** 🔴 CRITICAL — BasicChat-11FoundryClaude missing Azure.AI.OpenAI package.

**Trinity (Documentation Review):** 🟡 2 minor issues:
1. Line 59 in `02-retrieval-augmented-generation.md`: Ambiguous "Ollama (deprecated SK samples)" wording
2. Lines 79–80 in `10-WhatsNew/readme.md`: Duplicate lines + jumbled June 2025 section structure

**Morpheus (Architectural Review):** 🔴 BLOCKED — Build failure is critical; doc fixes required before merge.

### Phase 2 Fixes Applied

**Neo (Developer):** ✅ Commit 343b6b8 — Added explicit `Azure.AI.OpenAI` dependency to BasicChat-11FoundryClaude.csproj

**Coordinator (Documentation):** ✅ Commit 11fe522 — Fixed ambiguous Ollama language and removed duplicate lines in What's New section

### Verified Resolution
- BasicChat-11FoundryClaude now builds successfully
- Documentation clarity improved with corrected language and structure
- All 3 blocking issues resolved

### Lessons Learned
1. Transitive dependency tracking is critical when removing major packages (SK)
2. Build verification must include all non-deprecated projects, not spot-checks
3. Documentation reviews should catch copy-paste errors in high-visibility sections
4. CI should enforce: "All projects with `using Azure.AI.*` must have explicit NuGet references"
