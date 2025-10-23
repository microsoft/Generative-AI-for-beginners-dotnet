using Microsoft.Extensions.AI;

namespace HFMCP.GenImage.Web.Services;

public interface IAIService
{
    Task<AIResponse> SendMessageAsync(string message, CancellationToken cancellationToken = default);
    Task<bool> IsConfiguredAsync();
}

public class AIResponse
{
    public string? TextContent { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public bool IsError { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class ChatMessage
{
    public string Content { get; set; } = string.Empty;
    public bool IsUser { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsError { get; set; }
}
