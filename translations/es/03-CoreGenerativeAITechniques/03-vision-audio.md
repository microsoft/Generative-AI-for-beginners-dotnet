# Aplicaciones de IA para Visión y Audio

En esta lección, aprende cómo la IA de visión permite que tus aplicaciones generen e interpreten imágenes. La IA de audio permite a tus aplicaciones generar audio e incluso transcribirlo en tiempo real.

---

## Visión

[![Explicación de IA de visión](https://img.youtube.com/vi/QXbASt1KXuw/0.jpg)](https://youtu.be/QXbASt1KXuw?feature=shared)

_⬆️Haz clic en la imagen para ver el video⬆️_

Los enfoques de IA basados en visión se utilizan para generar e interpretar imágenes. Esto puede ser útil para una amplia gama de aplicaciones, como reconocimiento de imágenes, generación de imágenes y manipulación de imágenes. Los modelos actuales son multimodales, lo que significa que pueden aceptar una variedad de entradas, como texto, imágenes y audio, y generar una variedad de salidas. En este caso, nos vamos a enfocar en el reconocimiento de imágenes.

### Reconocimiento de imágenes con MEAI

El reconocimiento de imágenes va más allá de que el modelo de IA te diga qué cree que está presente en una imagen. También puedes hacer preguntas sobre la imagen, por ejemplo: _¿Cuántas personas hay presentes y está lloviendo?_

Bien, vamos a poner a prueba el modelo y preguntarle cuántos zapatos rojos hay en la primera foto y luego hacer que analice un recibo en alemán para saber cuánto debemos dejar de propina.

![Un collage que muestra ambas imágenes que usará el ejemplo. La primera son varios corredores pero solo se ven sus piernas. La segunda es un recibo de restaurante en alemán](../../../translated_images/example-visual-image.e2fc4ffa5f01b3d65bb9bd5d23eebf97513bf486b761209b28fea06b63a11f6c.es.png)

> 🧑‍💻**Código de ejemplo**: Puedes seguir [el código de ejemplo aquí](../../../03-CoreGenerativeAITechniques/src/Vision-01MEAI-AzureOpenAI).

1. Estamos utilizando MEAI y Azure OpenAI, así que instancia el `IChatClient` como lo hemos hecho antes. También comienza a crear un historial de conversación.

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

1. La siguiente parte es cargar la imagen en un objeto `AIContent` y configurarlo como parte de nuestra conversación, y luego enviarlo al modelo para que nos lo describa.

    ```csharp
    var imagePath = "FULL PATH TO THE IMAGE ON DISK";

    AIContent imageContent = new DataContent(File.ReadAllBytes(imagePath), "image/jpeg"); // the important part here is that we're loading it in bytes. The image could come from anywhere.

    var imageMessage = new ChatMessage(ChatRole.User, [imageContent]);

    messages.Add(imageMessage);

    var response = await chatClient.GetResponseAsync(messages);

    messages.Add(response.Message);

    Console.WriteLine(response.Message.Text);
    ```

1. Luego, para que el modelo trabaje en el recibo del restaurante -que está en alemán- y averigüe cuánto debemos dejar de propina:

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

Aquí hay un punto que quiero destacar. Estamos conversando con un modelo de lenguaje, o más apropiadamente, un modelo multimodal que puede manejar interacciones de texto, imagen (y audio). Y estamos llevando a cabo la conversación con el modelo de forma normal. Claro, es un tipo diferente de objeto el que estamos enviando al modelo, `AIContent` instead of a `string`, but the workflow is the same.

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

1. The first thing we'll do (after grabbing the key and region of the model's deployment) is instantiate a `SpeechTranslationConfig`. Esto nos permitirá dirigir al modelo para que tome audio hablado en inglés y lo traduzca a texto escrito en español.

    ```csharp
    var speechKey = "<FROM YOUR MODEL DEPLOYMENT>";
    var speechRegion = "<FROM YOUR MODEL DEPLOYMENT>";

    var speechTranslationConfig = SpeechTranslationConfig.FromSubscription(speechKey, speechRegion);
    speechTranslationConfig.SpeechRecognitionLanguage = "en-US";
    speechTranslationConfig.AddTargetLanguage("es-ES");
    ```

1. A continuación, necesitamos obtener acceso al micrófono y luego instanciar un objeto `TranslationRecognizer` que hará la comunicación con el modelo.

    ```csharp
    using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
    using var translationRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);
    ```

1. Finalmente, llamaremos al modelo y configuraremos una función para manejar su respuesta.
   
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

Usar IA para procesar audio es un poco diferente de lo que hemos estado haciendo porque estamos utilizando los servicios de Azure AI Speech para hacerlo, pero los resultados de traducir audio hablado a texto son bastante potentes.

> 🙋 **¿Necesitas ayuda?**: Si encuentras algún problema, [abre un issue en el repositorio](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

Tenemos otro ejemplo que [demuestra cómo realizar una conversación de audio en tiempo real con Azure Open AI](../../../03-CoreGenerativeAITechniques/src/Audio-02-RealTimeAudio) - ¡échale un vistazo!

## Recursos adicionales

- [Generar imágenes con IA y .NET](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-openai-generate-images?tabs=azd&pivots=openai)

## Lo que sigue

Has aprendido cómo agregar capacidades de visión y audio a tus aplicaciones .NET. En la próxima lección, descubre cómo crear IA que tenga cierta capacidad de actuar de manera autónoma.

👉 [Consulta Agentes de IA](./04-agents.md).

**Descargo de responsabilidad**:  
Este documento ha sido traducido utilizando servicios de traducción automática basados en inteligencia artificial. Si bien nos esforzamos por garantizar la precisión, tenga en cuenta que las traducciones automatizadas pueden contener errores o imprecisiones. El documento original en su idioma nativo debe considerarse la fuente autorizada. Para información crítica, se recomienda una traducción profesional realizada por humanos. No nos hacemos responsables de malentendidos o interpretaciones erróneas que puedan surgir del uso de esta traducción.