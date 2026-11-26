using Microsoft.Extensions.AI;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace FoundryLocal.Course;

internal static class FoundryCourseChecks
{
    public static readonly ChatOptions TextOptions = new()
    {
        Temperature = 0,
        MaxOutputTokens = 256,
        ToolMode = ChatToolMode.None
    };

    public static string RequireText(string? text) =>
        !string.IsNullOrWhiteSpace(text) ? text : throw new InvalidOperationException("The model returned no text.");

    public static bool ContainsOpeningTime(string text) =>
        Regex.IsMatch(text, @"\b9(?::00)?\s*a\.?m\.?\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static string Mode(string[] args, string defaultMode, params string[] allowed)
    {
        var positional = args.Where(arg => arg != "--prepare").ToArray();
        if (args.Count(arg => arg == "--prepare") > 1 || positional.Length > 1 ||
            (positional.Length == 1 && !allowed.Contains(positional[0])))
        {
            throw new ArgumentException($"Usage: [{string.Join('|', allowed)}] [--prepare]");
        }
        return positional.FirstOrDefault() ?? defaultMode;
    }

    public static Review ParseReview(string json)
    {
        var review = JsonSerializer.Deserialize<Review>(json, new JsonSerializerOptions
        {
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            RespectRequiredConstructorParameters = true
        }) ?? throw new InvalidOperationException("Expected a review object.");
        if (review.Sentiment != "Positive" || review.Rating != 5)
        {
            throw new InvalidOperationException("Expected Sentiment=Positive and Rating=5.");
        }
        return review;
    }

    public static void ValidateVector(ReadOnlyMemory<float> vector, int dimensions)
    {
        if (dimensions <= 0 || vector.Length != dimensions)
        {
            throw new InvalidOperationException($"Expected {dimensions} finite embedding dimensions; got {vector.Length}.");
        }
        double norm = 0;
        foreach (float value in vector.Span)
        {
            if (!float.IsFinite(value))
            {
                throw new InvalidOperationException("Embedding values must be finite.");
            }
            norm += (double)value * value;
        }
        if (norm == 0)
        {
            throw new InvalidOperationException("An embedding must not be a zero vector.");
        }
    }

    public static double Cosine(ReadOnlyMemory<float> left, ReadOnlyMemory<float> right)
    {
        ValidateVector(left, left.Length);
        ValidateVector(right, left.Length);
        double dot = 0, l2 = 0, r2 = 0;
        for (int i = 0; i < left.Length; i++)
        {
            dot += (double)left.Span[i] * right.Span[i];
            l2 += (double)left.Span[i] * left.Span[i];
            r2 += (double)right.Span[i] * right.Span[i];
        }
        return dot / Math.Sqrt(l2 * r2);
    }
}

internal sealed record Review(string Sentiment, int Rating);
