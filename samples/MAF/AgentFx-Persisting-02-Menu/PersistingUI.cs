/// <summary>
/// Small UI helper to centralize console input/output and make future console behavior changes easier.
/// Delegates formatted printing to `StreamConsoleHelper` where appropriate.
/// This class consolidates previous UIHelper and PersistingUI helper implementations.
/// </summary>
internal static class PersistingUI
{
    public static void Clear() => Console.Clear();

    public static void PrintMenu()
    {
        Console.WriteLine("Choose an option:");
        Console.WriteLine("1) Start a new thread (persisted)");
        Console.WriteLine("2) Start a simple session (no persisted thread)");
        Console.WriteLine("3) Load saved thread and continue");
        Console.WriteLine("0) Exit");
        Console.Write("Selection: ");
    }

    public static string ReadSelection()
    {
        var input = Console.ReadLine()?.Trim() ?? string.Empty;
        return input;
    }

    public static string PromptInput(string prompt)
    {
        Console.Write(prompt);
        var input = Console.ReadLine() ?? string.Empty;
        return input;
    }

    public static void PrintMessage(string message)
    {
        Console.WriteLine(message);
    }

    public static void WaitForKey()
    {
        Console.ReadKey(true);
    }
}