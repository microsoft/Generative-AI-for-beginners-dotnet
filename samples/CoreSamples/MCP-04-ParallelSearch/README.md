# MCP-04 - Parallel Search MCP Server

This file-based .NET 10 console sample connects to the public
[Parallel Search MCP Server](https://docs.parallel.ai/integrations/mcp/search-mcp) at
`https://search.parallel.ai/mcp`. It discovers the server tools, searches for information
about MCP, fetches the first result, and prints source URLs and excerpt previews.
Unlike the [Microsoft Learn sample](../MCP-03-MicrosoftLearn/README.md), it demonstrates
MCP tool calls directly rather than asking a model to decide when to use tools.

The sample calls the MCP tools directly, so it does not require an AI model, Parallel account,
API key, user secrets, or environment variables. Anonymous requests use the server's fast
search mode and are subject to lower rate limits than authenticated use.

## Run it

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0?WT.mc_id=academic-105485-koreyst)
(10.0.300 or later, for the shared-source `#:include` directive), then run from this directory:

```bash
dotnet run app.cs
```

The first run restores the pinned `ModelContextProtocol` package from NuGet. The sample prints
the available tools, up to three search results, and the fetched page, with a source URL and
up to 600 characters of the first excerpt for each result. Results vary as web content changes.
The whole MCP operation is bounded to 90 seconds.

## What it demonstrates

1. Connect to a remote MCP server over streamable HTTP without authentication.
2. Discover available tools with `ListToolsAsync()`.
3. Call `web_search` with its required objective and search query arguments.
4. Read the source URL from the result's JSON `url` field, not from incidental links in excerpts.
5. Pass that URL and the same session ID to `web_fetch`, then display a page excerpt.

`ParallelSearchResults.cs` handles the service's JSON response and is shared with the offline
regression tests. The sample fails with a nonzero exit code for tool errors, page-fetch errors,
missing results, invalid source URLs, or missing excerpts rather than reporting success.

## Free-tier limits and data flow

The sample requires an internet connection and sends its fixed public search objective, queries,
session ID, and selected result URL to Parallel. Do not replace them with sensitive information.
Returned page text is untrusted external content, not instructions to execute.

According to the [Search MCP documentation](https://docs.parallel.ai/integrations/mcp/search-mcp),
anonymous requests use `fast` mode with server-managed settings. Search overrides are ignored
without authentication. The free service is intended for exploration and light use, not an
availability guarantee; rate limits and service behavior can change. The documentation does
not specify a fixed anonymous request quota, so this sample does not promise one.

If you receive an HTTP 429 or a rate-limit tool error, wait and retry later. Check network access
and the service status for connection or timeout errors. Higher limits require an account and
authentication, which are intentionally outside this sample. Do not use `/mcp-oauth`, which
requires authentication, for the anonymous run.

## Validate

The default PowerShell script builds in Release and runs offline regression tests without
calling Parallel:

```powershell
.\validate.ps1
```

Add `-Live` for an opt-in anonymous end-to-end check. This makes one search and one fetch call,
and asserts that both print source URLs and excerpts:

```powershell
.\validate.ps1 -Live
```

Without PowerShell, run the same offline checks with the .NET CLI:

```bash
dotnet build app.cs --configuration Release
dotnet test ../MCP-04-ParallelSearch.Tests/MCP-04-ParallelSearch.Tests.csproj --configuration Release
```

Normal CI compiles the file-based sample and runs the offline tests. It does not call the live
service or consume its free-tier quota.
