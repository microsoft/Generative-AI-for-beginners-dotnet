# 04 — Gaps & Sample Proposals

What *was* missing in the repo to tell the OD805 story cleanly, and the **minimal**
samples that fill the gaps. The High-severity gap is now **closed** (see below).

## Gap summary

| Gap | Why it matters for OD805 | Severity | Status |
|---|---|---|---|
| No standalone **`Microsoft.Extensions.DataIngestion`** sample | Block 3B (the RAG "engine" reveal) had no focused demo; the only ingestion in-repo is the hand-rolled `DataIngestor` inside `ChatApp20` | **High** | ✅ **Closed** — `samples/CoreSamples/DataIngestion-01-Simple` built |
| Deprecated **HuggingFace MCP** demo (`MCP-01-HuggingFace`) | The tools block (Block 4) relied on a server/sample that is no longer supported | **High** | ✅ **Closed** — replaced by keyless `samples/CoreSamples/MCP-03-MicrosoftLearn` |
| No standalone **`Microsoft.Extensions.VectorData`** sample using `VectorStoreCollection` | `RAGSimple-02` uses manual `TensorPrimitives.CosineSimilarity`, not the official abstraction | **Medium** | ⚠️ Partially addressed — `DataIngestion-01-Simple` queries via `VectorStoreCollection.SearchAsync` |

> Everything else in the flow (MEAI, MCP, MAF, the chat app) is already well covered —
> see [03-building-blocks-and-samples-map.md](./03-building-blocks-and-samples-map.md).

## Background: what these libraries are (verified via Microsoft Learn)

- **`Microsoft.Extensions.VectorData`** — consistent abstraction over vector stores
  (InMemory, SqliteVec, Qdrant, Azure AI Search, CosmosDB, …). Model is annotated with
  `[VectorStoreKey]`, `[VectorStoreData]`, `[VectorStoreVector(dims, DistanceFunction=…)]`;
  you get a `VectorStoreCollection<TKey, TRecord>` to upsert and `SearchAsync`.
- **`Microsoft.Extensions.DataIngestion`** *(preview, e.g. `10.6.0-preview.*`)* — building
  blocks for RAG ingestion:
  - **Readers:** `MarkdownReader` (Markdig), `MarkItDown` (more coming: LlamaParse, Azure
    Document Intelligence).
  - **Chunkers:** `HeaderChunker`, token-based, `SemanticSimilarityChunker` (uses embeddings).
  - **Enrichers:** `SummaryEnricher`, `SentimentEnricher`, `KeywordEnricher`,
    `ClassificationEnricher`, `ImageAlternativeTextEnricher` (use MEAI).
  - **Writer:** `VectorStoreWriter<T>` → writes chunks + embeddings into a
    `Microsoft.Extensions.VectorData` store.
  - **Pipeline:** `IngestionPipeline<T>` chains reader → processors → chunker → writer.
  - Official quickstart exists: *"Process custom data for AI applications"*
    (`learn.microsoft.com/dotnet/ai/quickstarts/process-data`).

## Proposal A — `DataIngestion-01-Simple` (✅ BUILT — closes the High gap)

A tiny console app that builds a real `IngestionPipeline<T>` over Markdown files, then
runs a semantic query — the "engine view" of what `ChatApp20` does at startup.

- **Location:** `samples/CoreSamples/DataIngestion-01-Simple/`
- **Data:** `data/dotnet-ai-building-blocks.md` (describes MEAI / VectorData / DataIngestion / MAF).
- **As built:**

  ```csharp
  IngestionDocumentReader reader = new MarkdownReader();
  IngestionChunker<string> chunker = new SemanticSimilarityChunker(embeddingGenerator, chunkerOptions);
  IngestionChunkProcessor<string> summaryEnricher = new SummaryEnricher(new EnricherOptions(chatClient));
  using SqliteVectorStore vectorStore = new("Data Source=vectors.db;Pooling=false",
      new() { EmbeddingGenerator = embeddingGenerator });
  using VectorStoreWriter<string> writer = new(vectorStore, dimensionCount: 1536,
      new() { CollectionName = "buildingblocks" });

  using IngestionPipeline<string> pipeline = new(reader, chunker, writer)
      { ChunkProcessors = { summaryEnricher } };
  await foreach (var result in pipeline.ProcessAsync(new DirectoryInfo("./data"), "*.md")) { /* ... */ }

  // then: writer.VectorStoreCollection.SearchAsync(query, top: 3) and print top hits
  ```

- **Provider:** Azure OpenAI, **keyless** via `AzureCliCredential` (`az login`).
  Reads `AzureOpenAI:Endpoint`, `AzureOpenAI:ChatDeployment`, `AzureOpenAI:EmbeddingDeployment`.
- **Packages:** `Microsoft.Extensions.AI` 10.6.0, `Microsoft.Extensions.DataIngestion`
  10.6.0-preview.*, SqliteVec connector, `Microsoft.Bcl.Memory` 10.0.8 (pinned to clear a
  transitive advisory). Builds clean in Release.
- **Talking point on stage:** "Same `wwwroot/Data` idea as the chat app, but now you see
  every stage — read → chunk → enrich → embed → write — and the writer targets the *same*
  VectorData store abstraction."

## Proposal B — `VectorData-01` (OPTIONAL, fills the Medium gap)

A console sample using the **`VectorStoreCollection`** abstraction directly (not manual
cosine), so the "it's literally the same block as the app" line is exact.

- **Location:** `samples/CoreSamples/VectorData-01-InMemory/`
- **Shape:** define a `MovieVector` record with `[VectorStoreKey]` / `[VectorStoreData]` /
  `[VectorStoreVector(1536)]`, register `InMemoryVectorStore`, upsert the movie list,
  `collection.SearchAsync(query, top: 2)`.
- **Note:** could reuse `MEAIVectorsShared` (already references
  `Microsoft.Extensions.VectorData.Abstractions 10.0.0`).
- **Decision:** **nice-to-have.** If `RAGSimple-03/04` (which use real connectors) are
  shown briefly, Proposal A alone may be enough. Recommend building A first; add B only
  if the rehearsal exposes a weak spot.

## Recommendation / status

1. ✅ **Proposal A (`DataIngestion-01-Simple`) built** — directly fills Block 3B. SqliteVec
   chosen (matches `ChatApp20`); includes a `SummaryEnricher`.
2. ✅ **MCP block migrated** to `MCP-03-MicrosoftLearn` (keyless), retiring the deprecated
   HuggingFace MCP demo.
3. ✅ Both samples added to `CoreGenerativeAITechniques.sln`; full solution validated with
   `dotnet build … --configuration Release` (0 warnings / 0 errors).
4. ⏭️ **Proposal B (`VectorData-01`)** remains an optional stretch goal — `DataIngestion-01-Simple`
   already exercises `VectorStoreCollection.SearchAsync`, so it partially covers the Medium gap.

## Open questions for Bruno

- Should the new samples also be referenced from **lesson 03** docs
  (`03-AIPatternsAndApplications`), or stay sample-only for now?
- Still want a dedicated **`VectorData-01`** sample (Proposal B), or is the search path in
  `DataIngestion-01-Simple` enough?
