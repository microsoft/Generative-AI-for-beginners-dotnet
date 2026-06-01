# DataIngestion-01 — RAG ingestion with DataIngestion + VectorData

This sample demonstrates the full Retrieval-Augmented Generation (RAG) data pipeline
using the official .NET building blocks:

- **`Microsoft.Extensions.DataIngestion`** — the ingestion pipeline:
  **read → chunk → enrich → embed → write**
- **`Microsoft.Extensions.VectorData`** — the vector store abstraction used to
  **store** the chunks/embeddings and **search** them

## The pipeline

| Stage | Building block | What it does |
| --- | --- | --- |
| **Read** | `MarkdownReader` | Reads Markdown files from `./data` into a unified document model |
| **Chunk** | `SemanticSimilarityChunker` | Splits documents into semantically coherent chunks |
| **Enrich** | `SummaryEnricher` | Adds an AI-generated summary to each chunk to improve retrieval |
| **Embed + Write** | custom `IngestionChunkWriter` + `SqliteVecVectorStoreCollection` | Generates embeddings and writes chunks into a local sqlite-vec vector store (ElBruno connector, no Semantic Kernel) |
| **Search** | `VectorStoreCollection.SearchAsync` | Runs semantic search over the ingested content |

The reader, chunker, enricher and writer are composed into a single
`IngestionPipeline<T>` that processes every Markdown file in `./data`.

## Run it

Set the Azure OpenAI endpoint and deployments (keyless auth via Azure CLI):

```bash
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-endpoint>.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ChatDeployment" "gpt-5-mini"
dotnet user-secrets set "AzureOpenAI:EmbeddingDeployment" "text-embedding-3-small"
az login
dotnet run
```

After ingestion, type a question and the app returns the most relevant chunks with
their similarity scores. The vector data is persisted to a local `buildingblocks-vectors.db`
sqlite-vec file next to the app binary.
