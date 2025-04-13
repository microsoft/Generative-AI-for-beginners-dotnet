# 비전 및 오디오 AI 앱

이 강의에서는 비전 AI를 사용하여 앱이 이미지를 생성하고 해석하는 방법을 배웁니다. 오디오 AI는 앱이 오디오를 생성하고 실시간으로 이를 기록하는 기능을 제공합니다.

---

## 비전

[![비전 AI 설명 영상](https://img.youtube.com/vi/QXbASt1KXuw/0.jpg)](https://youtu.be/QXbASt1KXuw?feature=shared)

_⬆️이미지를 클릭하면 영상을 시청할 수 있습니다⬆️_

비전을 기반으로 한 AI 접근법은 이미지를 생성하고 해석하는 데 사용됩니다. 이는 이미지 인식, 이미지 생성, 이미지 편집 등 다양한 응용 분야에서 유용하게 활용될 수 있습니다. 현재의 모델들은 멀티모달(multimodal)로, 텍스트, 이미지, 오디오와 같은 다양한 입력을 받아 다양한 출력을 생성할 수 있습니다. 이번에는 이미지 인식에 초점을 맞춰보겠습니다.

### MEAI를 활용한 이미지 인식

이미지 인식은 단순히 AI 모델이 이미지에 무엇이 있는지 말해주는 것을 넘어섭니다. 이미지에 대해 질문을 할 수도 있습니다. 예를 들어: _몇 명의 사람이 있고 비가 오는지?_  

좋아요, 이제 모델을 테스트해 보겠습니다. 첫 번째 사진에서 빨간 신발이 몇 켤레인지 모델이 말할 수 있는지 확인하고, 독일어로 된 영수증을 분석하여 팁으로 얼마를 남겨야 할지 알아보겠습니다.

![예제에서 사용할 두 이미지를 보여주는 합성 이미지. 첫 번째는 여러 명의 러너들의 다리만 보이는 사진이고, 두 번째는 독일 레스토랑 영수증](../../../translated_images/example-visual-image.e2fc4ffa5f01b3d65bb9bd5d23eebf97513bf486b761209b28fea06b63a11f6c.ko.png)

> 🧑‍💻**샘플 코드**: [샘플 코드](../../../03-CoreGenerativeAITechniques/src/Vision-01MEAI-GitHubModels)를 참고하세요.

1. MEAI와 GitHub 모델을 사용하고 있으므로, 이전과 마찬가지로 `IChatClient`을 초기화합니다. 또한 채팅 기록을 생성하기 시작합니다.

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

1. 다음 단계는 이미지를 `AIContent` 객체에 로드하고 이를 대화의 일부로 설정한 다음, 모델에 보내서 설명을 요청하는 것입니다.

    ```csharp
    var imagePath = "FULL PATH TO THE IMAGE ON DISK";

    AIContent imageContent = new DataContent(File.ReadAllBytes(imagePath), "image/jpeg"); // the important part here is that we're loading it in bytes. The image could come from anywhere.

    var imageMessage = new ChatMessage(ChatRole.User, [imageContent]);

    messages.Add(imageMessage);

    var response = await chatClient.GetResponseAsync(messages);

    messages.Add(response.Message);

    Console.WriteLine(response.Message.Text);
    ```

1. 그런 다음 독일어로 된 레스토랑 영수증을 분석하여 팁으로 얼마를 남겨야 할지 알아보겠습니다.

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

이 부분에서 강조하고 싶은 점은 우리가 언어 모델, 더 정확히는 텍스트뿐만 아니라 이미지(그리고 오디오)와 같은 상호작용도 처리할 수 있는 멀티모달 모델과 대화를 나누고 있다는 것입니다. 우리는 일반적인 대화를 이어가고 있지만, 모델에 보내는 객체가 `AIContent` instead of a `string`, but the workflow is the same.

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

1. The first thing we'll do (after grabbing the key and region of the model's deployment) is instantiate a `SpeechTranslationConfig` 객체처럼 다를 뿐입니다. 이를 통해 모델에 음성 영어를 받아들여 이를 스페인어로 번역하도록 지시할 수 있습니다.

    ```csharp
    var speechKey = "<FROM YOUR MODEL DEPLOYMENT>";
    var speechRegion = "<FROM YOUR MODEL DEPLOYMENT>";

    var speechTranslationConfig = SpeechTranslationConfig.FromSubscription(speechKey, speechRegion);
    speechTranslationConfig.SpeechRecognitionLanguage = "en-US";
    speechTranslationConfig.AddTargetLanguage("es-ES");
    ```

1. 다음으로 마이크에 접근 권한을 얻고, 모델과 통신을 담당할 `TranslationRecognizer` 객체를 생성합니다.

    ```csharp
    using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
    using var translationRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);
    ```

1. 마지막으로 모델을 호출하고 반환값을 처리할 함수를 설정합니다.
   
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

오디오를 처리하는 AI는 우리가 지금까지 해왔던 작업과는 조금 다릅니다. Azure AI Speech 서비스를 사용하기 때문입니다. 하지만 음성 오디오를 텍스트로 번역하는 결과는 매우 강력합니다.

> 🙋 **도움이 필요하신가요?**: 문제가 발생하면 [저장소에서 이슈를 열어주세요](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

다른 예제도 있습니다. [Azure Open AI를 사용하여 실시간 오디오 대화를 수행하는 방법](../../../03-CoreGenerativeAITechniques/src/Audio-02-RealTimeAudio)을 확인해보세요!

## 추가 자료

- [AI와 .NET으로 이미지 생성하기](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-openai-generate-images?tabs=azd&pivots=openai)

## 다음 단계

.NET 애플리케이션에 비전과 오디오 기능을 추가하는 방법을 배웠습니다. 다음 강의에서는 자율적으로 행동할 수 있는 능력을 가진 AI를 만드는 방법을 알아보세요.

👉 [AI 에이전트 확인하기](./04-agents.md).

**면책 조항**:  
이 문서는 기계 기반 AI 번역 서비스를 사용하여 번역되었습니다. 정확성을 위해 최선을 다하고 있지만, 자동 번역에는 오류나 부정확성이 포함될 수 있습니다. 원본 문서의 모국어 버전을 권위 있는 자료로 간주해야 합니다. 중요한 정보의 경우, 전문적인 인간 번역을 권장합니다. 이 번역 사용으로 인해 발생하는 오해나 잘못된 해석에 대해 당사는 책임을 지지 않습니다.