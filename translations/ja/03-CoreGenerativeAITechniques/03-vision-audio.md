# ビジョンとオーディオAIアプリ

このレッスンでは、ビジョンAIを活用してアプリが画像を生成および解釈できるようにする方法を学びます。また、オーディオAIを使用してアプリが音声を生成したり、リアルタイムで文字起こしを行ったりすることも可能になります。

---

## ビジョン

[![ビジョンAI解説動画](https://img.youtube.com/vi/QXbASt1KXuw/0.jpg)](https://youtu.be/QXbASt1KXuw?feature=shared)

_⬆️画像をクリックして動画を視聴⬆️_

ビジョンベースのAIアプローチは、画像を生成したり解釈したりするために使用されます。これにより、画像認識、画像生成、画像操作など、さまざまな用途で役立ちます。現在のモデルはマルチモーダルであり、テキスト、画像、音声などさまざまな入力を受け取り、多様な出力を生成することができます。このセクションでは、特に画像認識に焦点を当てます。

### MEAIを使った画像認識

画像認識は、単にAIモデルが画像に何が写っているかを教えるだけではありません。画像について質問することもできます。例えば、_「何人の人が写っていて、雨が降っていますか？」_ といった具合です。

では、モデルを試してみましょう。最初の写真に赤い靴が何足あるかをモデルに尋ねてみます。そして、次にドイツ語で書かれたレシートを分析して、チップの金額を知ることに挑戦します。

![例で使用する画像2枚を示した合成画像。1枚目はランナーの脚だけが写っている写真。2枚目はドイツのレストランのレシート](../../../translated_images/example-visual-image.e2fc4ffa5f01b3d65bb9bd5d23eebf97513bf486b761209b28fea06b63a11f6c.ja.png)

> 🧑‍💻**サンプルコード**: [こちらのサンプルコード](../../../03-CoreGenerativeAITechniques/src/Vision-01MEAI-GitHubModels)を参照してください。

1. MEAIとGitHub Modelsを使用するので、これまでと同様に`IChatClient`をインスタンス化します。また、チャット履歴を作成し始めます。

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

1. 次に、画像を`AIContent`オブジェクトに読み込み、それを会話の一部として設定し、モデルに送信して説明を求めます。

    ```csharp
    var imagePath = "FULL PATH TO THE IMAGE ON DISK";

    AIContent imageContent = new DataContent(File.ReadAllBytes(imagePath), "image/jpeg"); // the important part here is that we're loading it in bytes. The image could come from anywhere.

    var imageMessage = new ChatMessage(ChatRole.User, [imageContent]);

    messages.Add(imageMessage);

    var response = await chatClient.GetResponseAsync(messages);

    messages.Add(response.Message);

    Console.WriteLine(response.Message.Text);
    ```

1. その後、ドイツ語で書かれたレストランのレシートをモデルに解析させ、どれくらいのチップを残すべきかを確認します。

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

ここで強調したい点があります。我々は言語モデル、より正確にはテキストだけでなく画像（および音声）とのやり取りも可能なマルチモーダルモデルと会話をしています。そして、通常通りモデルとの会話を続けています。もちろん、モデルに送信するオブジェクトの種類は異なりますが、`AIContent` instead of a `string`, but the workflow is the same.

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

1. The first thing we'll do (after grabbing the key and region of the model's deployment) is instantiate a `SpeechTranslationConfig`オブジェクトを使用することで、英語の音声を入力し、それをスペイン語の文章に翻訳するようモデルに指示できます。

    ```csharp
    var speechKey = "<FROM YOUR MODEL DEPLOYMENT>";
    var speechRegion = "<FROM YOUR MODEL DEPLOYMENT>";

    var speechTranslationConfig = SpeechTranslationConfig.FromSubscription(speechKey, speechRegion);
    speechTranslationConfig.SpeechRecognitionLanguage = "en-US";
    speechTranslationConfig.AddTargetLanguage("es-ES");
    ```

1. 次に、マイクへのアクセスを取得し、モデルとの通信を行う`TranslationRecognizer`オブジェクトを作成します。

    ```csharp
    using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
    using var translationRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);
    ```

1. 最後に、モデルを呼び出し、その戻り値を処理する関数を設定します。
   
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

音声を処理するAIの使用は、これまで行ってきたこととは少し異なります。なぜなら、Azure AI Speechサービスを使用しているからです。しかし、音声をテキストに翻訳する結果は非常に強力です。

> 🙋 **助けが必要ですか？**: 問題が発生した場合は、[リポジトリで問題を報告してください](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new)。

Azure Open AIを使用したリアルタイム音声会話の方法を[デモする別の例](../../../03-CoreGenerativeAITechniques/src/Audio-02-RealTimeAudio)もありますので、ぜひチェックしてください！

## 追加リソース

- [AIと.NETを使った画像生成](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-openai-generate-images?tabs=azd&pivots=openai)

## 次回

.NETアプリケーションにビジョンとオーディオの機能を追加する方法を学びました。次のレッスンでは、ある程度自律的に行動できるAIを作成する方法を学びます。

👉 [AIエージェントをチェック](./04-agents.md)。

**免責事項**:  
本書類は、AI翻訳サービスを使用して機械的に翻訳されています。正確性を期すよう努めておりますが、自動翻訳には誤りや不正確さが含まれる可能性があります。本書類の原文が正式な情報源と見なされるべきです。重要な情報については、専門の人間による翻訳を推奨します。本翻訳の使用に起因する誤解や解釈の誤りについて、当方は一切の責任を負いません。