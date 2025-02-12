# Retrieval-Augmented Generation (RAG)

In this lesson learn how to use **Retrieval-Augmented Generation (RAG)** in your AI applications. This technique can be used to augment the response of a language model with information retrieved from a data store - or chat with your data!

---

**INSERT: LESSON 3 RAG VIDEO HERE**

Retrieval Augmented Generation (RAG) is a technique used to augment the response of a language model with information retrieved from a data store.

There are 2 main phases in a RAG architecture: **Retrieval** and **Generation**.

- **Retrieval**: When the user poses a prompt, the system employs a retrieval mechanism of some sort to gather information from an external knowledge store. The knowledge store could be a vector database or a document, amongst other things.
- **Generation**: The retrieved information is then used to augment the user's prompt. This AI model processes both the retrieved info and the user's prompt to product an enriched response.

## Benefits of RAG

- **Improved accuracy**: By augmenting the prompt with relevant information, the model can generate more accurate responses and reduce hallucinations.
- **Up-to-date information**: The model can retrieve the most recent information from the knowledge store. Remember, the language model has a knowledge cutoff date and augmenting the prompt with the most recent information can improve the response.
- **Domain-specific knowledge**: The model can be passed very specific domain information making the model more effective in niche situations.

## Embeddings!

We've held off as long as we could to introduce the concept of embeddings. In the retrieval phase of RAG we do not want to pass the entire data store to the model to generate the response. We only want to grab the most relevant information. 

So we need a way to compare the user's prompt with the data in the knowledge store. So we can pull out the minimum amount of information needed to augment the prompt.

Thus we need to have a way to represent the data in the knowledge store. This is where embeddings come in. Embeddings are a way to represent data in a vector space. This will allow us to mathematically compare the similarity of the user's prompt with the data in the knowledge store so we can retrieve the most relevant information.

You may have heard of vector databases. These are databases that store data in a vector space. This allows for very fast retrieval of information based on similarity. You don't need to use a vector database to use RAG, but it is a common use case.

## Implementing RAG

We'll use the Microsoft.Extension.AI along with the [Microsoft.Extensions.VectorData](https://www.nuget.org/packages/Microsoft.Extensions.VectorData.Abstractions/) and [Microsoft.SemanticKernel.Connectors.InMemory](https://www.nuget.org/packages/Microsoft.SemanticKernel.Connectors.InMemory) libraries to implement RAG below. 

> 🧑‍💻**Sample code:** You can follow along with the [sample code here](../03-CoreGenerativeAITechniques/src/RAGSimple-02MEAIVectorsMemory/).
> 
> You can also see how to implement a RAG app [using Semantic Kernel by itself in our sample source code here](./src/RAGSimple-01SK/).

### Populating the knowledge store

1. First we need some knowledge data to store. We'll use a POCO class that represents movies.

    ```csharp
    public class Movie
    {
        [VectorStoreRecordKey]
        public int Key { get; set; }

        [VectorStoreRecordData]
        public string Title { get; set; }

        [VectorStoreRecordData]
        public string Description { get; set; }

        [VectorStoreRecordVector(384, DistanceFunction.CosineSimilarity)]
        public ReadOnlyMemory<float> Vector { get; set; }
    }
    ```

    Using the attributes like `[VectorStoreRecordKey]` makes it easier for the vector store implementations to map POCO objects to their underlying data models.

2. Of course we're going to need that knowledge data populated. Create a list of `Movie` objects, and create an `InMemoryVectorStore` that will have a collection of movies.

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

3. Our next task then is to convert our knowledge store (the `movieData` object) into embeddings and then store them into the in-memory vector store. When we create the embeddings we'll use a different model - an embeddings model instead of a language model.

    ```csharp
    var endpoint = new Uri("https://models.inference.ai.azure.com");
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

    Our generator object is of an `IEmbeddingGenerator<string, Embedding<float>>` type. This means it is expecting inputs of `string` and outputs of `Embedding<float>`. We're again using GitHub Models and that means the **Microsoft.Extensions.AI.AzureAIInference** package. But you could use **Ollama** or **Azure OpenAI** just as easily.

> 🗒️**Note:** Generally you'll only be creating embeddings for your knowledge store once and then storing them. This won't be done every single time you run the application. But since we're using an in-memory store, we need to because the data gets wiped every time the application restarts.

### Retrieving the knowledge

1. Now for the retrieval phase. We need to query the vectorized knowledge store to find the most relevant information based on the user's prompt. And to query the vectorized knowledge store that means we'll need to get the user's prompt into an embedding vector.

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

### Generating the response

Now we're on to the generation portion of RAG. This is where we provide the language model the additional context that the retrieval portion just found so it can better formulate a response. This will be a lot like the chat completions we've seen before - except now we're providing the model with the user's prompt and the retrieved information.

If you remember from before we use `ChatMessage` objects when carrying on a conversation with the model which have roles of **System**, **User**, and **Assistant**. Most of the time we'll probably be setting the search results as a **User** message.

So we could do something like the following while looping through the results of the vector search:

```csharp

// assuming chatClient is instatiated as before to a language model
// assuming the vector search is done as above
// assuming List<ChatMessage> conversation object is already instantiated and has a system prompt

conversation.Add(new ChatMessage(ChatRole.User, query)); // this is the user prompt

// ... do the vector search

// add the search results to the conversation
await foreach (var result in searchResults.Results)
{
    conversation.Add(new ChatMessage(ChatRole.User, $"This move is playing nearby: {result.Record.Title} and it's about {result.Record.Description}"));
}

// send the conversation to the model
var response = await chatClient.CompleteAsync(conversation);

// add the assistant message to the conversation
conversation.Add(new ChatMessage(ChatRole.Assistant, response.Message));

//display the conversation
Console.WriteLine($"Bot:> {response.Message.Text});
```

> 🙋 **Need help?**: If you encounter any issues, [open an issue in the repository](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

## Additional resources

- [GenAI for Beginners: RAG and Vector Databases](https://github.com/microsoft/generative-ai-for-beginners/blob/main/15-rag-and-vector-databases/README.md)
- [Build a .NET Vector AI Search App](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-ai-chat-with-data?tabs=azd&pivots=openai)

## Next up

Now that you've seen what it takes to implement RAG, you can see how it can be a powerful tool in your AI applications. It can provide more accurate responses, up-to-date information, and domain-specific knowledge to your users.

👉 [Next up let's learn about adding Vision and Audio to your AI applications](03-vision-audio.md).
