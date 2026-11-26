# Foundry Local embeddings, search and RAG

This example uses **local embeddings and local generation**, not Azure embeddings with local vector storage. Follow [setup](../../../01-IntroductionToGenerativeAI/setup-foundry-local.md), then run from repository root:

```powershell
dotnet run --project .\samples\CoreSamples\FoundryLocal-02RAG -- prepare --prepare
dotnet run --project .\samples\CoreSamples\FoundryLocal-02RAG -- answer
dotnet run --project .\samples\CoreSamples\FoundryLocal-02RAG -- unknown
dotnet run --project .\samples\CoreSamples\FoundryLocal-02RAG -- injection
```

`prepare --prepare` permits missing model downloads and retains them. Defaults are `phi-4-mini` for generation and `qwen3-embedding-0.6b` for vectors; override explicitly with `FOUNDRY_LOCAL_MODEL` / `FOUNDRY_LOCAL_EMBEDDING_MODEL`. Both models load, so memory requirements exceed either model alone.

The [source](./Program.cs) embeds three fixed documents, creates a tiny in-memory vector index, embeds the question, ranks documents by cosine similarity, and passes the top two excerpts to chat. Actual vector length defines the schema (1024 in the qualified model); every vector must be nonempty, finite, nonzero and dimension-consistent. No persisted index is shared across model versions, and no 384-dimensional Ollama assumption is copied.

Expected outcomes: `answer` ranks `library` first and cites `[library]` with `9 AM`; `unknown` does not invent the library director's name; `injection` treats an adversarial source excerpt as data rather than instructions. Failures return nonzero. These small fixtures demonstrate checks, not complete protection from hallucinations or prompt injection.

All inference uses SDK-managed loopback clients and a four-minute cancellation budget. No model downloads occur without `--prepare`; no cloud credentials or remote vector service are used.

Continue with [agents](../../MAF/MAF-FoundryLocal-01/README.md) and the [full route](../../../docs/foundry-local-learning-path.md).
