public class GameRenderer
{
    private readonly int _uiBarHeight;
    private readonly int _aiInfoLines;

    public GameRenderer(int uiBarHeight, int aiInfoLines)
    {
        _uiBarHeight = uiBarHeight;
        _aiInfoLines = aiInfoLines;
    }

    public void Render(
        RenderState frontBuffer,
        RenderState backBuffer,
        GameEntities entities,
        bool gameOver,
        bool win,
        string[] aiInstructions,
        object aiStateObj,
        bool showFps,
        int fps,
        double aiFps)
    {
        backBuffer.Clear();
        DrawBoundingBox(backBuffer);
        DrawUI(backBuffer, entities, aiStateObj, fps, aiFps);
        if (!gameOver && !win)
            backBuffer.Set(entities.Player.X + 1, entities.Player.Y + _uiBarHeight + 1, entities.Player.Symbol, entities.Player.Color);
        foreach (var e in entities.Enemies)
        {
            if (!e.IsAlive) continue;
            for (int i = 0; i < e.Shape.Length; i++)
                backBuffer.Set(e.X + 1 + i, e.Y + _uiBarHeight + 1, e.Shape[i], e.Color);
        }
        foreach (var b in entities.PlayerBullets)
            backBuffer.Set(b.X + 1, b.Y + _uiBarHeight + 1, b.Symbol, b.Color);
        foreach (var b in entities.EnemyBullets)
            backBuffer.Set(b.X + 1, b.Y + _uiBarHeight + 1, b.Symbol, b.Color);
        if (gameOver || win)
        {
            string msg = win ? "YOU WIN!" : "GAME OVER";
            string info = "Press Q to quit...";
            int mid = backBuffer.Cols / 2;
            for (int i = 0; i < msg.Length; i++)
                backBuffer.Set(mid - msg.Length / 2 + i, backBuffer.Rows / 2, msg[i], ConsoleColor.Magenta);
            for (int i = 0; i < info.Length; i++)
                backBuffer.Set(mid - info.Length / 2 + i, backBuffer.Rows / 2 + 2, info[i], ConsoleColor.Gray);
        }
        // AI info
        var aiState = aiStateObj.ToString();
        if (aiState != "Off")
        {
            int baseLine = backBuffer.Rows - _aiInfoLines - 1;
            for (int i = 0; i < _aiInfoLines; i++)
            {
                string line = aiInstructions != null && aiInstructions.Length > i && aiInstructions[i] != null ? aiInstructions[i] : string.Empty;
                int left = 2;
                for (int j = 0; j < line.Length && left + j < backBuffer.Cols - 1; j++)
                    backBuffer.Set(left + j, baseLine + i, line[j], ConsoleColor.Green);
            }
        }
        for (int y = 0; y < backBuffer.Rows; y++)
        {
            for (int x = 0; x < backBuffer.Cols; x++)
            {
                if (frontBuffer.CharBuffer[y, x] != backBuffer.CharBuffer[y, x] ||
                    frontBuffer.ColorBuffer[y, x] != backBuffer.ColorBuffer[y, x])
                {
                    Console.SetCursorPosition(x, y);
                    Console.ForegroundColor = backBuffer.ColorBuffer[y, x];
                    Console.Write(backBuffer.CharBuffer[y, x]);
                    frontBuffer.CharBuffer[y, x] = backBuffer.CharBuffer[y, x];
                    frontBuffer.ColorBuffer[y, x] = backBuffer.ColorBuffer[y, x];
                }
            }
        }
        Console.ResetColor();
    }

    private void DrawBoundingBox(RenderState buf)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        int w = buf.Cols;
        int h = buf.Rows;
        // Use Unicode box-drawing characters
        char tl = '┌', tr = '┐', bl = '└', br = '┘', hline = '─', vline = '│';
        buf.Set(0, 0, tl, ConsoleColor.Gray);
        buf.Set(w - 1, 0, tr, ConsoleColor.Gray);
        buf.Set(0, h - 1, bl, ConsoleColor.Gray);
        buf.Set(w - 1, h - 1, br, ConsoleColor.Gray);
        for (int x = 1; x < w - 1; x++)
        {
            buf.Set(x, 0, hline, ConsoleColor.Gray);
            buf.Set(x, h - 1, hline, ConsoleColor.Gray);
        }
        for (int y = 1; y < h - 1; y++)
        {
            buf.Set(0, y, vline, ConsoleColor.Gray);
            buf.Set(w - 1, y, vline, ConsoleColor.Gray);
        }
    }

    private void DrawUI(RenderState buf, GameEntities entities, object aiStateObj, int fps, double aiFps)
    {
        // First line: Score, Time, Bullets
        string score = $"Score: {entities.Score:0000}";
        string time = $"Time: 00s"; // Time can be injected if needed
        string bullets = $"Bullets: {entities.Player.MaxBullets - entities.Player.ActiveBullets}/{entities.Player.MaxBullets}";
        string ui1 = $"{score}   {time}   {bullets}";
        int left1 = (buf.Cols - ui1.Length) / 2;
        for (int i = 0; i < ui1.Length; i++)
            buf.Set(left1 + i, 1, ui1[i], ConsoleColor.White);

        // Second line: AI Mode and FPS (always show FPS)
        string aiMode = aiStateObj.ToString() switch
        {
            "Aoai" => "AI_MODE: AOAI",
            "Ollama" => "AI_MODE: OLLAMA",
            _ => "AI_MODE: False"
        };
        string fpsStr = $"FPS: {fps}";
        string aiFpsStr = $"FPS-AI: {aiFps:F2}";
        string ui2 = $"{aiMode}   {fpsStr}   {aiFpsStr}";
        int left2 = (buf.Cols - ui2.Length) / 2;
        for (int i = 0; i < ui2.Length; i++)
            buf.Set(left2 + i, 2, ui2[i], ConsoleColor.White);

        // Draw a separator line above the notification/AI area
        int separatorY = buf.Rows - _aiInfoLines - 2;
        for (int x = 1; x < buf.Cols - 1; x++)
            buf.Set(x, separatorY, '─', ConsoleColor.DarkGray);
    }
}
