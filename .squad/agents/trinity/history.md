# Trinity — History

## Project Context
- **Project:** Generative AI for Beginners .NET — hands-on .NET course for GenAI
- **Stack:** .NET 9+, C#, MEAI, Semantic Kernel, Azure OpenAI, Ollama
- **User:** Bruno Capuano

## Learnings

### PR #491 Documentation Review (Feb 2026)
- **Ambiguous terminology in beginner docs is high-risk**: "Ollama (deprecated SK samples)" creates confusion for learners—does Ollama deprecate SK, or are SK samples deprecated? Clarifying language is essential in a course that targets beginners.
- **Copy-paste errors in What's New sections reduce trust**: Duplicate lines in high-visibility changelog sections signal carelessness and hurt professional credibility. QA should include a "uniqueness scan" for What's New and changelog entries.
- **Deprecation messaging is well-standardized here**: Strikethrough + "(Deprecated)" + links to deprecated folder + MEAI alternative is a repeatable pattern that works across 8 files. Recommend this pattern for future deprecations.
- **Link integrity is solid**: All 12 verified links functional (deprecated folder, setup guides, alternative samples). The repo's documentation infrastructure is mature and reliable.
