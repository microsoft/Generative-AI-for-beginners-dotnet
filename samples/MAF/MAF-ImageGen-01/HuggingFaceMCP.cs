using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Client;

namespace MAF_ImageGen_01;

public class HuggingFaceMCP
{
    public static async Task<(McpClient Client, IList<McpClientTool> Tools)> GetHuggingFaceMCPClientAndToolsAsync()
    {
        var builder = Host.CreateApplicationBuilder();
        var config = builder.Configuration
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>()
            .Build();

        var hfHeaders = new Dictionary<string, string>
    {
        { "Authorization", $"Bearer {config["HF_API_KEY"]}" }
    };

        var clientTransport = new HttpClientTransport(
            new()
            {
                Name = "HF Server",
                Endpoint = new Uri("https://huggingface.co/mcp"),
                AdditionalHeaders = hfHeaders
            });

        var mcpClient = await McpClient.CreateAsync(clientTransport);
        var tools = await mcpClient.ListToolsAsync();

        return (mcpClient, tools);
    }
}
