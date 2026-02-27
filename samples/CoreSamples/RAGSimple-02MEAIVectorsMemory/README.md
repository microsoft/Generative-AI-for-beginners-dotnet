# RAG Simple - MEAI Vectors Memory

This sample demonstrates Retrieval-Augmented Generation (RAG) using Microsoft.Extensions.AI with GitHub Models and an in-memory vector store.

## Prerequisites

- .NET 9.0 SDK
- GitHub Token for accessing GitHub Models

## Setup

1. Set your GitHub Token as an environment variable:

   **Windows (PowerShell):**
   ```powershell
   $env:GITHUB_TOKEN="your-github-token-here"
   ```

   **Linux/macOS:**
   ```bash
   export GITHUB_TOKEN="your-github-token-here"
   ```

2. Get a GitHub Token:
   - Go to https://github.com/settings/tokens
   - Generate a new token with appropriate permissions
   - Or follow the setup guide: [Getting Started with GitHub Models](../../../02-SetupDevEnvironment/readme.md)

## Running the Sample

```bash
dotnet run
```

## What This Sample Does

1. Creates an in-memory vector store
2. Loads a collection of movies with descriptions
3. Generates embeddings for each movie using GitHub Models' text-embedding-3-small model
4. Performs a vector search based on a query
5. Returns the most relevant movies

## Code Overview

The sample uses:
- **Microsoft.Extensions.AI** for embeddings generation
- **Azure.AI.Inference** for connecting to GitHub Models
- **Microsoft.Extensions.VectorData** for in-memory vector storage

## Alternative Providers

See the other RAG samples (RAGSimple-03, RAGSimple-04) for Azure AI Search and Qdrant vector store examples.
