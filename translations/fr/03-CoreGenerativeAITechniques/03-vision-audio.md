# Applications d'IA pour la vision et l'audio

Dans cette leçon, découvrez comment l'IA de vision permet à vos applications de générer et d'interpréter des images. L'IA audio permet à vos applications de générer de l'audio et même de le transcrire en temps réel.

---

## Vision

[![Explication sur l'IA de vision](https://img.youtube.com/vi/QXbASt1KXuw/0.jpg)](https://youtu.be/QXbASt1KXuw?feature=shared)

_⬆️Cliquez sur l'image pour regarder la vidéo⬆️_

Les approches basées sur l'IA de vision sont utilisées pour générer et interpréter des images. Cela peut être utile pour un large éventail d'applications, telles que la reconnaissance d'images, la génération d'images et la manipulation d'images. Les modèles actuels sont multimodaux, ce qui signifie qu'ils peuvent accepter une variété d'entrées, comme du texte, des images et de l'audio, et générer une variété de sorties. Dans ce cas, nous allons nous concentrer sur la reconnaissance d'images.

### Reconnaissance d'images avec MEAI

La reconnaissance d'images va au-delà de demander au modèle d'IA ce qu'il pense être présent dans une image. Vous pouvez également poser des questions sur l'image, par exemple : _Combien de personnes sont présentes et pleut-il ?_

Très bien - nous allons mettre le modèle à l'épreuve et lui demander s'il peut nous dire combien de chaussures rouges se trouvent sur la première photo, puis lui faire analyser un reçu en allemand pour savoir combien laisser comme pourboire.

![Un montage montrant les deux images utilisées dans l'exemple. La première montre plusieurs coureurs, mais uniquement leurs jambes. La seconde est un reçu de restaurant allemand](../../../translated_images/example-visual-image.e2fc4ffa5f01b3d65bb9bd5d23eebf97513bf486b761209b28fea06b63a11f6c.fr.png)

> 🧑‍💻**Code d'exemple** : Vous pouvez suivre [le code d'exemple ici](../../../03-CoreGenerativeAITechniques/src/Vision-01MEAI-AzureOpenAI).

1. Nous utilisons MEAI et les Azure OpenAI, donc instanciez le `IChatClient` comme nous l'avons fait jusqu'à présent. Commencez également à créer un historique de conversation.

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

1. La prochaine étape consiste à charger l'image dans un objet `AIContent`, à l'intégrer à notre conversation, puis à l'envoyer au modèle pour qu'il la décrive pour nous.

    ```csharp
    var imagePath = "FULL PATH TO THE IMAGE ON DISK";

    AIContent imageContent = new DataContent(File.ReadAllBytes(imagePath), "image/jpeg"); // the important part here is that we're loading it in bytes. The image could come from anywhere.

    var imageMessage = new ChatMessage(ChatRole.User, [imageContent]);

    messages.Add(imageMessage);

    var response = await chatClient.GetResponseAsync(messages);

    messages.Add(response.Message);

    Console.WriteLine(response.Message.Text);
    ```

1. Ensuite, pour demander au modèle de travailler sur le reçu de restaurant - qui est en allemand - afin de déterminer combien de pourboire nous devrions laisser :

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

Voici un point que je souhaite souligner. Nous dialoguons avec un modèle linguistique, ou plus précisément un modèle multimodal capable de gérer des interactions textuelles ainsi que des interactions avec des images (et de l'audio). Et nous poursuivons la conversation avec le modèle comme d'habitude. Bien sûr, c'est un type d'objet différent que nous envoyons au modèle, `AIContent` instead of a `string`, but the workflow is the same.

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

1. The first thing we'll do (after grabbing the key and region of the model's deployment) is instantiate a `SpeechTranslationConfig`. Cela nous permet d'indiquer au modèle que nous allons utiliser de l'anglais parlé et le traduire en espagnol écrit.

    ```csharp
    var speechKey = "<FROM YOUR MODEL DEPLOYMENT>";
    var speechRegion = "<FROM YOUR MODEL DEPLOYMENT>";

    var speechTranslationConfig = SpeechTranslationConfig.FromSubscription(speechKey, speechRegion);
    speechTranslationConfig.SpeechRecognitionLanguage = "en-US";
    speechTranslationConfig.AddTargetLanguage("es-ES");
    ```

1. Ensuite, nous devons obtenir l'accès au microphone et créer un nouvel objet `TranslationRecognizer` qui communiquera avec le modèle.

    ```csharp
    using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
    using var translationRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);
    ```

1. Enfin, nous appellerons le modèle et configurerons une fonction pour gérer son retour.

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

L'utilisation de l'IA pour traiter de l'audio est un peu différente de ce que nous avons fait jusqu'à présent, car nous utilisons les services Azure AI Speech pour ce faire, mais les résultats de la traduction de l'audio parlé en texte sont assez impressionnants.

> 🙋 **Besoin d'aide ?** : Si vous rencontrez des problèmes, [ouvrez une issue dans le dépôt](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

Nous avons un autre exemple qui [démontre comment réaliser une conversation audio en temps réel avec Azure Open AI](../../../03-CoreGenerativeAITechniques/src/Audio-02-RealTimeAudio) - allez y jeter un œil !

## Ressources supplémentaires

- [Générer des images avec l'IA et .NET](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-openai-generate-images?tabs=azd&pivots=openai)

## Et ensuite

Vous avez appris comment ajouter des capacités de vision et d'audio à vos applications .NET. Dans la prochaine leçon, découvrez comment créer une IA capable d'agir de manière autonome.

👉 [Découvrez les agents IA](./04-agents.md).

**Avertissement** :  
Ce document a été traduit à l'aide de services de traduction automatisée basés sur l'intelligence artificielle. Bien que nous nous efforcions d'assurer l'exactitude, veuillez noter que les traductions automatiques peuvent contenir des erreurs ou des inexactitudes. Le document original dans sa langue d'origine doit être considéré comme la source faisant autorité. Pour des informations critiques, il est recommandé de faire appel à une traduction professionnelle humaine. Nous déclinons toute responsabilité en cas de malentendus ou d'interprétations erronées résultant de l'utilisation de cette traduction.