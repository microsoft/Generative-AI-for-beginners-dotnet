public class GameEntities
{
    public Player Player { get; private set; }
    public List<Enemy> Enemies { get; private set; }
    public List<Bullet> PlayerBullets { get; private set; }
    public List<Bullet> EnemyBullets { get; private set; }
    public int Score { get; set; }

    public GameEntities(int boxWidth, int boxHeight)
    {
        Player = new Player(boxWidth / 2, boxHeight - 2);
        PlayerBullets = new List<Bullet>();
        EnemyBullets = new List<Bullet>();
        Enemies = new List<Enemy>();
        // Top row (5 enemies): ><, oo, ><, oo, >< (Red)
        string[] topShapes = { "><", "oo", "><", "oo", "><" };
        int[] topX = { 4, 13, 22, 31, 40 };
        for (int i = 0; i < 5; i++)
            Enemies.Add(new Enemy(topX[i], 3, topShapes[i], ConsoleColor.Red));
        // Bottom row (3 enemies): /O\ (DarkYellow)
        for (int i = 0; i < 3; i++)
            Enemies.Add(new Enemy(8 + i * 16, 6, "/O\\", ConsoleColor.DarkYellow));
        Score = 0;
    }
}
