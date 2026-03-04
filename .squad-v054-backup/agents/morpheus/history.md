# Morpheus — History

## Project Context
- **Project:** Generative AI for Beginners .NET — hands-on .NET course for GenAI
- **Stack:** .NET 9+, C#, MEAI, Semantic Kernel, Azure OpenAI, Ollama
- **User:** Bruno Capuano

## Learnings

### PR #491 Architectural Review (Feb 2026)
- **Build failures are architectural blockers, not cosmetic issues**: BasicChat-11FoundryClaude's missing package is a systemic issue that reveals transitive dependency tracking gaps. Critical issues must block merge; minor issues can be conditional.
- **Phase 1 (Oracle + Tank + Trinity) → Phase 2 (Morpheus synthesis) is effective**: Morpheus's role as orchestrator synthesizing findings from three specialists ensures no single-specialist blind spots. This cascade model should be standard for major PRs.
- **Transitive dependency tracking needs CI enforcement**: Future deprecations should include a CI rule: "All projects with `using Azure.AI.*` must have explicit NuGet references." Prevent the error at source.
- **Documentation gaps are acceptable by design**: Root README.md lacking SK deprecation entry is fine because detailed info is in authoritative `10-WhatsNew/readme.md`. Clear ownership of "what goes where" in docs prevents redundancy.
