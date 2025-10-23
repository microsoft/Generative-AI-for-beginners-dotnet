public class Player
{
    public int X { get; set; }
    public int Y { get; set; }
    public int MaxBullets { get; } = 3;
    public int ActiveBullets { get; set; } = 0;
    public char Symbol => 'A';
    public ConsoleColor Color => ConsoleColor.Green;

    public Player(int startX, int startY)
    {
        X = startX;
        Y = startY;
    }

    public void MoveLeft(int minX)
    {
        if (X > minX) X--;
    }

    public void MoveRight(int maxX)
    {
        if (X < maxX) X++;
    }

    public bool CanShoot() => ActiveBullets < MaxBullets;

    public void OnBulletFired() => ActiveBullets++;
    public void OnBulletDestroyed() => ActiveBullets = Math.Max(0, ActiveBullets - 1);
}
