# Project Context

- **Owner:** Copilot
- **Project:** Generative AI for Beginners .NET — a hands-on course teaching .NET developers to build Generative AI applications with runnable samples.
- **Stack:** .NET 10+, Microsoft.Extensions.AI (MEAI), Microsoft Agent Framework (MAF), Azure OpenAI, Ollama, Aspire, Blazor
- **Created:** 2026-07-15

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
- SpaceAINet: `samples/AppsWithGenAI/SpaceAINet/` — AI Space Battle game; in-game keys toggle Azure (A) / Ollama (O), FPS (F), screenshot (S).
- HFMCP.GenImage: image generation Aspire app (`HFMCP.GenImage.sln`); Aspire MCP sample under `samples/PracticalSamples/src` run via `McpSample.AppHost`.
- AI logic must stay behind service classes — never call models directly from UI or game logic.
