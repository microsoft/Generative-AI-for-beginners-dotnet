// Space.AI.NET() - Main Entry Point
// Responsibilities:
// - Hide cursor
// - Show start screen
// - Read speed option (1, 2, 3, or ENTER for default)
// - Clear console
// - Start game loop

Console.CursorVisible = false;
StartScreen.Show();

int speed = 1; // Default: Slow
while (true)
{
    var key = Console.ReadKey(true).Key;
    speed = key switch
    {
        ConsoleKey.Enter or ConsoleKey.D1 or ConsoleKey.NumPad1 => 1,
        ConsoleKey.D2 or ConsoleKey.NumPad2 => 2,
        ConsoleKey.D3 or ConsoleKey.NumPad3 => 3,
        _ => speed
    };
    if (key is ConsoleKey.Enter or ConsoleKey.D1 or ConsoleKey.NumPad1 or ConsoleKey.D2 or ConsoleKey.NumPad2 or ConsoleKey.D3 or ConsoleKey.NumPad3)
        break;
}

Console.Clear();
GameManager.RunGameLoop(speed);
