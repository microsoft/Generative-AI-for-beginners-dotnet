# 視覺與音訊 AI 應用

在這堂課中，您將學習如何使用視覺 AI 讓您的應用程式能夠生成與解讀影像。而音訊 AI 則可以幫助您的應用程式生成音訊，甚至能即時進行語音轉錄。

---

## 視覺

[![視覺 AI 說明影片](https://img.youtube.com/vi/QXbASt1KXuw/0.jpg)](https://youtu.be/QXbASt1KXuw?feature=shared)

_⬆️點擊圖片觀看影片⬆️_

基於視覺的 AI 方法被用來生成與解讀影像。這在許多應用場景中都非常有用，例如影像辨識、影像生成以及影像操作。當前的模型是多模態的，也就是說，它們可以接受多種類型的輸入，例如文字、影像和音訊，並生成多種類型的輸出。在這裡，我們將專注於影像辨識。

### 使用 MEAI 進行影像辨識

影像辨識不僅僅是讓 AI 模型告訴您影像中有哪些內容，您還可以對影像提出問題，例如：_影像中有多少人？現在是否在下雨？_

好吧，讓我們測試一下模型的能力。我們將要求模型告訴我們第一張照片中有多少雙紅色鞋子，然後分析一張德文的餐廳收據，看看我們應該給多少小費。

![一個包含兩張影像的合成圖。第一張顯示幾個跑步者的腿部，第二張是一張德國餐廳的收據](../../../translated_images/example-visual-image.e2fc4ffa5f01b3d65bb9bd5d23eebf97513bf486b761209b28fea06b63a11f6c.tw.png)

> 🧑‍💻**範例程式碼**：您可以在[這裡找到範例程式碼](../../../03-CoreGenerativeAITechniques/src/Vision-01MEAI-GitHubModels)。

1. 我們將使用 MEAI 和 GitHub 模型，因此如同之前一樣初始化 `IChatClient`。同時，開始建立一個對話記錄。

    ```csharp
    IChatClient chatClient = new ChatCompletionsClient(
        endpoint: new Uri("https://models.inference.ai.azure.com"),
        new AzureKeyCredential(githubToken)) // make sure to grab githubToken from the secrets or environment
    .AsChatClient("gpt-4o-mini");

    List<ChatMessage> messages = 
    [
        new ChatMessage(ChatRole.System, "You are a useful assistant that describes images using a direct style."),
        new ChatMessage(ChatRole.User, "How many red shoes are in the photo?") // we'll start with the running photo
    ];
    ```

1. 接下來的步驟是將影像載入到 `AIContent` 物件中，並將其設定為我們對話的一部分，然後將其發送給模型進行描述。

    ```csharp
    var imagePath = "FULL PATH TO THE IMAGE ON DISK";

    AIContent imageContent = new DataContent(File.ReadAllBytes(imagePath), "image/jpeg"); // the important part here is that we're loading it in bytes. The image could come from anywhere.

    var imageMessage = new ChatMessage(ChatRole.User, [imageContent]);

    messages.Add(imageMessage);

    var response = await chatClient.GetResponseAsync(messages);

    messages.Add(response.Message);

    Console.WriteLine(response.Message.Text);
    ```

1. 然後，讓模型處理餐廳收據（這是一張德文收據），以找出我們應該留多少小費：

    ```csharp
    // this will go after the previous code block
    messages.Add(new ChatMessage(ChatRole.User, "This is a receipt from a lunch. I had the sausage. How much of a tip should I leave?"));

    var receiptPath = "FULL PATH TO THE RECEIPT IMAGE ON DISK";

    AIContent receiptContent = new DataContent(File.ReadAllBytes(receiptPath), "image/jpeg");
    var receiptMessage = new ChatMessage(ChatRole.User, [receiptContent]);

    messages.Add(receiptMessage);

    response = await chatClient.GetResponseAsync(messages);
    messages.Add(response.Message);

    Console.WriteLine(response.Message.Text);
    ```

這裡有一個重點我要強調。我們正在與一個語言模型對話，更準確地說，是一個可以處理文字、影像（以及音訊）互動的多模態模型。我們與模型的對話方式保持正常，但我們傳送給模型的對象是不同的，例如 `AIContent` instead of a `string`, but the workflow is the same.

> 🙋 **Need help?**: If you encounter any issues, [open an issue in the repository](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

## Audio AI

[![Audio AI explainer video](https://img.youtube.com/vi/fuquPXRNqCo/0.jpg)](https://youtu.be/fuquPXRNqCo?feature=shared)

_⬆️Click the image to watch the video⬆️_

Real-time audio techniques allow your apps to generate audio and transcribe it in real-time. This can be useful for a wide range of applications, such as voice recognition, speech synthesis, and audio manipulation.

But we're going to have to transition away from MEAI and from the model we were using to Azure AI Speech Services.

To setup an Azure AI Speech Service model, [follow these directions](../02-SetupDevEnvironment/getting-started-azure-openai.md) but instead of choosing an OpenAI model, choose **Azure-AI-Speech**.

> **🗒️Note:>** Audio is coming to MEAI, but as of this writing isn't available yet. When it is available we'll update this course.

### Implementing speech-to-text with Cognitive Services

You'll need the **Microsoft.CognitiveServices.Speech** NuGet package for this example.

> 🧑‍💻**Sample code**: You can follow [along with sample code here](../../../03-CoreGenerativeAITechniques/src/Audio-01-SpeechMic).

1. The first thing we'll do (after grabbing the key and region of the model's deployment) is instantiate a `SpeechTranslationConfig` 物件。這使我們能夠指示模型，我們將輸入英語語音並將其翻譯成西班牙語文字。

    ```csharp
    var speechKey = "<FROM YOUR MODEL DEPLOYMENT>";
    var speechRegion = "<FROM YOUR MODEL DEPLOYMENT>";

    var speechTranslationConfig = SpeechTranslationConfig.FromSubscription(speechKey, speechRegion);
    speechTranslationConfig.SpeechRecognitionLanguage = "en-US";
    speechTranslationConfig.AddTargetLanguage("es-ES");
    ```

1. 接下來，我們需要取得麥克風的存取權，並建立一個新的 `TranslationRecognizer` 物件，這個物件將與模型進行通訊。

    ```csharp
    using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
    using var translationRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);
    ```

1. 最後，我們呼叫模型並設置一個函數來處理返回結果。

    ```csharp
    var translationRecognitionResult = await translationRecognizer.RecognizeOnceAsync();
    OutputSpeechRecognitionResult(translationRecognitionResult);

    void OutputSpeechRecognitionResult(TranslationRecognitionResult translationRecognitionResult)
    {
        switch (translationRecognitionResult.Reason)
        {
            case ResultReason.TranslatedSpeech:
                Console.WriteLine($"RECOGNIZED: Text={translationRecognitionResult.Text}");
                foreach (var element in translationRecognitionResult.Translations)
                {
                    Console.WriteLine($"TRANSLATED into '{element.Key}': {element.Value}");
                }
                break;
            case ResultReason.NoMatch:
                // handle when speech could not be recognized
                break;
            case ResultReason.Canceled:
                // handle an error condition
                break;
        }
    }
    ```

使用 AI 處理音訊的方式與我們之前做的稍有不同，因為我們這次使用的是 Azure AI Speech 服務，但將語音音訊轉換為文字的結果非常強大。

> 🙋 **需要幫助嗎？**：如果您遇到任何問題，請[在倉庫中開啟一個問題](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new)。

我們還有另一個範例，[展示了如何使用 Azure Open AI 進行即時音訊對話](../../../03-CoreGenerativeAITechniques/src/Audio-02-RealTimeAudio)，快去看看吧！

## 其他資源

- [使用 AI 和 .NET 生成影像](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-openai-generate-images?tabs=azd&pivots=openai)

## 接下來

您已經學會了如何為 .NET 應用程式添加視覺和音訊功能。在下一堂課中，了解如何創建能夠自主行動的 AI。

👉 [了解 AI 代理](./04-agents.md)。

**免責聲明**：  
本文件是使用基於機器的人工智能翻譯服務進行翻譯的。儘管我們努力確保準確性，但請注意，自動翻譯可能包含錯誤或不準確之處。應以原始語言的文件作為權威來源。對於關鍵資訊，建議尋求專業人工翻譯。我們對因使用此翻譯而產生的任何誤解或誤讀概不負責。