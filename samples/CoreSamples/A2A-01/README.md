# A2A-01 — Minimal Agent-to-Agent (A2A) in one console run

This sample shows the **[Agent-to-Agent (A2A) protocol](https://a2a-protocol.org/latest/)**
end to end inside a single process. It is the natural way to *close* a talk on agents:
after building agents and giving them tools, A2A is how agents talk to **other agents** —
even ones written in a different language or framework. It's the "HTTP of agent communication."

## What it shows

A single console app plays **both** roles:

1. **Server** — builds a `writer-agent` (Azure OpenAI) and exposes it over A2A with
   `app.MapA2A(writerAgent, "/a2a/writer-agent")`.
2. **Client** — connects to that same endpoint with an `A2AClient`, wraps it as a standard
   `AIAgent`, and calls `RunAsync(...)`. The client never references the agent's
   implementation — only its A2A endpoint.

```text
[ A2AClient ] --(A2A protocol)--> [ /a2a/writer-agent ] --> [ writer-agent (Azure OpenAI) ]
```

## Run it

Set the Azure OpenAI endpoint and deployment (keyless auth via Azure CLI):

```bash
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-endpoint>.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:Deployment" "gpt-5-mini"
az login
dotnet run
```

> The A2A packages used here are **preview** (`Microsoft.Agents.AI.A2A`,
> `Microsoft.Agents.AI.Hosting.A2A.AspNetCore`). The sample uses a fixed local URL
> (`http://localhost:5099`) so the in-process client can reach the hosted agent.

## Learn more

- [Agent-to-Agent (A2A) with the Microsoft Agent Framework](https://learn.microsoft.com/agent-framework/agents/providers/agent-to-agent)
- [Hosting agents over A2A](https://learn.microsoft.com/agent-framework/hosting/agent-to-agent)
