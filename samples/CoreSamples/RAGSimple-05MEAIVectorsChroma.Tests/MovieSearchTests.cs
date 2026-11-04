using ChromaDB.Client;
using CommunityToolkit.VectorData.Chroma;
using DotNet.Testcontainers.Builders;
using Microsoft.Extensions.VectorData;

public sealed class MovieSearchTests
{
    [Fact]
    public async Task ChromaStoresAndSearchesTheSharedMovieRecords()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(3));
        var cancellationToken = timeout.Token;
        await using var container = new ContainerBuilder("chromadb/chroma:1.5.9")
            .WithPortBinding(8000, assignRandomHostPort: true)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(request => request
                    .ForPort(8000).ForPath("/api/v2/heartbeat")))
            .Build();
        await container.StartAsync(cancellationToken);

        var endpoint = $"http://{container.Hostname}:{container.GetMappedPublicPort(8000)}";
        using VectorStoreCollection<string, MovieVectorRecord> movies =
            new ChromaCollection<string, MovieVectorRecord>(
                new ChromaClient(endpoint), "movies", ownsClient: true);

        // The sample resets a collection that may or may not already exist.
        await movies.EnsureCollectionDeletedAsync(cancellationToken);
        await movies.EnsureCollectionExistsAsync(cancellationToken);
        await movies.EnsureCollectionExistsAsync(cancellationToken);

        var records = MovieFactory<string>.GetMovieList()
            .Select((movie, index) =>
            {
                var vector = new float[1536];
                vector[index] = 1;
                return new MovieVectorRecord
                {
                    Key = movie.Key,
                    Title = movie.Title!,
                    Description = movie.Description!,
                    Embedding = vector
                };
            })
            .ToList();

        foreach (var record in records)
        {
            await movies.UpsertAsync(record, cancellationToken);
        }

        foreach (var title in new[] { "Shrek", "The Matrix" })
        {
            var expected = Assert.Single(records, record => record.Title == title);
            var results = new List<VectorSearchResult<MovieVectorRecord>>();
            await foreach (var result in movies.SearchAsync(
                expected.Embedding, top: 2, cancellationToken: cancellationToken))
            {
                results.Add(result);
            }

            Assert.Equal(2, results.Count);
            Assert.Equal(expected.Key, results[0].Record.Key);
            Assert.Equal(title, results[0].Record.Title);
            Assert.Equal(expected.Description, results[0].Record.Description);
            Assert.Equal(1.0, results[0].Score!.Value, precision: 3);
            Assert.True(results[0].Score > results[1].Score);
        }

        using var reopened = new ChromaCollection<string, MovieVectorRecord>(
            new ChromaClient(endpoint), "movies", ownsClient: true);
        await reopened.EnsureCollectionExistsAsync(cancellationToken);
        foreach (var expected in records)
        {
            var stored = await reopened.GetAsync(expected.Key,
                new RecordRetrievalOptions { IncludeVectors = true }, cancellationToken);
            Assert.NotNull(stored);
            Assert.Equal(expected.Title, stored.Title);
            Assert.Equal(expected.Description, stored.Description);
            Assert.Equal(expected.Embedding.ToArray(), stored.Embedding.ToArray());
        }

        await movies.EnsureCollectionDeletedAsync(cancellationToken);
        Assert.False(await movies.CollectionExistsAsync(cancellationToken));
        await movies.EnsureCollectionExistsAsync(cancellationToken);
        Assert.Null(await movies.GetAsync(records[0].Key, cancellationToken: cancellationToken));
    }
}
