# Geração Aumentada por Recuperação (RAG)

Nesta lição, aprenda a usar a **Geração Aumentada por Recuperação (RAG)** em suas aplicações de IA. Essa técnica pode ser usada para enriquecer a resposta de um modelo de linguagem com informações recuperadas de um repositório de dados – ou para permitir que você "converse com seus dados"!

---

[![Vídeo explicativo sobre RAG](https://img.youtube.com/vi/mY7O0OY2vho/0.jpg)](https://youtu.be/mY7O0OY2vho?feature=shared)

_⬆️Clique na imagem para assistir ao vídeo⬆️_

A Geração Aumentada por Recuperação (RAG) é uma técnica usada para enriquecer a resposta de um modelo de linguagem com informações recuperadas de um repositório de dados.

Existem 2 fases principais em uma arquitetura RAG: **Recuperação** e **Geração**.

- **Recuperação**: Quando o usuário faz uma solicitação (prompt), o sistema utiliza algum mecanismo de recuperação para buscar informações de um repositório de conhecimento externo. Esse repositório pode ser um banco de dados vetorial, um documento, entre outros.
- **Geração**: As informações recuperadas são então usadas para enriquecer o prompt do usuário. O modelo de IA processa tanto as informações recuperadas quanto o prompt do usuário para produzir uma resposta mais completa.

## Benefícios do RAG

- **Melhoria na precisão**: Ao enriquecer o prompt com informações relevantes, o modelo pode gerar respostas mais precisas e reduzir alucinações.
- **Informações atualizadas**: O modelo pode recuperar as informações mais recentes do repositório de conhecimento. Lembre-se de que o modelo de linguagem possui uma data de corte para seu conhecimento, e enriquecer o prompt com informações mais recentes pode melhorar a resposta.
- **Conhecimento específico de domínio**: O modelo pode ser alimentado com informações muito específicas de um domínio, tornando-o mais eficaz em situações de nicho.

## Embeddings!

Adiantamos o máximo possível antes de introduzir o conceito de embeddings. Na fase de recuperação do RAG, não queremos passar todo o repositório de dados para o modelo gerar a resposta. Queremos apenas buscar as informações mais relevantes.

Por isso, precisamos de uma forma de comparar o prompt do usuário com os dados no repositório de conhecimento. Assim, podemos extrair a quantidade mínima de informações necessárias para enriquecer o prompt.

É aqui que os embeddings entram em cena. Embeddings são uma forma de representar dados em um espaço vetorial. Isso nos permite comparar matematicamente a similaridade entre o prompt do usuário e os dados no repositório de conhecimento, para que possamos recuperar as informações mais relevantes.

Você talvez já tenha ouvido falar de bancos de dados vetoriais. Esses bancos armazenam dados em um espaço vetorial, permitindo uma recuperação muito rápida de informações com base em similaridade. Não é obrigatório usar um banco de dados vetorial para implementar RAG, mas é um caso de uso comum.

## Implementando RAG

Usaremos o Microsoft.Extension.AI junto com as bibliotecas [Microsoft.Extensions.VectorData](https://www.nuget.org/packages/Microsoft.Extensions.VectorData.Abstractions/) e [Microsoft.Extensions.VectorData](https://www.nuget.org/packages/Microsoft.Extensions.VectorData.Abstractions/) para implementar o RAG abaixo.

> 🧑‍💻**Código de exemplo:** Você pode acompanhar com o [código de exemplo aqui](../../../03-CoreGenerativeAITechniques/src/RAGSimple-02MEAIVectorsMemory).

### Populando o repositório de conhecimento

1. Primeiro, precisamos de alguns dados de conhecimento para armazenar. Usaremos uma classe POCO que representa filmes.

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

    Usando os atributos como `[VectorStoreKey]` makes it easier for the vector store implementations to map POCO objects to their underlying data models.

2. Of course we're going to need that knowledge data populated. Create a list of `Movie` objects, and create an `InMemoryVectorStore`, que terá uma coleção de filmes.

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

3. Nossa próxima tarefa é converter nosso repositório de conhecimento (o objeto `movieData`) em embeddings e armazená-los no repositório vetorial em memória. Ao criar os embeddings, usaremos um modelo diferente – um modelo de embeddings em vez de um modelo de linguagem.

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

    Nosso objeto gerador é um `IEmbeddingGenerator<string, Embedding<float>>` type. This means it is expecting inputs of `string` and outputs of `Embedding<float>`. Estamos novamente utilizando os Azure OpenAI, o que significa o pacote **Microsoft.Extensions.AI.AzureAIInference**. Mas você também poderia usar **Ollama** ou **Azure OpenAI** com a mesma facilidade.

> 🗒️**Nota:** Geralmente, você criará embeddings para seu repositório de conhecimento apenas uma vez e os armazenará. Isso não será feito toda vez que a aplicação for executada. No entanto, como estamos usando um repositório em memória, precisamos recriá-los porque os dados são apagados toda vez que a aplicação é reiniciada.

### Recuperando o conhecimento

1. Agora para a fase de recuperação. Precisamos consultar o repositório de conhecimento vetorizado para encontrar as informações mais relevantes com base no prompt do usuário. E para consultar o repositório vetorizado, precisamos transformar o prompt do usuário em um vetor de embedding.

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

### Gerando a resposta

Agora passamos para a fase de geração do RAG. É aqui que fornecemos ao modelo de linguagem o contexto adicional que a fase de recuperação encontrou, para que ele possa formular uma resposta mais completa. Isso será muito semelhante às conclusões de chat que vimos antes – exceto que agora estamos fornecendo ao modelo o prompt do usuário e as informações recuperadas.

Se você se lembra, usamos objetos `ChatMessage` ao conversar com o modelo, que possuem os papéis de **System**, **User** e **Assistant**. Na maioria das vezes, provavelmente configuraremos os resultados da busca como uma mensagem de **User**.

Então, podemos fazer algo como o seguinte enquanto iteramos pelos resultados da busca vetorial:

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

> 🙋 **Precisa de ajuda?**: Se encontrar algum problema, [abra uma issue no repositório](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

## Recursos adicionais

- [GenAI para Iniciantes: RAG e Bancos de Dados Vetoriais](https://github.com/microsoft/generative-ai-for-beginners/blob/main/15-rag-and-vector-databases/README.md)
- [Crie um App de Busca Vetorial em .NET](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-ai-chat-with-data?tabs=azd&pivots=openai)

## Próximos passos

Agora que você viu o que é necessário para implementar RAG, pode perceber como ele pode ser uma ferramenta poderosa em suas aplicações de IA. Ele pode fornecer respostas mais precisas, informações atualizadas e conhecimento específico de domínio para seus usuários.

👉 [A seguir, vamos aprender a adicionar Visão e Áudio às suas aplicações de IA](03-vision-audio.md).

**Aviso Legal**:  
Este documento foi traduzido utilizando serviços de tradução automática baseados em IA. Embora nos esforcemos para garantir a precisão, esteja ciente de que traduções automáticas podem conter erros ou imprecisões. O documento original em seu idioma nativo deve ser considerado a fonte oficial. Para informações críticas, recomenda-se a tradução humana profissional. Não nos responsabilizamos por mal-entendidos ou interpretações equivocadas decorrentes do uso desta tradução.