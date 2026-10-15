using System.Text.Json;
using ModelContextProtocol.Protocol;

public sealed class ParallelSearchResultsTests
{
    private const string ValidResult = """
        {
          "results": [
            {
              "title": "MCP introduction",
              "url": "https://example.com/docs?q=mcp&version=10",
              "excerpts": ["An introduction to connecting models and tools."]
            }
          ]
        }
        """;

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ReadAcceptsTextAndStructuredResults(bool structured)
    {
        using var document = JsonDocument.Parse(ValidResult);
        var response = structured
            ? new CallToolResult { StructuredContent = document.RootElement }
            : TextResult(ValidResult);

        var page = Assert.Single(ParallelSearchResults.Read(response, "web_search"));

        Assert.Equal("MCP introduction", page.Title);
        Assert.Equal("https://example.com/docs?q=mcp&version=10", page.Source.OriginalString);
        Assert.Equal("An introduction to connecting models and tools.", Assert.Single(page.Excerpts));
    }

    [Fact]
    public void ReadUsesSourceFieldInsteadOfUrlsInMetadataOrExcerpts()
    {
        const string json = """
            {
              "help": "https://example.com/not-a-result",
              "results": [{
                "title": "MCP",
                "url": "https://example.com/docs(v10)",
                "excerpts": ["See https://example.com/another-page for more."]
              }]
            }
            """;

        var page = Assert.Single(ParallelSearchResults.Read(TextResult(json), "web_search"));

        Assert.Equal("https://example.com/docs(v10)", page.Source.OriginalString);
    }

    [Fact]
    public void ReadRejectsMcpToolErrors()
    {
        var response = TextResult("Rate limit exceeded. Retry later.");
        response.IsError = true;

        var error = Assert.Throws<InvalidOperationException>(
            () => ParallelSearchResults.Read(response, "web_search"));

        Assert.Contains("web_search failed", error.Message);
        Assert.Contains("Rate limit exceeded", error.Message);
    }

    [Fact]
    public void ReadRejectsFetchErrorsEvenWithResults()
    {
        var json = ValidResult.Replace(
            "\"results\":", "\"errors\": [{\"message\": \"Could not fetch page\"}], \"results\":");

        var error = Assert.Throws<InvalidOperationException>(
            () => ParallelSearchResults.Read(TextResult(json), "web_fetch"));

        Assert.Contains("web_fetch returned page errors", error.Message);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"results\":[]}")]
    [InlineData("{\"results\":null}")]
    public void ReadRejectsMissingOrEmptyResults(string json)
    {
        Assert.Throws<InvalidOperationException>(() => ParallelSearchResults.Read(TextResult(json), "web_search"));
    }

    [Theory]
    [InlineData("file:///tmp/private")]
    [InlineData("javascript:alert(1)")]
    [InlineData("/relative")]
    [InlineData("")]
    public void ReadRejectsNonWebSourceUrls(string url)
    {
        var json = ValidResult.Replace("https://example.com/docs?q=mcp&version=10", url);

        Assert.Throws<InvalidOperationException>(() => ParallelSearchResults.Read(TextResult(json), "web_search"));
    }

    [Theory]
    [InlineData("[]")]
    [InlineData("[null, \"  \"]")]
    public void ReadRejectsEmptyExcerpts(string excerpts)
    {
        var json = ValidResult.Replace("[\"An introduction to connecting models and tools.\"]", excerpts);

        Assert.Throws<InvalidOperationException>(() => ParallelSearchResults.Read(TextResult(json), "web_fetch"));
    }

    [Fact]
    public void ReadDoesNotHideInvalidJson()
    {
        Assert.ThrowsAny<JsonException>(() => ParallelSearchResults.Read(TextResult("not JSON"), "web_search"));
    }

    [Theory]
    [InlineData(599, false)]
    [InlineData(600, false)]
    [InlineData(601, true)]
    public void WritePrintsSourceAndBoundsExcerptPreview(int length, bool truncated)
    {
        var page = new ParallelPage("MCP", new Uri("https://example.com/docs"), [new string('x', length)]);
        using var writer = new StringWriter();

        ParallelSearchResults.Write(writer, [page]);

        var output = writer.ToString();
        Assert.Contains("MCP", output);
        Assert.Contains("Source: https://example.com/docs", output);
        var expected = new string('x', Math.Min(length, 600)) + (truncated ? "..." : "");
        Assert.Contains($"Excerpt: {expected}{Environment.NewLine}", output);
        Assert.DoesNotContain(new string('x', 601), output);
    }

    private static CallToolResult TextResult(string text) =>
        new() { Content = [new TextContentBlock { Text = text }] };
}
