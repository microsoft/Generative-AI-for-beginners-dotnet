# AzFunction-BasicChat-01

This sample shows the smallest complete path from an HTTP-triggered Azure Function to a chat model deployed in Microsoft Foundry.

Send one question:

```json
{
  "question": "What is retrieval-augmented generation?"
}
```

The Function sends that question to the configured model and returns one answer:

```json
{
  "question": "What is retrieval-augmented generation?",
  "answer": "Retrieval-augmented generation...",
  "model": "gpt-5-mini"
}
```

Each request is independent. This sample does not store conversation history. A future `AzFunction-BasicChat-02` sample can add that behavior without changing the basic Foundry connection demonstrated here.

## What this sample demonstrates

- A .NET 10 Azure Functions v4 app using the isolated worker model.
- An anonymous `POST /api/chat` HTTP trigger with a JSON request and response.
- A provider-neutral `IChatClient` from Microsoft.Extensions.AI.
- A Foundry model client created with `AzureOpenAIClient`.
- Keyless authentication with `DefaultAzureCredential`.
  - Local development uses your Azure CLI identity.
  - Azure uses the Function App's user-assigned managed identity.
- Deployment with Azure Developer CLI (`azd`) and Bicep to a Linux Flex Consumption plan.
- Least-privilege role assignment to an existing Foundry account.

## Request flow

```text
POST /api/chat
      |
      v
Azure Function -> IChatResponder -> IChatClient -> Foundry model deployment
      |
      v
JSON response
```

The Function logs the deployment name but does not log the question or answer.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0?wt.mc_id=generative-ai-dotnet)
- [Azure Functions Core Tools v4](https://learn.microsoft.com/azure/azure-functions/functions-run-local?wt.mc_id=generative-ai-dotnet)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli?wt.mc_id=generative-ai-dotnet)
- [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd?wt.mc_id=generative-ai-dotnet) for deployment
- [Azurite](https://learn.microsoft.com/azure/storage/common/storage-use-azurite?wt.mc_id=generative-ai-dotnet), or another development storage connection, for local Functions storage
- An existing Microsoft Foundry account with a deployed chat model

The deployment template expects the existing Foundry account and the new Function resources to use the same Azure subscription.

Use the Azure OpenAI-compatible endpoint shown by the model deployment's code sample. It normally has this form:

```text
https://<your-foundry-resource>.openai.azure.com/
```

The deployment name is the name you assigned when deploying the model, not necessarily the model catalog name.

## Run locally

### 1. Sign in and grant your developer identity access

```powershell
az login
```

Your signed-in identity needs the **Cognitive Services OpenAI User** role on the Foundry account. An administrator can assign it in the Azure portal or with Azure CLI:

```powershell
az role assignment create `
  --assignee "<your-user-object-id>" `
  --role "Cognitive Services OpenAI User" `
  --scope "<foundry-account-resource-id>"
```

### 2. Create local settings

From this sample folder:

```powershell
Copy-Item local.settings.json.example local.settings.json
```

Edit the two Foundry values:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "Foundry__Endpoint": "https://<your-foundry-resource>.openai.azure.com/",
    "Foundry__DeploymentName": "<your-model-deployment-name>"
  }
}
```

`local.settings.json` is ignored by Git. It contains no model key because this sample uses your Azure identity.

### 3. Start the Function

Start Azurite, then run:

```powershell
func start
```

The local endpoint is normally `http://localhost:7071/api/chat`.

### 4. Ask a question

With `curl`:

```powershell
curl.exe -X POST "http://localhost:7071/api/chat" `
  -H "Content-Type: application/json" `
  -d '{"question":"What is retrieval-augmented generation?"}'
```

Or with PowerShell:

```powershell
Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:7071/api/chat" `
  -ContentType "application/json" `
  -Body '{"question":"What is retrieval-augmented generation?"}'
```

## Deploy with `azd`

The Bicep templates create:

- A Linux Flex Consumption Function App.
- A storage account and deployment container.
- Application Insights and a Log Analytics workspace.
- A user-assigned managed identity for the Function App.
- Identity-based storage role assignments.
- A **Cognitive Services OpenAI User** role assignment on your existing Foundry account.

The templates do not create a Foundry account or deploy a model.

### 1. Sign in and create an environment

```powershell
azd auth login
azd env new
```

### 2. Configure the existing Foundry deployment

```powershell
azd env set FOUNDRY_ACCOUNT_NAME "<foundry-account-name>"
azd env set FOUNDRY_RESOURCE_GROUP_NAME "<foundry-resource-group-name>"
azd env set FOUNDRY_ENDPOINT "https://<your-foundry-resource>.openai.azure.com/"
azd env set FOUNDRY_DEPLOYMENT_NAME "<your-model-deployment-name>"
```

The deployer must be able to create role assignments on the existing Foundry account. The deployment fails explicitly if that permission is missing; it does not fall back to an API key.

### 3. Provision and deploy

```powershell
azd up
```

After deployment, append `/api/chat` to the `SERVICE_API_URI` output and send the same JSON request used locally.

### 4. Clean up

```powershell
azd down --purge
```

The `predown` hook first removes the Function identity's role assignment from the existing Foundry account. `azd` then deletes the Function resources created by the sample. It does not delete the existing Foundry account or model deployment.

## API behavior

The endpoint accepts only `POST` requests and expects `Content-Type: application/json`.

Questions are limited to 4,000 characters so the public demo endpoint has a simple, predictable bound without introducing model-specific token counting.

Invalid input returns `400 Bad Request`:

```json
{
  "error": "The question field is required."
}
```

Authentication, authorization, networking, and model-service failures remain failures. The sample does not return a placeholder answer that looks successful.

## Security note

The HTTP trigger is anonymous to keep the first learning scenario easy to call. It invokes a billable model, so do not expose this configuration as a production endpoint. Before production use, add caller authentication, authorization, rate limiting, input controls, and cost monitoring. The Function-to-Foundry connection is already keyless.

## Project structure

| Path | Purpose |
| --- | --- |
| `ChatFunction.cs` | Validates the HTTP request and maps the JSON response. |
| `FoundryChatResponder.cs` | Sends one question through `IChatClient`. |
| `Program.cs` | Configures dependency injection, credentials, and telemetry. |
| `AzFunction-BasicChat-01.Tests/` | Tests the HTTP contract without calling Azure. |
| `infra/` | Deploys the Function resources and role assignments. |
| `azure.yaml` | Connects the project and infrastructure to `azd`. |

## Deliberately out of scope

- Conversation history or session persistence
- Databases or vector stores
- Streaming responses
- Function calling or tools
- Model provisioning
- API-key authentication
- Production HTTP authentication and rate limiting

## Official documentation

- [Guide for running C# Azure Functions in the isolated worker model](https://learn.microsoft.com/azure/azure-functions/dotnet-isolated-process-guide?wt.mc_id=generative-ai-dotnet)
- [Create and deploy Azure Functions resources using Bicep](https://learn.microsoft.com/azure/azure-functions/functions-create-first-function-bicep?wt.mc_id=generative-ai-dotnet)
- [Foundry tools authentication and authorization using .NET](https://learn.microsoft.com/dotnet/ai/azure-ai-services-authentication?wt.mc_id=generative-ai-dotnet)
- [Use Azure OpenAI without keys](https://learn.microsoft.com/azure/developer/ai/keyless-connections?wt.mc_id=generative-ai-dotnet)
- [Azure.AI.OpenAI 2.x client configuration](https://learn.microsoft.com/azure/foundry-classic/openai/how-to/dotnet-migration?wt.mc_id=generative-ai-dotnet#client-configuration)
