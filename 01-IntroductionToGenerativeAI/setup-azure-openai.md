# Setting Up: Azure OpenAI

Use enterprise-grade models via your Azure subscription.

## 1. Create Resources
1. Go to the [Microsoft Azure AI Foundry Portal](https://ai.azure.com/).
2. Create a new **Hub** and **Project**.
    *   Region: Choose one close to you (e.g., East US 2).

## 2. Deploy a Model
1. Inside your project, go to **Models + endpoints**.
2. Click **Deploy model** -> **Deploy base model**.
3. Select **gpt-4o** or **gpt-4o-mini**.
4. Set the **Deployment name** (e.g., `gpt-4o-mini`).
    *   *Important:* Remember this name. You will need it for your configuration.

## 3. Get Credentials
You will need three pieces of information for the .NET apps:
1. **Endpoint:** (e.g., `https://my-hub.openai.azure.com/`)
2. **Deployment Name:** (e.g., `gpt-4o-mini`)
3. **API Key:** Found on the "Models + endpoints" page.

## 4. Configure Your Environment
You can set these as environment variables or User Secrets in .NET:

```bash
dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "https://..."
dotnet user-secrets set "AZURE_OPENAI_MODEL" "gpt-4o-mini"
dotnet user-secrets set "AZURE_OPENAI_APIKEY" "your-key-here"
```

[⬅️ Back to Introduction](./readme.md)
