# 聊天应用基础

在本课程中，我们将探讨如何使用语言模型的补全功能和.NET中的函数来构建聊天应用。我们还会学习如何利用 Microsoft Extensions AI (MEAI) 创建聊天机器人。

---

## 文本补全与聊天

[![文本补全与聊天视频](https://img.youtube.com/vi/Av1FCQf83QU/0.jpg)](https://youtu.be/Av1FCQf83QU?feature=shared)

_⬆️点击图片观看视频⬆️_

文本补全可能是 AI 应用中与语言模型交互的最基础形式。文本补全是根据输入（即提供给模型的提示）由模型生成的单次响应。

文本补全本身并不是一个聊天应用，它更像是一次性交互。你可以将文本补全用于诸如内容摘要或情感分析等任务。

### 文本补全

让我们看看如何使用 .NET 中的 **Microsoft.Extensions.AI** 库实现文本补全。

> 🧑‍💻**示例代码**：[这是一个可用的示例应用](../../../03-CoreGenerativeAITechniques/src/BasicChat-01MEAI)，你可以跟随学习。

```csharp

// this example illustrates using a model hosted on GitHub Models
IChatClient client = new ChatCompletionsClient(
    endpoint: new Uri("https://models.github.ai/inference"),
    new AzureKeyCredential(githubToken)) // githubToken is retrieved from the environment variables
    .AsChatClient("gpt-4o-mini");

// here we're building the prompt
StringBuilder prompt = new StringBuilder();
prompt.AppendLine("You will analyze the sentiment of the following product reviews. Each line is its own review. Output the sentiment of each review in a bulleted list and then provide a generate sentiment of all reviews. ");
prompt.AppendLine("I bought this product and it's amazing. I love it!");
prompt.AppendLine("This product is terrible. I hate it.");
prompt.AppendLine("I'm not sure about this product. It's okay.");
prompt.AppendLine("I found this product based on the other reviews. It worked for a bit, and then it didn't.");

// send the prompt to the model and wait for the text completion
var response = await client.GetResponseAsync(prompt.ToString());

// display the repsonse
Console.WriteLine(response.Message);

```

> 🗒️**注意：** 这个示例展示了 GitHub Models 作为托管服务。如果你想使用 Ollama，[请查看这个示例](../../../03-CoreGenerativeAITechniques/src/BasicChat-03Ollama)（它初始化了一个不同的 `IChatClient`）。
>
> 如果你想使用 Microsoft Foundry，你可以使用相同的代码，但需要更改端点和凭据。

> 🙋 **需要帮助？**：如果遇到任何问题，[请在仓库中提交问题](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new)。

### 聊天应用

构建一个聊天应用会稍微复杂一些。这需要与模型进行对话，用户可以发送提示，模型会响应。而且，就像任何对话一样，你需要保留对话的上下文或历史记录，以确保一切都合理。

#### 不同类型的聊天角色

在与模型的聊天中，发送给模型的消息可以有不同的类型。以下是一些例子：

* **System**: 系统消息引导模型响应的行为。它作为初始指令或提示，设置了对话的上下文、语气和边界。聊天的最终用户通常看不到这条消息，但它在塑造对话中非常重要。
* **User**: 用户消息是来自最终用户的输入或提示。它可以是一个问题、一条声明或一条命令。模型使用这条消息来生成响应。
* **Assistant**: 助手消息是由模型生成的响应。这些消息基于系统和用户消息生成，最终用户可以看到这些消息。

#### 管理聊天历史

在与模型的聊天过程中，你需要跟踪聊天的历史记录。这很重要，因为模型会根据系统消息以及用户和助手之间的所有交互来生成响应。每一条额外的消息都会为模型生成下一次响应提供更多上下文。

让我们看看如何使用 MEAI 构建一个聊天应用。

```csharp

// assume IChatClient is instantiated as before

List<ChatMessage> conversation =
[
    new (ChatRole.System, "You are a product review assistant. Your job is to help people write great product reviews. Keep asking questions on the person's experience with the product until you have enough information to write a review. Then write the review for them and ask if they are happy with it.")
];

Console.Write("Start typing a review (type 'q' to quit): ");

// Loop to read messages from the console
while (true)
{    
    string message = Console.ReadLine();

    if (message.ToLower() == "q")
    {
        break;
    }

    conversation.Add(new ChatMessage(ChatRole.User, message));

    // Process the message with the chat client (example)
    var response = await client.GetResponseAsync(conversation);
    conversation.Add(response.Message);
    
    Console.WriteLine(response.Message.Text);    
}

```


> 🙋 **需要帮助？**：如果遇到任何问题，[请在仓库中提交问题](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new)。

## 函数调用

[![函数调用讲解视频](https://img.youtube.com/vi/i84GijmGlYU/0.jpg)](https://youtu.be/i84GijmGlYU?feature=shared)

_⬆️点击图片观看视频⬆️_

在构建 AI 应用时，你并不仅限于基于文本的交互。你可以通过根据用户输入调用代码中预定义的函数来扩展聊天机器人的功能。换句话说，函数调用是模型与外部系统之间的桥梁。

> 🧑‍💻**示例代码**：[这是一个可用的示例应用](../../../03-CoreGenerativeAITechniques/src/MEAIFunctions)，你可以跟随学习。

### 在聊天应用中调用函数

要在 MEAI 中调用函数，需要进行一些设置步骤。

1. 首先，当然是定义你希望聊天机器人能够调用的函数。在这个示例中，我们将获取天气预报。

    ```csharp

    [Description("Get the weather")]
    static string GetTheWeather()
    {    
        var temperature = Random.Shared.Next(5, 20);

        var conditions = Random.Shared.Next(0, 1) == 0 ? "sunny" : "rainy";

        return $"The weather is {temperature} degrees C and {conditions}.";
    }

```

1. 接下来，我们将创建一个 `ChatOptions` 对象，告诉 MEAI 哪些函数可以被调用。

    ```csharp

    var chatOptions = new ChatOptions
    {
        Tools = [AIFunctionFactory.Create(GetTheWeather)]
    };

```

1. 当我们实例化 `IChatClient` 对象时，需要指定我们将使用函数调用。

    ```csharp
    IChatClient client = new ChatCompletionsClient(
        endpoint: new Uri("https://models.github.ai/inference"),
        new AzureKeyCredential(githubToken)) // githubToken is retrieved from the environment variables
    .AsChatClient("gpt-4o-mini")
    .AsBuilder()
    .UseFunctionInvocation()  // here we're saying that we could be invoking functions!
    .Build();
    ```

1. 最后，当我们与模型交互时，需要发送包含函数调用信息的 `ChatOptions` 对象，以便模型在需要获取天气信息时调用该函数。

    ```csharp
    var responseOne = await client.GetResponseAsync("What is today's date", chatOptions); // won't call the function

    var responseTwo = await client.GetResponseAsync("Should I bring an umbrella with me today?", chatOptions); // will call the function
    ```

> 🙋 **需要帮助？**：如果遇到任何问题，[请在仓库中提交问题](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new)。

## 总结

在本课程中，我们学习了如何使用文本补全、启动和管理聊天对话，以及在聊天应用中调用函数。

在下一课中，你将学习如何与数据进行聊天，并构建一种称为检索增强生成 (RAG) 模型的聊天机器人，还会学习如何在 AI 应用中处理视觉和音频！

## 额外资源

* [使用 .NET 构建 AI 聊天应用](https://learn.microsoft.com/dotnet/ai/quickstarts/get-started-openai?tabs=azd&pivots=openai)
* [执行本地 .NET 函数](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-azure-openai-tool?tabs=azd&pivots=openai)
* [与本地 AI 模型聊天](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-local-ai)

## 接下来

👉 [让我们构建一个 RAG 应用！](./02-retrieval-augmented-generation.md)

**免责声明**：  
本文档使用基于机器的人工智能翻译服务进行翻译。尽管我们尽力确保准确性，但请注意，自动翻译可能包含错误或不准确之处。应以原始语言的文档作为权威来源。对于关键信息，建议使用专业的人类翻译服务。因使用此翻译而引起的任何误解或误读，我们概不负责。
