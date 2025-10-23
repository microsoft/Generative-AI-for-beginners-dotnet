# Running AI Models Locally: AI Toolkit, Docker, and Foundry Local

## Table of Contents

- [Introduction](#running-ai-models-locally-ai-toolkit-docker-and-foundry-local)
- [AI Toolkit for Visual Studio Code](#ai-toolkit-for-visual-studio-code)
  - [Key Features](#key-features)
  - [Getting Started](#getting-started)
- [Docker Model Runner](#docker-model-runner)
  - [Key Features - Docker Model Runner](#key-features---docker-model-runner)
  - [Getting Started - Docker Model Runner](#getting-started---docker-model-runner)
- [Foundry Local](#foundry-local)
  - [Key Features - Foundry Local](#key-features---foundry-local)
  - [Getting Started - Foundry Local](#getting-started---foundry-local)
- [Sample Code: Using AI Toolkit for Visual Studio Code with .NET](#sample-code-using-ai-toolkit-for-visual-studio-code-with-net)
  - [Semantic Kernel with AI Toolkit](#1-semantic-kernel-with-ai-toolkit)
  - [Microsoft Extensions for AI with AI Toolkit](#2-microsoft-extensions-for-ai-with-ai-toolkit)
- [Sample Code: Using Docker Models with .NET](#sample-code-using-docker-models-with-net)
  - [Semantic Kernel with Docker Models](#1-semantic-kernel-with-docker-models)
  - [Microsoft Extensions for AI with Docker Models](#2-microsoft-extensions-for-ai-with-docker-models)
- [Sample Code: Using Foundry Local with .NET](#sample-code-using-foundry-local-with-net)
  - [Semantic Kernel with Foundry Local](#1-semantic-kernel-with-foundry-local)
  - [Microsoft Extensions for AI with Foundry Local](#2-microsoft-extensions-for-ai-with-foundry-local)
- [Running the Samples](#running-the-samples)
- [Comparing Local Model Runners](#comparing-local-model-runners)
- [Additional Resources](#additional-resources)
- [Summary](#summary)
- [Next Steps](#next-steps)

In this lesson, you'll learn how to run AI models locally using three popular approaches:

- **[AI Toolkit for Visual Studio Code](https://code.visualstudio.com/docs/intelligentapps/overview)** – A suite of tools for Visual Studio Code that enables running AI models locally
- **[Docker Model Runner](https://docs.docker.com/model-runner/)** – A containerized approach for running AI models with Docker
- **[Foundry Local](https://learn.microsoft.com/azure/ai-foundry/foundry-local/)** – A cross-platform, open-source solution for running Microsoft AI models locally

Running models locally provides several benefits:

- Data privacy – Your data never leaves your machine
- Cost efficiency – No usage charges for API calls
- Offline availability – Use AI even without internet connectivity
- Customization – Fine-tune models for specific use cases

## AI Toolkit for Visual Studio Code

The AI Toolkit for Visual Studio Code is a collection of tools and technologies that help you build and run AI applications locally on your PC. It leverages platform capabilities to optimize AI workloads.

### Key Features

- **DirectML** – Hardware-accelerated machine learning primitives
- **Windows AI Runtime (WinRT)** – Runtime environment for AI models
- **ONNX Runtime** – Cross-platform inference accelerator
- **Local model downloads** – Access to optimized models for Windows

### Getting Started

1. [Install the AI Toolkit for Visual Studio Code](https://code.visualstudio.com/docs/intelligentapps/overview)
2. Download a supported model
3. Use the APIs through .NET or other supported languages

> 📝 **Note**: AI Toolkit for Visual Studio Code requires Visual Studio Code and compatible hardware for optimal performance.

## Docker Model Runner

Docker Model Runner is a tool for running AI models in containers, making it easy to deploy and run inference workloads consistently across different environments.

### Key Features - Docker Model Runner

- **Containerized models** – Package models with their dependencies
- **Cross-platform** – Run on Windows, macOS, and Linux
- **Built-in API** – RESTful API for model interaction
- **Resource management** – Control CPU and memory usage

### Getting Started - Docker Model Runner

1. [Install Docker Desktop](https://www.docker.com/products/docker-desktop/)
2. Pull a model image
3. Run the model container
4. Interact with the model through the API

```bash
# Pull and run a Llama model
docker run -d -p 12434:8080 \
  --name deepseek-model \
  --runtime=nvidia \
  ghcr.io/huggingface/dockerfiles/model-runner:latest \
  deepseek-ai/deepseek-llm-7b-chat
```

## Foundry Local

[Foundry Local](https://learn.microsoft.com/azure/ai-foundry/foundry-local/) is an open-source, cross-platform solution for running Microsoft AI models on your own hardware. It supports Windows, Linux, and macOS, and is designed for privacy, performance, and flexibility.

- **Official documentation:** https://learn.microsoft.com/azure/ai-foundry/foundry-local/
- **GitHub repository:** https://github.com/microsoft/Foundry-Local/tree/main

### Key Features - Foundry Local

- **Cross-platform** – Windows, Linux, and macOS
- **Microsoft models** – Run models from Azure AI Foundry locally
- **REST API** – Interact with models using a local API endpoint
- **No cloud dependency** – All inference runs on your machine

### Getting Started - Foundry Local

1. [Read the official Foundry Local documentation](https://learn.microsoft.com/azure/ai-foundry/foundry-local/)
2. Download and install Foundry Local for your OS
3. Start the Foundry Local server and download a model
4. Use the REST API to interact with the model

## Sample Code: Using AI Toolkit for Visual Studio Code with .NET

The AI Toolkit for Visual Studio Code provides a way to run AI models locally on your machine. We have two examples that demonstrate how to interact with AI Toolkit models using .NET:

### 1. Semantic Kernel with AI Toolkit

The [AIToolkit-01-SK-Chat](../samples/CoreSamples/AIToolkit-01-SK-Chat/) project shows how to use Semantic Kernel to chat with a model running via AI Toolkit for Visual Studio Code.

```csharp
// Example code demonstrating AI Toolkit for Visual Studio Code with Semantic Kernel integration
// Configure to use a locally installed model through AI Toolkit for Visual Studio Code
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(
    modelId: modelId,
    endpoint: new Uri(endpoint),
    apiKey: apiKey);
var kernel = builder.Build();
```

### 2. Microsoft Extensions for AI with AI Toolkit

The [AIToolkit-02-MEAI-Chat](../samples/CoreSamples/AIToolkit-02-MEAI-Chat/) project demonstrates how to use Microsoft Extensions for AI to interact with AI Toolkit for Visual Studio Code models.

```csharp
// Example code demonstrating AI Toolkit for Visual Studio Code with MEAI
OpenAIClientOptions options = new OpenAIClientOptions();
options.Endpoint = new Uri(endpoint);
ApiKeyCredential credential = new ApiKeyCredential(apiKey);
// Create a chat client using local model through AI Toolkit for Visual Studio Code
ChatClient client = new OpenAIClient(credential, options).GetChatClient(modelId);
```

## Sample Code: Using Docker Models with .NET

In this repository, we have two examples that demonstrate how to interact with Docker-based models using .NET:

### 1. Semantic Kernel with Docker Models

The [DockerModels-01-SK-Chat](../samples/CoreSamples/DockerModels-01-SK-Chat/) project shows how to use Semantic Kernel to chat with a model running in Docker.

```csharp
var model = "ai/deepseek-r1-distill-llama";
var base_url = "http://localhost:12434/engines/llama.cpp/v1";
var api_key = "unused";

// Create a chat completion service
var builder = Kernel.CreateBuilder();
builder.AddOpenAIChatCompletion(modelId: model, apiKey: api_key, endpoint: new Uri(base_url));
var kernel = builder.Build();

var chat = kernel.GetRequiredService<IChatCompletionService>();
var history = new ChatHistory();
history.AddSystemMessage("You are a useful chatbot. Always reply in a funny way with short answers.");

// ... continue with chat functionality
```

### 2. Microsoft Extensions for AI with Docker Models

The [DockerModels-02-MEAI-Chat](../samples/CoreSamples/DockerModels-02-MEAI-Chat/) project demonstrates how to use Microsoft Extensions for AI to interact with Docker-based models.

```csharp
var model = "ai/deepseek-r1-distill-llama";
var base_url = "http://localhost:12434/engines/llama.cpp/v1";
var api_key = "unused";

OpenAIClientOptions options = new OpenAIClientOptions();
options.Endpoint = new Uri(base_url);
ApiKeyCredential credential = new ApiKeyCredential(api_key);

ChatClient client = new OpenAIClient(credential, options).GetChatClient(model);

// Build and send a prompt
StringBuilder prompt = new StringBuilder();
prompt.AppendLine("You will analyze the sentiment of the following product reviews...");
// ... add more text to the prompt

var response = await client.CompleteChatAsync(prompt.ToString());
Console.WriteLine(response.Value.Content[0].Text);
```

## Sample Code: Using Foundry Local with .NET

This repository includes two demos for Foundry Local:

### 1. Semantic Kernel with Foundry Local

The [AIFoundryLocal-01-SK-Chat](../samples/CoreSamples/AIFoundryLocal-01-SK-Chat/Program.cs) project shows how to use Semantic Kernel to chat with a model running via Foundry Local.

```csharp
#pragma warning disable SKEXP0001, SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0050, SKEXP0052
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text;

var model = "Phi-3.5-mini-instruct-cuda-gpu";
var baseUrl = "http://localhost:5273/v1";
var apiKey = "unused";

// Create a chat completion service
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(modelId: model, apiKey: apiKey, endpoint: new Uri(baseUrl))
    .Build();

var chat = kernel.GetRequiredService<IChatCompletionService>();
var history = new ChatHistory();
history.AddSystemMessage("You are a useful chatbot. Always reply in a funny way with short answers.");

var settings = new OpenAIPromptExecutionSettings
{
    MaxTokens = 50000,
    Temperature = 1
};

while (true)
{
    Console.Write("Q: ");
    var userQuestion = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userQuestion))
    {
        break;
    }
    history.AddUserMessage(userQuestion);

    var responseBuilder = new StringBuilder();
    Console.Write("AI: ");
    await foreach (var message in chat.GetStreamingChatMessageContentsAsync(history, settings, kernel))
    {
        responseBuilder.Append(message.Content);
        Console.Write(message.Content);
    }
    Console.WriteLine();

    history.AddAssistantMessage(responseBuilder.ToString());
}
```

### 2. Microsoft Extensions for AI with Foundry Local

The [AIFoundryLocal-01-MEAI-Chat](../samples/CoreSamples/AIFoundryLocal-01-MEAI-Chat/Program.cs) project demonstrates how to use Microsoft Extensions for AI to interact with Foundry Local models.

```csharp
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Text;

var model = "Phi-3.5-mini-instruct-cuda-gpu";
var baseUrl = "http://localhost:5273/v1";
var apiKey = "unused";

OpenAIClientOptions options = new OpenAIClientOptions();
options.Endpoint = new Uri(baseUrl);
ApiKeyCredential credential = new ApiKeyCredential(apiKey);

ChatClient client = new OpenAIClient(credential, options).GetChatClient(model);

// here we're building the prompt
StringBuilder prompt = new StringBuilder();
prompt.AppendLine("You will analyze the sentiment of the following product reviews. Each line is its own review. Output the sentiment of each review in a bulleted list and then provide a generate sentiment of all reviews. ");
prompt.AppendLine("I bought this product and it's amazing. I love it!");
prompt.AppendLine("This product is terrible. I hate it.");
prompt.AppendLine("I'm not sure about this product. It's okay.");
prompt.AppendLine("I found this product based on the other reviews. It worked for a bit, and then it didn't.");

// send the prompt to the model and wait for the text completion
var response = await client.CompleteChatAsync(prompt.ToString());

// display the response
Console.WriteLine(response.Value.Content[0].Text);
```

## Running the Samples

To run the samples in this repository:

1. Install Docker Desktop, AI Toolkit for Visual Studio Code, or Foundry Local as needed
2. Pull or download the required model
3. Start the local model server (Docker, AI Toolkit, or Foundry Local)
4. Navigate to one of the sample project directories
5. Run the project with `dotnet run`

## Comparing Local Model Runners

| Feature | AI Toolkit for Visual Studio Code | Docker Model Runner | Foundry Local |
|---------|-------------------------------|---------------------|--------------|
| Platform | Windows, macOS, Linux | Cross-platform | Cross-platform |
| Integration | Visual Studio Code APIs | REST API | REST API |
| Deployment | VS Code Extension | Container-based | Local installation |
| Hardware Acceleration | DirectML, DirectX | CPU, GPU | CPU, GPU |
| Models | Optimized for VS Code | Any containerized model | Microsoft Foundry models |

## Additional Resources

- [AI Toolkit for Visual Studio Code Documentation](https://code.visualstudio.com/docs/intelligentapps/overview)
- [Docker Model Runner Documentation](https://docs.docker.com/model-runner/)
- [Foundry Local Documentation](https://learn.microsoft.com/azure/ai-foundry/foundry-local/)
- [Foundry Local GitHub Repository](https://github.com/microsoft/Foundry-Local/tree/main)
- [Semantic Kernel Documentation](https://learn.microsoft.com/semantic-kernel/overview/)
- [Microsoft Extensions for AI Documentation](https://learn.microsoft.com/dotnet/ai/)

## Summary

Running AI models locally with AI Toolkit for Visual Studio Code, Docker Model Runner, or Foundry Local offers flexibility, privacy, and cost benefits. The samples in this repository demonstrate how to integrate these local models with your .NET applications using Semantic Kernel and Microsoft Extensions for AI.

## Next Steps

You've learned how to run AI models locally using AI Toolkit for Visual Studio Code, Docker Model Runner, and Foundry Local. Next, you'll explore the latest Azure OpenAI models for image and video generation.

👉 [Image and Video Generation with New Azure OpenAI Models](./07-ImageVideoGenerationNewModels.md)
