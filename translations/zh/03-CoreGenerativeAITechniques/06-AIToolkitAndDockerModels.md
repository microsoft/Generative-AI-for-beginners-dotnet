# 使用 AI Toolkit 和 Docker 在本地运行模型

在本课程中，您将学习如何使用 AI Toolkit 和 Docker 在本地设备或云环境中运行生成式 AI 模型。

---

## 介绍

在本地运行 AI 模型的能力提供了诸多优势，如隐私保护、成本降低和对模型执行的完全控制。在本课程中，您将学习如何使用 Microsoft AI Toolkit 和 Docker 运行各种模型。

## Microsoft AI Toolkit

Microsoft AI Toolkit 是一组工具和库的集合，允许您将本地 AI 模型集成到 .NET 应用程序中。它支持各种模型类型和任务。

### 在 .NET 中使用 AI Toolkit

以下是在 .NET 应用程序中使用 AI Toolkit 的示例：

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;

var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddAIToolkitChatCompletion(
    modelId: "models/phi3:latest", 
    endpoint: "http://localhost:8080/v1");
var kernel = kernelBuilder.Build();

var chatCompletion = kernel.GetRequiredService<IChatCompletionService>();
var chat = new ChatHistory();
chat.AddUserMessage("用简单的词语解释量子计算");
var response = await chatCompletion.GetChatMessageContentAsync(chat);
Console.WriteLine(response.Content);
```

## 用于 AI 模型的 Docker

Docker 是一个允许您在隔离容器中运行应用程序的平台。它是一个强大的工具，可以在一致的环境中运行 AI 模型。

### 使用 Docker 运行模型

您可以通过 Docker 运行各种 AI 模型：

```bash
docker run -d --gpus all -p 8080:8080 ghcr.io/microsoft/phi3:latest
```

### 示例应用程序

在 [DockerModels-01-SK-Chat](./src/DockerModels-01-SK-Chat) 和 [DockerModels-02-MEAI-Chat](./src/DockerModels-02-MEAI-Chat) 示例中，我们实现了使用 Semantic Kernel 和 Microsoft.Extensions.AI 来利用本地模型的应用程序。

## 总结

在本地运行模型可以让您对 AI 应用程序有更多的控制，并可以降低成本。使用 Microsoft AI Toolkit 和 Docker，您可以在 .NET 应用程序中部署广泛的模型。

**免责声明**：  
本文档是通过基于机器的人工智能翻译服务翻译的。尽管我们努力确保准确性，但请注意，自动翻译可能包含错误或不准确之处。应以原始语言的文档作为权威来源。对于关键信息，建议使用专业的人类翻译。对于因使用本翻译而导致的任何误解或误读，我们概不负责。
