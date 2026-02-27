# 情報検索強化生成 (Retrieval-Augmented Generation: RAG)

このレッスンでは、AIアプリケーションで**情報検索強化生成 (RAG)** を使用する方法を学びます。この技術は、データストアから情報を検索し、それを言語モデルの応答に追加することで、より豊かなやり取りを可能にします。いわば「データと対話する」ような体験を提供します！

---

[![RAG 解説動画](https://img.youtube.com/vi/mY7O0OY2vho/0.jpg)](https://youtu.be/mY7O0OY2vho?feature=shared)

_⬆️画像をクリックして動画を見る⬆️_

情報検索強化生成 (RAG) は、データストアから取得した情報を活用して、言語モデルの応答を補強する技術です。

RAG アーキテクチャには主に2つのフェーズがあります: **検索 (Retrieval)** と **生成 (Generation)**。

- **検索 (Retrieval)**: ユーザーがプロンプトを入力すると、システムは外部の知識ストアから情報を取得するための検索メカニズムを使用します。この知識ストアには、ベクターデータベースやドキュメントなどが含まれる場合があります。
- **生成 (Generation)**: 取得した情報をユーザーのプロンプトに加えます。この情報とプロンプトをAIモデルが処理し、より充実した応答を生成します。

## RAGのメリット

- **精度向上**: 関連する情報でプロンプトを補強することで、モデルはより正確な応答を生成し、誤った情報（幻覚）を減らせます。
- **最新情報の取得**: モデルは知識ストアから最新の情報を取得できます。言語モデルには知識のカットオフ日があるため、プロンプトに最新情報を加えることで応答の質を向上させることが可能です。
- **専門的な知識への対応**: 特定の分野に特化した情報をモデルに渡すことで、ニッチな状況でもより効果的な応答を提供できます。

## 埋め込み（Embeddings）について！

埋め込みの概念については、できるだけ触れずにきましたが、ここで登場します。RAG の検索フェーズでは、データストア全体をモデルに渡して応答を生成するわけではありません。必要な情報だけを取り出すことが求められます。

そのためには、ユーザーのプロンプトと知識ストア内のデータを比較する方法が必要です。これにより、プロンプトを補強するのに必要最小限の情報を抽出できます。

ここで埋め込みが役立ちます。埋め込みは、データをベクトル空間で表現する方法です。これにより、ユーザーのプロンプトと知識ストア内のデータの類似性を数学的に比較でき、最も関連性の高い情報を取得できます。

ベクターデータベースという言葉を聞いたことがあるかもしれません。これらはデータをベクトル空間に保存するデータベースで、類似性に基づく非常に高速な情報検索を可能にします。RAG を使用する際に必ずしもベクターデータベースが必要というわけではありませんが、一般的なユースケースです。

## RAGの実装

以下では、Microsoft.Extension.AI と [Microsoft.Extensions.VectorData](https://www.nuget.org/packages/Microsoft.Extensions.VectorData.Abstractions/) および [Microsoft.Extensions.VectorData](https://www.nuget.org/packages/Microsoft.Extensions.VectorData.Abstractions/) ライブラリを使用して RAG を実装します。

> 🧑‍💻**サンプルコード:** [こちらのサンプルコード](../../../03-CoreGenerativeAITechniques/src/RAGSimple-02MEAIVectorsMemory) を参考にしてください。

### 知識ストアの準備

1. まず、保存するための知識データが必要です。ここでは、映画を表す POCO クラスを使用します。

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

    `[VectorStoreKey]` makes it easier for the vector store implementations to map POCO objects to their underlying data models.

2. Of course we're going to need that knowledge data populated. Create a list of `Movie` objects, and create an `InMemoryVectorStore` のような属性を使用して、映画のコレクションを持つことができます。

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

2. 次に、知識ストア（`movieData` オブジェクト）を埋め込みに変換し、それをインメモリベクターストアに保存します。この埋め込みを作成する際には、言語モデルではなく埋め込みモデルを使用します。

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

    ここで使用するジェネレーターオブジェクトは、`IEmbeddingGenerator<string, Embedding<float>>` type. This means it is expecting inputs of `string` and outputs of `Embedding<float>` 型です。ここでは **Microsoft.Extensions.AI.AzureAIInference** パッケージを使用していますが、**Ollama** や **Azure OpenAI** を使用することも可能です。

> 🗒️**メモ:** 通常、知識ストア用の埋め込みは一度作成して保存します。毎回アプリケーションを実行するたびに作成するわけではありません。ただし、ここではインメモリストアを使用しているため、アプリケーションを再起動するたびにデータが消去されるため、再作成が必要です。

### 知識の検索

1. 次に検索フェーズです。ユーザーのプロンプトに基づいて、ベクトル化された知識ストアから最も関連性の高い情報を検索する必要があります。そのためには、ユーザーのプロンプトを埋め込みベクトルに変換する必要があります。

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

### 応答の生成

次は RAG の生成フェーズです。ここでは、検索フェーズで取得した追加のコンテキストを言語モデルに提供し、より適切な応答を生成します。これは以前見たチャット補完と非常に似ていますが、今回はユーザーのプロンプトと検索で取得した情報の両方をモデルに提供します。

以前説明したように、モデルとの会話を行う際には **System**、**User**、**Assistant** という役割を持つ `ChatMessage` オブジェクトを使用します。多くの場合、検索結果は **User** メッセージとして設定することが多いでしょう。

ベクトル検索の結果をループしながら、以下のような操作を行うことができます：

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

> 🙋 **サポートが必要ですか？**: 問題が発生した場合は、[リポジトリで issue を作成](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new) してください。

## 追加リソース

- [初心者向けGenAI: RAGとベクターデータベース](https://github.com/microsoft/generative-ai-for-beginners/blob/main/15-rag-and-vector-databases/README.md)
- [.NET Vector AI 検索アプリを構築する](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-ai-chat-with-data?tabs=azd&pivots=openai)

## 次のステップ

RAG の実装方法を学んだことで、これが AI アプリケーションにおいて強力なツールとなる理由が理解できたと思います。RAG は、より正確な応答、最新情報、専門的な知識をユーザーに提供することが可能です。

👉 [次は、AIアプリケーションに視覚と音声を追加する方法を学びましょう](03-vision-audio.md)。

**免責事項**:  
この文書は、機械翻訳AIサービスを使用して翻訳されています。正確性を追求しておりますが、自動翻訳には誤りや不正確さが含まれる場合がありますのでご了承ください。原文（原言語の文書）が正式な情報源として優先されるべきです。重要な情報については、専門の人間による翻訳をお勧めします。本翻訳の利用に起因する誤解や誤認について、当社は一切の責任を負いかねます。