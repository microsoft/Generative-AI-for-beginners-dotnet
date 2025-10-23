public class Bullet
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Direction { get; } // -1 = up, 1 = down
    public bool IsActive { get; set; } = true;
    public ConsoleColor Color { get; }
    public char Symbol { get; }

    public Bullet(int x, int y, int direction, ConsoleColor color, char symbol = '|')
    {
        X = x;
        Y = y;
        Direction = direction;
        Color = color;
        Symbol = symbol;
    }

    public void Move()
    {
        Y += Direction;
    }
}
