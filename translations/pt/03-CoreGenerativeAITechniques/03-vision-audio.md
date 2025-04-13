# Aplicativos de IA para Visão e Áudio

Nesta lição, você aprenderá como a IA de visão permite que seus aplicativos gerem e interpretem imagens. A IA de áudio possibilita que seus aplicativos gerem áudio e até mesmo o transcrevam em tempo real.

---

## Visão

[![Explicação sobre IA de Visão](https://img.youtube.com/vi/QXbASt1KXuw/0.jpg)](https://youtu.be/QXbASt1KXuw?feature=shared)

_⬆️Clique na imagem para assistir ao vídeo⬆️_

Abordagens de IA baseadas em visão são usadas para gerar e interpretar imagens. Isso pode ser útil para uma ampla gama de aplicações, como reconhecimento de imagens, geração de imagens e manipulação de imagens. Os modelos atuais são multimodais, o que significa que podem aceitar uma variedade de entradas, como texto, imagens e áudio, e gerar uma variedade de saídas. Neste caso, vamos focar no reconhecimento de imagens.

### Reconhecimento de imagens com MEAI

Reconhecimento de imagens vai além de o modelo de IA apenas dizer o que acha que está presente em uma imagem. Você também pode fazer perguntas sobre a imagem, por exemplo: _Quantas pessoas estão presentes e está chovendo?_

Ok - vamos colocar o modelo à prova e perguntar quantos sapatos vermelhos estão na primeira foto e, em seguida, pedir que ele analise um recibo em alemão para sabermos quanto de gorjeta devemos deixar.

![Um composto mostrando as duas imagens que o exemplo usará. A primeira mostra vários corredores, mas apenas suas pernas. A segunda é um recibo de restaurante em alemão](../../../translated_images/example-visual-image.e2fc4ffa5f01b3d65bb9bd5d23eebf97513bf486b761209b28fea06b63a11f6c.pt.png)

> 🧑‍💻**Código de exemplo**: Você pode [seguir o código de exemplo aqui](../../../03-CoreGenerativeAITechniques/src/Vision-01MEAI-GitHubModels).

1. Estamos usando MEAI e Modelos do GitHub, então instancie o `IChatClient` como temos feito. Também comece a criar um histórico de conversa.

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

1. A próxima parte é carregar a imagem em um objeto `AIContent` e configurá-lo como parte de nossa conversa, enviando-o para o modelo descrever para nós.

    ```csharp
    var imagePath = "FULL PATH TO THE IMAGE ON DISK";

    AIContent imageContent = new DataContent(File.ReadAllBytes(imagePath), "image/jpeg"); // the important part here is that we're loading it in bytes. The image could come from anywhere.

    var imageMessage = new ChatMessage(ChatRole.User, [imageContent]);

    messages.Add(imageMessage);

    var response = await chatClient.GetResponseAsync(messages);

    messages.Add(response.Message);

    Console.WriteLine(response.Message.Text);
    ```

1. Depois, peça ao modelo para analisar o recibo do restaurante - que está em alemão - para descobrir quanto de gorjeta devemos deixar:

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

Aqui está um ponto que quero enfatizar. Estamos conversando com um modelo de linguagem, ou mais apropriadamente, um modelo multimodal que pode lidar com interações de texto, imagem (e áudio). E estamos conduzindo a conversa com o modelo de forma normal. Claro, é um tipo diferente de objeto que estamos enviando ao modelo, `AIContent` instead of a `string`, but the workflow is the same.

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

1. The first thing we'll do (after grabbing the key and region of the model's deployment) is instantiate a `SpeechTranslationConfig`. Isso nos permitirá direcionar o modelo para que aceitemos entrada de áudio falado em inglês e traduzamos para texto escrito em espanhol.

    ```csharp
    var speechKey = "<FROM YOUR MODEL DEPLOYMENT>";
    var speechRegion = "<FROM YOUR MODEL DEPLOYMENT>";

    var speechTranslationConfig = SpeechTranslationConfig.FromSubscription(speechKey, speechRegion);
    speechTranslationConfig.SpeechRecognitionLanguage = "en-US";
    speechTranslationConfig.AddTargetLanguage("es-ES");
    ```

1. Em seguida, precisamos obter acesso ao microfone e criar um novo objeto `TranslationRecognizer`, que fará a comunicação com o modelo.

    ```csharp
    using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
    using var translationRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);
    ```

1. Por fim, chamaremos o modelo e configuraremos uma função para lidar com seu retorno.

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

Usar IA para processar áudio é um pouco diferente do que temos feito porque estamos utilizando os serviços de fala do Azure AI para isso, mas os resultados de traduzir áudio falado para texto são bastante impressionantes.

> 🙋 **Precisa de ajuda?**: Se você encontrar algum problema, [abra uma issue no repositório](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

Temos outro exemplo que [demonstra como realizar conversas de áudio em tempo real com o Azure Open AI](../../../03-CoreGenerativeAITechniques/src/Audio-02-RealTimeAudio) - confira!


## Recursos adicionais

- [Gerar imagens com IA e .NET](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-openai-generate-images?tabs=azd&pivots=openai)


## Próximos passos

Você aprendeu como adicionar capacidades de visão e áudio às suas aplicações .NET, na próxima lição descubra como criar uma IA com alguma habilidade de agir de forma autônoma.

👉 [Confira Agentes de IA](./04-agents.md).

**Aviso Legal**:  
Este documento foi traduzido utilizando serviços de tradução baseados em IA. Embora nos esforcemos para garantir a precisão, esteja ciente de que traduções automatizadas podem conter erros ou imprecisões. O documento original em seu idioma nativo deve ser considerado a fonte oficial. Para informações críticas, recomenda-se a tradução profissional humana. Não nos responsabilizamos por quaisquer mal-entendidos ou interpretações incorretas decorrentes do uso desta tradução.