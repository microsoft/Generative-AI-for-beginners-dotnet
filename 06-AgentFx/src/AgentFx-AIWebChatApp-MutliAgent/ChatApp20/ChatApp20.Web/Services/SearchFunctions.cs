using System.ComponentModel;

namespace ChatApp20.Web.Services;

/// <summary>
/// Functions exposed to the AI Agent. Wraps <see cref="SemanticSearch"/> so we can inject dependencies via DI.
/// </summary>
public class SearchFunctions
{
    private readonly SemanticSearch _semanticSearch;

    public SearchFunctions(SemanticSearch semanticSearch)
    {
        _semanticSearch = semanticSearch;
    }

    [Description("Searches for information using a phrase or keyword")]
    public async Task<IEnumerable<string>> SearchAsync(
        [Description("The phrase to search for.")] string searchPhrase,
        [Description("If possible, specify the filename to search that file only. If not provided or empty, the search includes all files.")] string? filenameFilter = null)
    {
        // Perform semantic search over ingested chunks. We interpret filenameFilter as documentId filter.
        var results = await _semanticSearch.SearchAsync(searchPhrase, filenameFilter, maxResults: 5);
        return results.Select(result =>
            $"<result filename=\"{result.DocumentId}\" page_number=\"{result.PageNumber}\">{result.Text}</result>");
    }
}
