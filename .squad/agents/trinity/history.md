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

### Translation Maintenance Post-Deprecation (Feb 2026)
- **8-language translation updates require a synchronization checklist**: Manual updates across ES, ZH-CN, FR, JA, KO, DE, PT-BR, IT are prone to drift if not tracked methodically. Recommend creating a "translation sync" template for future deprecations.
- **Terminology consistency across languages is more important than literal translation**: "Archived Semantic Kernel RAG examples" works better than locale-specific phrasings because it aligns with English docs and reduces confusion for bilingual learners.
- **Cultural nuance matters for beginners**: Japanese and Korean translations require slightly different deprecation phrasing due to language structure. Always review locale-specific translations with native speakers or established cultural guidelines.
- **15-file updates across 8 languages is a 1-person workload**: Trinity completed this in a single session. Document this scope as a "standard deprecation task size" for planning future translation work.
