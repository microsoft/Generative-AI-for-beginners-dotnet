using SpaceAINet.Screenshot;
using System.Diagnostics;
using System.Text;

public static class GameManager
{
    private enum AIState { Off, Aoai, Ollama }
    private const int BoxWidth = 58;
    private const int BoxHeight = 20;
    private const int UiBarHeight = 2;
    private const int AiInfoLines = 4;

    private static GameEntities _entities;
    private static RenderState _frontBuffer;
    private static RenderState _backBuffer;
    private static int _elapsedSeconds;
    private static Stopwatch _timer;
    private static int _gameSpeedMs;
    private static readonly Random _rand = new();
    private static bool _running = true;
    private static int _enemyDirection = 1;
    private static int _enemyMoveCounter;
    private static int _enemyMoveInterval = 8;
    private static int _enemyShootCounter;
    private static int _enemyShootInterval = 18;
    private static bool _gameOver;
    private static bool _win;
    private static AIState _aiState = AIState.Off;
    private static string[] _aiInstructions = new string[AiInfoLines];
    private static IGameActionProvider _aiProvider;
    private static string _lastAction = "undefined";
    private static string _lastFrameString;
    private static string _pendingAIFrameString;
    private static string? _aiEndpoint;
    private static string? _aiModelId;
    private static string? _aiApiKey;
    private static Task<GameActionResult?> _aiAnalysisTask;
    private static string _lastAppliedAIAction;
    private static DateTime _lastAIInstructionTime;
    private static AIInputManager _aiInputManager;
    private static GameRenderer _gameRenderer;

    // FPS tracking
    private static int _frameCount = 0;
    private static int _lastFps = 0;
    private static int _aiFrameCount = 0;
    private static double _lastAiFps = 0;
    private static DateTime _lastFpsUpdate = DateTime.Now;
    private static bool _showFps = false;
    // Use nullable event to avoid CS8618
    public static event Action? AIFrameProcessed;

    // Helper to clear AI instructions
    private static void ClearAIInstructions() => Array.Clear(_aiInstructions, 0, _aiInstructions.Length);

    public static void RunGameLoop(int speed)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var configManager = new GameConfigManager();
        _aiEndpoint = configManager.AzureOpenAIEndpoint;
        _aiModelId = configManager.AzureOpenAIModel;
        _aiApiKey = configManager.AzureOpenAIApiKey;
        StartScreen.Show();
        Console.Clear();
        _aiProvider = null;
        _aiState = AIState.Off;
        _gameSpeedMs = speed switch { 2 => 60, 3 => 35, _ => 100 };
        int rows = BoxHeight + UiBarHeight + 2 + AiInfoLines;
        int cols = BoxWidth + 2;
        _frontBuffer = new RenderState(rows, cols);
        _backBuffer = new RenderState(rows, cols);
        InitEntities();
        ScreenshotService.Initialize();
        _timer = Stopwatch.StartNew();
        _elapsedSeconds = 0;
        _running = true;
        _gameRenderer = new GameRenderer(UiBarHeight, AiInfoLines);
        _frameCount = 0;
        _lastFps = 0;
        _lastFpsUpdate = DateTime.Now;
        _showFps = false;
        AIFrameProcessed += () => _aiFrameCount++;
        while (_running)
        {
            int frameStart = Environment.TickCount;
            _elapsedSeconds = (int)_timer.Elapsed.TotalSeconds;
            // Unified input processing
            ProcessInput();
            if (_aiState != AIState.Off)
            {
                EnsureAIInputManager();
                _aiInputManager.HandleAIInputNonBlocking(_frontBuffer.CharBuffer);
            }
            UpdateEntities();

            // FPS calculation
            _frameCount++;
            var now = DateTime.Now;
            double seconds = (now - _lastFpsUpdate).TotalSeconds;
            if (seconds >= 1)
            {
                _lastFps = _frameCount;
                _frameCount = 0;
                _lastAiFps = _aiFrameCount / seconds;
                _aiFrameCount = 0;
                _lastFpsUpdate = now;
            }

            _gameRenderer.Render(_frontBuffer, _backBuffer, _entities, _gameOver, _win, _aiInstructions, _aiState, true, _lastFps, _lastAiFps);
            int frameTime = Environment.TickCount - frameStart;
            int sleep = _gameSpeedMs - frameTime;
            if (sleep > 0) Thread.Sleep(sleep);
        }
    }

    // Unified input processing for both global and gameplay keys
    private static void ProcessInput()
    {
        if (!Console.KeyAvailable)
            return;

        var key = Console.ReadKey(true);

        // Global keys (work in any mode)
        switch (key.Key)
        {
            case ConsoleKey.A:
                if (_aiState == AIState.Aoai)
                {
                    _aiState = AIState.Off;
                    _aiProvider = null;
                    ClearAIInstructions();
                }
                else
                {
                    _aiProvider = new AoaiGameActionProvider(_aiEndpoint, _aiModelId, _aiApiKey);
                    _aiState = AIState.Aoai;
                    ClearAIInstructions();
                }
                return;
            case ConsoleKey.O:
                if (_aiState == AIState.Ollama)
                {
                    _aiState = AIState.Off;
                    _aiProvider = null;
                    ClearAIInstructions();
                }
                else
                {
                    _aiProvider = new OllamaGameActionProvider();
                    _aiState = AIState.Ollama;
                    ClearAIInstructions();
                }
                return;
            case ConsoleKey.Q:
                _running = false;
                return;
            case ConsoleKey.F:
                _showFps = !_showFps;
                return;
            case ConsoleKey.S:
                // Screenshot is now a global key
                ScreenshotService.Capture(_frontBuffer.CharBuffer, _frontBuffer.ColorBuffer);
                return;
        }

        // Only process gameplay keys if not in AI mode
        if (_aiState == AIState.Off)
        {
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    _entities.Player.MoveLeft(0);
                    break;
                case ConsoleKey.RightArrow:
                    _entities.Player.MoveRight(BoxWidth - 1);
                    break;
                case ConsoleKey.Spacebar:
                    if (_entities.Player.CanShoot())
                    {
                        _entities.PlayerBullets.Add(new Bullet(_entities.Player.X, _entities.Player.Y - 1, -1, ConsoleColor.Cyan));
                        _entities.Player.OnBulletFired();
                    }
                    break;
            }
        }
    }

    private static void EnsureAIInputManager()
    {
        if (_aiInputManager == null)
        {
            _aiInputManager = new AIInputManager(
                _aiProvider,
                _frontBuffer.Cols - 4,
                ApplyAIAction,
                GetFrameString,
                _aiInstructions
            );
        }
    }

    private static void InitEntities() => _entities = new GameEntities(BoxWidth, BoxHeight);

    private static void ApplyAIAction(string action)
    {
        switch (action)
        {
            case "move_left":
                _entities.Player.MoveLeft(0);
                break;
            case "move_right":
                _entities.Player.MoveRight(BoxWidth - 1);
                break;
            case "fire":
                if (_entities.Player.CanShoot())
                {
                    _entities.PlayerBullets.Add(new Bullet(_entities.Player.X, _entities.Player.Y - 1, -1, ConsoleColor.Cyan));
                    _entities.Player.OnBulletFired();
                }
                break;
        }
    }

    private static void UpdateEntities()
    {
        for (int i = _entities.PlayerBullets.Count - 1; i >= 0; i--)
        {
            var b = _entities.PlayerBullets[i];
            b.Move();
            if (b.Y < 1)
            {
                _entities.PlayerBullets.RemoveAt(i);
                _entities.Player.OnBulletDestroyed();
                continue;
            }
            foreach (var e in _entities.Enemies)
            {
                if (e.IsAlive && b.Y == e.Y && b.X >= e.X && b.X < e.X + e.Width)
                {
                    e.IsAlive = false;
                    _entities.Score += 100;
                    _entities.PlayerBullets.RemoveAt(i);
                    _entities.Player.OnBulletDestroyed();
                    break;
                }
            }
        }
        _enemyMoveCounter++;
        if (_enemyMoveCounter >= _enemyMoveInterval)
        {
            _enemyMoveCounter = 0;
            bool edge = false;
            foreach (var e in _entities.Enemies)
            {
                if (!e.IsAlive) continue;
                int nextX = e.X + _enemyDirection;
                if (nextX < 0 || nextX + e.Width > BoxWidth)
                {
                    edge = true;
                    break;
                }
            }
            if (edge)
            {
                foreach (var e in _entities.Enemies)
                    if (e.IsAlive) e.Y++;
                _enemyDirection *= -1;
            }
            else
            {
                foreach (var e in _entities.Enemies)
                    if (e.IsAlive) e.X += _enemyDirection;
            }
        }
        _enemyShootCounter++;
        if (_enemyShootCounter >= _enemyShootInterval)
        {
            _enemyShootCounter = 0;
            int aliveCount = 0;
            foreach (var e in _entities.Enemies)
                if (e.IsAlive) aliveCount++;
            if (aliveCount > 0)
            {
                int shooterIdx = _rand.Next(aliveCount);
                int found = 0;
                foreach (var e in _entities.Enemies)
                {
                    if (!e.IsAlive) continue;
                    if (found == shooterIdx)
                    {
                        _entities.EnemyBullets.Add(new Bullet(e.X + e.Width / 2, e.Y + 1, 1, ConsoleColor.Yellow, 'v'));
                        break;
                    }
                    found++;
                }
            }
        }
        for (int i = _entities.EnemyBullets.Count - 1; i >= 0; i--)
        {
            var b = _entities.EnemyBullets[i];
            b.Move();
            if (b.Y >= BoxHeight - 1)
            {
                _entities.EnemyBullets.RemoveAt(i);
                continue;
            }
            if (b.Y == _entities.Player.Y && b.X == _entities.Player.X)
            {
                _gameOver = true;
                _running = false;
                return;
            }
        }
        foreach (var e in _entities.Enemies)
        {
            if (e.IsAlive && e.Y >= _entities.Player.Y)
            {
                _gameOver = true;
                _running = false;
                return;
            }
        }
        if (_entities.Enemies.All(e => !e.IsAlive))
        {
            _win = true;
            _running = false;
        }
    }

    private static string GetFrameString(char[,] charBuffer)
    {
        var now = DateTime.Now;
        var sb = new StringBuilder();
        sb.Append($"FRAME - {now:yyyy-MM-dd HH:mm:ss.fff}\n");
        int rows = charBuffer.GetLength(0);
        int cols = charBuffer.GetLength(1);
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
                sb.Append(charBuffer[y, x]);
            sb.Append('\n');
        }
        return sb.ToString();
    }

    public static (char[,], ConsoleColor[,]) GetRenderState() => _frontBuffer.GetStateCopy();
    public static void IncrementAIFrame() => AIFrameProcessed?.Invoke();
}
