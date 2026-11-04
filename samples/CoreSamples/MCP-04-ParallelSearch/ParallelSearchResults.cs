using System.Text.Json;
using ModelContextProtocol.Protocol;

internal sealed record ParallelPage(string Title, Uri Source, IReadOnlyList<string> Excerpts);

internal static class ParallelSearchResults
{
    internal static IReadOnlyList<ParallelPage> Read(CallToolResult response, string toolName)
    {
        var text = string.Join("\n", response.Content.OfType<TextContentBlock>().Select(block => block.Text));
        if (response.IsError == true)
        {
            throw new InvalidOperationException($"{toolName} failed: {text}");
        }

        // Parallel returns JSON in a text block; MCP also supports structured results.
        using var document = JsonDocument.Parse(response.StructuredContent?.GetRawText() ?? text);
        var data = document.RootElement;
        if (data.TryGetProperty("errors", out var errors) &&
            errors.ValueKind == JsonValueKind.Array && errors.GetArrayLength() > 0)
        {
            throw new InvalidOperationException($"{toolName} returned page errors: {errors}");
        }

        if (!data.TryGetProperty("results", out var results) ||
            results.ValueKind != JsonValueKind.Array || results.GetArrayLength() == 0)
        {
            throw new InvalidOperationException($"{toolName} returned no results.");
        }

        var pages = new List<ParallelPage>();
        foreach (var result in results.EnumerateArray())
        {
            var url = result.GetProperty("url").GetString();
            if (!Uri.TryCreate(url, UriKind.Absolute, out var source) ||
                (source.Scheme != Uri.UriSchemeHttp && source.Scheme != Uri.UriSchemeHttps))
            {
                throw new InvalidOperationException($"{toolName} returned an invalid source URL.");
            }

            var excerpts = result.GetProperty("excerpts").EnumerateArray()
                .Select(excerpt => excerpt.GetString())
                .OfType<string>()
                .Where(excerpt => !string.IsNullOrWhiteSpace(excerpt))
                .ToArray();
            if (excerpts.Length == 0)
            {
                throw new InvalidOperationException($"{toolName} returned no excerpts for {source}.");
            }

            var title = result.GetProperty("title").GetString() ?? source.ToString();
            pages.Add(new ParallelPage(title, source, excerpts));
        }

        return pages;
    }

    internal static void Write(TextWriter writer, IEnumerable<ParallelPage> pages)
    {
        const int previewLength = 600;
        foreach (var page in pages)
        {
            var excerpt = page.Excerpts[0];
            var preview = excerpt.Length > previewLength ? excerpt[..previewLength] + "..." : excerpt;
            writer.WriteLine(page.Title);
            writer.WriteLine($"Source: {page.Source}");
            writer.WriteLine($"Excerpt: {preview}");
            writer.WriteLine();
        }
    }
}
