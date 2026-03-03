# Grundlagen von Chat-Apps

In dieser Lektion werden wir die Grundlagen des Erstellens von Chat-Anwendungen mit Sprachmodell-Vervollständigungen und Funktionen in .NET untersuchen. Außerdem sehen wir uns an, wie Microsoft Extensions AI (MEAI) genutzt werden können, um Chatbots zu erstellen. Wir werden auch Funktionsaufrufe zu erstellen – Funktionen, die vom Chatbot basierend auf den Eingaben des Benutzers aufgerufen werden.

---

## Textvervollständigungen und Chat

[![Video zu Textvervollständigungen und Chat](https://img.youtube.com/vi/Av1FCQf83QU/0.jpg)](https://youtu.be/Av1FCQf83QU?feature=shared)

_⬆️Klicken Sie auf das Bild, um das Video anzusehen⬆️_

Textvervollständigungen sind möglicherweise die grundlegendste Form der Interaktion mit einem Sprachmodell in einer KI-Anwendung. Eine Textvervollständigung ist eine einzelne Antwort, die das Modell basierend auf der eingegebenen Eingabe oder Aufforderung generiert.

Eine Textvervollständigung allein ist keine Chat-Anwendung; es handelt sich um eine einmalige Interaktion. Textvervollständigungen können beispielsweise für Aufgaben wie Inhaltszusammenfassungen oder Sentiment-Analysen verwendet werden.

### Textvervollständigungen

Schauen wir uns an, wie Sie Textvervollständigungen mit der **Microsoft.Extensions.AI**-Bibliothek in .NET verwenden können.

> 🧑‍💻**Beispielcode**: [Hier ist ein funktionierendes Beispiel dieser Anwendung](../../../03-CoreGenerativeAITechniques/src/BasicChat-01MEAI), das Sie nachverfolgen können.

```csharp

// this example illustrates using a model hosted on Azure OpenAI
IChatClient client = new AzureOpenAIClient(
    new Uri(config["endpoint"]),
    new ApiKeyCredential(config["apikey"]))
    .GetChatClient("gpt-4o-mini")
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

> 🗒️**Hinweis:** Dieses Beispiel zeigt Azure OpenAI als Hosting-Service. Wenn Sie Ollama verwenden möchten, [sehen Sie sich dieses Beispiel an](../../../03-CoreGenerativeAITechniques/src/BasicChat-03Ollama) (es instanziiert einen anderen `IChatClient`).
>
> Wenn Sie Microsoft Foundry verwenden möchten, können Sie denselben Code verwenden, müssen jedoch den Endpunkt und die Anmeldedaten ändern.

> 🙋 **Benötigen Sie Hilfe?**: Wenn Sie auf Probleme stoßen, [öffnen Sie ein Issue im Repository](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

### Chat-Anwendungen

Das Erstellen einer Chat-Anwendung ist etwas komplexer. Es wird eine Konversation mit dem Modell geben, bei der der Benutzer Eingaben senden und das Modell antworten kann. Wie bei jeder Konversation müssen Sie sicherstellen, dass der Kontext oder die Historie der Unterhaltung beibehalten wird, damit alles Sinn ergibt.

#### Verschiedene Chat-Rollen

Während eines Chats mit dem Modell können die an das Modell gesendeten Nachrichten verschiedene Typen haben. Hier sind einige Beispiele:

* **System**: Die Systemnachricht leitet das Verhalten der Antworten des Modells. Sie dient als erste Anweisung oder Aufforderung, die den Kontext, den Ton und die Grenzen der Unterhaltung festlegt. Der Endbenutzer dieses Chats sieht diese Nachricht normalerweise nicht, aber sie ist sehr wichtig, um die Konversation zu gestalten.
* **Benutzer**: Die Benutzernachricht ist die Eingabe oder Aufforderung des Endbenutzers. Es kann sich um eine Frage, eine Aussage oder einen Befehl handeln. Das Modell verwendet diese Nachricht, um eine Antwort zu generieren.
* **Assistent**: Die Assistentennachricht ist die vom Modell generierte Antwort. Diese Nachrichten basieren auf den System- und Benutzernachrichten und werden vom Modell generiert. Der Endbenutzer sieht diese Nachrichten.

#### Verwaltung der Chathistorie

Während des Chats mit dem Modell müssen Sie die Chathistorie im Auge behalten. Dies ist wichtig, da das Modell Antworten basierend auf der Systemnachricht sowie allen Hin- und Her-Nachrichten zwischen Benutzer und Assistent generiert. Jede zusätzliche Nachricht fügt mehr Kontext hinzu, den das Modell verwendet, um die nächste Antwort zu erstellen.

Schauen wir uns an, wie Sie eine Chat-Anwendung mit MEAI erstellen würden.

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


> 🙋 **Benötigen Sie Hilfe?**: Wenn Sie auf Probleme stoßen, [öffnen Sie ein Issue im Repository](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

## Funktionsaufrufe

[![Video zur Erklärung von Funktionsaufrufen](https://img.youtube.com/vi/i84GijmGlYU/0.jpg)](https://youtu.be/i84GijmGlYU?feature=shared)

_⬆️Klicken Sie auf das Bild, um das Video anzusehen⬆️_

Beim Erstellen von KI-Anwendungen sind Sie nicht auf textbasierte Interaktionen beschränkt. Es ist möglich, die Funktionalität des Chatbots zu erweitern, indem vordefinierte Funktionen in Ihrem Code basierend auf Benutzereingaben aufgerufen werden. Mit anderen Worten, Funktionsaufrufe dienen als Brücke zwischen dem Modell und externen Systemen.

> 🧑‍💻**Beispielcode**: [Hier ist ein funktionierendes Beispiel dieser Anwendung](../../../03-CoreGenerativeAITechniques/src/MEAIFunctions), das Sie nachverfolgen können.

### Funktionsaufrufe in Chat-Anwendungen

Es gibt ein paar Schritte, die Sie einrichten müssen, um Funktionen mit MEAI aufzurufen.

1. Zuerst definieren Sie natürlich die Funktion, die der Chatbot aufrufen können soll. In diesem Beispiel holen wir uns die Wettervorhersage.

    ```csharp

    [Description("Get the weather")]
    static string GetTheWeather()
    {    
        var temperature = Random.Shared.Next(5, 20);

        var conditions = Random.Shared.Next(0, 1) == 0 ? "sunny" : "rainy";

        return $"The weather is {temperature} degrees C and {conditions}.";
    }

```

1. Als Nächstes erstellen wir ein `ChatOptions`-Objekt, das MEAI mitteilt, welche Funktionen verfügbar sind.

    ```csharp

    var chatOptions = new ChatOptions
    {
        Tools = [AIFunctionFactory.Create(GetTheWeather)]
    };

```

1. Wenn wir das `IChatClient`-Objekt instanziieren, geben wir an, dass wir Funktionsaufrufe verwenden möchten.

    ```csharp
    IChatClient client = new AzureOpenAIClient(
    new Uri(config["endpoint"]),
    new ApiKeyCredential(config["apikey"]))
    .GetChatClient("gpt-4o-mini")
    .AsIChatClient()
    .AsBuilder()
    .UseFunctionInvocation()  // here we're saying that we could be invoking functions!
    .Build();
    ```

1. Schließlich senden wir bei der Interaktion mit dem Modell das `ChatOptions`-Objekt, das die Funktion angibt, die das Modell aufrufen könnte, falls es die Wetterinformationen benötigt.

    ```csharp
    var responseOne = await client.GetResponseAsync("What is today's date", chatOptions); // won't call the function

    var responseTwo = await client.GetResponseAsync("Should I bring an umbrella with me today?", chatOptions); // will call the function
    ```

> 🙋 **Benötigen Sie Hilfe?**: Wenn Sie auf Probleme stoßen, [öffnen Sie ein Issue im Repository](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

## Zusammenfassung

In dieser Lektion haben wir gelernt, wie man Textvervollständigungen verwendet, eine Chat-Konversation startet und verwaltet sowie Funktionen in Chat-Anwendungen aufruft.

In der nächsten Lektion erfahren Sie, wie Sie mit Daten chatten und ein sogenanntes Retrieval Augmented Generation (RAG)-Modell erstellen – und wie Sie mit Vision und Audio in einer KI-Anwendung arbeiten!

## Zusätzliche Ressourcen

* [Erstellen Sie eine KI-Chat-App mit .NET](https://learn.microsoft.com/dotnet/ai/quickstarts/get-started-openai?tabs=azd&pivots=openai)
* [Führen Sie eine lokale .NET-Funktion aus](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-azure-openai-tool?tabs=azd&pivots=openai)
* [Chatten Sie mit einem lokalen KI-Modell](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-local-ai)

## Als Nächstes

👉 [Erstellen wir eine RAG-App!](./02-retrieval-augmented-generation.md)

**Haftungsausschluss**:  
Dieses Dokument wurde mit KI-gestützten maschinellen Übersetzungsdiensten übersetzt. Obwohl wir uns um Genauigkeit bemühen, weisen wir darauf hin, dass automatisierte Übersetzungen Fehler oder Ungenauigkeiten enthalten können. Das Originaldokument in seiner jeweiligen Ausgangssprache sollte als maßgebliche Quelle betrachtet werden. Für kritische Informationen wird eine professionelle menschliche Übersetzung empfohlen. Wir übernehmen keine Haftung für Missverständnisse oder Fehlinterpretationen, die sich aus der Nutzung dieser Übersetzung ergeben.
