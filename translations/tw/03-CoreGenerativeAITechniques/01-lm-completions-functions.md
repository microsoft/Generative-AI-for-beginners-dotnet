# 聊天應用基礎

在這節課中，我們將探索如何使用語言模型的補全功能和 .NET 中的功能來構建聊天應用。我們還會學習如何使用 Semantic Kernel 和 Microsoft Extensions AI (MEAI) 來創建聊天機器人，以及如何使用 Semantic Kernel 創建插件，讓聊天機器人根據用戶的輸入調用特定功能。

---

## 文本補全與聊天功能

[![文本補全與聊天功能影片](https://img.youtube.com/vi/Av1FCQf83QU/0.jpg)](https://youtu.be/Av1FCQf83QU?feature=shared)

_⬆️點擊圖片觀看影片⬆️_

文本補全可能是 AI 應用中與語言模型互動的最基本形式。文本補全是模型根據輸入（或提示）生成的一次性響應。

文本補全本身並不是一個聊天應用，它是一種一次性互動。您可以將文本補全用於內容摘要或情感分析等任務。

### 文本補全

以下是如何使用 **Microsoft.Extensions.AI** 庫在 .NET 中進行文本補全的範例：

> 🧑‍💻**範例程式碼**：[這裡有一個可以運行的範例應用](../../../03-CoreGenerativeAITechniques/src/BasicChat-01MEAI)，您可以跟著操作。

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

> 🗒️**注意：** 這個範例使用了 GitHub Models 作為託管服務。如果您想使用 Ollama，[請參考這個範例](../../../03-CoreGenerativeAITechniques/src/BasicChat-03Ollama)（它實例化了一個不同的 `IChatClient`）。
>
> 如果您想使用 Microsoft Foundry，可以使用相同的程式碼，但需要更改端點和憑證。

> 🙋 **需要幫助？**：如果遇到問題，[在倉庫中提交一個 issue](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new)。

### 聊天應用

構建聊天應用會稍微複雜一些。聊天過程中，使用者可以發送提示，模型會回應。而且，像任何對話一樣，您需要保存上下文或對話歷史，這樣才能讓對話保持連貫。

#### 不同類型的聊天角色

與模型進行聊天時，發送給模型的訊息可以分為不同類型。以下是一些例子：

* **System（系統）**：系統訊息用於指導模型的回應行為。它作為初始指令或提示，設定對話的上下文、語氣和邊界。最終用戶通常看不到這個訊息，但它對於塑造對話非常重要。
* **User（用戶）**：用戶訊息是最終用戶的輸入或提示，可以是問題、陳述或指令。模型會根據這個訊息生成回應。
* **Assistant（助手）**：助手訊息是模型生成的回應，基於系統和用戶訊息產生，最終用戶可以看到這些訊息。

#### 管理聊天歷史

在與模型的聊天過程中，您需要追蹤聊天歷史。這很重要，因為模型會根據系統訊息以及用戶和助手之間的所有對話來生成回應。每新增一條訊息，都會為模型提供更多上下文來生成下一次回應。

以下是如何使用 MEAI 構建聊天應用的範例：

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

> 🗒️**注意：** 這也可以使用 Semantic Kernel 實現。[在這裡查看程式碼](../../../03-CoreGenerativeAITechniques/src/BasicChat-02SK)。

> 🙋 **需要幫助？**：如果遇到問題，[在倉庫中提交一個 issue](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new)。

## 函數調用

[![函數說明影片](https://img.youtube.com/vi/i84GijmGlYU/0.jpg)](https://youtu.be/i84GijmGlYU?feature=shared)

_⬆️點擊圖片觀看影片⬆️_

構建 AI 應用時，不僅限於基於文本的互動。您可以通過根據用戶輸入調用程式碼中預定義的函數來擴展聊天機器人的功能。換句話說，函數調用是模型與外部系統之間的橋樑。

> 🧑‍💻**範例程式碼**：[這裡有一個可以運行的範例應用](../../../03-CoreGenerativeAITechniques/src/MEAIFunctions)，您可以跟著操作。

### 在聊天應用中調用函數

要在 MEAI 中調用函數，您需要完成以下幾個設置步驟：

1. 首先，定義您希望聊天機器人能調用的函數。在這個範例中，我們將獲取天氣預報。

    ```csharp

    [Description("Get the weather")]
    static string GetTheWeather()
    {    
        var temperature = Random.Shared.Next(5, 20);

        var conditions = Random.Shared.Next(0, 1) == 0 ? "sunny" : "rainy";

        return $"The weather is {temperature} degrees C and {conditions}.";
    }

```

1. 接下來，創建一個 `ChatOptions` 物件，告訴 MEAI 哪些函數可以使用。

    ```csharp

    var chatOptions = new ChatOptions
    {
        Tools = [AIFunctionFactory.Create(GetTheWeather)]
    };

```

1. 當我們實例化 `IChatClient` 物件時，需要指定我們將使用函數調用功能。

    ```csharp
    IChatClient client = new ChatCompletionsClient(
        endpoint: new Uri("https://models.github.ai/inference"),
        new AzureKeyCredential(githubToken)) // githubToken is retrieved from the environment variables
    .AsChatClient("gpt-4o-mini")
    .AsBuilder()
    .UseFunctionInvocation()  // here we're saying that we could be invoking functions!
    .Build();
    ```

1. 最後，在與模型互動時，我們會傳送 `ChatOptions` 物件，指定如果需要天氣資訊，模型可以調用的函數。

    ```csharp
    var responseOne = await client.GetResponseAsync("What is today's date", chatOptions); // won't call the function

    var responseTwo = await client.GetResponseAsync("Should I bring an umbrella with me today?", chatOptions); // will call the function
    ```

> 🙋 **需要幫助？**：如果遇到問題，[在倉庫中提交一個 issue](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new)。

## 總結

在這節課中，我們學習了如何使用文本補全功能、啟動和管理聊天對話，以及在聊天應用中調用函數。

在下一節課中，您將學習如何開始與數據進行對話，並構建所謂的檢索增強生成 (RAG) 模型聊天機器人，以及在 AI 應用中處理視覺和音頻的內容！

## 更多資源

* [使用 .NET 構建 AI 聊天應用](https://learn.microsoft.com/dotnet/ai/quickstarts/get-started-openai?tabs=azd&pivots=openai)
* [執行本地 .NET 函數](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-azure-openai-tool?tabs=azd&pivots=openai)
* [與本地 AI 模型進行聊天](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-local-ai)

## 下一步

👉 [讓我們開始構建 RAG 應用吧！](./02-retrieval-augmented-generation.md)

**免責聲明**：  
本文檔使用基於機器的人工智能翻譯服務進行翻譯。我們致力於提供準確的翻譯，但請注意，自動翻譯可能包含錯誤或不準確之處。應以原文檔的母語版本作為權威來源。對於關鍵資訊，建議尋求專業人工翻譯。我們對因使用本翻譯而引起的任何誤解或誤讀概不負責。
