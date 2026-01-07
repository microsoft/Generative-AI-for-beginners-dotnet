
/// <summary>
/// Small helper to keep console printing consistent and easy to read.
/// Uses simple colored output and labeled sections to make the streaming flow obvious.
/// </summary>
public static class ConsoleHelper
{
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

    public static void PrintLabeled(string label, string content)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"{label}:");
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
}