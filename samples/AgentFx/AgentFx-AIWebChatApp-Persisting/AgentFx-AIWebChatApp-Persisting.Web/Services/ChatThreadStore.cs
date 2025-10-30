using System.Text.Json;
using Microsoft.Agents.AI;

namespace AgentFx_AIWebChatApp_Persisting.Web.Services;

/// <summary>
/// File-based persistence for an <see cref="AgentThread"/> using the Agent Framework's
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
    /// Loads a persisted thread; if none exists, returns a new thread instance from the provided agent.
    /// </summary>
    public async Task<AgentThread> LoadThreadAsync(AIAgent agent, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            return agent.GetNewThread();
        }
        var json = await File.ReadAllTextAsync(_filePath, cancellationToken);
        if (string.IsNullOrWhiteSpace(json))
        {
            return agent.GetNewThread();
        }
        var element = JsonSerializer.Deserialize<JsonElement>(json, _jsonOptions);
        return agent.DeserializeThread(element, _jsonOptions);
    }

    /// <summary>
    /// Persists the specified thread state to disk (overwrite).
    /// </summary>
    public async Task SaveThreadAsync(AgentThread thread, CancellationToken cancellationToken = default)
    {
        var serialized = thread.Serialize(_jsonOptions).GetRawText();
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
