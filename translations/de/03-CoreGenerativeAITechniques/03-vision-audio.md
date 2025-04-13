# Vision- und Audio-AI-Apps

In dieser Lektion erfährst du, wie Vision-AI deinen Apps ermöglicht, Bilder zu generieren und zu interpretieren. Audio-AI bietet deinen Apps die Möglichkeit, Audio zu erzeugen und es sogar in Echtzeit zu transkribieren.

---

## Vision

[![Vision AI-Erklärung](https://img.youtube.com/vi/QXbASt1KXuw/0.jpg)](https://youtu.be/QXbASt1KXuw?feature=shared)

_⬆️Klicke auf das Bild, um das Video anzusehen⬆️_

Vision-basierte AI-Ansätze werden verwendet, um Bilder zu generieren und zu interpretieren. Dies kann für eine Vielzahl von Anwendungen nützlich sein, wie z. B. Bilderkennung, Bildgenerierung und Bildbearbeitung. Aktuelle Modelle sind multimodal, was bedeutet, dass sie verschiedene Eingaben wie Text, Bilder und Audio akzeptieren und unterschiedliche Ausgaben erzeugen können. In diesem Fall konzentrieren wir uns auf die Bilderkennung.

### Bilderkennung mit MEAI

Bilderkennung bedeutet mehr, als dass das AI-Modell dir sagt, was es in einem Bild erkennt. Du kannst dem Modell auch Fragen zum Bild stellen, zum Beispiel: _Wie viele Personen sind zu sehen, und regnet es?_

Okay – wir werden das Modell auf die Probe stellen und es fragen, wie viele rote Schuhe auf dem ersten Foto zu sehen sind, und es dann eine Quittung auf Deutsch analysieren lassen, damit wir wissen, wie viel Trinkgeld wir geben sollen.

![Eine Komposition mit beiden Bildern, die im Beispiel verwendet werden. Das erste zeigt mehrere Läufer, aber nur deren Beine. Das zweite ist eine deutsche Restaurantquittung](../../../translated_images/example-visual-image.e2fc4ffa5f01b3d65bb9bd5d23eebf97513bf486b761209b28fea06b63a11f6c.de.png)

> 🧑‍💻**Beispielcode**: Du kannst [dem Beispielcode hier folgen](../../../03-CoreGenerativeAITechniques/src/Vision-01MEAI-GitHubModels).

1. Wir verwenden MEAI und GitHub-Modelle, also instanziere die `IChatClient` wie gewohnt. Beginne auch damit, eine Chat-Historie zu erstellen.

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

1. Im nächsten Schritt wird das Bild in ein `AIContent`-Objekt geladen, das als Teil unseres Gesprächs verwendet wird. Dieses wird dann an das Modell gesendet, damit es für uns beschrieben wird.

    ```csharp
    var imagePath = "FULL PATH TO THE IMAGE ON DISK";

    AIContent imageContent = new DataContent(File.ReadAllBytes(imagePath), "image/jpeg"); // the important part here is that we're loading it in bytes. The image could come from anywhere.

    var imageMessage = new ChatMessage(ChatRole.User, [imageContent]);

    messages.Add(imageMessage);

    var response = await chatClient.GetResponseAsync(messages);

    messages.Add(response.Message);

    Console.WriteLine(response.Message.Text);
    ```

1. Danach lassen wir das Modell die Restaurantquittung analysieren – die auf Deutsch ist –, um herauszufinden, wie viel Trinkgeld wir geben sollten:

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

Hier möchte ich einen wichtigen Punkt hervorheben. Wir führen ein Gespräch mit einem Sprachmodell, genauer gesagt mit einem multimodalen Modell, das sowohl Text- als auch Bild- (und Audio-)Interaktionen verarbeiten kann. Und wir setzen das Gespräch mit dem Modell wie gewohnt fort. Sicher, es ist ein anderer Objekttyp, den wir an das Modell senden, `AIContent` instead of a `string`, but the workflow is the same.

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

1. The first thing we'll do (after grabbing the key and region of the model's deployment) is instantiate a `SpeechTranslationConfig`-Objekt. Dies ermöglicht es uns, dem Modell mitzuteilen, dass wir gesprochenes Englisch aufnehmen und in geschriebenes Spanisch übersetzen.

    ```csharp
    var speechKey = "<FROM YOUR MODEL DEPLOYMENT>";
    var speechRegion = "<FROM YOUR MODEL DEPLOYMENT>";

    var speechTranslationConfig = SpeechTranslationConfig.FromSubscription(speechKey, speechRegion);
    speechTranslationConfig.SpeechRecognitionLanguage = "en-US";
    speechTranslationConfig.AddTargetLanguage("es-ES");
    ```

1. Als Nächstes benötigen wir Zugriff auf das Mikrofon und erstellen ein neues `TranslationRecognizer`-Objekt, das die Kommunikation mit dem Modell übernimmt.

    ```csharp
    using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
    using var translationRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);
    ```

1. Schließlich rufen wir das Modell auf und richten eine Funktion ein, um dessen Rückgabe zu verarbeiten.
   
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

Die Verwendung von AI zur Verarbeitung von Audio unterscheidet sich etwas von dem, was wir bisher getan haben, da wir Azure AI Speech Services nutzen. Die Ergebnisse der Übersetzung von gesprochenem Audio in Text sind jedoch beeindruckend.

> 🙋 **Brauchst du Hilfe?**: Falls du auf Probleme stößt, [eröffne ein Issue im Repository](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

Wir haben ein weiteres Beispiel, das [zeigt, wie man Echtzeit-Audiogespräche mit Azure Open AI führt](../../../03-CoreGenerativeAITechniques/src/Audio-02-RealTimeAudio) – schau es dir an!


## Zusätzliche Ressourcen

- [Bilder mit AI und .NET generieren](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-openai-generate-images?tabs=azd&pivots=openai)


## Als Nächstes

Du hast gelernt, wie du Vision- und Audio-Funktionen zu deinen .NET-Anwendungen hinzufügen kannst. In der nächsten Lektion erfährst du, wie du AI erstellst, die eine gewisse Autonomie besitzt.

👉 [Schau dir AI-Agents an](./04-agents.md).

**Haftungsausschluss**:  
Dieses Dokument wurde mithilfe von KI-gestützten maschinellen Übersetzungsdiensten übersetzt. Obwohl wir uns um Genauigkeit bemühen, weisen wir darauf hin, dass automatisierte Übersetzungen Fehler oder Ungenauigkeiten enthalten können. Das Originaldokument in seiner ursprünglichen Sprache sollte als maßgebliche Quelle betrachtet werden. Für kritische Informationen wird eine professionelle menschliche Übersetzung empfohlen. Wir übernehmen keine Haftung für Missverständnisse oder Fehlinterpretationen, die durch die Nutzung dieser Übersetzung entstehen.