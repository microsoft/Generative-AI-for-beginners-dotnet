# Squad Team

> Generative-AI-for-beginners-dotnet

## Coordinator

| Name | Role | Notes |
|------|------|-------|
| Squad | Coordinator | Routes work, enforces handoffs and reviewer gates. |

## Members

| Name | Role | Charter | Status |
|------|------|---------|--------|
| Morpheus | Lead / Architect | `.squad/agents/morpheus/charter.md` | ✅ Active |
| Tank | AI / Backend Dev | `.squad/agents/tank/charter.md` | ✅ Active |
| Trinity | Apps / Samples Dev | `.squad/agents/trinity/charter.md` | ✅ Active |
| Mouse | Tester / QA | `.squad/agents/mouse/charter.md` | ✅ Active |
| Dozer | DevRel / Docs | `.squad/agents/dozer/charter.md` | ✅ Active |
| Scribe | Session Logger | `.squad/agents/scribe/charter.md` | 📋 Silent |
| Ralph | Work Monitor | — | 🔄 Monitor |


<!-- copilot-auto-assign: false -->

| Name | Role | Charter | Status |
|------|------|---------|--------|
| @copilot | Coding Agent | — | 🤖 Coding Agent |

### Capabilities

**🟢 Good fit — auto-route when enabled:**
- Bug fixes with clear reproduction steps
- Test coverage (adding missing tests, fixing flaky tests)
- Lint/format fixes and code style cleanup
- Dependency updates and version bumps
- Small isolated features with clear specs
- Boilerplate/scaffolding generation
- Documentation fixes and README updates

**🟡 Needs review — route to @copilot but flag for squad member PR review:**
- Medium features with clear specs and acceptance criteria
- Refactoring with existing test coverage
- API endpoint additions following established patterns
- Migration scripts with well-defined schemas

**🔴 Not suitable — route to squad member instead:**
- Architecture decisions and system design
- Multi-system integration requiring coordination
- Ambiguous requirements needing clarification
- Security-critical changes (auth, encryption, access control)
- Performance-critical paths requiring benchmarking
- Changes requiring cross-team discussion

## Project Context

- **Owner:** Copilot
- **Stack:** .NET 10+, Microsoft.Extensions.AI (MEAI), Microsoft Agent Framework (MAF), Azure OpenAI, Ollama, Aspire, Blazor
- **Description:** A hands-on course teaching .NET developers to build Generative AI applications with runnable samples.
- **Project:** Generative-AI-for-beginners-dotnet
- **Created:** 2026-07-15
