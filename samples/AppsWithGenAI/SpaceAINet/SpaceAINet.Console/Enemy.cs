public class Enemy
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Shape { get; }
    public ConsoleColor Color { get; }
    public bool IsAlive { get; set; } = true;
    public bool CanShoot { get; set; } = false;

    public Enemy(int x, int y, string shape, ConsoleColor color)
    {
        X = x;
        Y = y;
        Shape = shape;
        Color = color;
    }

    public int Width => Shape.Length;
}
