# Fully local semantic search and RAG

Requires .NET 10 or later and a running Ollama server. Download the models first:

```powershell
ollama pull all-minilm
ollama pull phi4-mini
```

From the repository root:

```powershell
dotnet run --project .\samples\CoreSamples\Ollama-02RAG
dotnet run --project .\samples\CoreSamples\Ollama-02RAG -- "What is the library director's name?"
```

The sample ingests three synthetic documents, generates 384-dimensional embeddings with `all-minilm`, stores them with the official VectorData abstraction and sqlite-vec connector, retrieves the top two excerpts, then sends them to `phi4-mini` for an answer.

The default question should retrieve `library` first and answer **9 AM**, citing `[library]`. The second question has no answer in the corpus, so the model should say it does not know from these sources. Retrieval alone does not establish that an answer exists. A grounding prompt reduces hallucinations but is not a production safety guarantee; the smoke checks measure both known and unknown questions.

Separate `OllamaApiClient` instances implement `IChatClient` and `IEmbeddingGenerator<string, Embedding<float>>`. Changing the embedding model never changes the chat model. The record schema and runtime checks both require 384 dimensions. A different embedding model requires a matching schema and a fresh index.

A uniquely named temporary database is created and deleted on each run. It never reuses or deletes a learner's existing index. In a real application, persist and version your index by embedding model and document revision instead.

`OLLAMA_ENDPOINT`, `OLLAMA_MODEL`, and `OLLAMA_EMBEDDING_MODEL` override defaults. There are no Azure packages, logins, API keys, remote document sources, or user-secrets reads. Initial NuGet/model downloads need internet; inference and storage are local.

[Embeddings lesson](../../../03-AIPatternsAndApplications/01-embeddings-semantic-search.md) | [RAG lesson](../../../03-AIPatternsAndApplications/02-retrieval-augmented-generation.md) | [Ollama path](../../../docs/ollama-learning-path.md)
