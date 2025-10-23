using ModelContextProtocol.Client;

namespace HFMCP.GenImage.Web.Services;

public interface IMCPService
{
    Task<IList<McpClientTool>> GetHuggingFaceToolsAsync();
    Task<bool> IsHuggingFaceConfiguredAsync();
}

public class MCPService : IMCPService
{
    private readonly IAIConfigurationService _configurationService;
    private readonly ILogger<MCPService> _logger;
    private IMcpClient? _mcpClient;

    public MCPService(IAIConfigurationService configurationService, ILogger<MCPService> logger)
    {
        _configurationService = configurationService;
        _logger = logger;
    }

    public async Task<bool> IsHuggingFaceConfiguredAsync()
    {
        var config = await _configurationService.GetConfigurationAsync();
        return !string.IsNullOrEmpty(config.HuggingFaceToken);
    }

    public async Task<IList<McpClientTool>> GetHuggingFaceToolsAsync()
    {
        try
        {
            var config = await _configurationService.GetConfigurationAsync();

            if (string.IsNullOrEmpty(config.HuggingFaceToken))
            {
                return new List<McpClientTool>();
            }

            // Initialize MCP client if not already done
            if (_mcpClient == null)
            {
                var hfHeaders = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {config.HuggingFaceToken}" }
                };

                var clientTransport = new SseClientTransport(
                    new()
                    {
                        Name = "HF Server",
                        Endpoint = new Uri(config.HuggingFaceMCPServer),
                        AdditionalHeaders = hfHeaders
                    });

                _mcpClient = await McpClientFactory.CreateAsync(clientTransport);
            }

            // Get the available tools
            var tools = await _mcpClient.ListToolsAsync();
            return tools;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Hugging Face MCP tools");
            return new List<McpClientTool>();
        }
    }
}
