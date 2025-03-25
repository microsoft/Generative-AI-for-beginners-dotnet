
using MEAI.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.OpenApi.Services;
using OpenAI.VectorStores;
using System.Reflection.Emit;

namespace MEAI.Services
{
    public class AIMovieSearchService<T> : IMovieSearchService<T>
    {
        private static bool _initialized = false;

        private readonly ILogger<AIMovieSearchService<T>> _logger;
        private readonly IVectorStore _vectorStore;
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;

        public AIMovieSearchService(ILogger<AIMovieSearchService<T>> logger, IVectorStore vectorStore,
            [FromKeyedServices("Gemini")] IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
        { 
            _logger = logger;
            _vectorStore = vectorStore;
            _embeddingGenerator = embeddingGenerator;
        }

        public async Task<bool> Refresh(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Rebuild vectors");

            var movies = _vectorStore.GetCollection<T, MovieVector<T>>("movies");
            await movies.CreateCollectionIfNotExistsAsync();

            var movieData = MovieFactory<T>.GetMovieVectorList();

            foreach (var movie in movieData)
            {
                movie.Vector = await _embeddingGenerator.GenerateEmbeddingVectorAsync(movie.Description);
                await movies.UpsertAsync(movie);
            }

            _initialized = true;

            return true;
        }

        public async Task<IEnumerable<MatchedMovie<T>>> Search(string query, SearchOptions searchOption, CancellationToken cancellationToken = default)
        {
             if (string.IsNullOrWhiteSpace(query))
                return [];

            if (!_initialized)
                await Refresh(cancellationToken);

            var queryEmbedding = await _embeddingGenerator.GenerateEmbeddingVectorAsync(query);

            var movies = _vectorStore.GetCollection<T, MovieVector<T>>("movies");

            var searchOptions = new VectorSearchOptions<MovieVector<T>>()
            {
                Top = searchOption.Top,
                Skip = searchOption.Skip
            };

            var results = await movies.VectorizedSearchAsync(queryEmbedding, searchOptions);
            var matchedMovies = new List<MatchedMovie<T>>();
            await foreach (var result in results.Results)
            {
                matchedMovies.Add(new MatchedMovie<T>() { Key = result.Record.Key, Title = result.Record.Title, Score = result.Score });
            }

            return matchedMovies;
        }
    }
}
