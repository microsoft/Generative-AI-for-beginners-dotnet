# 视觉与音频 AI 应用

在本课中，学习如何通过视觉 AI 让你的应用生成和解析图像。音频 AI 则可以让你的应用生成音频，甚至实时转录音频内容。

---

## 视觉

[![视觉 AI 介绍视频](https://img.youtube.com/vi/QXbASt1KXuw/0.jpg)](https://youtu.be/QXbASt1KXuw?feature=shared)

_⬆️点击图片观看视频⬆️_

基于视觉的 AI 方法被用于生成和解析图像。这在许多场景中非常有用，例如图像识别、图像生成和图像编辑。当前的模型是多模态的，这意味着它们可以接受多种输入类型，例如文本、图像和音频，并生成多种输出。在这里，我们将重点关注图像识别。

### 使用 MEAI 进行图像识别

图像识别不仅仅是让 AI 模型告诉你图像中有什么内容。你还可以对图像提问，例如：_图中有多少人？现在是否在下雨？_

好了——我们要测试一下模型的能力，先让它告诉我们第一张照片中有多少双红鞋，然后分析一张德语的餐馆收据，看看我们应该给多少小费。

![一个复合图，展示了示例中将使用的两张图片。第一张是几位跑步者的腿部特写，第二张是一张德国餐馆的收据](../../../translated_images/example-visual-image.e2fc4ffa5f01b3d65bb9bd5d23eebf97513bf486b761209b28fea06b63a11f6c.zh.png)

> 🧑‍💻**示例代码**：你可以[在这里找到示例代码](../../../03-CoreGenerativeAITechniques/src/Vision-01MEAI-AzureOpenAI)。

1. 我们使用的是 MEAI 和 Azure OpenAI，所以像之前一样实例化 `IChatClient`。同时，开始创建一个聊天历史记录。

    ```csharp
    IChatClient chatClient = new AzureOpenAIClient(
    new Uri(config["endpoint"]),
    new ApiKeyCredential(config["apikey"]))
    .GetChatClient("gpt-5-mini")
    .AsIChatClient();

    List<ChatMessage> messages = 
    [
        new ChatMessage(ChatRole.System, "You are a useful assistant that describes images using a direct style."),
        new ChatMessage(ChatRole.User, "How many red shoes are in the photo?") // we'll start with the running photo
    ];
    ```

2. 接下来，将图像加载到一个 `AIContent` 对象中，并将其作为对话的一部分发送给模型，让它为我们描述图像。

    ```csharp
    var imagePath = "FULL PATH TO THE IMAGE ON DISK";

    AIContent imageContent = new DataContent(File.ReadAllBytes(imagePath), "image/jpeg"); // the important part here is that we're loading it in bytes. The image could come from anywhere.

    var imageMessage = new ChatMessage(ChatRole.User, [imageContent]);

    messages.Add(imageMessage);

    var response = await chatClient.GetResponseAsync(messages);

    messages.Add(response.Message);

    Console.WriteLine(response.Message.Text);
    ```

3. 然后，让模型处理德语餐馆的收据，看看我们应该给多少小费：

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

这里有一个我想特别强调的地方。我们正在与一个语言模型对话，更准确地说，是一个能够处理文本、图像（以及音频）交互的多模态模型。而且，我们像平常一样与模型进行对话。当然，我们发送给模型的对象类型不同，比如 `AIContent` instead of a `string`, but the workflow is the same.

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

1. The first thing we'll do (after grabbing the key and region of the model's deployment) is instantiate a `SpeechTranslationConfig` 对象。这将使我们能够告诉模型，我们将输入的内容从口语英语翻译为书面西班牙语。

    ```csharp
    var speechKey = "<FROM YOUR MODEL DEPLOYMENT>";
    var speechRegion = "<FROM YOUR MODEL DEPLOYMENT>";

    var speechTranslationConfig = SpeechTranslationConfig.FromSubscription(speechKey, speechRegion);
    speechTranslationConfig.SpeechRecognitionLanguage = "en-US";
    speechTranslationConfig.AddTargetLanguage("es-ES");
    ```

4. 接下来，我们需要获取麦克风的访问权限，然后创建一个 `TranslationRecognizer` 对象，用于与模型进行通信。

    ```csharp
    using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
    using var translationRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);
    ```

5. 最后，我们调用模型并设置一个函数来处理返回结果。

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

使用 AI 处理音频与我们之前做的事情略有不同，因为我们使用的是 Azure AI Speech 服务，但将语音音频翻译为文本的结果非常强大。

> 🙋 **需要帮助？**：如果你遇到任何问题，[可以在仓库中提交问题](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new)。

我们还有另一个示例，[展示了如何使用 Azure Open AI 进行实时音频对话](../../../03-CoreGenerativeAITechniques/src/Audio-02-RealTimeAudio)——快来看看吧！


## 其他资源

- [使用 AI 和 .NET 生成图像](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-openai-generate-images?tabs=azd&pivots=openai)


## 下一步

你已经学会了如何为 .NET 应用添加视觉和音频功能，在下一课中，学习如何创建具备一定自主行动能力的 AI。

👉 [查看 AI 代理](./04-agents.md)。

**免责声明**：  
本文件使用基于机器的人工智能翻译服务进行翻译。尽管我们努力确保翻译的准确性，但请注意，自动翻译可能包含错误或不准确之处。应以原始语言的文件作为权威来源。对于关键信息，建议寻求专业人工翻译。因使用本翻译而引起的任何误解或误读，我们概不负责。