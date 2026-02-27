# Retrieval-Augmented Generation (RAG)

이 강의에서는 AI 애플리케이션에서 **Retrieval-Augmented Generation (RAG)**을 사용하는 방법을 배웁니다. 이 기술은 데이터 저장소에서 정보를 검색하여 언어 모델의 응답을 보강하거나, 데이터를 기반으로 채팅을 구현하는 데 사용할 수 있습니다!

---

[![RAG 설명 동영상](https://img.youtube.com/vi/mY7O0OY2vho/0.jpg)](https://youtu.be/mY7O0OY2vho?feature=shared)

_⬆️이미지를 클릭하면 동영상을 볼 수 있습니다⬆️_

Retrieval-Augmented Generation (RAG)은 데이터 저장소에서 검색된 정보를 사용하여 언어 모델의 응답을 보강하는 데 사용되는 기술입니다.

RAG 아키텍처에는 두 가지 주요 단계가 있습니다: **검색(Retrieval)**과 **생성(Generation)**.

- **검색(Retrieval)**: 사용자가 프롬프트를 입력하면, 시스템은 검색 메커니즘을 사용하여 외부 지식 저장소에서 정보를 가져옵니다. 지식 저장소는 벡터 데이터베이스나 문서일 수 있습니다.
- **생성(Generation)**: 검색된 정보는 사용자의 프롬프트를 보강하는 데 사용됩니다. AI 모델은 검색된 정보와 사용자의 프롬프트를 처리하여 더욱 풍부한 응답을 생성합니다.

## RAG의 장점

- **정확도 향상**: 관련 정보를 프롬프트에 추가함으로써 모델이 더 정확한 응답을 생성하고 환각(hallucination)을 줄일 수 있습니다.
- **최신 정보 제공**: 모델은 지식 저장소에서 최신 정보를 검색할 수 있습니다. 언어 모델은 특정 지식 컷오프 날짜를 가지고 있으므로, 최신 정보를 프롬프트에 추가하면 응답 품질이 개선됩니다.
- **도메인별 지식 활용**: 모델에 특정 도메인 정보를 전달하여, 특화된 상황에서도 더 효과적으로 작동할 수 있습니다.

## 임베딩(Embeddings)!

우리가 최대한 늦게 소개하려고 했던 개념인 임베딩을 이제 다룰 때가 되었습니다. RAG의 검색 단계에서는 데이터 저장소 전체를 모델에 전달하여 응답을 생성하고 싶지 않습니다. 우리는 가장 관련성이 높은 정보만 가져오고 싶습니다.

따라서 사용자의 프롬프트와 지식 저장소의 데이터를 비교할 방법이 필요합니다. 이를 통해 프롬프트를 보강하는 데 필요한 최소한의 정보를 추출할 수 있습니다.

여기서 임베딩이 등장합니다. 임베딩은 데이터를 벡터 공간에서 표현하는 방법입니다. 이를 통해 사용자의 프롬프트와 지식 저장소의 데이터 간의 유사성을 수학적으로 비교할 수 있어, 가장 관련성이 높은 정보를 검색할 수 있습니다.

아마도 벡터 데이터베이스에 대해 들어본 적이 있을 것입니다. 벡터 데이터베이스는 데이터를 벡터 공간에 저장하며, 유사성에 기반한 정보를 매우 빠르게 검색할 수 있도록 합니다. RAG를 사용하기 위해 반드시 벡터 데이터베이스가 필요한 것은 아니지만, 일반적인 사용 사례 중 하나입니다.

## RAG 구현하기

아래에서는 Microsoft.Extension.AI와 함께 [Microsoft.Extensions.VectorData](https://www.nuget.org/packages/Microsoft.Extensions.VectorData.Abstractions/) 및 [Microsoft.Extensions.VectorData](https://www.nuget.org/packages/Microsoft.Extensions.VectorData.Abstractions/) 라이브러리를 사용하여 RAG를 구현합니다.

> 🧑‍💻**샘플 코드:** [여기에서 샘플 코드를 확인하며 따라 해보세요](../../../03-CoreGenerativeAITechniques/src/RAGSimple-02MEAIVectorsMemory).

### 지식 저장소 채우기

1. 먼저 저장할 지식 데이터를 준비해야 합니다. 여기서는 영화를 나타내는 POCO 클래스를 사용합니다.

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

2. Of course we're going to need that knowledge data populated. Create a list of `Movie` objects, and create an `InMemoryVectorStore`와 같은 속성을 사용하여 영화 컬렉션을 포함한 인메모리 벡터 저장소를 구성합니다.

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

3. 다음으로, 우리의 지식 저장소(`movieData` 객체)를 임베딩으로 변환한 후, 이를 인메모리 벡터 저장소에 저장해야 합니다. 임베딩을 생성할 때는 언어 모델 대신 임베딩 모델을 사용합니다.

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

    생성기 객체는 `IEmbeddingGenerator<string, Embedding<float>>` type. This means it is expecting inputs of `string` and outputs of `Embedding<float>` 유형입니다. GitHub Models를 사용하며, 이는 **Microsoft.Extensions.AI.AzureAIInference** 패키지를 의미합니다. 하지만 **Ollama**나 **Azure OpenAI**를 사용하는 것도 가능합니다.

> 🗒️**참고:** 일반적으로 지식 저장소에 대한 임베딩은 한 번만 생성한 후 저장합니다. 매번 애플리케이션을 실행할 때마다 이를 생성하지는 않습니다. 하지만 우리는 인메모리 저장소를 사용하기 때문에, 애플리케이션이 재시작될 때마다 데이터가 삭제되어 다시 생성해야 합니다.

### 지식 검색

1. 이제 검색 단계로 넘어갑니다. 벡터화된 지식 저장소를 쿼리하여 사용자의 프롬프트에 기반한 가장 관련성이 높은 정보를 찾아야 합니다. 벡터화된 지식 저장소를 쿼리하려면 사용자의 프롬프트를 임베딩 벡터로 변환해야 합니다.

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

### 응답 생성

이제 RAG의 생성 단계로 넘어갑니다. 여기서는 검색 단계에서 찾은 추가적인 컨텍스트를 언어 모델에 제공하여 더 나은 응답을 생성합니다. 이는 이전에 본 대화 완성과 매우 유사합니다. 하지만 이번에는 사용자의 프롬프트와 검색된 정보를 모델에 제공합니다.

이전에 배운 것처럼, 모델과 대화를 이어가기 위해 **System**, **User**, **Assistant** 역할을 가진 `ChatMessage` 객체를 사용합니다. 대부분의 경우, 검색 결과를 **User** 메시지로 설정할 가능성이 높습니다.

따라서 벡터 검색 결과를 반복 처리하면서 다음과 같은 작업을 수행할 수 있습니다:

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

> 🙋 **도움이 필요하신가요?**: 문제가 발생하면 [저장소에 이슈를 열어주세요](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

## 추가 자료

- [초보자를 위한 생성 AI: RAG 및 벡터 데이터베이스](https://github.com/microsoft/generative-ai-for-beginners/blob/main/15-rag-and-vector-databases/README.md)
- [.NET 벡터 AI 검색 앱 만들기](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-ai-chat-with-data?tabs=azd&pivots=openai)

## 다음 단계

이제 RAG를 구현하는 데 필요한 내용을 배웠으니, 이 기술이 AI 애플리케이션에서 얼마나 강력한 도구가 될 수 있는지 알게 되었을 것입니다. RAG는 더 정확한 응답, 최신 정보, 그리고 도메인별 지식을 사용자에게 제공할 수 있습니다.

👉 [다음으로는 AI 애플리케이션에 Vision과 Audio를 추가하는 방법을 배워봅시다](03-vision-audio.md).

**면책 조항**:  
이 문서는 AI 기반 기계 번역 서비스를 사용하여 번역되었습니다. 정확성을 위해 노력하고 있지만, 자동 번역에는 오류나 부정확성이 포함될 수 있습니다. 원문이 작성된 언어의 문서를 권위 있는 출처로 간주해야 합니다. 중요한 정보의 경우, 전문적인 인간 번역을 권장합니다. 이 번역 사용으로 인해 발생하는 오해나 잘못된 해석에 대해 당사는 책임을 지지 않습니다.