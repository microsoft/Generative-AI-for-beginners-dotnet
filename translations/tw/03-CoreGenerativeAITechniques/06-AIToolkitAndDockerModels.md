# 使用 AI Toolkit 和 Docker 在本地運行模型

在本課程中，您將學習如何使用 AI Toolkit 和 Docker 在本地設備或雲環境中運行生成式 AI 模型。

---

## 介紹

在本地運行 AI 模型的能力提供了諸多優勢，如隱私保護、成本降低和對模型執行的完全控制。在本課程中，您將學習如何使用 Microsoft AI Toolkit 和 Docker 運行各種模型。

## Microsoft AI Toolkit

Microsoft AI Toolkit 是一組工具和庫的集合，允許您將本地 AI 模型集成到 .NET 應用程式中。它支持各種模型類型和任務。

### 在 .NET 中使用 AI Toolkit

以下是在 .NET 應用程式中使用 AI Toolkit 的示例：

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
chat.AddUserMessage("用簡單的詞語解釋量子計算");
var response = await chatCompletion.GetChatMessageContentAsync(chat);
Console.WriteLine(response.Content);
```

## 用於 AI 模型的 Docker

Docker 是一個允許您在隔離容器中運行應用程式的平台。它是一個強大的工具，可以在一致的環境中運行 AI 模型。

### 使用 Docker 運行模型

您可以通過 Docker 運行各種 AI 模型：

```bash
docker run -d --gpus all -p 8080:8080 ghcr.io/microsoft/phi3:latest
```

### 範例應用程式

在 [DockerModels-01-SK-Chat](./src/DockerModels-01-SK-Chat) 和 [DockerModels-02-MEAI-Chat](./src/DockerModels-02-MEAI-Chat) 示例中，我們實現了使用 Semantic Kernel 和 Microsoft.Extensions.AI 來利用本地模型的應用程式。

## 總結

在本地運行模型可以讓您對 AI 應用程式有更多的控制，並可以降低成本。使用 Microsoft AI Toolkit 和 Docker，您可以在 .NET 應用程式中部署廣泛的模型。

**免責聲明**：  
本文檔使用機器翻譯服務進行翻譯。儘管我們努力保證準確性，但請注意，自動翻譯可能包含錯誤或不準確之處。應以原始語言的文件作為權威來源。對於關鍵資訊，建議尋求專業人工翻譯。我們對使用本翻譯而引起的任何誤解或錯誤解釋不承擔責任。
