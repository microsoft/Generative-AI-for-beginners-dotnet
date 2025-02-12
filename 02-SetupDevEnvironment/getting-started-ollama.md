# Setting Up the Development Envionment with Ollama

If you want to use Ollama to run local models for this course, follow the steps in this guide.

Don't want to use Azure OpenAI?

👉 [To use GitHub Models this is the guide for you](README.md)
👉 [Here are the steps for Ollama](getting-started-ollama.md)

## Creating a GitHub Codespace

Let's create a GitHub Codespace to develop with for the rest of this course.

1. Open this repository's main page in a new window by [right-clicking here](https://github.com/microsoft/Generative-AI-for-beginners-dotnet) and selecting **Open in new window** from the context menu
1. Fork this repo into your GitHub account by clicking the **Fork** button in the top right corner of the page
1. Click the **Code** dropdown button and then select the **Codespaces** tab
1. Select the **...** option (the three dots) and choose **New with options...**

![Creating a Codespace with custom options](./images/creating-codespace.png)

### Choosing Your development container

From the **Dev container configuration** dropdown, select one of the following options:

**Option 1: C# (.NET)** : This is the option you should use if you plan to use GitHub Models or Azure OpenAI and is our recommended way to complete this course. It has all the core .NET development tools needed for the rest of the course and a fast startup time

**Option 2: C# (.NET) - Ollama**: This is the one you want for running models locally with Ollama. It includes all the core .NET development in addition to Ollama, but has a slower start-up time, five minutes on average. [Follow this guide](getting-started-ollama.md) if you want to use Ollama

You can leave the rest of the settings as they are. Click the **Create codespace** button to start the Codespace creation process.

![Selecting your development container configuration](./images/select-container-codespace.png)

## Verifying your Codespace is running correctly with Ollama

Once your Codespace is fully loaded and configured, lets run a sample app to verify everything is working correctly:

1. Open the terminal. You can open a terminal window by typing **Ctrl+\`** (backtick) on Windows or **Cmd+`** on macOS.

1. Switch to the proper directory by running the following command:

    ```bash
    cd 02-SetupDevEnvironment/src/BasicChat-03Ollama/
    ```

1. Then run the application with the following command:

    ```bash
    dotnet run
    ```

1. It may take a couple of seconds, but eventually the application should output a message similar to the following:

    ```bash
    AI, or Artificial Intelligence, refers to the development of computer systems that can perform tasks that typically require human intelligence, such as:

    1. Learning: AI systems can learn from data and improve their performance over time.
    2. Reasoning: AI systems can draw conclusions and make decisions based on the data they have been trained on.
    
    ...
    ```

> 🙋 **Need help?**: Something not working? [Open an issue](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new?template=Blank+issue) and we'll help you out.

## Swap out the model in Ollama

One of the cool things about Ollama is that it's easy to change models. The current app uses the "**llama3.2**" model. Let’s switch it up and try the "**phi3.5**" model instead.

1. Download the Phi3.5 model running the comamnd from the terminal:

    ```bash
    ollama pull phi3.5
    ```

    You can learn mode about the [Phi3.5](https://ollama.com/library/phi3.5) and other available models in the [Ollama library](https://ollama.com/library/).

1. Edit the initialization of the chat client in `Program.cs` to use the new model::

    ```csharp
    IChatClient client = new OllamaChatClient(new Uri("http://localhost:11434/"), "phi3.5");
    ```

1. Finally, run the app with the following command:

    ```bash
    dotnet run
    ```

1. You’ve just switched to a new model. Notice how the response is longer and more detailed.

    ```bash
    Artificial Intelligence (AI) refers to the simulation of human intelligence processes by machines, especially computer systems. These processes include learning (the acquisition of information and accumulation of knowledge), reasoning (using the acquired knowledge to make deductions or decisions), and self-correction. AI can manifest in various forms:

    1. **Narrow AI** – Designed for specific tasks, such as facial recognition software, voice assistants like Siri or Alexa, autonomous vehicles, etc., which operate under a limited preprogrammed set of behaviors and rules but excel within their domain when compared to humans in these specialized areas.

    2. **General AI** – Capable of understanding, learning, and applying intelligence broadly across various domains like human beings do (natural language processing, problem-solving at a high level). General AIs are still largely theoretical as we haven't yet achieved this form to the extent necessary for practical applications beyond narrow tasks.
    
    ...
    ```

> 🙋 **Need help?**: Something not working? [Open an issue](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new?template=Blank+issue) and we'll help you out.

## Summary

In this lesson, you learned how to set up your development environment for the rest of the course. You created a GitHub Codespace and configured it to use Ollama. You also updated the sample code to use change models easily.

### Additional Resources

- [Ollama Models](https://ollama.com/search)
- [Working with GitHub Codespaces](https://docs.github.com/en/codespaces/getting-started)
- [Microsoft Extensions for AI Documentation](https://learn.microsoft.com/en-us/dotnet/)

## Next Steps

Next, we'll explore how to create your first AI application! 🚀

👉 [Core Generative AI Techniques](../03-CoreGenerativeAITechniques/readme.md)
