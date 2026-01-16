# Setting Up: GitHub Codespaces & GitHub Models (Recommended)

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
    *   Best for using **Cloud Models** (GitHub Models / Azure OpenAI).
    *   Fastest startup time.
*   **Option 2: C# (.NET) - Ollama**
    *   Select this **ONLY** if you want to run models locally inside the Codespace (slower startup).

Click **Create codespace**.

---

## 2. Setup GitHub Models (Free AI Access)

While your Codespace loads, let's get you free access to AI models like GPT-4o.

### Create a Personal Access Token (PAT)
1. Go to [GitHub Settings](https://github.com/settings/profile) -> **Settings**.
2. Scroll down on the left to **Developer settings**.
3. Select **Personal access tokens** → **Tokens (classic)**.
4. Click **Generate new token (classic)**.
    *   **Note:** Course Token
    *   **Expiration:** 7 days (or as needed)
    *   **Scopes:** ✅ Check the box for **`models:read`**. This is required.

![Adding the Tokens(classic)](./images/tokens-classic-github.png)

5. **Copy the token.** You will need it shortly.

---

## 3. Verify the Setup

Once your Codespace is running:

1. Open the terminal (**Ctrl+`**).
2. Set your token as a secret (optional, or paste it when prompted by the app).
3. You are ready for Lesson 02!

[⬅️ Back to Introduction](./readme.md)
