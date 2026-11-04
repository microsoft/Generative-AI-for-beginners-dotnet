#!/usr/bin/env dotnet
#:package ModelContextProtocol@2.1.0
#:property TargetFramework=net10.0
#:include ParallelSearchResults.cs

using ModelContextProtocol.Client;

const string searchObjective = "Find an introduction to the Model Context Protocol.";
string[] searchQueries = ["Model Context Protocol introduction", "MCP specification overview"];
var sessionId = Guid.NewGuid().ToString();
using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(90));
var cancellationToken = timeout.Token;

var transport = new HttpClientTransport(new HttpClientTransportOptions
{
    Name = "Parallel Search MCP",
    Endpoint = new Uri("https://search.parallel.ai/mcp")
});

await using var mcpClient = await McpClient.CreateAsync(transport, cancellationToken: cancellationToken);
var tools = await mcpClient.ListToolsAsync(cancellationToken: cancellationToken);

Console.WriteLine("Connected to Parallel Search MCP. Available tools:");
foreach (var tool in tools)
{
    Console.WriteLine($"  - {tool.Name}");
}

if (!tools.Any(tool => tool.Name == "web_search") ||
    !tools.Any(tool => tool.Name == "web_fetch"))
{
    throw new InvalidOperationException(
        "The Parallel Search MCP server must expose web_search and web_fetch.");
}

Console.WriteLine();
Console.WriteLine("Searching the web...");

var searchResult = await mcpClient.CallToolAsync(
    "web_search",
    new Dictionary<string, object?>
    {
        ["objective"] = searchObjective,
        ["search_queries"] = searchQueries,
        ["session_id"] = sessionId
    },
    cancellationToken: cancellationToken);

var searchPages = ParallelSearchResults.Read(searchResult, "web_search");
Console.WriteLine("Search results:");
ParallelSearchResults.Write(Console.Out, searchPages.Take(3));

var resultUri = searchPages[0].Source;
Console.WriteLine($"Fetching the first result: {resultUri}");
Console.WriteLine();

var fetchResult = await mcpClient.CallToolAsync(
    "web_fetch",
    new Dictionary<string, object?>
    {
        ["urls"] = new[] { resultUri.ToString() },
        ["objective"] = "Return a concise overview and useful excerpts.",
        ["search_queries"] = searchQueries,
        ["session_id"] = sessionId
    },
    cancellationToken: cancellationToken);

var fetchedPages = ParallelSearchResults.Read(fetchResult, "web_fetch");
Console.WriteLine("Fetched page:");
ParallelSearchResults.Write(Console.Out, fetchedPages);
