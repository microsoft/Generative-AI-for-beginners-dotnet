# Semantic Kernel Deprecation & NuGet Package Upgrades

**Date:** 2029-02-23
**Branch:** `dozer/sk-deprecation-nuget-upgrades`  
**Agent:** Dozer (Upgrade Engineer)

## Summary

Migration from Semantic Kernel to Microsoft.Extensions.AI (MEAI) as the standard AI abstraction layer, along with NuGet package version alignment across all projects.

## Changes

### Phase 1: SK Deprecation — Pure SK Samples Moved

11 pure Semantic Kernel projects moved to `samples/deprecated/`:

| Project | Reason |
|---------|--------|
| BasicChat-02SK | SK-only chat sample |
| BasicChat-04OllamaSK | SK + Ollama chat |
| AIFoundryLocal-01-SK-Chat | SK + Foundry Local |
| AIToolkit-01-SK-Chat | SK + AI Toolkit |
| DockerModels-01-SK-Chat | SK + Docker Models |
| RAGSimple-01SK | SK RAG with Memory plugin |
| RAGSimple-10SKOllama | SK RAG + Ollama + KernelMemory |
| RAGSimple-15Ollama-DeepSeekR1 | SK RAG + DeepSeek R1 |
| ReasoningLabs-01-SK | SK reasoning sample |
| SKFunctions01 | SK function calling |
| OpenAI-FileProcessing-Pdf-01 | SK + OpenAI PDF processing |

### Phase 2: Documentation Updates

Updated lesson docs to reference deprecated folder:

- `03-CoreGenerativeAITechniques/01-lm-completions-functions.md`
- `03-CoreGenerativeAITechniques/02-retrieval-augmented-generation.md`
- `03-CoreGenerativeAITechniques/readme.md`
- `03-CoreGenerativeAITechniques/06-LocalModelRunners.md`
- `02-SetupDevEnvironment/readme.md`
- `samples/CoreSamples/RAGSimple-02MEAIVectorsMemory/README.md`

### Phase 3: Mixed Project SK Removal

Projects that had both MEAI and SK — removed SK dependency:

- BasicChat-05AIFoundryModels
- BasicChat-11FoundryClaude

### Phase 4: NuGet Package Alignment

Aligned fragmented package versions across **74 .csproj files** with **288 version updates**:

**Key Package Upgrades (rc → GA):**

- `Microsoft.Extensions.AI` → 10.2.0 (from 10.0.0-rc.2)
- `Microsoft.Extensions.AI.OpenAI` → 10.2.0-preview.1.26063.2
- `System.Drawing.Common` → 10.0.0 GA (from rc.2)
- `Azure.Identity` → 1.18.0-beta.2
- `Azure.AI.OpenAI` → 10.2.0 (added explicit dependency to BasicChat-05AIFoundryModels, previously transitive via SK)
- `OpenAI` → 2.8.0
- `OpenTelemetry.*` → 1.14.0
- `OllamaSharp` → 5.4.12

### Phase 5: docs/changelog Structure

Created `docs/changelog/` for tracking all repo changes. Oracle (Changelog Agent) now tracks changes here as part of standard workflow.

## Impact

- **Breaking:** SK-only samples no longer in main samples path
- **Migration:** MEAI is now the sole supported AI abstraction
- **Translations:** 8 translation files still reference old paths (follow-up needed)

## Commits

| Commit | Agent | Description |
|--------|-------|-------------|
| `74b1b43` | Dozer | Move 11 pure Semantic Kernel samples to deprecated folder |
| `4de5d54` | Dozer | Update lesson docs to reference deprecated SK samples (6 English docs) |
| `cd8ef7a` | Dozer | Remove Semantic Kernel from mixed MEAI projects (BasicChat-05, BasicChat-11) |
| `930580d` | Dozer | Align NuGet package versions across 74 files (288 updates), rc→GA upgrades |
| `6ce5357` | Oracle | Update Oracle charter to track docs/changelog and notify Niobe |

## Follow-up Items

- [x] Move 11 pure SK samples to deprecated/ — *COMPLETED (Phase 1)*
- [x] Update lesson docs to reference deprecated SK samples — *COMPLETED (Phase 2, 6 docs)*
- [x] Remove SK from mixed MEAI projects (BasicChat-05, BasicChat-11) — *COMPLETED (Phase 3)*
- [x] Align NuGet packages across 74 files — *COMPLETED (Phase 4, 288 version updates)*
- [x] Create docs/changelog/ structure and archive — *COMPLETED (Phase 5)*
- [x] Update Oracle charter — *COMPLETED (Phase 5)*
- [x] Replace SK connectors in RAGSimple-02/03/04 with native clients — *DONE: SK connectors replaced with native clients (InMemory→cosine similarity, AzureAISearch→Azure.Search.Documents, Qdrant→Qdrant.Client)*
- [ ] Replace SK SqliteVec in MAF projects with MEAI VectorData — *DEFERRED: SK retained with improved TODO comments. No MEAI-native sqlite-vec or text chunker packages exist yet. Will revisit when these ship.*
- [x] Update translation files (zh, fr, pt, es, de, ja, ko, tw) — *COMPLETED: 15 files across 8 languages updated by Trinity*
- [x] Upgrade remaining NuGet packages to latest versions — *COMPLETED by Dozer: Microsoft.Extensions.AI 10.2.0→10.3.0, AI.OpenAI preview→10.3.0 GA, Configuration packages 10.0.1→10.0.3 across 79 files*
