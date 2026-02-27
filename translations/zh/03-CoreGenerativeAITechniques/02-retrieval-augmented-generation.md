# 检索增强生成 (RAG)

在本课程中，学习如何在您的 AI 应用中使用 **检索增强生成 (Retrieval-Augmented Generation, RAG)**。这种技术可以用来通过从数据存储中检索信息来增强语言模型的响应——也可以理解为“与您的数据对话”！

---

[![RAG 讲解视频](https://img.youtube.com/vi/mY7O0OY2vho/0.jpg)](https://youtu.be/mY7O0OY2vho?feature=shared)

_⬆️点击图片观看视频⬆️_

检索增强生成 (RAG) 是一种通过从数据存储中检索信息来增强语言模型响应的技术。

RAG 架构主要分为两个阶段：**检索** 和 **生成**。

- **检索**：当用户提出一个提示时，系统会使用某种检索机制从外部知识库中获取信息。知识库可以是向量数据库、文档或其他形式的存储。
- **生成**：检索到的信息会用于增强用户的提示。AI 模型会处理检索到的信息和用户的提示，生成一个更丰富的响应。

## RAG 的优势

- **提高准确性**：通过为提示增加相关信息，模型可以生成更准确的响应，并减少“幻觉”现象。
- **最新信息**：模型可以从知识库中检索最新的信息。请记住，语言模型有一个知识截止日期，通过为提示增加最新信息可以改善响应效果。
- **领域特定知识**：模型可以被提供非常具体的领域信息，从而在特定场景中表现得更高效。

## 嵌入 (Embeddings)!

我们尽量推迟了介绍嵌入的概念，但现在是时候了。在 RAG 的检索阶段，我们不希望将整个数据存储传递给模型来生成响应。我们只想提取最相关的信息。

因此，我们需要一种方法将用户的提示与知识库中的数据进行比较，以便提取最少量的必要信息来增强提示。

这就需要我们用一种方式来表示知识库中的数据。这就是嵌入的作用。嵌入是一种在向量空间中表示数据的方式。这使我们能够通过数学方法比较用户提示与知识库数据的相似性，从而检索出最相关的信息。

您可能听说过向量数据库。这是一种在向量空间中存储数据的数据库。它允许基于相似性非常快速地检索信息。尽管使用 RAG 并不一定需要向量数据库，但它是一个常见的使用场景。

## 实现 RAG

我们将使用 `Microsoft.Extension.AI`，以及 [Microsoft.Extensions.VectorData](https://www.nuget.org/packages/Microsoft.Extensions.VectorData.Abstractions/) 和 [Microsoft.Extensions.VectorData](https://www.nuget.org/packages/Microsoft.Extensions.VectorData.Abstractions/) 库来实现 RAG。

> 🧑‍💻**示例代码**：您可以在[这里的示例代码](../../../03-CoreGenerativeAITechniques/src/RAGSimple-02MEAIVectorsMemory)中跟随学习。

### 填充知识库

1. 首先，我们需要一些知识数据来存储。我们将使用一个代表电影的 POCO 类。

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

    使用类似 `[VectorStoreKey]` makes it easier for the vector store implementations to map POCO objects to their underlying data models.

2. Of course we're going to need that knowledge data populated. Create a list of `Movie` objects, and create an `InMemoryVectorStore` 的属性，这将包含一组电影数据。

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

3. 接下来，我们需要将知识库 (`movieData` 对象) 转换为嵌入，并将它们存储到内存向量存储中。当我们创建嵌入时，将使用不同的模型——一个嵌入模型，而不是语言模型。

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

    我们的生成器对象是一个 `IEmbeddingGenerator<string, Embedding<float>>` type. This means it is expecting inputs of `string` and outputs of `Embedding<float>`。我们再次使用 Azure OpenAI，这意味着需要 **Microsoft.Extensions.AI.AzureAIInference** 包。但您也可以同样轻松地使用 **Ollama** 或 **Azure OpenAI**。

> 🗒️**注意**：通常您只需要为您的知识库创建一次嵌入，然后将其存储起来。这不会在每次运行应用程序时都重新创建。但由于我们使用的是内存存储，因此每次应用程序重启时数据都会被清空，所以需要重新创建。

### 检索知识

1. 现在进入检索阶段。我们需要查询向量化的知识库，以根据用户的提示找到最相关的信息。而要查询向量化的知识库，这意味着我们需要将用户的提示转换为一个嵌入向量。

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

### 生成响应

现在进入 RAG 的生成阶段。这是我们向语言模型提供检索阶段找到的额外上下文信息的地方，以便它能够更好地生成响应。这将与之前看到的聊天补全类似——只是现在我们为模型提供了用户的提示和检索到的信息。

如果您还记得，在与模型进行对话时，我们使用 `ChatMessage` 对象，这些对象的角色包括 **System**、**User** 和 **Assistant**。大多数情况下，我们可能会将搜索结果设置为 **User** 消息。

因此，我们可以在遍历向量搜索结果时执行类似以下的操作：

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

> 🙋 **需要帮助？**：如果您遇到任何问题，请[在代码库中提交问题](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new)。

## 其他资源

- [生成式 AI 入门：RAG 和向量数据库](https://github.com/microsoft/generative-ai-for-beginners/blob/main/15-rag-and-vector-databases/README.md)
- [构建 .NET 向量 AI 搜索应用](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-ai-chat-with-data?tabs=azd&pivots=openai)

## 下一步

现在您已经了解了如何实现 RAG，可以看到它在 AI 应用中是一种强大的工具。它可以为用户提供更准确的响应、最新的信息以及领域特定的知识。

👉 [接下来让我们学习如何为您的 AI 应用添加视觉和音频功能](03-vision-audio.md)。

**免责声明**：  
本文档是使用基于机器的人工智能翻译服务翻译的。尽管我们努力确保准确性，但请注意，自动翻译可能包含错误或不准确之处。应以原始语言的文档作为权威来源。对于关键信息，建议寻求专业的人类翻译服务。对于因使用本翻译而引起的任何误解或误读，我们概不负责。