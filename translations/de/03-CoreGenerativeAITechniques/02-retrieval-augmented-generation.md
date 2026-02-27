# Retrieval-Augmented Generation (RAG)

In dieser Lektion lernen Sie, wie Sie **Retrieval-Augmented Generation (RAG)** in Ihren KI-Anwendungen nutzen können. Mit dieser Technik können Sie die Antworten eines Sprachmodells mit Informationen aus einem Datenspeicher anreichern – oder mit Ihren Daten chatten!

---

[![RAG Erklärvideo](https://img.youtube.com/vi/mY7O0OY2vho/0.jpg)](https://youtu.be/mY7O0OY2vho?feature=shared)

_⬆️Klicken Sie auf das Bild, um das Video anzusehen⬆️_

Retrieval-Augmented Generation (RAG) ist eine Technik, die verwendet wird, um die Antworten eines Sprachmodells mit Informationen aus einem Datenspeicher zu erweitern.

Eine RAG-Architektur besteht aus zwei Hauptphasen: **Retrieval** und **Generation**.

- **Retrieval (Abruf)**: Wenn der Benutzer eine Eingabe stellt, verwendet das System einen Abrufmechanismus, um Informationen aus einem externen Wissensspeicher zu sammeln. Der Wissensspeicher kann eine Vektordatenbank, ein Dokument oder Ähnliches sein.
- **Generation (Erzeugung)**: Die abgerufenen Informationen werden genutzt, um die Eingabe des Benutzers zu erweitern. Das KI-Modell verarbeitet sowohl die abgerufenen Informationen als auch die Eingabe des Benutzers, um eine bereicherte Antwort zu erzeugen.

## Vorteile von RAG

- **Verbesserte Genauigkeit**: Durch die Anreicherung der Eingabe mit relevanten Informationen kann das Modell genauere Antworten generieren und Halluzinationen reduzieren.
- **Aktuelle Informationen**: Das Modell kann die neuesten Informationen aus dem Wissensspeicher abrufen. Denken Sie daran, dass das Sprachmodell ein Wissensstichtagsdatum hat, und die Anreicherung der Eingabe mit den neuesten Informationen die Antworten verbessern kann.
- **Fachspezifisches Wissen**: Das Modell kann mit sehr spezifischen Informationen aus einem bestimmten Fachgebiet versorgt werden, was es effektiver in Nischensituationen macht.

## Embeddings!

Wir haben uns so lange wie möglich zurückgehalten, das Konzept der Embeddings einzuführen. In der Retrieval-Phase von RAG möchten wir nicht den gesamten Datenspeicher an das Modell übergeben, um eine Antwort zu generieren. Stattdessen möchten wir nur die relevantesten Informationen abrufen.

Wir brauchen also eine Möglichkeit, die Eingabe des Benutzers mit den Daten im Wissensspeicher zu vergleichen, um die minimal benötigten Informationen zur Erweiterung der Eingabe herauszufiltern.

Hier kommen Embeddings ins Spiel. Embeddings sind eine Möglichkeit, Daten in einem Vektorraum darzustellen. Damit können wir mathematisch die Ähnlichkeit zwischen der Benutzereingabe und den Daten im Wissensspeicher vergleichen, um die relevantesten Informationen abzurufen.

Vielleicht haben Sie schon von Vektordatenbanken gehört. Diese speichern Daten in einem Vektorraum, was eine sehr schnelle Informationsabfrage basierend auf Ähnlichkeit ermöglicht. Sie müssen keine Vektordatenbank verwenden, um RAG zu nutzen, aber es ist ein häufiger Anwendungsfall.

## RAG implementieren

Wir verwenden Microsoft.Extension.AI zusammen mit den Bibliotheken [Microsoft.Extensions.VectorData](https://www.nuget.org/packages/Microsoft.Extensions.VectorData.Abstractions/) und [Microsoft.Extensions.VectorData](https://www.nuget.org/packages/Microsoft.Extensions.VectorData.Abstractions/), um RAG zu implementieren.

> 🧑‍💻**Beispielcode:** Sie können dem [Beispielcode hier](../../../03-CoreGenerativeAITechniques/src/RAGSimple-02MEAIVectorsMemory) folgen.  

### Den Wissensspeicher befüllen

1. Zuerst benötigen wir einige Wissensdaten, die gespeichert werden sollen. Wir verwenden eine POCO-Klasse, die Filme repräsentiert.

    ```csharp
    public class Movie
    {
        [VectorStoreKey]
        public int Key { get; set; }

        [VectorStoreData]
        public string Title { get; set; }

        [VectorStoreData]
        public string Description { get; set; }

        [VectorStoreVector(384, DistanceFunction.CosineSimilarity)]
        public ReadOnlyMemory<float> Vector { get; set; }
    }
    ```

    Mit Attributen wie `[VectorStoreKey]` makes it easier for the vector store implementations to map POCO objects to their underlying data models.

2. Of course we're going to need that knowledge data populated. Create a list of `Movie` objects, and create an `InMemoryVectorStore`, die eine Sammlung von Filmen enthalten wird.

    ```csharp
    var movieData = new List<Movie>
    {
        new Movie { Key = 1, Title = "The Matrix", Description = "A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers." },
        new Movie { Key = 2, Title = "Inception", Description = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O." },
        new Movie { Key = 3, Title = "Interstellar", Description = "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival." }
    };

    var vectorStore = new InMemoryVectorStore();
    var movies = vectorStore.GetCollection<int, Movie>("movies");
    await movies.CreateCollectionIfNotExistsAsync();

    ```

3. Als Nächstes müssen wir unseren Wissensspeicher (das `movieData`-Objekt) in Embeddings umwandeln und diese dann im In-Memory-Vektorspeicher speichern. Dabei verwenden wir ein anderes Modell – ein Embeddings-Modell anstelle eines Sprachmodells.

    ```csharp
    var endpoint = new Uri("https://<your-endpoint>.services.ai.azure.com/");
    var modelId = "text-embedding-3-small";
    var credential = new AzureKeyCredential(githubToken); // githubToken is retrieved from the environment variables

    IEmbeddingGenerator<string, Embedding<float>> generator =
            new AzureOpenAIClient(new Uri(config["endpoint"]), new ApiKeyCredential(config["apikey"])).GetEmbeddingClient("text-embedding-3-small")
        .AsEmbeddingGenerator(modelId);

    foreach (var movie in movieData)
    {
        // generate the embedding vector for the movie description
        movie.Vector = await generator.GenerateEmbeddingVectorAsync(movie.Description);
        
        // add the overall movie to the in-memory vector store's movie collection
        await movies.UpsertAsync(movie);
    }
    ```

    Unser Generator-Objekt ist vom Typ `IEmbeddingGenerator<string, Embedding<float>>` type. This means it is expecting inputs of `string` and outputs of `Embedding<float>`. Wir verwenden erneut Azure OpenAI, was das **Microsoft.Extensions.AI.AzureAIInference**-Paket erfordert. Sie könnten jedoch genauso gut **Ollama** oder **Azure OpenAI** verwenden.

> 🗒️**Hinweis:** Normalerweise erstellen Sie Embeddings für Ihren Wissensspeicher nur einmal und speichern sie dann. Dies wird nicht jedes Mal durchgeführt, wenn Sie die Anwendung starten. Da wir jedoch einen In-Memory-Speicher verwenden, müssen wir dies tun, da die Daten bei jedem Neustart der Anwendung gelöscht werden.

### Wissen abrufen

1. Nun zur Retrieval-Phase. Wir müssen den vektorisierten Wissensspeicher abfragen, um die relevantesten Informationen basierend auf der Benutzereingabe zu finden. Dazu müssen wir die Benutzereingabe in einen Embedding-Vektor umwandeln.

    ```csharp
    // generate the embedding vector for the user's prompt
    var query = "I want to see family friendly movie";
    var queryEmbedding = await generator.GenerateEmbeddingVectorAsync(query);

    var searchOptions = new VectorSearchOptions
    {
        Top = 1,
        VectorPropertyName = "Vector"
    };

    // search the knowledge store based on the user's prompt
    var searchResults = await movies.VectorizedSearchAsync(queryEmbedding, searchOptions);

    // let's see the results just so we know what they look like
    await foreach (var result in searchResults.Results)
    {
        Console.WriteLine($"Title: {result.Record.Title}");
        Console.WriteLine($"Description: {result.Record.Description}");
        Console.WriteLine($"Score: {result.Score}");
        Console.WriteLine();
    }
    ```

### Die Antwort generieren

Nun kommen wir zur Generation-Phase von RAG. Hier wird das Sprachmodell mit dem zusätzlichen Kontext, der in der Retrieval-Phase gefunden wurde, versorgt, um eine bessere Antwort zu formulieren. Dies ähnelt den Chat-Komplettierungen, die wir bereits gesehen haben – nur dass wir dem Modell jetzt sowohl die Benutzereingabe als auch die abgerufenen Informationen bereitstellen.

Wenn Sie sich erinnern, verwenden wir `ChatMessage`-Objekte, wenn wir mit dem Modell interagieren. Diese haben die Rollen **System**, **User** und **Assistant**. Meistens werden wir die Suchergebnisse wahrscheinlich als **User**-Nachricht setzen.

Wir könnten also so etwas wie das Folgende tun, während wir die Ergebnisse der Vektorsuche durchlaufen:

```csharp

// assuming chatClient is instatiated as before to a language model
// assuming the vector search is done as above
// assuming List<ChatMessage> conversation object is already instantiated and has a system prompt

conversation.Add(new ChatMessage(ChatRole.User, query)); // this is the user prompt

// ... do the vector search

// add the search results to the conversation
await foreach (var result in searchResults.Results)
{
    conversation.Add(new ChatMessage(ChatRole.User, $"This movie is playing nearby: {result.Record.Title} and it's about {result.Record.Description}"));
}

// send the conversation to the model
var response = await chatClient.GetResponseAsync(conversation);

// add the assistant message to the conversation
conversation.Add(new ChatMessage(ChatRole.Assistant, response.Message));

//display the conversation
Console.WriteLine($"Bot:> {response.Message.Text});
```

> 🙋 **Brauchen Sie Hilfe?**: Wenn Sie auf Probleme stoßen, [öffnen Sie ein Issue im Repository](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

## Zusätzliche Ressourcen

- [GenAI für Anfänger: RAG und Vektordatenbanken](https://github.com/microsoft/generative-ai-for-beginners/blob/main/15-rag-and-vector-databases/README.md)
- [Erstellen Sie eine .NET Vector AI Search App](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-ai-chat-with-data?tabs=azd&pivots=openai)

## Nächste Schritte

Nachdem Sie nun gesehen haben, wie RAG implementiert wird, können Sie erkennen, wie es ein leistungsstarkes Werkzeug für Ihre KI-Anwendungen sein kann. Es kann genauere Antworten, aktuelle Informationen und fachspezifisches Wissen für Ihre Benutzer bereitstellen.

👉 [Als Nächstes lernen wir, wie man Vision und Audio zu KI-Anwendungen hinzufügt](03-vision-audio.md).

**Haftungsausschluss**:  
Dieses Dokument wurde mit KI-gestützten maschinellen Übersetzungsdiensten übersetzt. Obwohl wir uns um Genauigkeit bemühen, weisen wir darauf hin, dass automatisierte Übersetzungen Fehler oder Ungenauigkeiten enthalten können. Das Originaldokument in seiner ursprünglichen Sprache sollte als maßgebliche Quelle betrachtet werden. Für kritische Informationen wird eine professionelle menschliche Übersetzung empfohlen. Wir übernehmen keine Haftung für Missverständnisse oder Fehlinterpretationen, die sich aus der Nutzung dieser Übersetzung ergeben.