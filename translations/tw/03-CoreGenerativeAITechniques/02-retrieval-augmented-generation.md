# 檢索增強生成 (Retrieval-Augmented Generation, RAG)

在本課程中，學習如何在您的 AI 應用中使用 **檢索增強生成 (RAG)** 技術。這項技術可以用來透過從資料庫檢索的資訊來增強語言模型的回應——也就是與您的資料互動！

---

[![RAG 解說影片](https://img.youtube.com/vi/mY7O0OY2vho/0.jpg)](https://youtu.be/mY7O0OY2vho?feature=shared)

_⬆️點擊圖片觀看影片⬆️_

檢索增強生成 (RAG) 是一種技術，用於透過從資料庫檢索的資訊來增強語言模型的回應。

RAG 架構主要包含兩個階段：**檢索** 和 **生成**。

- **檢索**：當使用者提出一個提示時，系統會使用某種檢索機制，從外部知識庫中收集相關資訊。這個知識庫可以是向量資料庫、文件等。
- **生成**：接著，檢索到的資訊會用來增強使用者的提示。AI 模型會處理這些檢索到的資訊以及使用者的提示，生成一個更豐富的回應。

## RAG 的好處

- **提升準確性**：透過為提示增強相關資訊，模型可以生成更準確的回應並減少幻覺現象。
- **即時資訊**：模型可以從知識庫檢索最新的資訊。記住，語言模型有知識截止日期，透過增強提示以包含最新資訊可以改善回應。
- **領域專精知識**：模型可以接收非常專業的領域資訊，使其在特定情境中更有效。

## 嵌入 (Embeddings)!

我們儘可能延後介紹嵌入的概念。在 RAG 的檢索階段，我們並不希望將整個資料庫傳遞給模型來生成回應，而是只想取得最相關的資訊。

因此，我們需要一種方法來比較使用者的提示與知識庫中的資料，從而提取出最少量的必要資訊來增強提示。

這就是嵌入的用途。嵌入是一種將資料表示在向量空間中的方法。這讓我們可以數學上比較使用者提示與知識庫資料的相似性，從而檢索出最相關的資訊。

您可能聽說過向量資料庫。這些資料庫將資料存儲在向量空間中，從而可以根據相似性快速檢索資訊。使用 RAG 並不一定需要使用向量資料庫，但這是一種常見的應用場景。

## 實現 RAG

我們將使用 Microsoft.Extension.AI 與 [Microsoft.Extensions.VectorData](https://www.nuget.org/packages/Microsoft.Extensions.VectorData.Abstractions/) 和 [Microsoft.SemanticKernel.Connectors.InMemory](https://www.nuget.org/packages/Microsoft.SemanticKernel.Connectors.InMemory) 庫來實現 RAG。

> 🧑‍💻**範例程式碼：** 您可以參考 [這裡的範例程式碼](../../../03-CoreGenerativeAITechniques/src/RAGSimple-02MEAIVectorsMemory)。
> 
> 有關已存檔的 Semantic Kernel RAG 範例，請參閱[已棄用的範例](../../../samples/deprecated/)資料夾。

### 填充知識庫

1. 首先，我們需要一些知識數據來存儲。我們將使用一個表示電影的 POCO 類別。

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

    使用類似 `[VectorStoreKey]` makes it easier for the vector store implementations to map POCO objects to their underlying data models.

2. Of course we're going to need that knowledge data populated. Create a list of `Movie` objects, and create an `InMemoryVectorStore` 的屬性，這將包含一系列的電影。

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

3. 接下來，我們的任務是將知識庫（`movieData` 物件）轉換為嵌入，然後將它們存儲到記憶體中的向量存儲中。在創建嵌入時，我們將使用一個不同的模型——嵌入模型，而不是語言模型。

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

    我們的生成器物件是 `IEmbeddingGenerator<string, Embedding<float>>` type. This means it is expecting inputs of `string` and outputs of `Embedding<float>`。我們再次使用 GitHub Models，這意味著需要 **Microsoft.Extensions.AI.AzureAIInference** 套件。但您也可以輕鬆地使用 **Ollama** 或 **Azure OpenAI**。

> 🗒️**注意：** 通常，您只會為知識庫創建一次嵌入，然後存儲它們。不會在每次運行應用時都這麼做。但由於我們使用的是記憶體存儲，因此每次應用重啟時都需要重新執行這個步驟，因為數據會被清除。

### 檢索知識

1. 接下來是檢索階段。我們需要查詢向量化的知識庫，根據使用者的提示找到最相關的資訊。為了查詢向量化的知識庫，意味著我們需要將使用者的提示轉換為嵌入向量。

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

### 生成回應

現在進入 RAG 的生成部分。在這個階段，我們將檢索部分找到的額外上下文提供給語言模型，以便其能更好地生成回應。這與之前看到的聊天補全非常相似——只不過現在我們提供的是使用者的提示和檢索到的資訊。

如果您還記得，當與模型進行對話時，我們會使用 `ChatMessage` 物件，其角色分為 **System**、**User** 和 **Assistant**。大多數情況下，我們可能會將檢索結果設為 **User** 訊息。

因此，我們可以在向量檢索結果的迴圈中執行如下操作：

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

> 🙋 **需要幫助嗎？**：如果您遇到任何問題，請 [在倉庫中提出問題](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new)。

## 其他資源

- [生成式 AI 初學者指南：RAG 和向量資料庫](https://github.com/microsoft/generative-ai-for-beginners/blob/main/15-rag-and-vector-databases/README.md)
- [構建 .NET 向量 AI 搜索應用](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-ai-chat-with-data?tabs=azd&pivots=openai)

## 下一步

現在您已經了解如何實現 RAG，可以看到它如何成為 AI 應用中的一個強大工具。它能為使用者提供更準確的回應、即時資訊以及領域專精知識。

👉 [接下來，我們來學習如何將視覺和音頻功能添加到您的 AI 應用中](03-vision-audio.md)。

**免責聲明**：  
本文件使用基於機器的人工智能翻譯服務進行翻譯。儘管我們努力保證翻譯的準確性，但請注意，自動翻譯可能包含錯誤或不準確之處。應以原始語言的文件作為權威來源。對於關鍵資訊，建議尋求專業人工翻譯。我們對因使用本翻譯而引起的任何誤解或錯誤解讀概不負責。