# DecisionAgent-01-FoundryHosted

A .NET 10 sample that routes synthetic support tickets with
**Microsoft-Decision-1**, a native Microsoft Foundry decision model. A custom
Microsoft Agent Framework `AIAgent` calls the model through the native
SystemOne endpoint and returns a typed result through the Foundry Responses
protocol. No chat model is used to make or explain the routing decision.

The native decision client is the community-maintained
[`ElBruno.AI.Decisions.Foundry`](https://www.nuget.org/packages/ElBruno.AI.Decisions.Foundry)
package. `IDecisionClient` and `ChooseAsync` are not official
Microsoft.Extensions.AI APIs. The package is isolated behind the agent, and the
public `RoutingDecision` contract is kept in its own project.

## What this sample demonstrates

- One `AIAgent` implementation runs locally and as a Foundry Hosted Agent.
- A bounded choice decision routes a ticket to `billing`, `technical-support`,
  `account-access`, or `manual-review`.
- The result keeps provider confidence separate from each option's probability.
- Usage is optional: missing usage remains `null`, while reported zero remains
  `0`.
- The answer is a deterministic confirmation suggestion, not generated
  reasoning or an action. A person must confirm the route.
- The agent accepts exactly one non-empty text message of at most 64 KiB in
  UTF-8. It rejects conversation history, system messages, and non-text input.
- Cancellation, authorization errors, and provider failures remain failures.
  They never turn into a successful `manual-review` result.
- Offline tests use synthetic decisions, fake HTTP, and fake credentials. They
  do not call Foundry or prove model accuracy or hosted identity permissions.

## Project layout

```text
DecisionAgent-01-FoundryHosted/
├── azure.yaml
├── global.json                      # Pinned .NET 10 SDK feature band
├── src/
│   ├── SupportRouting.Contracts/     # Public, provider-independent result types
│   ├── SupportRoutingAgent/          # AIAgent, native transport, Responses host
│   └── SupportRoutingCaller/         # Authenticated C# caller for a deployed agent
├── tests/
│   └── SupportRoutingAgent.Tests/    # Offline agent, transport, host, and caller tests
├── README.md
└── THIRD-PARTY-NOTICES.md
```

## Requirements

| Requirement | Purpose |
|---|---|
| .NET 10 SDK | Build the hosted agent, caller, and tests |
| Azure CLI | Local developer identity and hosted-agent caller identity |
| Azure Developer CLI with `azure.ai.agents` extension | Deploy and invoke the Hosted Agent |
| Existing Microsoft Foundry project | Hosts the agent, no new project is defined in this sample |
| Existing Microsoft-Decision-1 deployment | Native inference target, no chat model is created |
| Authorized Foundry identities | Local inference, agent deployment, agent runtime inference, and caller invocation use distinct identities |

Sign in manually with `az login` before running the local agent or caller.
Never put credentials, endpoints, or tenant-specific configuration in source
control. The project and deployment names below are placeholders.

## Endpoint and identity map

Keep the four endpoints and identities distinct:

| Value | Meaning | Used by |
|---|---|---|
| `AZURE_AI_PROJECT_ENDPOINT` | Foundry project endpoint used by `azd` to host the agent | Deployment |
| `DECISION_ENDPOINT` | Resource-root endpoint for native decision inference, such as `https://<resource>.services.ai.azure.com/` | Agent's decision client |
| `DECISION_DEPLOYMENT` | Name of the existing Microsoft-Decision-1 deployment, not a chat model | Agent's decision client |
| `AGENT_OPENAI_BASE_URL` | OpenAI-compatible base URL for the deployed Hosted Agent | C# caller |

Locally, `AzureCliCredential` represents the developer. In Foundry, the agent
uses its system-assigned managed identity for the native decision request. The
C# caller uses the caller's Azure CLI identity to invoke the agent endpoint.
An endpoint token does not itself grant permission to invoke that endpoint.

For callers using Foundry's Azure RBAC authorization scheme, the current
authorization guidance identifies **Foundry Agent Consumer** at the project or
individual-agent scope as the least-privilege built-in role for endpoint-only
callers. See [Configure agent endpoint authorization](https://learn.microsoft.com/azure/foundry/agents/how-to/configure-agent-endpoint-authorization?WT.mc_id=academic-105485-koreyst).

The agent-instance identity's native decision-inference permission must be
verified for the target Foundry resource. A prior prototype only established
that an account-scoped **Foundry User** assignment worked in one environment.
That assignment is broad and is not recommended here as a least-privilege
answer. This sample creates no role assignments and does not widen access
automatically. Confirm the currently supported inference role and narrowest
scope with the resource owner before deployment.

## Run offline tests

From this directory:

```powershell
dotnet restore .\tests\SupportRoutingAgent.Tests --locked-mode
dotnet test .\tests\SupportRoutingAgent.Tests -c Release --no-restore
```

The project files pin direct NuGet versions, and the lock files pin the
resolved dependency graphs. These tests are fully offline.

## Run the agent locally

The developer identity needs permission to invoke the existing decision
deployment. Configure the **resource root** and **deployment name**, not the
project endpoint or hosted-agent endpoint:

```powershell
$env:Decision__Endpoint = 'https://<foundry-resource>.services.ai.azure.com/'
$env:Decision__Deployment = '<existing-decision-deployment>'
$env:PORT = '8097'
dotnet run --project .\src\SupportRoutingAgent -c Release --no-launch-profile
```

The local Responses host has no application authentication. Keep the port
private, do not expose or tunnel it, and use only synthetic ticket text. The
decision transport logs HTTP status only. The Foundry host disables GenAI
message-content capture in telemetry, but telemetry settings are separate from
protocol/session storage.

In another PowerShell terminal, send a synthetic ticket:

```powershell
$body = @{
    input = 'I was charged twice for my subscription.'
    stream = $false
    store = $false
} | ConvertTo-Json

$response = Invoke-RestMethod 'http://localhost:8097/responses' `
    -Method Post -ContentType 'application/json' -Body $body
if ($response.status -ne 'completed') {
    throw "Agent response status: $($response.status)"
}
$response.output[0].content[0].text | ConvertFrom-Json
```

The returned text contains a JSON `RoutingDecision`:

```json
{
  "team": "billing",
  "confidence": 0.61,
  "probabilities": {
    "billing": 0.7,
    "technical-support": 0.1,
    "account-access": 0.1,
    "manual-review": 0.1
  },
  "usage": {
    "inputTokens": 12,
    "outputTokens": 0,
    "totalTokens": null,
    "reasoningTokens": null,
    "cachedTokens": null
  },
  "answer": "Suggested team: billing. A person should confirm the route before taking action."
}
```

This illustrates the JSON shape only. The numbers above are synthetic test
fixtures. Live labels, probabilities, confidence, and usage vary by inference
and are not guaranteed. The agent does not create, assign, or modify a support
ticket.

## Deploy to an existing Foundry project

This sample reuses an existing Foundry project and decision deployment. Its
`azure.yaml` contains no model `deployments` block or unrelated chat model. It
does not create a Foundry project or configure role assignments.

1. Sign in as a deployer who is allowed to deploy hosted agents into the chosen
   project. Install Azure CLI, Azure Developer CLI, and the agent extension if
   needed:

   ```powershell
   az login
   azd extension install azure.ai.agents
   ```

2. From this sample directory, create a fresh local `azd` environment and set
   the existing resource values:

   ```powershell
   azd env new <environment-name>
   azd env set AZURE_SUBSCRIPTION_ID '<subscription-id>'
   azd env set AZURE_LOCATION '<project-resource-location>'
   azd env set AZURE_AI_PROJECT_ENDPOINT 'https://<project>.services.ai.azure.com/api/projects/<project-name>'
   azd env set DECISION_ENDPOINT 'https://<foundry-resource>.services.ai.azure.com/'
   azd env set DECISION_DEPLOYMENT '<existing-decision-deployment>'
   azd env get-values
   azd ai project show
   ```

   Confirm that the project endpoint, native decision resource root, and
   deployment name are the intended existing values. `.azure` environment data
   is local and ignored by Git.

3. Before deployment, verify the hosted agent identity can call the decision
   deployment at the narrowest supported scope. Do not copy the prototype's
   broad account-level role assignment as a workaround. Then deploy the code
   from this directory:

   ```powershell
   azd deploy
   azd ai agent show --output json
   ```

   Review the CLI output to confirm this agent is registered in the selected
   project. Do not run `azd up` or provision a new project/model for this sample.

4. Invoke one synthetic ticket with the CLI and check the final response status:

   ```powershell
   azd ai agent invoke --new-session --new-conversation --protocol responses `
       --timeout 180 'I was charged twice for my subscription.'
   ```

   A successful HTTP exchange alone is not enough. Confirm the final Responses
   status is `completed`, then inspect the output text for the typed decision.
   Record the session ID created by this command and stop only that session:

   ```powershell
   azd ai agent sessions stop '<session-id-created-by-this-invocation>'
   ```

   Do not use blanket resource-group deletion or `azd down` against a reused
   project.

## Invoke the deployed agent from C#

The standalone caller requests an Entra token for
`https://ai.azure.com/.default`, sends `model`, `input`, `stream: false`, and
`store: false` to the agent's OpenAI-compatible `/responses` endpoint, verifies
the final status is `completed`, then deserializes the response text into the
shared `RoutingDecision` type. Non-success HTTP responses, missing output, and
`response.failed` are exceptions, not successful decisions.

Set `AGENT_OPENAI_BASE_URL` to the agent's OpenAI-compatible base URL from the
deployed agent details, and `AGENT_NAME` to the hosted agent name. Do not use the
project endpoint or decision resource root here:

```powershell
$env:AGENT_OPENAI_BASE_URL = 'https://<agent-endpoint-base-url>/'
$env:AGENT_NAME = 'support-routing'
dotnet run --project .\src\SupportRoutingCaller -c Release -- `
    'I was charged twice for my subscription.'
```

The caller uses `AzureCliCredential`; sign in as the caller identity and grant
the endpoint interaction permission at project or agent scope. The request
sets `store` to `false` and uses no previous response ID. If you instead create
a session with the `azd` CLI, stop only the session ID from that invocation.

## Costs, data handling, and publication evidence

Local inference still incurs model charges. Hosted agent execution, deployment
artifacts, and telemetry can also incur charges. Check current
[Microsoft Foundry pricing](https://azure.microsoft.com/pricing/details/ai-foundry/?WT.mc_id=academic-105485-koreyst)
before running a live deployment.

Use synthetic tickets only. Telemetry message-content capture and Responses
protocol storage are separate controls. The native transport logger records no
request body, response body, ticket text, or credentials. Do not commit `.azure`
state, tokens, private endpoints, or logs.

No live deployment or Foundry portal capture was performed while preparing this
sample. Do not treat the offline tests as live inference evidence or fabricate
screenshots. Before publication, run the sample in an authorized test project
and capture genuine portal conversation and trace views with synthetic data,
redacting tokens and personal identifiers. If Foundry does not expose a native
decision-model span, document the supported instrumentation and capture the
actual available view instead.

## Decision API evolution

This sample intentionally keeps the community decision client behind
`IDecisionClient` and keeps the result contract independent of that package.
The experimental proposals for provider-neutral decision abstractions and
interoperability are [dotnet/extensions#7795](https://github.com/dotnet/extensions/pull/7795)
and [dotnet/extensions#7796](https://github.com/dotnet/extensions/pull/7796).
They do not establish a release date or provide an official Foundry decision
provider. Recheck their status before publication; do not depend on unmerged
APIs.

See [source attribution and notices](THIRD-PARTY-NOTICES.md) for adapted code
and dependency notes.
