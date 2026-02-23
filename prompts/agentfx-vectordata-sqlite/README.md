# AgentFx VectorData Sqlite — External Library Prompt

## Status

🟢 **DONE** — All 5 AgentFx projects now use `ElBruno.Connectors.SqliteVec` instead of `Microsoft.SemanticKernel.Connectors.SqliteVec` and `Microsoft.SemanticKernel.Core`.

## Goal

Create an external NuGet library (e.g., `ElBruno.VectorData.Sqlite` or similar) that provides a drop-in replacement for SK's SqliteVec connector, using only MEAI abstractions + native sqlite-vec.

Once published, the 5 AgentFx projects will replace:
- `Microsoft.SemanticKernel.Connectors.SqliteVec` → the new library
- `Microsoft.SemanticKernel.Core` → a simple `TextSplitter` utility (included in the library or inline)

## Affected Projects

| Project | TFM | SK Packages | DI Call |
|---------|-----|-------------|---------|
| AgentFx-AIWebChatApp-Simple | net9.0 | SK.Core 1.71.0, SK.Connectors.SqliteVec 1.68.0-preview | `AddSqliteCollection<string, T>("data-chatapp20-*", connStr)` |
| AgentFx-AIWebChatApp-Middleware | net9.0 | same | same |
| AgentFx-AIWebChatApp-MutliAgent | net9.0 | same | same |
| AgentFx-AIWebChatApp-Persisting | net9.0 | same | `AddSqliteCollection<string, T>("data-chatapp21-*", connStr)` |
| AgentFx-AIWebChatApp-AG-UI | net10.0 | same | `AddSqliteCollection<string, T>("data-agentfx-*", connStr)` |

## Prompt File

See [PROMPT.md](./PROMPT.md) for a comprehensive prompt to use when building the external library.
