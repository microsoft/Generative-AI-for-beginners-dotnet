public class RenderState
{
    public char[,] CharBuffer { get; }
    public ConsoleColor[,] ColorBuffer { get; }
    public int Rows { get; }
    public int Cols { get; }

    public RenderState(int rows, int cols)
    {
        Rows = rows;
        Cols = cols;
        CharBuffer = new char[rows, cols];
        ColorBuffer = new ConsoleColor[rows, cols];
        Clear();
    }

    public void Clear()
    {
        for (int y = 0; y < Rows; y++)
            for (int x = 0; x < Cols; x++)
            {
                CharBuffer[y, x] = ' ';
                ColorBuffer[y, x] = ConsoleColor.Black;
            }
    }

    public void Set(int x, int y, char ch, ConsoleColor color)
    {
        if (x >= 0 && x < Cols && y >= 0 && y < Rows)
        {
            CharBuffer[y, x] = ch;
            ColorBuffer[y, x] = color;
        }
    }

    public (char[,], ConsoleColor[,]) GetStateCopy()
    {
        var chars = (char[,])CharBuffer.Clone();
        var colors = (ConsoleColor[,])ColorBuffer.Clone();
        return (chars, colors);
    }
}
