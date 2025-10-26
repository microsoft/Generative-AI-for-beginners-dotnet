using HFMCP.GenImage.Web.Services;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using Azure.AI.OpenAI;
using Azure.AI.Inference;
using Azure;
using System.ClientModel;
using System.Text;
using System.Text.Json;

namespace HFMCP.GenImage.Web.Services;

public class AIService : IAIService
{
    private readonly IAIConfigurationService _configurationService;
    private readonly ILogger<AIService> _logger;
    private readonly HttpClient _httpClient;
    private IMcpClient? _mcpClient;

    public AIService(IAIConfigurationService configurationService, ILogger<AIService> logger, HttpClient httpClient)
    {
        _configurationService = configurationService;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<bool> IsConfiguredAsync()
    {
        return await _configurationService.IsConfigurationValidAsync();
    }

    public async Task<AIResponse> SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        try
        {
            var config = await _configurationService.GetConfigurationAsync();

            if (string.IsNullOrEmpty(config.HuggingFaceToken))
            {
                return new AIResponse
                {
                    IsError = true,
                    ErrorMessage = "AI configuration is incomplete. Please configure your Hugging Face token in Settings. GitHub Models or Azure OpenAI is also required for chat functionality."
                };
            }

            // Check if at least one AI provider is configured for chat
            if (string.IsNullOrEmpty(config.GitHubToken) &&
                (string.IsNullOrEmpty(config.AzureOpenAIEndpoint) || string.IsNullOrEmpty(config.AzureOpenAIApiKey)))
            {
                return new AIResponse
                {
                    IsError = true,
                    ErrorMessage = "Please configure either GitHub Models token or Azure OpenAI credentials for chat functionality."
                };
            }

            // Initialize MCP client for Hugging Face tools
            await InitializeMCPClientAsync(config);

            // Get MCP tools
            var tools = await GetMCPToolsAsync();

            // Get the appropriate chat client
            IChatClient chatClient = await GetChatClientAsync(config);

            // Create chat options with MCP tools
            var chatOptions = new ChatOptions
            {
                Tools = [.. tools],
                ModelId = config.ModelName
            };

            // Send message with MCP tool support
            var response = await chatClient.GetResponseAsync(message, chatOptions, cancellationToken);

            var responseText = response.Text ?? "No response received";
            var imageUrls = ExtractImageUrls(responseText);

            return new AIResponse
            {
                TextContent = responseText,
                ImageUrls = imageUrls,
                IsError = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message to AI service with MCP");
            return new AIResponse
            {
                IsError = true,
                ErrorMessage = $"An error occurred: {ex.Message}"
            };
        }
    }

    private async Task InitializeMCPClientAsync(AIConfiguration config)
    {
        if (_mcpClient != null) return;

        try
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
            _logger.LogInformation("MCP client initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize MCP client");
            throw;
        }
    }

    private async Task<IList<McpClientTool>> GetMCPToolsAsync()
    {
        if (_mcpClient == null) return new List<McpClientTool>();

        try
        {
            var tools = await _mcpClient.ListToolsAsync();
            _logger.LogInformation($"Retrieved {tools.Count} MCP tools");
            return tools;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving MCP tools");
            return new List<McpClientTool>();
        }
    }

    private async Task<IChatClient> GetChatClientAsync(AIConfiguration config)
    {
        // Prioritize Azure OpenAI, then fallback to GitHub Models
        if (!string.IsNullOrEmpty(config.AzureOpenAIEndpoint) && !string.IsNullOrEmpty(config.AzureOpenAIApiKey))
        {
            return await CreateAzureOpenAIChatClientAsync(config);
        }
        else if (!string.IsNullOrEmpty(config.GitHubToken))
        {
            return CreateGitHubModelsChatClient(config);
        }

        throw new InvalidOperationException("No configured AI provider available for chat.");
    }

    private Task<IChatClient> CreateAzureOpenAIChatClientAsync(AIConfiguration config)
    {
        var endpoint = config.AzureOpenAIEndpoint?.TrimEnd('/') ?? "";
        var apiKey = new ApiKeyCredential(config.AzureOpenAIApiKey ?? "");

        var azureClient = new AzureOpenAIClient(new Uri(endpoint), apiKey);
        var chatClient = azureClient.GetChatClient(config.ModelName);

        // Use the extension method like in the sample
        var client = chatClient.AsIChatClient()
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();

        return Task.FromResult(client);
    }

    private IChatClient CreateGitHubModelsChatClient(AIConfiguration config)
    {
        var completionsClient = new ChatCompletionsClient(
            endpoint: new Uri("https://models.github.ai/inference"),
            new AzureKeyCredential(config.GitHubToken!));

        // Use the extension method like in the sample
        return completionsClient.AsIChatClient(config.ModelName)
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();
    }

    private List<string> ExtractImageUrls(string responseText)
    {
        var imageUrls = new List<string>();

        if (string.IsNullOrEmpty(responseText))
            return imageUrls;

        try
        {
            // Pattern to match markdown image syntax: ![alt text](url)
            var markdownImagePattern = @"!\[.*?\]\((https?://[^\s\)]+(?:\.(jpg|jpeg|png|gif|bmp|webp|svg))?)\)";
            var markdownMatches = System.Text.RegularExpressions.Regex.Matches(responseText, markdownImagePattern,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            foreach (System.Text.RegularExpressions.Match match in markdownMatches)
            {
                if (match.Groups.Count > 1)
                {
                    var url = match.Groups[1].Value;
                    if (!imageUrls.Contains(url))
                        imageUrls.Add(url);
                }
            }

            // Pattern to match direct HTTP/HTTPS URLs that end with image extensions
            var directImagePattern = @"(https?://[^\s]+\.(?:jpg|jpeg|png|gif|bmp|webp|svg))";
            var directMatches = System.Text.RegularExpressions.Regex.Matches(responseText, directImagePattern,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            foreach (System.Text.RegularExpressions.Match match in directMatches)
            {
                var url = match.Groups[1].Value;
                if (!imageUrls.Contains(url))
                    imageUrls.Add(url);
            }

            // Pattern to match Hugging Face specific URLs (like the one in the example)
            var huggingFacePattern = @"(https?://[^\s]*\.hf\.space[^\s]*\.(?:jpg|jpeg|png|gif|bmp|webp|svg)[^\s]*)";
            var hfMatches = System.Text.RegularExpressions.Regex.Matches(responseText, huggingFacePattern,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            foreach (System.Text.RegularExpressions.Match match in hfMatches)
            {
                var url = match.Groups[1].Value;
                // Clean up any trailing punctuation or characters
                url = System.Text.RegularExpressions.Regex.Replace(url, @"[^\w\-\./?=&:]+$", "");
                if (!imageUrls.Contains(url))
                    imageUrls.Add(url);
            }

            _logger.LogInformation($"Extracted {imageUrls.Count} image URLs from response");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting image URLs from response");
        }

        return imageUrls;
    }
}

// Response models for Azure OpenAI API
public class AzureOpenAIResponse
{
    public Choice[]? choices { get; set; }
}

// Response models for GitHub Models API
public class GitHubModelsResponse
{
    public Choice[]? choices { get; set; }
}

public class Choice
{
    public Message? message { get; set; }
}

public class Message
{
    public string? content { get; set; }
}
