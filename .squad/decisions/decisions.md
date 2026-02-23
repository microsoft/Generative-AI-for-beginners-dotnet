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
