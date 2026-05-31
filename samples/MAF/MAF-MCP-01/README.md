# MAF-MCP-01 — A MAF agent that uses MCP tools

This sample shows a **Microsoft Agent Framework (MAF)** `AIAgent` that uses **MCP tools**
from the public **[Microsoft Learn MCP Server](https://learn.microsoft.com/training/support/mcp)**.

The Microsoft Learn MCP Server is a public, **keyless**, streamable-HTTP MCP server at
`https://learn.microsoft.com/api/mcp`. It exposes documentation tools such as:

- `microsoft_docs_search` — search official Microsoft/Azure documentation
- `microsoft_docs_fetch` — fetch a full docs page as Markdown
- `microsoft_code_sample_search` — find official code samples

## What it shows

1. Connect to the Learn MCP Server with `McpClient` over an HTTP transport.
2. Discover the tools with `ListToolsAsync()`.
3. Hand those tools to a MAF agent: `chatClient.AsAIAgent(name, instructions, tools: [.. tools])`.
4. Call `agent.RunAsync(...)` — the agent decides when to invoke the MCP tools and
   grounds its answer in the live Microsoft Learn docs (with a docs link).

Unlike a raw chat + tools call, the **agent owns the tools** and runs the tool loop itself,
which is the building block for tool-using and multi-agent scenarios.

## Run it

Set the Azure OpenAI endpoint and deployment (keyless auth via Azure CLI):

```bash
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-endpoint>.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:Deployment" "gpt-5-mini"
az login
dotnet run
```

> No API key or token is needed for the Microsoft Learn MCP Server itself.

## Learn more

- [Microsoft Agent Framework](https://learn.microsoft.com/agent-framework/)
- [Model Context Protocol in .NET](https://learn.microsoft.com/dotnet/ai/get-started-mcp)
