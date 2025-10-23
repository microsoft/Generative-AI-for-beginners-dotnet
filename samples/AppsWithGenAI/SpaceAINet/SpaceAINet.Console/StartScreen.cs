public static class StartScreen
{
    public static void Show()
    {
        int width = Console.WindowWidth;
        int height = Console.WindowHeight;
        int bufferHeight = Console.BufferHeight;
        Console.Clear();

        // Title
        string title = "Space.AI.NET()";
        int titleLeft = (width - title.Length) / 2;
        int titleTop = height / 8;
        if (titleTop >= 0 && titleTop < bufferHeight)
        {
            Console.SetCursorPosition(titleLeft, titleTop);
            if (Console.ForegroundColor != ConsoleColor.Cyan)
                Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(title);
            Console.ResetColor();
        }

        // Subtitle
        string subtitle = "Built with .NET + AI for galactic defense";
        int subtitleLeft = (width - subtitle.Length) / 2;
        int subtitleTop = titleTop + 2;
        if (subtitleTop >= 0 && subtitleTop < bufferHeight)
        {
            Console.SetCursorPosition(subtitleLeft, subtitleTop);
            if (Console.ForegroundColor != ConsoleColor.Gray)
                Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(subtitle);
            Console.ResetColor();
        }

        // Two-column layout for controls and options
        string[] leftCol =
        {
            "[Controls]",
            "RIGHT / LEFT : Move",
            "SPACE : Shoot",
            "Q     : Quit",
            "",
            "[Game Speed]",
            "1: Slow (default)",
            "2: Medium",
            "3: Fast",
            "ENTER: Default"
        };
        string[] rightCol =
        {
            "[AI Modes]",
            "A : Azure OpenAI",
            "O : Ollama",
            "(Switch anytime)",
            "",
            "[Tools]",
            "F : Toggle FPS",
            "S : Save Frame",
            "(any mode)",
            ""
        };
        int colPad = 4;
        int leftColMax = leftCol.Max(static s => s.Length);
        int rightColMax = rightCol.Max(static s => s.Length);
        int colWidth = Math.Max(leftColMax, rightColMax) + colPad;
        int rowCount = Math.Max(leftCol.Length, rightCol.Length);
        int startTop = titleTop + 5;
        int leftStart = (width - (colWidth * 2)) / 2;
        for (int i = 0; i < rowCount; i++)
        {
            int lineTop = startTop + i;
            if (lineTop >= 0 && lineTop < bufferHeight)
            {
                string left = i < leftCol.Length ? leftCol[i] : string.Empty;
                string right = i < rightCol.Length ? rightCol[i] : string.Empty;
                Console.SetCursorPosition(leftStart, lineTop);
                Console.Write(left.PadRight(colWidth));
                Console.Write(right);
            }
        }

        // Learn More link at the bottom, centered
        string learnMore = "Learn more: https://aka.ms/spaceainet";
        int learnMoreTop = startTop + rowCount + 2;
        int learnMoreLeft = (width - learnMore.Length) / 2;
        if (learnMoreTop >= 0 && learnMoreTop < bufferHeight)
        {
            Console.SetCursorPosition(learnMoreLeft, learnMoreTop);
            if (Console.ForegroundColor != ConsoleColor.DarkCyan)
                Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(learnMore);
            Console.ResetColor();
        }
    }
}
