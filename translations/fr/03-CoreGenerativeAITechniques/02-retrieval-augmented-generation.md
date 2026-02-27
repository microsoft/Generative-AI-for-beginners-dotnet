# Génération Augmentée par Récupération (RAG)

Dans cette leçon, découvrez comment utiliser la **Génération Augmentée par Récupération (RAG)** dans vos applications d'IA. Cette technique permet d'enrichir la réponse d'un modèle de langage en y intégrant des informations récupérées depuis une base de données - ou d'interagir directement avec vos données !

---

[![Vidéo explicative sur RAG](https://img.youtube.com/vi/mY7O0OY2vho/0.jpg)](https://youtu.be/mY7O0OY2vho?feature=shared)

_⬆️Cliquez sur l'image pour regarder la vidéo⬆️_

La Génération Augmentée par Récupération (RAG) est une technique utilisée pour enrichir la réponse d'un modèle de langage avec des informations extraites d'une base de données.

L'architecture RAG repose sur deux phases principales : **Récupération** et **Génération**.

- **Récupération** : Lorsqu'un utilisateur soumet une requête, le système utilise un mécanisme de récupération pour collecter des informations depuis une base de connaissances externe. Cette base de connaissances peut être une base de données vectorielle, un document, ou autre.
- **Génération** : Les informations récupérées sont ensuite utilisées pour enrichir la requête de l'utilisateur. Le modèle d'IA traite à la fois les informations récupérées et la requête pour produire une réponse enrichie.

## Avantages de RAG

- **Précision améliorée** : En enrichissant la requête avec des informations pertinentes, le modèle peut générer des réponses plus précises et réduire les hallucinations.
- **Informations à jour** : Le modèle peut récupérer les informations les plus récentes depuis la base de connaissances. Rappelons que les modèles de langage ont une date limite de connaissances, et enrichir la requête avec des données récentes peut améliorer la réponse.
- **Connaissances spécifiques au domaine** : Le modèle peut intégrer des informations très spécifiques à un domaine, ce qui le rend plus efficace dans des contextes spécialisés.

## Les embeddings !

Nous avons attendu le plus longtemps possible avant d'introduire le concept des embeddings. Dans la phase de récupération de RAG, nous ne voulons pas transmettre l'intégralité de la base de données au modèle pour générer une réponse. Nous souhaitons uniquement extraire les informations les plus pertinentes.

Il nous faut donc un moyen de comparer la requête de l'utilisateur avec les données de la base de connaissances afin de récupérer le strict minimum nécessaire pour enrichir la requête.

C'est ici qu'interviennent les embeddings. Les embeddings permettent de représenter des données dans un espace vectoriel. Cela nous permet de comparer mathématiquement la similarité entre la requête de l'utilisateur et les données de la base de connaissances, pour récupérer les informations les plus pertinentes.

Vous avez peut-être entendu parler des bases de données vectorielles. Ce sont des bases de données qui stockent des données dans un espace vectoriel, permettant une récupération très rapide basée sur la similarité. Vous n'êtes pas obligé d'utiliser une base de données vectorielle pour appliquer RAG, mais c'est un cas d'usage courant.

## Implémentation de RAG

Nous utiliserons la bibliothèque Microsoft.Extension.AI ainsi que [Microsoft.Extensions.VectorData](https://www.nuget.org/packages/Microsoft.Extensions.VectorData.Abstractions/) et [Microsoft.Extensions.VectorData](https://www.nuget.org/packages/Microsoft.Extensions.VectorData.Abstractions/) pour implémenter RAG ci-dessous.

> 🧑‍💻**Exemple de code :** Suivez l'exemple de code [ici](../../../03-CoreGenerativeAITechniques/src/RAGSimple-02MEAIVectorsMemory).

### Remplissage de la base de connaissances

1. Tout d'abord, nous avons besoin de données à stocker. Nous allons utiliser une classe POCO qui représente des films.

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

    En utilisant des attributs comme `[VectorStoreKey]` makes it easier for the vector store implementations to map POCO objects to their underlying data models.

2. Of course we're going to need that knowledge data populated. Create a list of `Movie` objects, and create an `InMemoryVectorStore`, nous aurons une collection de films.

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

3. Ensuite, nous devons convertir notre base de connaissances (l'objet `movieData`) en embeddings, puis les stocker dans une base vectorielle en mémoire. Pour créer ces embeddings, nous utiliserons un modèle différent - un modèle d'embeddings au lieu d'un modèle de langage.

    ```csharp
    var endpoint = new Uri("https://models.github.ai/inference");
    var modelId = "text-embedding-3-small";
    var credential = new AzureKeyCredential(githubToken); // githubToken is retrieved from the environment variables

    IEmbeddingGenerator<string, Embedding<float>> generator =
            new EmbeddingsClient(endpoint, credential)
        .AsEmbeddingGenerator(modelId);

    foreach (var movie in movieData)
    {
        // generate the embedding vector for the movie description
        movie.Vector = await generator.GenerateEmbeddingVectorAsync(movie.Description);
        
        // add the overall movie to the in-memory vector store's movie collection
        await movies.UpsertAsync(movie);
    }
    ```

    Notre objet générateur est de type `IEmbeddingGenerator<string, Embedding<float>>` type. This means it is expecting inputs of `string` and outputs of `Embedding<float>`. Nous utilisons à nouveau les modèles GitHub, ce qui nécessite le package **Microsoft.Extensions.AI.AzureAIInference**. Cependant, vous pourriez tout aussi bien utiliser **Ollama** ou **Azure OpenAI**.

> 🗒️**Remarque :** En général, vous ne créerez les embeddings pour votre base de connaissances qu'une seule fois avant de les stocker. Cela ne sera pas fait à chaque fois que vous exécutez l'application. Cependant, comme nous utilisons une base en mémoire, il est nécessaire de recréer les embeddings à chaque redémarrage de l'application.

### Récupération des connaissances

1. Passons maintenant à la phase de récupération. Nous devons interroger la base vectorielle pour trouver les informations les plus pertinentes en fonction de la requête de l'utilisateur. Pour cela, il faut transformer la requête de l'utilisateur en un vecteur d'embedding.

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

### Génération de la réponse

Nous arrivons maintenant à la phase de génération de RAG. Ici, nous fournissons au modèle de langage le contexte supplémentaire trouvé lors de la phase de récupération, afin qu'il puisse formuler une réponse plus pertinente. Cela ressemblera beaucoup aux complétions de chat que nous avons vues précédemment - sauf que cette fois, nous fournissons au modèle à la fois la requête de l'utilisateur et les informations récupérées.

Rappelez-vous, lorsque nous avons utilisé des objets `ChatMessage` pour interagir avec le modèle, ceux-ci avaient des rôles comme **System**, **User**, et **Assistant**. La plupart du temps, nous définirons probablement les résultats de recherche comme un message **User**.

Ainsi, nous pourrions faire quelque chose comme ce qui suit en parcourant les résultats de la recherche vectorielle :

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

> 🙋 **Besoin d'aide ?** : Si vous rencontrez des problèmes, [ouvrez un ticket dans le dépôt](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

## Ressources supplémentaires

- [GenAI pour les débutants : RAG et bases de données vectorielles](https://github.com/microsoft/generative-ai-for-beginners/blob/main/15-rag-and-vector-databases/README.md)
- [Créer une application de recherche IA vectorielle avec .NET](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-ai-chat-with-data?tabs=azd&pivots=openai)

## Prochaine étape

Maintenant que vous avez vu comment implémenter RAG, vous comprenez à quel point cet outil peut être puissant dans vos applications d'IA. Il peut fournir des réponses plus précises, des informations actualisées, et des connaissances spécifiques à un domaine à vos utilisateurs.

👉 [Prochaine étape : apprenez à ajouter la vision et l'audio à vos applications d'IA](03-vision-audio.md).

**Avertissement** :  
Ce document a été traduit à l'aide de services de traduction automatique basés sur l'IA. Bien que nous nous efforcions d'assurer l'exactitude, veuillez noter que les traductions automatisées peuvent contenir des erreurs ou des inexactitudes. Le document original dans sa langue d'origine doit être considéré comme la source faisant autorité. Pour des informations critiques, il est recommandé de faire appel à une traduction humaine professionnelle. Nous déclinons toute responsabilité en cas de malentendus ou de mauvaises interprétations résultant de l'utilisation de cette traduction.