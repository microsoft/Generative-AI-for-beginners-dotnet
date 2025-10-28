
using System.ClientModel;

/// <summary>
/// Small helper to keep console printing consistent and easy to read.
/// Uses simple colored output and labeled sections to make the streaming flow obvious.
/// Renamed to StreamConsoleHelper to avoid conflicts with other helpers in the solution.
/// </summary>
internal static class StreamConsoleHelper
{
    private static readonly System.Text.StringBuilder _accum = new System.Text.StringBuilder();
    private static string? _firstTimestamp;

    public static void PrintHeader(string text)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string('=',60));
        Console.WriteLine(text);
        Console.WriteLine(new string('=',60));
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void PrintSection(string title)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"--- {title} ---");
        Console.ResetColor();
    }

    public static void PrintUpdate(string updateText, string? continuationToken)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"[{timestamp}] ");
        Console.ResetColor();

        // Print the update text (streaming chunk)
        if (string.IsNullOrEmpty(updateText))
        {
            Console.WriteLine("(empty update)");
        }
        else
        {
            Console.WriteLine(updateText);
        }

        // Print a compact representation of the continuation token for debugging
        if (!string.IsNullOrWhiteSpace(continuationToken))
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($" token: {Truncate(continuationToken,60)}");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Prepare internal accumulator to join token fragments into readable sentences.
    /// Call before starting the continuation streaming loop.
    /// </summary>
    public static void StartAccumulatedStream()
    {
        _accum.Clear();
        _firstTimestamp = null;
    }

    /// <summary>
    /// Accumulates small token fragments and prints a joined sentence when a sentence terminator
    /// or newline is encountered, or when accumulated length grows large.
    /// This makes streaming tokens readable as sentences or phrases instead of individual tokens.
    /// </summary>
    public static void AccumulateAndPrint(string updateText, string? continuationToken)
    {
        if (string.IsNullOrEmpty(updateText))
        {
            return; // nothing to accumulate
        }

        if (_accum.Length == 0)
        {
            _firstTimestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        }

        _accum.Append(updateText);

        // Decide when to flush: sentence end, newline, or buffer too long
        var trimmed = updateText.TrimEnd();
        bool endsWithSentence = trimmed.EndsWith('.') || trimmed.EndsWith('!') || trimmed.EndsWith('?');
        bool containsNewline = updateText.Contains("\n");
        bool tooLong = _accum.Length > 250;

        if (endsWithSentence || containsNewline || tooLong)
        {
            FlushAccumulatedInternal(continuationToken);
        }
    }

    /// <summary>
    /// Flushes any remaining accumulated fragments (prints the partial sentence if present).
    /// </summary>
    public static void FlushAccumulated()
    {
        if (_accum.Length > 0)
        {
            FlushAccumulatedInternal(null);
        }
    }

    private static void FlushAccumulatedInternal(string? continuationToken)
    {
        var ts = _firstTimestamp ?? DateTime.Now.ToString("HH:mm:ss.fff");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"[{ts}] ");
        Console.ResetColor();

        var textToPrint = _accum.ToString().Replace("\n", " ").Trim();
        Console.WriteLine(textToPrint);

        if (!string.IsNullOrWhiteSpace(continuationToken))
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($" token: {Truncate(continuationToken,60)}");
            Console.ResetColor();
        }

        _accum.Clear();
        _firstTimestamp = null;
    }

    public static void PrintLabeled(string label, string content)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write($"{label}: ");
        Console.ResetColor();
        Console.WriteLine(content);
    }

    public static void PrintInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void PrintFooter(string text)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine();
        Console.WriteLine(new string('=',60));
        Console.WriteLine(text);
        Console.WriteLine(new string('=',60));
        Console.ResetColor();
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
    }

    internal static void PrintUpdate(string text)
    {
        PrintUpdate(text, "");
    }

    internal static void AccumulateAndPrint(string text)
    {
        AccumulateAndPrint(text, "");
    }
}
