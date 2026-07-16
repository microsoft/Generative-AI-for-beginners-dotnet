# MCP-03 — Microsoft Learn MCP Server (C# MCP SDK)

This sample uses the **C# MCP SDK** (`ModelContextProtocol`) to connect to the public
**[Microsoft Learn MCP Server](https://learn.microsoft.com/training/support/mcp)** and lets
an Azure OpenAI model call its documentation tools through
**Microsoft.Extensions.AI** function invocation.

The Microsoft Learn MCP Server is a public, **keyless**, streamable-HTTP MCP server at
`https://learn.microsoft.com/api/mcp`. It exposes tools such as:

- `microsoft_docs_search` — search official Microsoft/Azure documentation
- `microsoft_docs_fetch` — fetch a full docs page as Markdown
- `microsoft_code_sample_search` — find official code samples

## What it shows

The sample asks the **same question twice** to make the value of MCP obvious:

```
What is the latest version of Microsoft Agent Framework for C#?
```

1. **BEFORE — no MCP:** the model answers only from its frozen training knowledge, so
   for a fast-moving topic the answer is often stale or vague.
2. **AFTER — with the Microsoft Learn MCP Server:** the model calls the Learn tools and
   answers from the live documentation, including an up-to-date version and a docs link.

Under the hood:

1. Create an `McpClient` over an HTTP transport to the Learn MCP endpoint.
2. Discover the available tools with `ListToolsAsync()`.
3. Pass those tools into `ChatOptions` and enable `UseFunctionInvocation()`.
4. The model decides when to call the Learn tools to ground its answer.

## Run it

Set the Azure OpenAI endpoint and deployment (keyless auth via Azure CLI):

```bash
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-endpoint>.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:Deployment" "gpt-5-mini"
az login
dotnet run
```

> No API key or token is needed for the Microsoft Learn MCP Server itself.
