using Spectre.Console;

public static class SpectreConsoleOutput
{
    public static void DisplayTitle(string title = "TITLE")
    {
        AnsiConsole.Write(new FigletText(title).Centered().Color(Color.Purple));
    }

    public static void DisplayTitleH1(string subtitle)
    {
        // create a line using the lenght of the subtitle text, and for each caracter use the "="
        var line = new string('=', subtitle.Length);

        AnsiConsole.MarkupLine($"[bold][green]===={line}====[/][/]");
        AnsiConsole.MarkupLine($"[bold][green]=== {subtitle} ===[/][/]");
        AnsiConsole.MarkupLine($"[bold][green]===={line}====[/][/]");
        AnsiConsole.MarkupLine($"");
    }

    public static void DisplayTitleH2(string subtitle)
    {
        AnsiConsole.MarkupLine($"[bold][blue]=== {subtitle} ===[/][/]");
        AnsiConsole.MarkupLine($"");
    }

    public static void DisplayTitleH3(string subtitle)
    {
        AnsiConsole.MarkupLine($"[bold]>> {subtitle}[/]");
        AnsiConsole.MarkupLine($"");
    }

    public static void DisplaySeparator()
    {
        AnsiConsole.MarkupLine($"");
        AnsiConsole.MarkupLine($"[bold][blue]==============[/][/]");
        AnsiConsole.MarkupLine($"");
    }

    public static void WriteGreen(string message)
    {
        AnsiConsole.Markup($"[green]{message}[/]");
    }

    public static void DisplayQuestion(string question)
    {
        AnsiConsole.MarkupLine($"[bold][blue]>>Q: {question}[/][/]");
        AnsiConsole.MarkupLine($"");
    }
    public static void DisplayAnswerStart(string answerPrefix)
    {
        AnsiConsole.Markup($"[bold][blue]>> {answerPrefix}:[/][/]");
    }

    public static void DisplayFilePath(string prefix, string filePath)
    {
        var path = new TextPath(filePath);

        AnsiConsole.Markup($"[bold][blue]>> {prefix}: [/][/]");
        AnsiConsole.Write(path);
        AnsiConsole.MarkupLine($"");
    }

    public static void DisplaySubtitle(string prefix, string content)
    {
        AnsiConsole.Markup($"[bold][blue]>> {prefix}: [/][/]");
        AnsiConsole.WriteLine(content);
        AnsiConsole.MarkupLine($"");
    }

    public static int AskForNumber(string question)
    {
        var number = AnsiConsole.Ask<int>(@$"[green]{question}[/]");
        return number;
    }

    public static string AskForString(string question)
    {
        var response = AnsiConsole.Ask<string>(@$"[green]{question}[/]");
        return response;
    }

    public static void DisplayTablePrompts(string systemPrompt, string userPrompt)
    {
        DisplayTitleH1("Prompts used");

        var tablePrompts = new Table();
        tablePrompts.AddColumn("Type");
        tablePrompts.AddColumn("Content");
        tablePrompts.AddRow(new Text("System Prompt"), new Text(systemPrompt));
        tablePrompts.AddEmptyRow();
        tablePrompts.AddRow(new Text("User Prompt"), new Text(userPrompt));
        AnsiConsole.Write(tablePrompts);
    }
}