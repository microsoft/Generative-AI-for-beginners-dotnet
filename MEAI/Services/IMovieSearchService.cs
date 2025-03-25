using MEAI.Models;

namespace MEAI.Services
{
    public interface IMovieSearchService<T>
    {
        public Task<bool> Refresh(CancellationToken cancellationToken = default);

        public Task<IEnumerable<MatchedMovie<T>>> Search(string query, SearchOptions searchOption, CancellationToken cancellationToken = default);
    }

    public class MatchedMovie<T> : Movie<T>
    {
        public double? Score { get; set; }
    }

    public class SearchOptions
    {
        public int Top { get; init; } = 3;

        public int Skip { get; init; } = 0;
    }
}
