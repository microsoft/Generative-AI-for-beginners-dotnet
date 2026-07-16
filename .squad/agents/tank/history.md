# Project Context

- **Owner:** Copilot
- **Project:** Generative AI for Beginners .NET — a hands-on course teaching .NET developers to build Generative AI applications with runnable samples.
- **Stack:** .NET 10+, Microsoft.Extensions.AI (MEAI), Microsoft Agent Framework (MAF), Azure OpenAI, Ollama, Aspire, Blazor
- **Created:** 2026-07-15

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
- Core samples: `samples/CoreSamples/CoreGenerativeAITechniques.sln`; MAF samples: `samples/MAF/MAF-Demos.slnx` (25+ projects).
- Ollama needs the model pulled (e.g. `ollama pull phi4-mini`, `ollama pull all-minilm`) and the server at `http://localhost:11434`.
- Azure secrets: `AZURE_OPENAI_ENDPOINT`, `AZURE_OPENAI_MODEL`, `AZURE_OPENAI_APIKEY` via `dotnet user-secrets`.
