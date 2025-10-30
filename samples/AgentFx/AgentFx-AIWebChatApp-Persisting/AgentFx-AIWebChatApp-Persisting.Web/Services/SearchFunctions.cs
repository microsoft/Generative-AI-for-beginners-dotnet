using System.ComponentModel;

namespace AgentFx_AIWebChatApp_Persisting.Web.Services;

/// <summary>
/// Functions exposed to the AI Agent. Wraps SemanticSearch so dependencies are injected.
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
 var results = await _semanticSearch.SearchAsync(searchPhrase, filenameFilter, maxResults:5);
 return results.Select(result =>
 $"<result filename=\"{result.DocumentId}\" page_number=\"{result.PageNumber}\">{result.Text}</result>");
 }
}
