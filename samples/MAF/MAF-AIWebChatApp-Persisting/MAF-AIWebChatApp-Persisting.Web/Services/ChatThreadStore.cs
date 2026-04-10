using System.Text.Json;
using Microsoft.Agents.AI;

namespace MAF_AIWebChatApp_Persisting.Web.Services;

/// <summary>
/// File-based persistence for an <see cref="AgentSession"/> using the Agent Framework's
/// built-in serialization. Stores JSON in wwwroot/App_Data/agent-thread.json.
/// </summary>
public class ChatThreadStore
{
    private readonly string _filePath;
    private static readonly JsonSerializerOptions _jsonOptions = JsonSerializerOptions.Web;

    public ChatThreadStore(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.WebRootPath, "App_Data");
        Directory.CreateDirectory(dataDir);
        _filePath = Path.Combine(dataDir, "agent-thread.json");
    }

    /// <summary>
    /// Loads a persisted session; if none exists, returns a new session instance from the provided agent.
    /// </summary>
    public async Task<AgentSession> LoadThreadAsync(AIAgent agent, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            return await agent.CreateSessionAsync();
        }
        var json = await File.ReadAllTextAsync(_filePath, cancellationToken);
        if (string.IsNullOrWhiteSpace(json))
        {
            return await agent.CreateSessionAsync();
        }
        var element = JsonSerializer.Deserialize<JsonElement>(json, _jsonOptions);
        return await agent.DeserializeSessionAsync(element, _jsonOptions);
    }

    /// <summary>
    /// Persists the specified session state to disk (overwrite).
    /// </summary>
    public async Task SaveThreadAsync(AIAgent agent, AgentSession session, CancellationToken cancellationToken = default)
    {
        var serialized = (await agent.SerializeSessionAsync(session, _jsonOptions)).GetRawText();
        await File.WriteAllTextAsync(_filePath, serialized, cancellationToken);
    }

    /// <summary>
    /// Deletes the persisted thread file (used on hard reset).
    /// </summary>
    public void DeletePersistedThread()
    {
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }
    }
}
