# BasicChat-05AIFoundryModels — Foundry model swap + Integrated Security

The **Block 2 (Foundations / MEAI)** live demo for OD805. One `IChatClient`, one
**Microsoft Foundry** endpoint, many models — swap the model by changing a single string,
and authenticate the way you should in Foundry: **Integrated Security (Microsoft Entra ID)**.

## What it shows

1. **Chat with a deployment name + endpoint + apikey** — the familiar key-based path.
2. **Foundry model swap** — change only `AzureOpenAI:Deployment` to switch
   **`gpt-5.5` → `grok-4`** (same code, same endpoint, same `IChatClient`).
3. **Integrated Security (recommended)** — drop the key entirely and use
   `AzureCliCredential` / Microsoft Entra ID. This is the way to work in Foundry.

> Then the demo switches to **`BasicChat-03Ollama`** to run the same `IChatClient`
> against a local model — zero changes to app logic.

## Config (User Secrets)

```bash
cd samples/CoreSamples/BasicChat-05AIFoundryModels

# Foundry endpoint (one endpoint hosts many models)
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-foundry-resource>.openai.azure.com/"

# Model = deployment name. Swap this to change models (gpt-5.5 -> grok-4 -> ...)
dotnet user-secrets set "AzureOpenAI:Deployment" "gpt-5.5"

# Auth mode: "integrated" (recommended) or "apikey"
dotnet user-secrets set "AzureOpenAI:AuthMode" "integrated"

# Only needed when AuthMode=apikey
dotnet user-secrets set "AzureOpenAI:ApiKey" "<your-key>"
```

## Run the demo (suggested on-stage order)

```bash
# 1. Key auth with gpt-5.5
dotnet user-secrets set "AzureOpenAI:AuthMode" "apikey"
dotnet user-secrets set "AzureOpenAI:Deployment" "gpt-5.5"
dotnet run app.cs

# 2. Foundry model swap: gpt-5.5 -> grok-4 (same endpoint, same code)
dotnet user-secrets set "AzureOpenAI:Deployment" "grok-4"
dotnet run app.cs

# 3. Integrated Security (the recommended way) — no key in config
dotnet user-secrets set "AzureOpenAI:AuthMode" "integrated"
az login
dotnet run app.cs
```

The console prints the active **model** and **auth** mode, and labels each answer with
`AI [<deployment>]:` so the model swap is visible while presenting.

> Integrated Security requires the signed-in account to have the **Azure AI Developer**
> role on the Foundry resource.
