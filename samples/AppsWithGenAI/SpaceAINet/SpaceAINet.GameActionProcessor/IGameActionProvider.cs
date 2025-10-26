
public interface IGameActionProvider
{
    Task<GameActionResult?> AnalyzeFrameAsync(byte[] currentFrame, string lastAction);

    Task<GameActionResult?> AnalyzeFrameWithStringsAsync(string currentFrameString, string lastAction);
}