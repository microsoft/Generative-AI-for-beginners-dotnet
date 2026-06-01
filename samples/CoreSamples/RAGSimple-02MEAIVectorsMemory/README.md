# RAG Simple - MEAI Vectors Memory

This sample demonstrates semantic search / RAG building blocks using `Microsoft.Extensions.AI`, the official `Microsoft.Extensions.VectorData` abstractions, and a local sqlite-vec store backed by `ElBruno.Connectors.SqliteVec`.

## Prerequisites

- .NET 10.0 SDK
- Azure OpenAI / Microsoft Foundry endpoint
- Azure CLI signed in with `az login`

## Setup

Set your Azure OpenAI endpoint using user secrets:

```bash
cd samples/CoreSamples/RAGSimple-02MEAIVectorsMemory
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-endpoint>.services.ai.azure.com/"
dotnet user-secrets set "AzureOpenAI:EmbeddingDeployment" "text-embedding-3-small"
az login
```

## Running the Sample

```bash
dotnet run
```

## What This Sample Does

1. Creates a local sqlite-vec backed vector collection
2. Loads a collection of movies with descriptions
3. Generates embeddings for each movie using Azure OpenAI's `text-embedding-3-small` model
4. Performs a vector search based on a query
5. Returns the most relevant movies

## Code Overview

The sample uses:

- **Microsoft.Extensions.AI** for embeddings generation
- **Azure.AI.OpenAI** for connecting to Azure OpenAI / Microsoft Foundry
- **Microsoft.Extensions.VectorData** for the official record/search abstraction
- **ElBruno.Connectors.SqliteVec** for the local sqlite-vec store implementation

> Note: the sample name is kept for continuity in the course material, but the implementation no longer uses Semantic Kernel's in-memory connector.

## Alternative Providers

See the other RAG samples (RAGSimple-03, RAGSimple-04) for Azure AI Search and Qdrant vector store examples.
