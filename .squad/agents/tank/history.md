# Tank — History

## Project Context
- **Project:** Generative AI for Beginners .NET — hands-on .NET course for GenAI
- **Stack:** .NET 9+, C#, MEAI, Semantic Kernel, Azure OpenAI, Ollama
- **User:** Bruno Capuano

## Learnings

### PR #491 Build Review (Feb 2026)
- **Transitive dependency tracking is critical**: When a major package (SK) is removed, audit all affected projects for direct type usage. BasicChat-11FoundryClaude used `Azure.AI.OpenAI` types but lacked explicit package reference—was relying on transitive from SK.
- **Spot-check builds are insufficient**: Tank checked 6 projects; if only checked 5, build failure would have shipped. Future: CI must build ALL non-deprecated samples, not just a selection.
- **Pattern matching reduces errors**: BasicChat-05AIFoundryModels had the fix applied correctly (Azure.AI.OpenAI 2.8.0-beta.1 added); BasicChat-11 was missed. Pre-review audits of "which projects use Azure.AI.*" would prevent this.
- **NuGet version consistency is strong**: Across 5 projects tested, Microsoft.Extensions.AI 10.2.0 was uniform—version management is solid, just need explicit reference tracking.
