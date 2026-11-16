# RAG Simple - MEAI Vectors Milvus

This sample runs the movie search from [RAGSimple-02MEAIVectorsMemory](../RAGSimple-02MEAIVectorsMemory/) against [Milvus](https://milvus.io/), an open-source vector database running in Docker. It uses [CommunityToolkit.VectorData.Milvus](https://www.nuget.org/packages/CommunityToolkit.VectorData.Milvus), the .NET Community Toolkit provider for `Microsoft.Extensions.VectorData`.

The samples share `MovieFactory` and `MovieVectorRecord` from `MEAIVectorsShared`. Keyless Azure OpenAI embedding generation, movie upserts, and `SearchAsync` stay the same. Only the backing collection changes:

```csharp
using VectorStoreCollection<string, MovieVectorRecord> movies =
    new MilvusCollection<string, MovieVectorRecord>(
        new MilvusClient("localhost"), "movies", ownsClient: true);
```

This demonstrates the retrieval building block of RAG, not chat response generation.

## Prerequisites

- .NET 11 SDK or later
- Docker running with Linux containers
- An Azure OpenAI embedding deployment of `text-embedding-3-small`, using its default 1536 dimensions
- Azure CLI, signed in with `az login`, and permission to use the deployment (for example, the **Cognitive Services OpenAI User** role)

See the [Azure OpenAI setup guide](../../../01-IntroductionToGenerativeAI/setup-azure-openai.md) for provider setup.

## Run the sample

Start a local Milvus server in a separate terminal. The provider requires Milvus 2.6 or later, and the version matches the integration test:

```bash
docker run --rm --name rag-milvus -p 127.0.0.1:19530:19530 -e DEPLOY_MODE=STANDALONE -e ETCD_USE_EMBED=true -e COMMON_STORAGETYPE=local milvusdb/milvus:v2.6.25 milvus run standalone
```

Wait until the server is ready. Port 19530 must be free. The loopback binding keeps this unauthenticated demo server accessible only locally.

From this sample directory, configure the same user secrets as RAGSimple-02:

```bash
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-endpoint>.services.ai.azure.com/"
dotnet user-secrets set "AzureOpenAI:EmbeddingDeployment" "text-embedding-3-small"
az login
dotnet run
```

No API key is needed. These secrets use the shared `genai-beginners-dotnet` secrets ID, so existing settings from RAGSimple-02 are reused.

The sample prints the six movie titles, embeds their descriptions, and returns two results with scores, titles, and descriptions for each query. Expect **Shrek** for the family-friendly ogre query and **The Matrix** for the hacker/simulated-reality query. Exact rankings and scores depend on the embedding model.

**Demo data reset:** every run deletes and recreates the `movies` collection to avoid duplicate movies with newly generated keys. Use a dedicated local Milvus instance with no valuable `movies` collection. The sample does not delete any other collection.

The collection and embedding generator are disposed when the app exits. Milvus stores vectors independently of the app. This Docker command uses no persistent volume, so stopping the container discards its data.

To stop the server, press Ctrl+C in its terminal or run:

```bash
docker stop rag-milvus
```

## Focused integration test

With Docker running, from the repository root:

```powershell
dotnet test samples\CoreSamples\RAGSimple-06MEAIVectorsMilvus.Tests\RAGSimple-06MEAIVectorsMilvus.Tests.csproj
```

The test starts an isolated Milvus 2.6.25 container on a random port and removes it afterward. It uses deterministic 1536-dimensional vectors, with no Azure credentials or model calls, to check collection creation, movie metadata round-tripping, top-two cosine retrieval, persistence across client instances, and collection reset. This checks the database/provider path, not the semantic quality of Azure embeddings.
