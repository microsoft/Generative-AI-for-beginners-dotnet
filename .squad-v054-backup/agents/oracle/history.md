# Oracle — History

## Project Context
- **Project:** Generative AI for Beginners .NET — hands-on .NET course for GenAI
- **Stack:** .NET 9+, C#, MEAI, Semantic Kernel, Azure OpenAI, Ollama
- **User:** Bruno Capuano
- **Owned files:** Root README.md (What's New section), 10-WhatsNew/readme.md

## Learnings

### SK Deprecation Cycle (Feb 2026)
- **Big migrations are _multi-phase_**: SK deprecation required 5 phases (move samples → update docs → remove from mixed projects → align NuGet → create changelog archive)
- **Changelog structure matters**: Created `docs/changelog/` to track all major repo changes persistently. Each changelog doc now includes phases, impacts, commits, and follow-up items.
- **Version alignment is tedious but essential**: 74 .csproj files × multiple package upgrades = 288 total changes. Pre-planning file lists by project structure helps.
- **Mixed projects (MEAI + SK) complicate deprecation**: BasicChat-05 and BasicChat-11 needed surgical SK removal while keeping MEAI — plan for these early.
- **Translation lag is normal**: 8 localized docs still reference old paths; this is a follow-up that can be batched later.
- **Key insight for changelog entries**: Include commit SHAs, agent names, and explicit counts (11 samples, 74 files, 288 updates) so future readers understand scope without digging into git.

### PR #491 Review Cycle (Feb 2026)
- **Changelog accuracy 95% is acceptable**: Root README.md "What's New" gap is acceptable by design—detailed entry correctly placed in authoritative `10-WhatsNew/readme.md`. Optional follow-up PR can add brief mention to root README.
- **Multi-agent reviews catch systemic issues**: Oracle's changelog accuracy check alone would have missed Tank's build failure and Trinity's doc ambiguities. Phase 1 (Oracle + Tank + Trinity) + Phase 2 (Morpheus synthesis + Neo/Coordinator fixes) is the right structure.
