using Microsoft.Extensions.AI;

public class GameActionProviderBase: IGameActionProvider
{
    public IChatClient chat;
    public readonly string promptTemplate = @"Act as a video game player, with high expertise playing Retro Invaders.
    Your main objective is to kill all the enemy ships while avoiding enemy projectiles. Prioritize eliminating enemy ships over just surviving. Do not get stuck in the corners: if the player ship is at the leftmost or rightmost edge, do not stay there for long, even if it is temporarily safe from enemy attacks, because you will never win the game by staying in a corner.
    You can fire up to 3 times in a row (up to 3 bullets on screen at once). Firing (shooting) is essential to win: fire as often as possible when it is safe and there is a clear shot at an enemy. Do not hesitate to shoot if you have a chance to hit an enemy and you are not in immediate danger.
    Your job is to analyze three game frames understanding the time of each one; use the last performed action, and define the next step to be taken to win the game.
    The game is Retro Invaders, so the only possible actions are: move_left, move_right, fire, stop.
    If there is no available action return stop.
    ===
    The last action performed was: '{0}'.
    Use the current frame to understand the movement and position of game elements.
    ===
    The output should be a JSON object with 2 fields: 'nextaction', and 'explanation'.
    ===
    Sample JSON output :
    '{{ 'nextaction': 'move_right', 'explanation': 'Moving right will help avoid incoming fire and position for a better shot.' }}'
    '{{ 'nextaction': 'move_left', 'explanation': 'Moving left will avoid an alien projectile.' }}'
    '{{ 'nextaction': 'fire', 'explanation': 'Firing at the enemies is crucial to reduce their numbers and ensure survival.' }}'
    '{{ 'nextaction': 'stop', 'explanation': 'Stopping now to avoid unnecessary movement.' }}'";

    public async virtual Task<GameActionResult?> AnalyzeFrameAsync(byte[] currentFrame, string lastAction)
    {
        string prompt = string.Format(promptTemplate, lastAction);
        string llmResponse = string.Empty;

        AIContent aic1 = new DataContent(currentFrame, "image/jpeg");
        List<ChatMessage> messages = new()
        {
            new(ChatRole.User, prompt),
            new(ChatRole.User, [aic1])
        };

        var completionUpdates = await chat.GetResponseAsync(messages);
        llmResponse = completionUpdates.Text;

        llmResponse = CleanLlmJsonResponse(llmResponse);
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<GameActionResult>(llmResponse);
        }
        catch (System.Text.Json.JsonException)
        {
            // Log or handle the error as needed
            return null;
        }
    }

    public async virtual Task<GameActionResult?> AnalyzeFrameWithStringsAsync(string frame1, string lastAction) {
        string prompt = string.Format(promptTemplate, lastAction);
        string llmResponse = string.Empty;

        List<ChatMessage> messages = new()
        {
            new(ChatRole.User, prompt),
            new(ChatRole.User, frame1)
        };

        var completionUpdates = await chat.GetResponseAsync(messages);
        llmResponse = completionUpdates.Text;

        llmResponse = CleanLlmJsonResponse(llmResponse);
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<GameActionResult>(llmResponse);
        }
        catch (System.Text.Json.JsonException)
        {
            // Log or handle the error as needed
            return null;
        }
    }

    public string CleanLlmJsonResponse(string llmResponse)
    {
        if (string.IsNullOrWhiteSpace(llmResponse))
            return llmResponse;
        var lines = llmResponse.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        int start = System.Array.FindIndex(lines, l => l.TrimStart().StartsWith("{"));
        int end = System.Array.FindLastIndex(lines, l => l.TrimEnd().EndsWith("}"));
        string json = null;
        if (start >= 0 && end >= start)
        {
            var jsonLines = lines[start..(end + 1)];
            json = string.Join("\n", jsonLines).Trim();
        }
        else
        {
            json = llmResponse.Trim();
        }
        int lastBrace = json.LastIndexOf('}');
        if (lastBrace >= 0 && lastBrace < json.Length - 1)
        {
            json = json.Substring(0, lastBrace + 1);
        }
        while (json.Length > 0 && json[^1] != '}')
        {
            json = json.Substring(0, json.Length - 1).TrimEnd();
        }
        if (json.EndsWith("}."))
        {
            json = json.Substring(0, json.Length - 1);
        }
        return json;
    }
}
