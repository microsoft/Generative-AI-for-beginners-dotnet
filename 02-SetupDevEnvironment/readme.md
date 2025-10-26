# Setting Up the Development Environment for This Course

This lesson will guide you through setting up your development environment for this course. To ensure your success we've prepared a devcontainer configuration that will provide all the tooling you need to complete the course. You can run the devcontainer in GitHub Codespaces (recommended) or locally on your machine. And we also demonstrate how to set up your GitHub access tokens to interact with GitHub Models.

*We have you covered with guides to setup [Azure OpenAI](getting-started-azure-openai.md) and [Ollama](getting-started-ollama.md), if desired.*

---

## What you'll learn in this lesson:

- ⚡ How to setup a development environment with GitHub Codepaces
- 🤖 Configure your development environment to access LLMs via GitHub Models, Azure OpenAI, or Ollama
- 🛠️ Industry-standard tools configuration with .devcontainer
- 🎯 Finally, everything is ready to complete the rest of the course

Let's dive in and set up your development environment! 🏃‍♂️

[![Watch the Video Tutorial](./images/LIM_GAN_02_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson2-setupdevenv)

_⬆️Click the image to watch the video⬆️_

## Which AI service should I use for this course?

We provide instructions for setting up your development environment with GitHub Models, Azure OpenAI, and Ollama. You can choose the one that best fits your needs. We recommend using GitHub Models for this course, but you can use any of the three services.

Here's a quick rundown of the services:

- **GitHub Models**: A free service to get started with that allows you to test and interact with various AI models directly within your development environment. It's easy to use from Codespaces and a great way to experiment with different models and understand their capabilities before implementation.
- **Azure OpenAI**: A paid service that provides access to a wide range of AI models. It includes all of the benefits that you'd come to expect from Azure include robust security and scalability. This is a great option for this course if you already have access to an Azure subscription.
- **Ollama**: Ollama allows you to run AI models locally on your machine or within a Codespace or devcontainer for free. It's a great option if you prefer to run the models locally, but it requires more hardware resources and can be slower than the cloud-based options.

> If **GitHub Models** is your choice, follow the rest of this document to set up your development environment with GitHub Models.
> - **Azure OpenAI** have your eye? [This is the document for you](getting-started-azure-openai.md).
> - **Ollama** your choice? [This guide has the info you need](getting-started-ollama.md).

### **NOTE for local models with Ollama:**

The Ollama Codespace will provision all the necessary models that you need. However, if you are working in local mode, once you have installed Ollama, you need to pull the models for the lessons you want to run.

- For lesson "**02 - Setting Up for .NET Development with Generative AI**" and project [BasicChat-03Ollama](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/tree/main/samples/CoreSamples/BasicChat-03Ollama) you need to pull a model like [phi4-mini](https://ollama.com/library/phi4-mini) or [llama3.2](https://ollama.com/library/llama3.2)  by entering in terminal

```bash
ollama pull phi4-mini
```

- For lesson "**03 - Core Generative AI Techniques with .NET**", when running the ollama projects like [RAGSimple-10SKOllama](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/tree/main/samples/CoreSamples/RAGSimple-10SKOllama),  you need to pull the models [all-minilm](https://ollama.com/library/all-minilm) and [phi4-mini](https://ollama.com/library/phi4-mini) by entering in terminal:

```bash
ollama pull phi4-mini
ollama pull all-minilm
```

## Learn and test AI models with GitHub Models

**GitHub Models** provides an intuitive way to experiment with various AI models directly within your development environment. This feature allows developers to test and interact with different models, understanding their capabilities and limitations before implementation. Through a simple interface, you can explore model responses, evaluate performance, and determine the best fit for your application requirements. Hosted within GitHub's infrastructure, these models offer reliable access and consistent performance, making them ideal for development and testing phases. Best of all, there is a free tier to start your exploration without any cost.

![Image for GitHub Models page, demonstrating multiple generative AI models](./images/github-models-webpage.png)

## Pre-flight check: Setting up GitHub Access Tokens

Before we do anything else, we need to configure essential security credentials that will enable our Codespace to interact with GitHub Models and execute our applications securely.

### Creating a Personal Access Token for GitHub Model access

1. Navigate to [GitHub Settings](https://github.com/settings/profile):

    - Click your profile picture in the top-right corner
    - Select **Settings** from the dropdown menu

    ![GitHub Settings](./images/settings-github.png)

1. Access [Developer Settings](https://github.com/settings/apps):

    - Scroll down the left sidebar
    - Click on **Developer settings** (usually at the bottom)

    ![Developer Settings](./images/developer-settings-github.png)

1. Generate a New Token:

    - Select **Personal access tokens** → **Tokens (classic)**

        ![Adding the Tokens(classic)](./images/tokens-classic-github.png)

    - In the dropdown in the middle of the page, click **Generate new token (classic)**

        ![Create your Token](./images/token-generate-github.png)

    - Under "Note", provide a descriptive name (e.g., `GenAI-DotNet-Course-Token`)
    - Set an expiration date (recommended: 7 days for security best practices)
    - **Important**: You must select the **`models:read`** permission scope for this token to work with GitHub Models.

> 💡 **Security Tip**: Always use the minimum required scope and shortest practical expiration time for your access tokens. This follows the principle of least privilege and helps maintain your account's tokens safe.

## Creating a GitHub Codespace

Let's create a GitHub Codespace to use for the rest of this course.

1. Open this repository's main page in a new window by [right-clicking here](https://github.com/microsoft/Generative-AI-for-beginners-dotnet) and selecting **Open in new window** from the context menu
1. Fork this repo into your GitHub account by clicking the **Fork** button in the top right corner of the page
1. Click the **Code** dropdown button and then select the **Codespaces** tab
1. Select the **...** option (the three dots) and choose **New with options...**

![Creating a Codespace with custom options](./images/creating-codespace.png)

### Choosing Your development container

From the **Dev container configuration** dropdown, select one of the following options:

**Option 1: C# (.NET)** : This is the option you should use if you plan to use GitHub Models and is our recommended way to complete this course. It has all the core .NET development tools needed for the rest of the course and a fast startup time

**Option 2: C# (.NET) - Ollama**: Ollama allows you to run the demos without needing to connect to GitHub Models or Azure OpenAI. It includes all the core .NET development in addition to Ollama, but has a slower start-up time, five minutes on average. [Follow this guide](getting-started-ollama.md) if you want to use Ollama

> 💡**Tip** : When creating your codespace, please, use the region closest to you if you have the option in the menu. Using a region far away, can cause errors in the creation.

Click the **Create codespace** button to start the Codespace creation process.

![Selecting your development container configuration](./images/select-container-codespace.png)

## Verifying your Codespace is running correctly with GitHub Models

Once your Codespace is fully loaded and configured, let's run a sample app to verify everything is working correctly:

1. Open the terminal. You can open a terminal window by typing **Ctrl+\`** (backtick) on Windows or **Cmd+`** on macOS.

1. Switch to the proper directory by running the following command:

   If you're using Windows Command Prompt (CMD) or PowerShell:
    ```bash
    cd samples\CoreSamples\BasicChat-01MEAI
    ```
    or If you're using Linux, macOS, Git Bash, WSL, or the VS Code terminal
   ```bash
   cd samples/CoreSamples/BasicChat-01MEAI
   ```

   > **Note**: GitHub Codespaces runs a Linux environment, so always use forward slashes (`/`) in paths, regardless of your local operating system.

1. Then run the application with the following command:

    ```bash
    dotnet run
    ```

1. It may take a couple of seconds, but eventually the application should output a message similar to the following:

    ```bash
    AI, or artificial intelligence, refers to the simulation of human intelligence in machines that are programmed to think and learn like humans. It is a broad field of computer science that focuses on creating systems and algorithms capable of performing tasks that typically require human intelligence. These tasks include problem-solving,

    ...
    ```

> 🙋 **Need help?**: Something not working? [Open an issue](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new?template=Blank+issue) and we'll help you out.

## Troubleshooting

### Error: Authentication Failed or 401 (Unauthorized)

If you encounter an error like:
```
Azure.RequestFailedException: Status: 401 (Unauthorized)
ErrorCode: (empty)
Content: (empty)
```

Or see errors in the stack trace mentioning:
```
at Azure.AI.Inference.ChatCompletionsClient.CompleteAsync(...)
at Microsoft.Extensions.AI.AzureAIInferenceChatClient.GetResponseAsync(...)
```

This typically means your GitHub Personal Access Token is missing the required permissions:

1. **Missing `models:read` scope**: As of May 15, 2025, GitHub Models requires the `models:read` permission scope for Personal Access Tokens. If you created your token before this date or didn't select this scope, you'll get authentication errors.

**Quick Fix**:
1. Go to [GitHub Settings → Developer settings → Personal access tokens → Tokens (classic)](https://github.com/settings/tokens)
2. Find your existing token (e.g., `GenAI-DotNet-Course-Token`)
3. Click **Edit** or **Delete** and create a new one
4. When creating the new token, make sure to check the **`models:read`** scope checkbox
5. Copy the new token and update your `GITHUB_TOKEN` environment variable

To update the token in your Codespace:
- Delete your existing Codespace
- Update the repository secrets if you're using Codespaces secrets
- Create a new Codespace with the updated token

### Error: Unknown model or 404 (Not Found)

If you encounter an error like:
```
Azure.RequestFailedException: Unknown model: /Phi-3.5-MoE-instruct
Status: 404 (Not Found)
ErrorCode: unknown_model
```

This typically means one of the following:

1. **Your code references a deprecated model**: Some models have been deprecated or removed from GitHub Models. For example, `Phi-3.5-MoE-instruct` was deprecated in September 2025. Make sure your code uses the current supported model.

2. **Your repository fork is outdated**: If you forked this repository before recent updates, your code might reference old model names. To fix this:
   - Pull the latest changes from the upstream repository
   - Ensure your `Program.cs` file in `samples/CoreSamples/BasicChat-01MEAI` uses `Phi-4-mini-instruct` (not older model names)

3. **Model name typo**: Double-check that the model name in your code exactly matches the available models in [GitHub Models](https://github.com/marketplace?type=models).

**Quick Fix**: 
- Open `/samples/CoreSamples/BasicChat-01MEAI/Program.cs`
- Verify the model name is `Phi-4-mini-instruct`:
  ```csharp
  .AsIChatClient("Phi-4-mini-instruct");
  ```

For the latest list of available models, visit the [GitHub Models Marketplace](https://github.com/marketplace?type=models).

### Model Migration Guide

If you're updating old code that used deprecated models, here's a quick reference:

| Deprecated Model (GitHub Models) | Current Replacement |
|----------------------------------|---------------------|
| Phi-3.5-MoE-instruct | Phi-4-mini-instruct |
| Phi-3.5-mini-instruct | Phi-4-mini-instruct |

> **Note**: This migration guide applies to **GitHub Models** only. If you're using local model runners like Ollama or Foundry Local, those platforms may still support Phi-3.5 models. Check your specific platform's documentation for available models.

## Summary

In this lesson, you learned how to set up your development environment for the rest of the course. You created a GitHub Codespace and configured it to use GitHub Models, Azure OpenAI, or Ollama. You also learned how to create a personal access token for GitHub Models and how to run a sample application to verify everything is working correctly.

### Additional Resources

- Test this guide with other hosting providers!
    - [Azure OpenAI](getting-started-azure-openai.md)
    - [Ollama](getting-started-ollama.md)
- [GitHub Codespaces Documentation](https://docs.github.com/en/codespaces)
- [GitHub Models Documentation](https://docs.github.com/en/github-models/prototyping-with-ai-models)

## Next Steps

Next, we'll explore how to create your first AI application! 🚀

👉 [Core Generative AI Techniques](../03-CoreGenerativeAITechniques/readme.md)
