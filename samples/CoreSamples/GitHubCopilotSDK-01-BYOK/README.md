# GitHub Copilot SDK — BYOK with Microsoft Foundry

This .NET 10 console sample uses the GitHub Copilot SDK with a model hosted by
Microsoft Foundry or Azure OpenAI. It demonstrates a minimal BYOK (Bring Your Own Key)
setup:

- `ProviderConfig` routes inference to an OpenAI-compatible endpoint.
- `AzureCliCredential` is used by default after `az login`.
- An optional API key switches authentication to the `api-key` header.
- The sample creates one Copilot SDK session and sends one question.

## Prerequisites

- .NET 10 SDK
- GitHub Copilot CLI available on `PATH` (`copilot --version`)
- Azure CLI (only for the Entra ID authentication mode)
- A model deployment in Microsoft Foundry or Azure OpenAI

## Configuration

This sample uses the repository's shared user-secrets keys:

- `AzureOpenAI:Endpoint`
- `AzureOpenAI:Deployment`
- `AzureOpenAI:ApiKey` (optional)

All samples in this repository share the User Secrets ID `genai-beginners-dotnet`.

### Option A — Use the repository setup scripts

If you already ran the setup at the repository root, this sample is configured
and no extra step is needed:

```powershell
./setup.ps1          # after an azd deployment
./setup-secrets.ps1 -Endpoint "https://<resource>.services.ai.azure.com"
```

### Option B — Run this sample on its own

You can configure only this sample without deploying anything else. From this
folder:

```powershell
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<resource>.services.ai.azure.com"
dotnet user-secrets set "AzureOpenAI:Deployment" "<your-deployment-name>"
```

The endpoint can be a resource URL or a URL that already ends in `/openai/v1/`.
The application appends the path when it is not present.

## Authentication

By default the sample uses `AzureCliCredential`, so sign in first:

```powershell
az login
```

Your identity needs the **Cognitive Services OpenAI User** role on the resource.

To use an API key instead, set the shared key:

```powershell
dotnet user-secrets set "AzureOpenAI:ApiKey" "<your-api-key>"
```

When an API key is present, the sample uses the `api-key` header and does not
require `az login`.

## Run

```powershell
dotnet run
```

Provide a different question as command-line arguments:

```powershell
dotnet run -- "Explain BYOK in one sentence."
```

## Expected output

```text
Model: <your-deployment-name>
Authentication: AzureCliCredential
Question: What is Microsoft Foundry, and how does it relate to the GitHub Copilot SDK?

Answer:
<model response>
```

The sample prints the selected model and authentication mode, but never prints
the endpoint or credentials. Tools, skills, configuration discovery, and Git
operations are disabled so the example focuses on one model request.

## Troubleshooting

| Problem | Fix |
| --- | --- |
| `The required user secret ... is missing` | Set `AzureOpenAI:Endpoint` and `AzureOpenAI:Deployment`. |
| `AuthenticationFailedException` | Run `az login`, or provide an API key. |
| The endpoint is rejected | The endpoint must be an HTTPS URL. |
| The Copilot CLI is not found | Install the GitHub Copilot CLI and confirm `copilot` is on `PATH`. |

## Clean up

```powershell
dotnet user-secrets clear
```

