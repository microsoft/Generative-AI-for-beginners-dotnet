using System.Drawing;
using System.Drawing.Imaging;

namespace SpaceAINet.Screenshot;

public static class ScreenshotService
{
    private static readonly string Folder = "screenshots";
    private static int _counter = 0;
    private static readonly string FontName = "Consolas";
    private static readonly int FontSize = 16;

    public static void Initialize()
    {
        if (Directory.Exists(Folder))
            Directory.Delete(Folder, true);
        Directory.CreateDirectory(Folder);
        _counter = 0;
    }

    public static void Capture(char[,] chars, ConsoleColor[,] colors)
    {
        int rows = chars.GetLength(0);
        int cols = chars.GetLength(1);
        int cellWidth = FontSize;
        int cellHeight = FontSize + 2;
        using var bmp = new Bitmap(cols * cellWidth, rows * cellHeight);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.Black);
        using var font = new Font(FontName, FontSize, FontStyle.Regular, GraphicsUnit.Pixel);
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                char ch = chars[y, x];
                if (ch == ' ') continue;
                var color = ToDrawingColor(colors[y, x]);
                using var brush = new SolidBrush(color);
                g.DrawString(ch.ToString(), font, brush, x * cellWidth, y * cellHeight);
            }
        }
        string baseName = $"screenshot_{_counter++:D4}";
        string imagePath = Path.Combine(Folder, baseName + ".png");
        bmp.Save(imagePath, ImageFormat.Png);

        // Save the frame as a text file
        string textPath = Path.Combine(Folder, baseName + ".txt");
        using (var writer = new StreamWriter(textPath))
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    writer.Write(chars[y, x]);
                }
                writer.WriteLine();
            }
        }
    }

    public static byte[] GetJpegBytes(char[,] chars, ConsoleColor[,] colors)
    {
        int rows = chars.GetLength(0);
        int cols = chars.GetLength(1);
        int cellWidth = FontSize;
        int cellHeight = FontSize + 2;
        using var bmp = new Bitmap(cols * cellWidth, rows * cellHeight);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.Black);
        using var font = new Font(FontName, FontSize, FontStyle.Regular, GraphicsUnit.Pixel);
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                char ch = chars[y, x];
                if (ch == ' ') continue;
                var color = ToDrawingColor(colors[y, x]);
                using var brush = new SolidBrush(color);
                g.DrawString(ch.ToString(), font, brush, x * cellWidth, y * cellHeight);
            }
        }
        using var ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Jpeg);
        return ms.ToArray();
    }

    private static Color ToDrawingColor(ConsoleColor c)
    {
        return c switch
        {
            ConsoleColor.Black => Color.Black,
            ConsoleColor.DarkBlue => Color.DarkBlue,
            ConsoleColor.DarkGreen => Color.DarkGreen,
            ConsoleColor.DarkCyan => Color.DarkCyan,
            ConsoleColor.DarkRed => Color.DarkRed,
            ConsoleColor.DarkMagenta => Color.DarkMagenta,
            ConsoleColor.DarkYellow => Color.FromArgb(128, 128, 0),
            ConsoleColor.Gray => Color.Gray,
            ConsoleColor.DarkGray => Color.DarkGray,
            ConsoleColor.Blue => Color.Blue,
            ConsoleColor.Green => Color.Green,
            ConsoleColor.Cyan => Color.Cyan,
            ConsoleColor.Red => Color.Red,
            ConsoleColor.Magenta => Color.Magenta,
            ConsoleColor.Yellow => Color.Yellow,
            ConsoleColor.White => Color.White,
            _ => Color.White
        };
    }
}
