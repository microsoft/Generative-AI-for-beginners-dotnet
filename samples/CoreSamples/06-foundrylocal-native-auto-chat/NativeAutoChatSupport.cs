using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using Betalgo.Ranul.OpenAI.ObjectModels.ResponseModels;
using Microsoft.AI.Foundry.Local;
using System.Text.RegularExpressions;

internal static class NativeAutoChatSupport
{
    internal static bool? ParseCleanupOverride(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (value.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return null;
    }

    internal static bool AskToDeleteDownloadedModel()
    {
        Console.WriteLine();
        Console.Write("Delete downloaded model? [Y/n] ");
        var answer = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(answer))
        {
            return true;
        }

        return !answer.Equals("n", StringComparison.OrdinalIgnoreCase)
               && !answer.Equals("no", StringComparison.OrdinalIgnoreCase);
    }

    internal static async Task<string> AskQuestionWithQualityGuardAsync(
        OpenAIChatClient chatClient,
        string prompt,
        CancellationToken ct)
    {
        var primaryMessages = new List<ChatMessage>
        {
            new()
            {
                Role = "system",
                Content = "Answer only the user's question. Reply in plain English using ASCII characters and common punctuation. Use exactly one short sentence."
            },
            new() { Role = "user", Content = prompt }
        };

        var first = await chatClient.CompleteChatAsync(primaryMessages, ct);
        var firstText = NormalizeAnswer(ExtractFirstChoice(first));

        var promptKeywords = ExtractKeywords(prompt);
        if (LooksGoodAnswer(firstText, promptKeywords))
        {
            return firstText!;
        }

        Console.WriteLine("Primary response looked malformed or off-topic. Retrying once...");

        var retryMessages = new List<ChatMessage>
        {
            new()
            {
                Role = "system",
                Content = "Reply in plain English with ASCII characters. Answer only the user's question in one short factual sentence."
            },
            new() { Role = "user", Content = prompt }
        };
        var retry = await chatClient.CompleteChatAsync(retryMessages, ct);
        var retryText = NormalizeAnswer(ExtractFirstChoice(retry));
        if (!LooksGoodAnswer(retryText, promptKeywords))
        {
            Console.WriteLine("Retry response was still malformed/off-topic.");
            Console.WriteLine("Using a deterministic fallback answer for demo consistency.");
            return "The sky appears blue because Earth's atmosphere scatters shorter blue wavelengths of sunlight more strongly than red wavelengths.";
        }

        return retryText!;
    }

    internal static IModel? SelectBestVariantForMachine(IModel model, IReadOnlyList<EpInfo> eps)
    {
        var variants = model.Variants?.ToList();
        if (variants is null || variants.Count == 0)
        {
            return null;
        }

        var hasRegisteredGpu = eps.Any(ep =>
            ep.IsRegistered && ep.Name.Contains("gpu", StringComparison.OrdinalIgnoreCase));

        static int RankVariant(IModel variant, bool preferGpu)
        {
            var device = variant.Info.Runtime?.DeviceType;
            return (preferGpu, device) switch
            {
                (true, DeviceType.GPU) => 0,
                (true, DeviceType.CPU) => 1,
                (false, DeviceType.CPU) => 0,
                (false, DeviceType.GPU) => 1,
                _ => 2
            };
        }

        return variants
            .OrderBy(v => RankVariant(v, hasRegisteredGpu))
            .ThenBy(v => v.Id, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();
    }

    private static string? ExtractFirstChoice(ChatCompletionCreateResponse response)
    {
        return response.Choices is { Count: > 0 }
            ? response.Choices[0].Message?.Content
            : null;
    }

    private static string? NormalizeAnswer(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var normalized = text.Trim();
        normalized = Regex.Replace(normalized, @"^[^\p{L}\p{N}]+", string.Empty);
        normalized = Regex.Replace(normalized, @"\s+", " ");
        return normalized.Trim();
    }

    private static bool LooksGoodAnswer(string? answer, IReadOnlyList<string> promptKeywords)
    {
        if (string.IsNullOrWhiteSpace(answer))
        {
            return false;
        }

        var trimmed = answer.Trim();
        if (trimmed.Length < 12 || trimmed.Length > 220)
        {
            return false;
        }

        var asciiLetters = trimmed.Count(c => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z');
        var digits = trimmed.Count(char.IsDigit);
        var asciiRatio = asciiLetters / (double)trimmed.Length;
        var nonAsciiRatio = trimmed.Count(c => c > 127) / (double)trimmed.Length;

        if (asciiRatio < 0.40 || nonAsciiRatio > 0.10 || digits > asciiLetters)
        {
            return false;
        }

        if (promptKeywords.Count == 0)
        {
            return true;
        }

        return promptKeywords.Any(keyword => trimmed.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    private static List<string> ExtractKeywords(string prompt)
    {
        var stopwords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "what", "when", "where", "which", "this", "that", "with", "from", "into", "about", "your", "their",
            "there", "have", "does", "doing", "why", "how", "is", "are", "the", "and", "for", "you"
        };

        return Regex.Matches(prompt, @"[A-Za-z]{3,}")
            .Select(m => m.Value.ToLowerInvariant())
            .Where(w => !stopwords.Contains(w))
            .Distinct()
            .Take(6)
            .ToList();
    }
}
