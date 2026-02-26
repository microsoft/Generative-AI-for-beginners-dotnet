# Local Model Runners

In this lesson, you'll learn how to run AI models locally using three popular approaches, and how to integrate them with your .NET applications.

---

[![Local Model Runners](./images/LIM_GAN_07_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson3-patterns)

_Click the image to watch the video_

---

## Why Run Models Locally?

Running models locally provides several benefits:

- **Data privacy** – Your data never leaves your machine
- **Cost efficiency** – No usage charges for API calls
- **Offline availability** – Use AI even without internet connectivity
- **Customization** – Fine-tune models for specific use cases

This lesson covers three popular approaches:

- **[AI Toolkit for Visual Studio Code](https://code.visualstudio.com/docs/intelligentapps/overview)** – A suite of tools for Visual Studio Code that enables running AI models locally
- **[Docker Model Runner](https://docs.docker.com/model-runner/)** – A containerized approach for running AI models with Docker
- **[Foundry Local](https://learn.microsoft.com/azure/ai-foundry/foundry-local/)** – A cross-platform, open-source solution for running Microsoft AI models locally

---

## Part 1: AI Toolkit for Visual Studio Code

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

---

## Part 2: Docker Model Runner

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

---

## Part 3: Foundry Local

[Foundry Local](https://learn.microsoft.com/azure/ai-foundry/foundry-local/) is an open-source, cross-platform solution for running Microsoft AI models on your own hardware. It supports Windows, Linux, and macOS, and is designed for privacy, performance, and flexibility.

- **Official documentation:** <https://learn.microsoft.com/azure/ai-foundry/foundry-local/>
- **GitHub repository:** <https://github.com/microsoft/Foundry-Local/tree/main>

### Key Features - Foundry Local

- **Cross-platform** – Windows, Linux, and macOS
- **Microsoft models** – Run models from Microsoft Foundry locally
- **REST API** – Interact with models using a local API endpoint
- **No cloud dependency** – All inference runs on your machine

### Getting Started - Foundry Local

1. [Read the official Foundry Local documentation](https://learn.microsoft.com/azure/ai-foundry/foundry-local/)
2. Download and install Foundry Local for your OS
3. Start the Foundry Local server and download a model
4. Use the REST API to interact with the model

---

## Part 4: Using AI Toolkit for Visual Studio Code with .NET

The AI Toolkit for Visual Studio Code provides a way to run AI models locally on your machine. We have two examples that demonstrate how to interact with AI Toolkit models using .NET:

### 1. ~~Semantic Kernel with AI Toolkit~~ (Deprecated)

> ⚠️ This sample has been moved to [`samples/deprecated/AIToolkit-01-SK-Chat`](../samples/deprecated/AIToolkit-01-SK-Chat/). See the MEAI example below instead.

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

---

## Part 5: Using Docker Models with .NET

In this repository, we have two examples that demonstrate how to interact with Docker-based models using .NET:

### 1. ~~Semantic Kernel with Docker Models~~ (Deprecated)

> ⚠️ This sample has been moved to [`samples/deprecated/DockerModels-01-SK-Chat`](../samples/deprecated/DockerModels-01-SK-Chat/). See the MEAI example below instead.

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

---

## Part 6: Using Foundry Local with .NET

This repository includes two demos for Foundry Local:

### 1. ~~Semantic Kernel with Foundry Local~~ (Deprecated)

> ⚠️ This sample has been moved to [`samples/deprecated/AIFoundryLocal-01-SK-Chat`](../samples/deprecated/AIFoundryLocal-01-SK-Chat/). See the MEAI example below instead.

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

---

## Running the Samples

To run the samples in this repository:

1. Install Docker Desktop, AI Toolkit for Visual Studio Code, or Foundry Local as needed
2. Pull or download the required model
3. Start the local model server (Docker, AI Toolkit, or Foundry Local)
4. Navigate to one of the sample project directories
5. Run the project with `dotnet run`

---

## Comparing Local Model Runners

| Feature | AI Toolkit for Visual Studio Code | Docker Model Runner | Foundry Local |
|---------|-------------------------------|---------------------|--------------|
| Platform | Windows, macOS, Linux | Cross-platform | Cross-platform |
| Integration | Visual Studio Code APIs | REST API | REST API |
| Deployment | VS Code Extension | Container-based | Local installation |
| Hardware Acceleration | DirectML, DirectX | CPU, GPU | CPU, GPU |
| Models | Optimized for VS Code | Any containerized model | Microsoft Foundry models |

---

## Sample Code Reference

| Sample | Description |
|--------|-------------|
| [AIToolkit-02-MEAI-Chat](../samples/CoreSamples/AIToolkit-02-MEAI-Chat/) | AI Toolkit with Microsoft Extensions for AI |
| [DockerModels-02-MEAI-Chat](../samples/CoreSamples/DockerModels-02-MEAI-Chat/) | Docker Model Runner with MEAI |
| [AIFoundryLocal-01-MEAI-Chat](../samples/CoreSamples/AIFoundryLocal-01-MEAI-Chat/) | Foundry Local with MEAI |

---

## Additional Resources

- [AI Toolkit for Visual Studio Code Documentation](https://code.visualstudio.com/docs/intelligentapps/overview)
- [Docker Model Runner Documentation](https://docs.docker.com/model-runner/)
- [Foundry Local Documentation](https://learn.microsoft.com/azure/ai-foundry/foundry-local/)
- [Foundry Local GitHub Repository](https://github.com/microsoft/Foundry-Local/tree/main)
- [Semantic Kernel Documentation](https://learn.microsoft.com/semantic-kernel/overview/)
- [Microsoft Extensions for AI Documentation](https://learn.microsoft.com/dotnet/ai/)

---

## Lesson Complete

You now know how to run AI models locally using three approaches:

- **AI Toolkit for VS Code**: Integrated development experience with DirectML acceleration
- **Docker Model Runner**: Containerized, cross-platform model execution
- **Foundry Local**: Microsoft models running entirely on your hardware

Combined with the cloud-based patterns from earlier parts, you can now choose the right deployment approach for any scenario.

---

## What's Next?

You're ready to move beyond patterns into building **AI Agents**, systems that can plan, use tools, and collaborate autonomously.

[Continue to Lesson 4: AI Agents with Microsoft Agent Framework →](../04-AgentsWithMAF/readme.md)
