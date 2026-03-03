# Prompt: Build a MEAI-native VectorStoreCollection for sqlite-vec

Use this prompt in a new repository to create an external .NET library that replaces `Microsoft.SemanticKernel.Connectors.SqliteVec` with a pure MEAI implementation.

---

## The Prompt

```
Create a .NET class library called "VectorData.Sqlite" that provides a drop-in replacement
for Microsoft.SemanticKernel.Connectors.SqliteVec, using only MEAI abstractions and native
sqlite-vec — zero Semantic Kernel dependency.

## What SK's SqliteVec connector provides (the API surface to replicate)

1. **`AddSqliteCollection<TKey, TRecord>(collectionName, connectionString)`**
   - A DI extension method that registers `VectorStoreCollection<TKey, TRecord>` as a singleton
   - Opens a SQLite database with the `sqlite-vec` extension loaded
   - Creates tables matching the record model (auto-mapped from MEAI VectorData attributes)
   - The registered collection is injected into services like DataIngestor and SemanticSearch

2. **`VectorStoreCollection<TKey, TRecord>`** — The abstract class from
   `Microsoft.Extensions.VectorData.Abstractions` with these key operations:
   - `EnsureCollectionExistsAsync()` — CREATE TABLE + vec virtual table
   - `CollectionExistsAsync()` — check if collection table exists
   - `DeleteCollectionAsync()` — DROP TABLE
   - `UpsertAsync(record)` / `UpsertAsync(IEnumerable<record>)` — INSERT OR REPLACE
   - `GetAsync(TKey)` / `GetAsync(IEnumerable<TKey>)` — SELECT by key
   - `GetAsync(Expression<Func<TRecord, bool>> filter, int top)` — SELECT with filter
   - `DeleteAsync(TKey)` / `DeleteAsync(IEnumerable<TKey>)` — DELETE by key
   - `SearchAsync(string query, int maxResults, VectorSearchOptions<TRecord>)` — embed query
     string via IEmbeddingGenerator, then find nearest neighbors in vec table

## Record model attributes (from Microsoft.Extensions.VectorData)

The library must discover these via reflection to auto-map records to SQL:

```csharp
public class IngestedChunk
{
    private const int VectorDimensions = 1536;
    private const string VectorDistanceFunction = DistanceFunction.CosineDistance;

    [VectorStoreKey]
    public required string Key { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public required string DocumentId { get; set; }

    [VectorStoreData]
    public int PageNumber { get; set; }

    [VectorStoreData]
    public required string Text { get; set; }

    [VectorStoreVector(VectorDimensions, DistanceFunction = VectorDistanceFunction)]
    public string? Vector => Text;  // Auto-embedded: the string value is embedded at upsert time
}

public class IngestedDocument
{
    private const int VectorDimensions = 2;
    private const string VectorDistanceFunction = DistanceFunction.CosineDistance;

    [VectorStoreKey]
    public required string Key { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public required string SourceId { get; set; }

    [VectorStoreData]
    public required string DocumentId { get; set; }

    [VectorStoreData]
    public required string DocumentVersion { get; set; }

    [VectorStoreVector(VectorDimensions, DistanceFunction = VectorDistanceFunction)]
    public ReadOnlyMemory<float> Vector { get; set; } = new ReadOnlyMemory<float>([0, 0]);
}
```

## How the consuming code uses it

### DI Registration (Program.cs):
```csharp
var vectorStorePath = Path.Combine(AppContext.BaseDirectory, "vector-store.db");
var vectorStoreConnectionString = $"Data Source={vectorStorePath}";
builder.Services.AddSqliteVecCollection<string, IngestedChunk>("data-chatapp20-chunks", vectorStoreConnectionString);
builder.Services.AddSqliteVecCollection<string, IngestedDocument>("data-chatapp20-documents", vectorStoreConnectionString);
```

### SemanticSearch service (injected VectorStoreCollection):
```csharp
public class SemanticSearch(VectorStoreCollection<string, IngestedChunk> vectorCollection)
{
    public async Task<IReadOnlyList<IngestedChunk>> SearchAsync(string text, string? documentIdFilter, int maxResults)
    {
        var nearest = vectorCollection.SearchAsync(text, maxResults, new VectorSearchOptions<IngestedChunk>
        {
            Filter = documentIdFilter is { Length: > 0 } ? record => record.DocumentId == documentIdFilter : null,
        });
        return await nearest.Select(result => result.Record).ToListAsync();
    }
}
```

### DataIngestor service (CRUD operations):
```csharp
public class DataIngestor(
    VectorStoreCollection<string, IngestedChunk> chunksCollection,
    VectorStoreCollection<string, IngestedDocument> documentsCollection)
{
    // Uses: EnsureCollectionExistsAsync, UpsertAsync, DeleteAsync, GetAsync(filter, top)
}
```

## NuGet packages to use

- `Microsoft.Data.Sqlite` — SQLite driver for .NET
- `sqlite-vec` — Native sqlite-vec extension binaries (NuGet: sqlite-vec, prerelease OK)
- `Microsoft.Extensions.VectorData.Abstractions` v9.7.0+ — VectorStoreCollection base class & attributes
- `Microsoft.Extensions.AI.Abstractions` v10.3.0+ — IEmbeddingGenerator interface
- `Microsoft.Extensions.DependencyInjection.Abstractions` — For the DI extension method

## Target frameworks

`net9.0;net10.0` (both are used by consuming projects)

## Implementation approach

1. **Schema discovery**: In constructor, use reflection to find all properties with
   [VectorStoreKey], [VectorStoreData], [VectorStoreVector] attributes. Build column
   mappings (name, CLR type → SQLite type, indexed flag, vector dimensions).

2. **Table creation**: CREATE TABLE with columns for key + data properties.
   CREATE VIRTUAL TABLE vec_{name} USING vec0(...) for the vector column.
   Rows in the vec table reference rows in the main table by rowid or key.

3. **Upsert**: INSERT OR REPLACE into main table. For vector properties:
   - If the property type is `string` (like `Vector => Text`), embed the string via
     IEmbeddingGenerator before storing the float[] in the vec table
   - If the property type is `ReadOnlyMemory<float>`, store directly in vec table

4. **Search**: Embed the query string via IEmbeddingGenerator. Query the vec table:
   `SELECT rowid, distance FROM vec_{name} WHERE embedding MATCH ? ORDER BY distance LIMIT ?`
   Join with main table to get full records. Apply the Filter expression in-memory
   (datasets are small in these educational samples).

5. **GetAsync(filter)**: Load records from main table, apply the filter Func<TRecord, bool>
   in memory. Return top N.

6. **sqlite-vec loading**: On each new SqliteConnection, call the appropriate API to load
   the extension. The `sqlite-vec` NuGet provides `SqliteVec.Install(connection)` or
   similar — check the package's public API.

## Also include: TextSplitter

A simple static utility to replace SK's `TextChunker.SplitPlainTextParagraphs`:

```csharp
public static class TextSplitter
{
    public static List<string> SplitParagraphs(IEnumerable<string> texts, int maxWordsPerChunk)
    {
        var allText = string.Join(" ", texts);
        var words = allText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var chunks = new List<string>();
        for (int i = 0; i < words.Length; i += maxWordsPerChunk)
        {
            var chunk = string.Join(" ", words.Skip(i).Take(maxWordsPerChunk));
            if (!string.IsNullOrWhiteSpace(chunk))
                chunks.Add(chunk);
        }
        return chunks;
    }
}
```

## Quality bar

- All public types must have XML doc comments
- Must compile against both net9.0 and net10.0
- Include a README.md with usage examples
- Include a simple console sample project that demonstrates the library
- Run dotnet format before any commit

## Reference implementations

Study these repos for patterns (they show MEAI-native RAG without SK):
- https://github.com/elbruno/elbruno.localembeddings/tree/main/samples/RagOllama
- https://github.com/elbruno/elbruno.localembeddings/tree/main/samples/RagFoundryLocal
```

---

## After the library is published

Once the library is on NuGet, update the 5 MAF projects:

1. Remove `Microsoft.SemanticKernel.Core` and `Microsoft.SemanticKernel.Connectors.SqliteVec`
2. Add the new NuGet package
3. Replace `AddSqliteCollection` → `AddSqliteVecCollection` in Program.cs
4. Replace `TextChunker.SplitPlainTextParagraphs` → `TextSplitter.SplitParagraphs` in PDFDirectorySource.cs
5. Remove `#pragma warning disable/restore SKEXP0050` and SK using statements
6. Build and verify all 5 projects
7. Update `docs/changelog/2025-sk-deprecation-nuget-upgrades.md` to mark the MAF item as DONE
