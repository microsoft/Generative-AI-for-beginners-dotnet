# Dozer — History

## Project Context
- **Project:** Generative AI for Beginners .NET — hands-on .NET course for GenAI
- **Stack:** .NET 10+, C#, MEAI, Semantic Kernel, Azure OpenAI, Ollama
- **User:** Bruno Capuano
- **Owned scope:** NuGet package upgrades, .NET framework upgrades, build verification across all projects

## Learnings

### NuGet Upgrade Cycle (Feb 2026)
- **Preview-to-GA transitions are high-impact**: Microsoft.Extensions.AI.OpenAI's graduation from preview (10.2.0-preview.1.26063.2) to GA (10.3.0) is a significant reliability milestone. Prioritize these in upgrade cycles.
- **Constraints-driven upgrades require clear documentation**: SK connector packages (InMemory, AzureAISearch, Qdrant, SqliteVec) remain on preview versions because MEAI equivalents don't exist yet. Documenting which packages are constrained and why prevents regressions in future upgrades.
- **Build verification across all projects is non-negotiable**: Spot-checking 2-3 projects misses transitive dependency issues (as discovered in PR #491). Recommend adding a "full build matrix" CI job that touches all non-deprecated projects.
- **Version consistency in Configuration packages simplifies maintenance**: Grouping Microsoft.Extensions.Configuration family upgrades (10.0.1 → 10.0.3) reduces cognitive load and makes future audits easier.
- **Major version bumps should be explicit decisions, not missed opportunities**: VectorData.Abstractions (9.7.0 → 10.0.0 available) was deferred in RAGSimple per constraints, but tracking these "deferred majors" helps prioritize future upgrade sessions.

### MAF Package Version Alignment (Apr 2026)
- **Aligned 5 packages across 22 MAF .csproj files** — all builds green (0 errors):
  - PdfPig: `0.1.14-alpha` → `0.1.13` (stable release, 1 project)
  - Aspire.Azure.AI.OpenAI: `9.5.2-preview` → `13.1.0-preview.1.25616.3` (3 projects)
  - Azure.AI.OpenAI: `2.8.0-beta.1` → `2.9.0-beta.1` (14 projects)
  - Azure.Identity: `1.18.0` → `1.20.0` (6 projects)
  - OpenAI SDK: `2.9.1` → `2.10.0` (10 projects; MAF01 confirmed to not reference OpenAI directly)
- **Key takeaway**: Alpha/pre-release NuGet artifacts (e.g. PdfPig 0.1.14-alpha) should always be flagged and pinned to a stable release. Consistency across the MAF solution prevents "works on my machine" issues when different projects resolve different transitive versions.
