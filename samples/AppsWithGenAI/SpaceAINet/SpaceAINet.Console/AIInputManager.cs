public class AIInputManager
{
    private readonly IGameActionProvider _aiProvider;
    private readonly int _notifWidth;
    private readonly Action<string> _applyAIAction;
    private readonly Func<char[,], string> _getFrameString;
    private readonly string[] _aiInstructions;
    private string _lastAction = "undefined";
    private string _lastFrameString;
    private string _pendingAIFrameString;
    private Task<GameActionResult?> _aiAnalysisTask;
    private string _lastAppliedAIAction;
    private DateTime _lastAIInstructionTime;

    public AIInputManager(
        IGameActionProvider aiProvider,
        int notifWidth,
        Action<string> applyAIAction,
        Func<char[,], string> getFrameString,
        string[] aiInstructions)
    {
        _aiProvider = aiProvider;
        _notifWidth = notifWidth;
        _applyAIAction = applyAIAction;
        _getFrameString = getFrameString;
        _aiInstructions = aiInstructions;
    }

    public void HandleAIInputNonBlocking(char[,] frontBuffer)
    {
        if (_aiAnalysisTask != null && _aiAnalysisTask.IsCompleted)
        {
            var result = _aiAnalysisTask.Result;
            _aiAnalysisTask = null;
            _lastFrameString = _pendingAIFrameString;
            _pendingAIFrameString = null;
            // AI frame processed, increment FPS-AI
            GameManager.IncrementAIFrame();
            if (result != null && !string.IsNullOrEmpty(result.nextaction))
            {
                _lastAIInstructionTime = DateTime.Now;
                string dateStr = _lastAIInstructionTime.ToString("yyyy-MM-dd HH:mm:ss");
                string actionStr = result.nextaction ?? string.Empty;
                string aiLabel = "AI";
                int minSpace = 2;
                int used = aiLabel.Length + minSpace + dateStr.Length + minSpace + actionStr.Length;
                int remaining = _notifWidth - used;
                int leftPad = remaining > 0 ? remaining / 2 : 0;
                int rightPad = remaining > 0 ? remaining - leftPad : 0;
                string line1 = aiLabel + new string(' ', minSpace + leftPad) + dateStr + new string(' ', minSpace + rightPad) + actionStr;
                if (line1.Length > _notifWidth) line1 = line1[.._notifWidth];
                string explanation = result.explanation ?? string.Empty;
                var wrapLines = new List<string>();
                if (explanation.Length > 0)
                {
                    var words = explanation.Split(' ');
                    string current = string.Empty;
                    foreach (var word in words)
                    {
                        if ((current.Length + word.Length + (current.Length > 0 ? 1 : 0)) > _notifWidth)
                        {
                            wrapLines.Add(current);
                            current = word;
                        }
                        else
                        {
                            current += (current.Length > 0 ? " " : string.Empty) + word;
                        }
                    }
                    if (current.Length > 0) wrapLines.Add(current);
                }
                for (int i = 0; i < 3; i++)
                    _aiInstructions[i + 1] = i < wrapLines.Count ? wrapLines[i] : string.Empty;
                _aiInstructions[0] = line1;
                _lastAction = result.nextaction;
                _lastAppliedAIAction = result.nextaction;
                _applyAIAction(_lastAppliedAIAction);
            }
            else
            {
                _lastAIInstructionTime = DateTime.Now;
                string aiLabel = "AI";
                string dateStr = _lastAIInstructionTime.ToString("yyyy-MM-dd HH:mm:ss");
                string noAction = "No action";
                int minSpace = 2;
                int used = aiLabel.Length + minSpace + dateStr.Length + minSpace + noAction.Length;
                int remaining = _notifWidth - used;
                int leftPad = remaining > 0 ? remaining / 2 : 0;
                int rightPad = remaining > 0 ? remaining - leftPad : 0;
                string line1 = aiLabel + new string(' ', minSpace + leftPad) + dateStr + new string(' ', minSpace + rightPad) + noAction;
                if (line1.Length > _notifWidth) line1 = line1[.._notifWidth];
                _aiInstructions[0] = line1;
                _aiInstructions[1] = result?.explanation ?? string.Empty;
                _aiInstructions[2] = string.Empty;
                _aiInstructions[3] = string.Empty;
                _lastAppliedAIAction = null;
            }
        }
        if (_aiAnalysisTask == null)
        {
            string currentFrameString = _getFrameString(frontBuffer);
            _pendingAIFrameString = currentFrameString;
            _aiAnalysisTask = _aiProvider.AnalyzeFrameWithStringsAsync(currentFrameString, _lastAction);
        }
        if (_aiAnalysisTask != null && !_aiAnalysisTask.IsCompleted && !string.IsNullOrEmpty(_lastAppliedAIAction))
        {
            if (_lastAppliedAIAction is "move_left" or "move_right")
                _applyAIAction(_lastAppliedAIAction);
        }
    }
}
