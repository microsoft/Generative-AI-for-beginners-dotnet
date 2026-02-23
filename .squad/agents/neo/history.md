# Neo — History

## Project Context
- **Project:** Generative AI for Beginners .NET — hands-on .NET course for GenAI
- **Stack:** .NET 9+, C#, MEAI, Semantic Kernel, Azure OpenAI, Ollama
- **User:** Bruno Capuano

## Learnings
- As of July 2025, no standalone MEAI-native NuGet packages exist for InMemory, AzureAISearch, Qdrant, or SqliteVec vector stores. The SK connector packages remain the only implementations.
- `Microsoft.SemanticKernel.Text.TextChunker` has no MEAI-native replacement. SK Core is still required for text chunking.
- `AddSqliteCollection<>()` extension method is provided by `Microsoft.SemanticKernel.Connectors.SqliteVec` with no MEAI equivalent.
- The Middleware AgentFx project has a pre-existing NU1605 package downgrade error (MEAI 10.2.0 vs 9.10.1) unrelated to connector changes.

### PR #491 Build Fix (Feb 2026)
- **Transitive dependencies must be tracked explicitly**: BasicChat-11FoundryClaude was missing explicit `Azure.AI.OpenAI` reference because SK provided it transitively. When removing SK, explicit references are not added automatically—manual audit required.
- **Verify the pattern, not just the fix**: BasicChat-05AIFoundryModels already had the correct fix applied (Azure.AI.OpenAI 2.8.0-beta.1). Use this as a template when similar issues arise in other projects.
- **Document patterns for future developers**: Created commit message guidance: when removing a major package, check (1) what transitive deps it provides, (2) does code directly reference types from those deps, (3) add explicit refs before removal.
- **Build verification is defensive**: The fix is simple (one line XML), but it protects course users from a build failure on a prominent sample. Defensive QA at this level saves support overhead.
