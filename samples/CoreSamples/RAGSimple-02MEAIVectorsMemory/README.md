# RAG Simple - MEAI Vectors Memory

This sample demonstrates Retrieval-Augmented Generation (RAG) using Microsoft.Extensions.AI with Azure OpenAI and an in-memory vector store.

## Prerequisites

- .NET 9.0 SDK
- Azure OpenAI / Microsoft Foundry endpoint and API key

## Setup

Set your Azure OpenAI credentials using user secrets:

```bash
cd samples/CoreSamples/RAGSimple-02MEAIVectorsMemory
dotnet user-secrets set "endpoint" "https://<your-endpoint>.services.ai.azure.com/"
dotnet user-secrets set "apikey" "<your-api-key>"
dotnet user-secrets set "embeddingModelName" "text-embedding-3-small"
```

## Running the Sample

```bash
dotnet run
```

## What This Sample Does

1. Creates an in-memory vector store
2. Loads a collection of movies with descriptions
3. Generates embeddings for each movie using Azure OpenAI's text-embedding-3-small model
4. Performs a vector search based on a query
5. Returns the most relevant movies

## Code Overview

The sample uses:
- **Microsoft.Extensions.AI** for embeddings generation
- **Azure.AI.OpenAI** for connecting to Azure OpenAI / Microsoft Foundry
- **Microsoft.Extensions.VectorData** for in-memory vector storage

## Alternative Providers

See the other RAG samples (RAGSimple-03, RAGSimple-04) for Azure AI Search and Qdrant vector store examples.
