# Project Context

- **Owner:** Copilot
- **Project:** Generative AI for Beginners .NET — a hands-on course teaching .NET developers to build Generative AI applications with runnable samples.
- **Stack:** .NET 10+, Microsoft.Extensions.AI (MEAI), Microsoft Agent Framework (MAF), Azure OpenAI, Ollama, Aspire, Blazor
- **Created:** 2026-07-15

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
- No automated unit/integration test suite yet — validation is via CI build verification + manual sample runs.
- Required before commit: `dotnet format`, `dotnet build`, and `dotnet format --verify-no-changes` where linting is enabled.
- CI builds these solutions in Release: CoreSamples, Aspire.MCP.Sample, HFMCP.GenImage, SpaceAINet, MAF-Demos.
