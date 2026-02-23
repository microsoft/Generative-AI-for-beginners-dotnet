# Neo — History

## Project Context
- **Project:** Generative AI for Beginners .NET — hands-on .NET course for GenAI
- **Stack:** .NET 9+, C#, MEAI, Semantic Kernel, Azure OpenAI, Ollama
- **User:** Bruno Capuano

## Learnings
- As of July 2025, no standalone MEAI-native NuGet packages exist for InMemory, AzureAISearch, Qdrant, or SqliteVec vector stores. The SK connector packages remain the only implementations.
- `Microsoft.SemanticKernel.Text.TextChunker` has no MEAI-native replacement. SK Core is still required for text chunking.
- `AddSqliteCollection<>()` extension method is provided by `Microsoft.SemanticKernel.Connectors.SqliteVec` with no MEAI equivalent.
- The Middleware AgentFx project has a pre-existing NU1605 package downgrade error (MEAI 10.2.0 vs 9.10.1) unrelated to connector changes.
