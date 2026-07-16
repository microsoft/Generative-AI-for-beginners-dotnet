# Project Context

- **Owner:** Copilot
- **Project:** Generative AI for Beginners .NET — a hands-on course teaching .NET developers to build Generative AI applications with runnable samples.
- **Stack:** .NET 10+, Microsoft.Extensions.AI (MEAI), Microsoft Agent Framework (MAF), Azure OpenAI, Ollama, Aspire, Blazor
- **Created:** 2026-07-15

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
- Lessons live in numbered folders `01-`–`05-`; all code samples live under `samples/` (CoreSamples, MAF, AppsWithGenAI, PracticalSamples).
- CI-validated solutions: CoreSamples, Aspire.MCP.Sample, HFMCP.GenImage, SpaceAINet, MAF-Demos.
- Never hardcode API keys — use `dotnet user-secrets` or environment variables. AI providers abstracted behind service classes.
