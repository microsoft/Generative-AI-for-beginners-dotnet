# Setting Up the Development Environment for Azure OpenAI

If you want to use Azure AI Foundry models for your .NET AI apps in this course, follow the steps in this guide.

Don't want to use Azure OpenAI?

👉 [To use GitHub Models this is the guide for you](README.md)
👉 [Here are the steps for Ollama](getting-started-ollama.md)

## Create the Azure AI Foundry resources

To use Azure AI Foundry models, you need to create a hub and project in the Azure AI Foundry portal. Then you'll need to deploy a model. This section will show you how to do that.

### Create a Hub and Project in Azure AI Foundry

1. Go to the [Azure AI Foundry Portal](https://ai.azure.com/).
1. Sign in with your Azure account.
1. Select **All hubs + projects** from the left-hand menu and then click the **+ New hub** from the dropdown. (Note: You may have to click on **+ New project** first to see the **+ New hub** option).
    ![Create a new hub](./images/ai-foundry-hub-selection.png)
1. A new window will open. Fill in the details for your hub:
    - Give your hub a name (e.g., "MyAIHub").
    - Choose a region closest to you.
    - Select the appropriate subscription and resource group.
    - You can leave the rest of the settings as they are.
    - Click **Next**.
    - Review the details and click **Create**.
1. Once your hub is created, the portal will open its details page. Click the **Create Project** button.
    - Give your project a name (e.g., "GenAINET") or accept the default.
    - Click **Create**.

🎉 **Done!** You’ve just created your first project in Azure AI Foundry.

### Deploy a Language Model in Azure AI Foundry

Now, let’s deploy a **gpt-4o-mini** model to your project:

1. In the Azure AI Foundry portal, navigate to your project (it should automatically open after creating it).
1. Click on **Models and Endpoints** from the left-hand menu and then the **Deploy Model** button.
1. Select **Deploy base model** from the dropdown.
1. Search for **gpt-4o-mini** in the model catalog.
1. Select the model and click the **Confirm** button.
1. Specify a deployment name (e.g., "gpt-4o-mini"). You can leave the rest of the options as they are.
1. Click **Deploy** and wait for the model to be provisioned.
1. Once deployed, note the **Model Name**, **Target URI**, and **API Key** from the model details page.

🎉 **Done!** You’ve deployed your first Large Language Model in Azure AI Foundry.

![Model deployed, copy model name, endpoint url and apikey](./images/deploytoazure-20-copymodelinfo.png)

> 📝 **Note:** The endpoint maybe similar to `https://< your hub name>.openai.azure.com/openai/deployments/gpt-4o-mini/chat/completions?api-version=2024-08-01-preview`. The endpoint name that we need is only `https://< your hub name >.openai.azure.com/`*.

## Adding the Azure AI API Key to your Codespace's Secrets

To be secure, let's add the API key you just created to your Codespace's secrets.

1. Make sure you have forked this repository to your GitHub account.
1. Go to the **Settings** tab of your forked repository then expand **Secrets and variables** on the left-hand menu and select **Codespaces**.

    ![Adding a new Codespace secret](./images/codespaces-secret.jpeg)
1. Name your secret **AZURE_AI_KEY**.
1. Paste the API key you copied from the Azure AI Foundry portal into the **Secret** field.

## Creating a GitHub Codespace

Let's create a GitHub Codespace to develop with for the rest of this course.

1. Open this repository's main page in a new window by [right-clicking here](https://github.com/microsoft/Generative-AI-for-beginners-dotnet) and selecting **Open in new window** from the context menu
1. Fork this repo into your GitHub account by clicking the **Fork** button in the top right corner of the page
1. Click the **Code** dropdown button and then select the **Codespaces** tab
1. Select the **...** option (the three dots) and choose **New with options...**

![Creating a Codespace with custom options](./images/creating-codespace.png)

### Choosing Your development container

From the **Dev container configuration** dropdown, select one of the following options:

**Option 1: C# (.NET)** : This is the option you should use if you plan to use GitHub Models or Azure OpenAI. It has all the core .NET development tools needed for the rest of the course and a fast startup time

**Option 2: C# (.NET) - Ollama**: Ollama allows you to run the demos without needing to connect to GitHub Models or Azure OpenAI. It includes all the core .NET development in addition to Ollama, but has a slower start-up time, five minutes on average. [Follow this guide](getting-started-ollama.md) if you want to use Ollama

You can leave the rest of the settings as they are. Click the **Create codespace** button to start the Codespace creation process.

![Selecting your development container configuration](./images/select-container-codespace.png)

## Update the sample code to use Azure OpenAI and your new model

Now let’s update the code to use the newly deployed model. First we'll need to add some NuGet packages to work with Azure OpenAI.

1. Open the terminal and switch to the project directory:

    ```bash
    cd 02-SetupDevEnvironment/src/BasicChat-01MEAI/
    ```

1. Run the following commands to add the required package:

    ```bash
    dotnet add package Azure.AI.OpenAI
    dotnet add package Microsoft.Extensions.AI.OpenAI --version 9.1.0-preview.1.25064.3
    ```

[More information about Azure.AI.OpenAI](https://www.nuget.org/packages/Azure.AI.OpenAI/2.1.0#show-readme-container).

1. Open `/workspaces/Generative-AI-for-beginners-dotnet/02-SettingUp.NETDev/src/BasicChat-01MEAI/Program.cs`.

    Add the following using statements at the top of the file:


    ```csharp
    using System.ClientModel;
    using Azure.AI.OpenAI;
    using Microsoft.Extensions.AI;

1. Create new variables to hold the model name, endpoint, and API key:

    ```csharp
    var deploymentName = "< deployment name > "; // e.g. "gpt-4o-mini"
    var endpoint = new Uri("< endpoint >"); // e.g. "https://< your hub name >.openai.azure.com/"
    var apiKey = new ApiKeyCredential(Environment.GetEnvironmentVariable("AZURE_AI_SECRET"));
    ```

    Making sure to replace `< deployment name >`, and `< endpoint >` with the values you noted above.

1. Replace the `IChatClient` creation with the following code:

    ```csharp
    IChatClient client = new AzureOpenAIClient(
        endpoint,
        apiKey)
    .AsChatClient(deploymentName);
    ```

1. Run the following command in the terminal:

    ```bash
    dotnet run
    ```

1. You should see output similar to the following:

    ```bash
    Artificial Intelligence (AI) refers to the simulation of human intelligence in machines that are programmed to think and learn like humans. AI encompasses a variety of technologies and approaches that enable computers and systems to perform tasks that typically require human intelligence. These tasks include:

    1. **Learning**: The ability to improve performance based on experience, often through algorithms that analyze data.
    
    ...
    ```

> 🙋 **Need help?**: Something not working? [Open an issue](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new?template=Blank+issue) and we'll help you out.

## Summary

In this lesson, you learned how to set up your development environment for the rest of the course. You created a GitHub Codespace and configured it to use Azure OpenAI. You also updated the sample code to use the newly deployed model in Azure AI Foundry.

### Additional Resources

- [Azure AI Foundry Documentation](https://learn.microsoft.com/en-us/azure/ai-services/)
- [Working with GitHub Codespaces](https://docs.github.com/en/codespaces/getting-started)
- [How to Deploy Models in Azure AI Foundry](https://learn.microsoft.com/en-us/azure/ai-services/deploy/)
- [Azure.AI.OpenAI NuGet Package](https://www.nuget.org/packages/Azure.AI.OpenAI)

## Next Steps

Next, we'll explore how to create your first AI application! 🚀

👉 [Core Generative AI Techniques](../03-CoreGenerativeAITechniques/readme.md)
