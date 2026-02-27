# Setting Up: GitHub Codespaces

This is the fastest way to get started. You'll run VS Code entirely in your browser, backed by a pre-configured cloud environment (Codespace).

## 1. Create a GitHub Codespace

1. Open this repository's main page in a new window by [right-clicking here](https://github.com/microsoft/Generative-AI-for-beginners-dotnet) and selecting **Open in new window**.
2. **Fork** this repo into your GitHub account (top right corner).
3. Click the **Code** dropdown button and select the **Codespaces** tab.
4. Click the **...** (three dots) and choose **New with options...**.

![Creating a Codespace with custom options](./images/creating-codespace.png)

### Choose Your Container
From the **Dev container configuration** dropdown, select:

*   **Option 1: C# (.NET)** (Recommended)
    *   Best for using **Cloud Models** (Azure OpenAI / Microsoft Foundry).
    *   Fastest startup time.
*   **Option 2: C# (.NET) - Ollama**
    *   Select this **ONLY** if you want to run models locally inside the Codespace (slower startup).

Click **Create codespace**.

---

## 2. Setup Azure OpenAI / Microsoft Foundry

While your Codespace loads, configure your AI provider credentials.

### Set User Secrets

Once your Codespace is running, open the terminal (**Ctrl+`**) and navigate to a sample project:

```bash
cd samples/CoreSamples/BasicChat-01MEAI
dotnet user-secrets set "endpoint" "https://<your-endpoint>.services.ai.azure.com/"
dotnet user-secrets set "apikey" "<your-api-key>"
dotnet user-secrets set "deploymentName" "gpt-4o-mini"
```

Replace the values with your Azure OpenAI or Microsoft Foundry endpoint, API key, and deployment name.

---

## 3. Verify the Setup

Once your Codespace is running:

1. Open the terminal (**Ctrl+`**).
2. Set your user secrets as described above.
3. You are ready for Lesson 02!

[⬅️ Back to Introduction](./readme.md)
