# Conceptos Básicos de una App de Chat

En esta lección, exploraremos los fundamentos para construir aplicaciones de chat utilizando completaciones de modelos de lenguaje y funciones en .NET. También veremos cómo usar Microsoft Extensions AI (MEAI) para crear chatbots, y cómo emplear crear funciones o funcionalidades que el chatbot puede invocar según la entrada del usuario.

---

## Completaciones de texto y chat

[![Video sobre completaciones de texto y chat](https://img.youtube.com/vi/Av1FCQf83QU/0.jpg)](https://youtu.be/Av1FCQf83QU?feature=shared)

_⬆️Haz clic en la imagen para ver el video⬆️_

Las completaciones de texto son posiblemente la forma más básica de interacción con el modelo de lenguaje en una aplicación de IA. Una completación de texto es una única respuesta generada por el modelo basada en la entrada o prompt proporcionado al modelo.

Una completación de texto por sí sola no es una aplicación de chat; es una interacción de "una vez y listo". Podrías usar completaciones de texto para tareas como resúmenes de contenido o análisis de sentimiento.

### Completaciones de texto

Veamos cómo usarías completaciones de texto utilizando la biblioteca **Microsoft.Extensions.AI** en .NET.

> 🧑‍💻**Código de ejemplo**: [Aquí tienes un ejemplo funcional de esta aplicación](../../../03-CoreGenerativeAITechniques/src/BasicChat-01MEAI) que puedes seguir.

```csharp

// this example illustrates using a model hosted on Azure OpenAI
IChatClient client = new AzureOpenAIClient(
    new Uri(config["endpoint"]),
    new ApiKeyCredential(config["apikey"]))
    .GetChatClient("gpt-5-mini")
    .AsIChatClient();

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

> 🗒️**Nota:** Este ejemplo muestra Azure OpenAI como el servicio de alojamiento. Si deseas usar Ollama, [revisa este ejemplo](../../../03-CoreGenerativeAITechniques/src/BasicChat-03Ollama) (se instancia un `IChatClient` diferente).
>
> Si deseas usar Microsoft Foundry, puedes usar el mismo código, pero necesitarás cambiar el endpoint y las credenciales.

> 🙋 **¿Necesitas ayuda?**: Si encuentras algún problema, [abre un issue en el repositorio](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

### Aplicaciones de chat

Construir una aplicación de chat es un poco más complejo. Habrá una conversación con el modelo, donde el usuario puede enviar prompts y el modelo responderá. Y como en cualquier conversación, necesitarás asegurarte de mantener el contexto o historial de la conversación para que todo tenga sentido.

#### Diferentes tipos de roles en el chat

Durante un chat con el modelo, los mensajes enviados al modelo pueden tener diferentes tipos. Aquí algunos ejemplos:

* **Sistema**: El mensaje del sistema guía el comportamiento de las respuestas del modelo. Sirve como la instrucción inicial o prompt que establece el contexto, tono y límites de la conversación. El usuario final de ese chat generalmente no ve este mensaje, pero es muy importante para dar forma a la conversación.
* **Usuario**: El mensaje del usuario es la entrada o prompt del usuario final. Puede ser una pregunta, una afirmación o un comando. El modelo usa este mensaje para generar una respuesta.
* **Asistente**: El mensaje del asistente es la respuesta generada por el modelo. Estos mensajes se basan en los mensajes del sistema y del usuario, y son generados por el modelo. El usuario final ve estos mensajes.

#### Gestión del historial del chat

Durante el chat con el modelo, necesitarás llevar un registro del historial del chat. Esto es importante porque el modelo generará respuestas basadas en el mensaje del sistema, y luego en todo el intercambio entre los mensajes del usuario y del asistente. Cada mensaje adicional agrega más contexto que el modelo usa para generar la siguiente respuesta.

Echemos un vistazo a cómo construirías una aplicación de chat utilizando MEAI.

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


> 🙋 **¿Necesitas ayuda?**: Si encuentras algún problema, [abre un issue en el repositorio](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

## Llamadas a funciones

[![Video explicativo sobre funciones](https://img.youtube.com/vi/i84GijmGlYU/0.jpg)](https://youtu.be/i84GijmGlYU?feature=shared)

_⬆️Haz clic en la imagen para ver el video⬆️_

Al construir aplicaciones de IA, no estás limitado solo a interacciones basadas en texto. Es posible extender la funcionalidad del chatbot llamando funciones predefinidas en tu código según la entrada del usuario. En otras palabras, las llamadas a funciones sirven como un puente entre el modelo y sistemas externos.

> 🧑‍💻**Código de ejemplo**: [Aquí tienes un ejemplo funcional de esta aplicación](../../../03-CoreGenerativeAITechniques/src/MEAIFunctions) que puedes seguir.

### Llamadas a funciones en aplicaciones de chat

Hay algunos pasos de configuración que necesitas realizar para llamar funciones con MEAI.

1. Primero, por supuesto, define la función que deseas que el chatbot pueda llamar. En este ejemplo, vamos a obtener el pronóstico del clima.

    ```csharp

    [Description("Get the weather")]
    static string GetTheWeather()
    {    
        var temperature = Random.Shared.Next(5, 20);

        var conditions = Random.Shared.Next(0, 1) == 0 ? "sunny" : "rainy";

        return $"The weather is {temperature} degrees C and {conditions}.";
    }

    ```

2. Luego, vamos a crear un objeto `ChatOptions` que le indicará a MEAI qué funciones están disponibles.

    ```csharp

    var chatOptions = new ChatOptions
    {
        Tools = [AIFunctionFactory.Create(GetTheWeather)]
    };

    ```

3. Cuando instanciemos el objeto `IChatClient`, querremos especificar que usaremos invocación de funciones.

    ```csharp
    IChatClient client = new AzureOpenAIClient(
    new Uri(config["endpoint"]),
    new ApiKeyCredential(config["apikey"]))
    .GetChatClient("gpt-5-mini")
    .AsIChatClient()
    .AsBuilder()
    .UseFunctionInvocation()  // here we're saying that we could be invoking functions!
    .Build();
    ```

4. Finalmente, cuando interactuemos con el modelo, enviaremos el objeto `ChatOptions` que especifica la función que el modelo podría llamar si necesita obtener información del clima.

    ```csharp
    var responseOne = await client.GetResponseAsync("What is today's date", chatOptions); // won't call the function

    var responseTwo = await client.GetResponseAsync("Should I bring an umbrella with me today?", chatOptions); // will call the function
    ```

> 🙋 **¿Necesitas ayuda?**: Si encuentras algún problema, [abre un issue en el repositorio](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

## Resumen

En esta lección, aprendimos cómo usar completaciones de texto, iniciar y gestionar una conversación de chat, y llamar funciones en aplicaciones de chat.

En la próxima lección, verás cómo comenzar a interactuar con datos y construir lo que se conoce como un chatbot con un modelo de Generación Aumentada por Recuperación (RAG), ¡y trabajar con visión y audio en una aplicación de IA!

## Recursos adicionales

* [Construye una app de chat con IA en .NET](https://learn.microsoft.com/dotnet/ai/quickstarts/get-started-openai?tabs=azd&pivots=openai)
* [Ejecuta una función local en .NET](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-azure-openai-tool?tabs=azd&pivots=openai)
* [Chatea con un modelo de IA local](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-local-ai)

## Próximos pasos

👉 [¡Construyamos una app RAG!](./02-retrieval-augmented-generation.md)

**Descargo de responsabilidad**:  
Este documento ha sido traducido utilizando servicios de traducción automática basados en inteligencia artificial. Si bien nos esforzamos por garantizar la precisión, tenga en cuenta que las traducciones automáticas pueden contener errores o imprecisiones. El documento original en su idioma nativo debe considerarse como la fuente autorizada. Para información crítica, se recomienda una traducción profesional realizada por humanos. No nos hacemos responsables de malentendidos o interpretaciones erróneas que puedan surgir del uso de esta traducción.
