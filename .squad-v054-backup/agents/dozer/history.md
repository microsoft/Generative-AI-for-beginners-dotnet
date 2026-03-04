# Dozer — History

## Project Context
- **Project:** Generative AI for Beginners .NET — hands-on .NET course for GenAI
- **Stack:** .NET 9+, C#, MEAI, Semantic Kernel, Azure OpenAI, Ollama
- **User:** Bruno Capuano
- **Owned scope:** NuGet package upgrades, .NET framework upgrades, build verification across all projects

## Learnings

### NuGet Upgrade Cycle (Feb 2026)
- **Preview-to-GA transitions are high-impact**: Microsoft.Extensions.AI.OpenAI's graduation from preview (10.2.0-preview.1.26063.2) to GA (10.3.0) is a significant reliability milestone. Prioritize these in upgrade cycles.
- **Constraints-driven upgrades require clear documentation**: SK connector packages (InMemory, AzureAISearch, Qdrant, SqliteVec) remain on preview versions because MEAI equivalents don't exist yet. Documenting which packages are constrained and why prevents regressions in future upgrades.
- **Build verification across all projects is non-negotiable**: Spot-checking 2-3 projects misses transitive dependency issues (as discovered in PR #491). Recommend adding a "full build matrix" CI job that touches all non-deprecated projects.
- **Version consistency in Configuration packages simplifies maintenance**: Grouping Microsoft.Extensions.Configuration family upgrades (10.0.1 → 10.0.3) reduces cognitive load and makes future audits easier.
- **Major version bumps should be explicit decisions, not missed opportunities**: VectorData.Abstractions (9.7.0 → 10.0.0 available) was deferred in RAGSimple per constraints, but tracking these "deferred majors" helps prioritize future upgrade sessions.
