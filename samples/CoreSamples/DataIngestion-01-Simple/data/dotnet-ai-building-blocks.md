# The .NET AI building blocks

Microsoft provides a set of `Microsoft.Extensions.*` building blocks that make it easy
to add intelligence to C# applications without leaving the .NET ecosystem you already
know.

## Microsoft.Extensions.AI (MEAI)

`Microsoft.Extensions.AI` is the foundation. It defines provider-agnostic abstractions
such as `IChatClient` (to chat with a model) and `IEmbeddingGenerator` (to create
embeddings). Because your application code depends on the abstraction instead of a
specific SDK, you can swap providers — for example Azure OpenAI in the cloud and Ollama
running locally — without changing your application logic.

## Microsoft.Extensions.VectorData

`Microsoft.Extensions.VectorData` provides a consistent abstraction over many vector
stores, including in-memory, SQLite, Qdrant, Azure AI Search, and Cosmos DB. You annotate
a model with attributes like `[VectorStoreKey]`, `[VectorStoreData]`, and
`[VectorStoreVector]`, and then use a `VectorStoreCollection` to upsert records and run
semantic similarity searches. The same code works across every supported store.

## Microsoft.Extensions.DataIngestion

`Microsoft.Extensions.DataIngestion` provides the building blocks for Retrieval-Augmented
Generation (RAG) ingestion pipelines. A pipeline reads documents, chunks them, optionally
enriches the chunks with AI (summaries, sentiment, keywords), generates embeddings, and
writes the result into a vector store. The `IngestionPipeline<T>` type chains readers,
chunkers, processors, and writers together so you can build complete workflows with very
little code.

## Microsoft Agent Framework (MAF)

The Microsoft Agent Framework builds on top of these foundations to create agents and
multi-agent workflows. An agent combines a chat model, tools, and memory into a single
reusable unit, and workflows let multiple agents collaborate to solve more complex tasks.
