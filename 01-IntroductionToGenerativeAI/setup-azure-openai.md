# Setting Up Azure OpenAI Resources for This Course

Welcome! 👋 This guide walks you through setting up the Azure OpenAI resources you'll need to run the samples in this course. Don't worry if you're new to Azure — we've made it as straightforward as possible. Pick the path that fits your situation and you'll be up and running in minutes.

## Prerequisites

Before you start, make sure you have:

- ✅ **Azure subscription** — [Create a free account](https://azure.microsoft.com/free/?WT.mc_id=academic-105485-koreyst) (includes $200 in credits for 30 days)
- ✅ **.NET 10 SDK or later** — [Download here](https://dotnet.microsoft.com/download?WT.mc_id=academic-105485-koreyst)
- ✅ **Azure Developer CLI (azd)** — Only needed for automated setup — [Install here](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd?WT.mc_id=academic-105485-koreyst)

---

## Option 1: Automated Setup (Recommended)

**Best for:** First-time Azure users or anyone who wants everything set up automatically.

The `setup.ps1` script in the repository root handles the entire process — it deploys Azure resources, configures your models, and wires up your .NET secrets so every sample just works.

### Run the script

```powershell
./setup.ps1
```

### Follow the interactive prompts

The script uses `azd up` under the hood, which will ask you to:

1. **Sign in** to your Azure account (a browser window opens)
2. **Select a subscription** — pick the one you'd like to use
3. **Choose a region** — East US 2, West US 2, and North Europe tend to have the best availability
4. **Name your environment** — any short name works (e.g., `genai-course`)

Sit back for a couple of minutes while the resources deploy. ☕

### Expected output

When everything succeeds, you'll see a summary like this:

```
========================================
  Setup Summary:
========================================
  Secrets ID:           genai-beginners-dotnet
  Chat Deployment:      gpt-5-mini
  Embedding Deployment: text-embedding-3-small
  Endpoint:             https://openai-genai-course-abc123.openai.azure.com/
========================================

Setup complete. Navigate to a sample folder and run:
  dotnet run
```

### Verify it works

```powershell
cd samples/CoreSamples/BasicChat-01MEAI
dotnet run
```

If you see the AI respond, you're all set! 🎉

---

## Option 2: I Already Have Azure OpenAI Resources

**Best for:** Users who already have `gpt-5-mini` and/or `text-embedding-3-small` deployed in their Azure subscription.

You don't need to deploy anything new — just tell the samples where your resources are. There are two ways to do this.

### Using the setup-secrets script

The quickest approach. From the repository root, run:

```powershell
./setup-secrets.ps1 -Endpoint "https://my-resource.openai.azure.com/"
```

That's it! The script defaults to `gpt-5-mini` and `text-embedding-3-small` as deployment names.

**Parameters:**

| Parameter | Required | Default | Description |
|-----------|----------|---------|-------------|
| `-Endpoint` | ✅ Yes | — | Your Azure OpenAI endpoint URL |
| `-Deployment` | No | `gpt-5-mini` | Your chat model deployment name |
| `-EmbeddingDeployment` | No | `text-embedding-3-small` | Your embedding model deployment name |

If your deployment names are different from the defaults:

```powershell
./setup-secrets.ps1 -Endpoint "https://my-resource.openai.azure.com/" -Deployment "my-gpt-model" -EmbeddingDeployment "my-embedding-model"
```

### Using dotnet user-secrets directly

If you prefer setting things manually:

```powershell
dotnet user-secrets set --id genai-beginners-dotnet "AzureOpenAI:Endpoint" "https://my-resource.openai.azure.com/"
dotnet user-secrets set --id genai-beginners-dotnet "AzureOpenAI:Deployment" "gpt-5-mini"
dotnet user-secrets set --id genai-beginners-dotnet "AzureOpenAI:EmbeddingDeployment" "text-embedding-3-small"
```

> 💡 **Tip:** All samples share the same User Secrets ID (`genai-beginners-dotnet`), so you only configure once and every sample picks up the values automatically.

---

## Option 3: Manual Deployment in Microsoft Foundry

**Best for:** Users who want to understand the Azure resources or need custom configuration.

### Step 1: Create an Azure AI Foundry Hub

1. Go to [Microsoft Azure AI Foundry Portal](https://ai.azure.com/)
2. Sign in with your Azure account
3. Click **+ New project**
4. Give it a name (e.g., `genai-beginners`)
5. Select a **Hub** (create a new one if needed)
   - Region: Choose one close to you (East US 2, West US 2, North Europe recommended)
6. Click **Create**

### Step 2: Deploy the Chat Model (gpt-5-mini)

1. Inside your project, go to **Models + endpoints** in the left sidebar
2. Click **Deploy model** → **Deploy base model**
3. Search for and select **gpt-5-mini**
4. Set the **Deployment name** to `gpt-5-mini` (keep it simple — this matches the course defaults)
5. Click **Deploy**

### Step 3: Deploy the Embedding Model (text-embedding-3-small)

> 📝 **Note:** This is only needed for RAG and semantic search samples. You can skip this step if you only want to run chat samples.

1. Go to **Models + endpoints** again
2. Click **Deploy model** → **Deploy base model**
3. Search for and select **text-embedding-3-small**
4. Set the **Deployment name** to `text-embedding-3-small`
5. Click **Deploy**

### Step 4: Get Your Credentials

You need three values from the portal:

1. **Endpoint** — Found on the project Overview page or Models + endpoints page
   - Looks like: `https://your-resource-name.openai.azure.com/`
2. **Chat Deployment Name** — The name you gave in Step 2 (e.g., `gpt-5-mini`)
3. **Embedding Deployment Name** — The name you gave in Step 3 (e.g., `text-embedding-3-small`)

> 💡 **Tip:** You do NOT need an API key if you're using `DefaultAzureCredential` (which is what most samples use). If a sample requires an API key, it will be mentioned in that sample's README.

### Step 5: Configure Your Environment

Run the setup-secrets script with your values:

```powershell
./setup-secrets.ps1 -Endpoint "https://your-resource-name.openai.azure.com/"
```

Or set secrets manually (see Option 2 above).

---

## Verify Your Setup

After any option, verify everything works:

```powershell
# Check your secrets are set
dotnet user-secrets list --id genai-beginners-dotnet

# Run a sample
cd samples/CoreSamples/BasicChat-01MEAI
dotnet run
```

You should see your three secrets listed (`AzureOpenAI:Endpoint`, `AzureOpenAI:Deployment`, `AzureOpenAI:EmbeddingDeployment`) and the sample should start chatting with the AI.

---

## Additional Secrets for Specialized Samples

Some samples require extra configuration beyond the core Azure OpenAI setup. Each sample's README specifies exactly what's needed, but here's a quick reference:

| Sample Type | Additional Secrets | Used By |
|-------------|-------------------|---------|
| AI Foundry Agents | `AIFoundry:Endpoint`, `AIFoundry:TenantId` | Agent framework samples |
| RAG / Search | `AzureAISearch:Endpoint`, `AzureAISearch:Key` | Semantic search samples |
| Audio / Speech | `AzureSpeech:Key`, `AzureSpeech:Region` | Voice and transcription |

```powershell
# Example: Setting AI Foundry secrets
dotnet user-secrets set --id genai-beginners-dotnet "AIFoundry:Endpoint" "https://your-project.ai.azure.com/"
dotnet user-secrets set --id genai-beginners-dotnet "AIFoundry:TenantId" "your-tenant-id"
```

---

## Resource Cleanup

When you're done with the course, remove Azure resources to avoid charges:

```powershell
./cleanup.ps1
```

This deletes all Azure resources and clears your local secrets.

### Manual Cleanup

If the script doesn't work:

```powershell
azd down --purge --force
dotnet user-secrets clear --id genai-beginners-dotnet
Remove-Item -Path ".azure" -Recurse -Force
```

---

## 🔍 Want to Know More? How the Scripts Work

### What `setup.ps1` does under the hood

The setup script automates these steps:

1. **Checks prerequisites** — Verifies `azd` and `dotnet` are installed
2. **Deploys Azure infrastructure** — Runs `azd up`, which uses Bicep templates (`infra/main.bicep`) to create:
   - An Azure OpenAI Service (Cognitive Services account, S0 tier)
   - A `gpt-5-mini` model deployment (GlobalStandard SKU, 10K tokens-per-minute)
   - A `text-embedding-3-small` model deployment (Standard SKU, 10K tokens-per-minute)
3. **Extracts the endpoint** — Reads the deployed endpoint from `azd env get-values`
4. **Configures .NET User Secrets** — Sets `AzureOpenAI:Endpoint`, `AzureOpenAI:Deployment`, and `AzureOpenAI:EmbeddingDeployment` under the shared secrets ID `genai-beginners-dotnet`
5. **Detects your tenant** — If Azure CLI is installed, reminds you to `az login --tenant <id>` for token-based auth

All samples share the same User Secrets ID (`genai-beginners-dotnet`), so you configure once and every sample picks up the values automatically.

### What `cleanup.ps1` does under the hood

1. **Deletes Azure resources** — Runs `azd down --purge --force` to remove all deployed resources and purge soft-deleted resources
2. **Removes local state** — Deletes the `.azure/` folder (azd environment config)
3. **Clears secrets** — Runs `dotnet user-secrets clear --id genai-beginners-dotnet`

### What `setup-secrets.ps1` does under the hood

This is a simpler script for users who already have Azure resources:

1. Takes your endpoint (required), deployment name (defaults to `gpt-5-mini`), and embedding deployment name (defaults to `text-embedding-3-small`) as parameters
2. Runs `dotnet user-secrets set` for each value using the shared ID `genai-beginners-dotnet`
3. Prints a summary of what was configured
4. Shows additional secrets you might need for specialized samples

### The infrastructure (Bicep templates)

The `infra/` folder contains Azure Bicep templates that define what gets deployed:

- `main.bicep` — Orchestrator that creates a resource group and deploys the modules
- `modules/cognitive-services.bicep` — Azure OpenAI account (S0 tier)
- `modules/model-deployment-chat.bicep` — gpt-5-mini deployment (version 2025-08-07, GlobalStandard SKU)
- `modules/model-deployment-embedding.bicep` — text-embedding-3-small deployment (version 1, Standard SKU)

You can customize these templates before running `setup.ps1` — for example, to change the model, region, or capacity.

---

## Troubleshooting

### "azd up" fails with a subscription error

```powershell
az account set --subscription "<subscription-id>"
```

Then re-run `./setup.ps1`.

### Region not available for Azure OpenAI

Check [available regions](https://learn.microsoft.com/azure/ai-services/openai/concepts/models?WT.mc_id=academic-105485-koreyst) and try East US 2, West US 2, or North Europe.

### Quota exceeded (429 errors)

Wait a few minutes for rate limits to reset, or [request a quota increase](https://portal.azure.com/?WT.mc_id=academic-105485-koreyst) in the Azure Portal.

### User secrets not found

```powershell
dotnet user-secrets list --id genai-beginners-dotnet
```

If empty, re-run setup or set secrets manually.

### PowerShell execution policy error

```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

---

[⬅️ Back to Introduction](./readme.md)
