using HFMCP.GenImage.Web.Services;
using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

namespace HFMCP.GenImage.Web.Services;

public class AIConfigurationService : IAIConfigurationService
{
    private readonly IDataProtector _protector;
    private readonly ILogger<AIConfigurationService> _logger;
    private const string ConfigurationKey = "AIConfiguration";

    public AIConfigurationService(IDataProtectionProvider dataProtectionProvider, ILogger<AIConfigurationService> logger)
    {
        _protector = dataProtectionProvider.CreateProtector("HFMCP.GenImage.AIConfiguration");
        _logger = logger;
    }

    public Task<AIConfiguration> GetConfigurationAsync()
    {
        try
        {
            // In a real application, this would read from a secure store
            // For development, we'll use user secrets and in-memory storage
            var configJson = Environment.GetEnvironmentVariable("AI_CONFIG");

            if (string.IsNullOrEmpty(configJson))
            {
                // Return default configuration
                return Task.FromResult(new AIConfiguration());
            }

            var decryptedJson = _protector.Unprotect(configJson);
            var config = JsonSerializer.Deserialize<AIConfiguration>(decryptedJson);
            return Task.FromResult(config ?? new AIConfiguration());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading AI configuration");
            return Task.FromResult(new AIConfiguration());
        }
    }

    public Task SaveConfigurationAsync(AIConfiguration configuration)
    {
        try
        {
            var configJson = JsonSerializer.Serialize(configuration);
            var encryptedJson = _protector.Protect(configJson);

            // In a real application, this would save to a secure store
            // For development, we'll use environment variables
            Environment.SetEnvironmentVariable("AI_CONFIG", encryptedJson);

            _logger.LogInformation("AI configuration saved successfully");
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving AI configuration");
            throw;
        }
    }

    public async Task<bool> IsConfigurationValidAsync()
    {
        var config = await GetConfigurationAsync();
        // Hugging Face token is required, plus at least one AI provider (GitHub Models or Azure OpenAI)
        var hasHuggingFace = !string.IsNullOrEmpty(config.HuggingFaceToken);
        var hasGitHubModels = !string.IsNullOrEmpty(config.GitHubToken);
        var hasAzureOpenAI = !string.IsNullOrEmpty(config.AzureOpenAIEndpoint) && !string.IsNullOrEmpty(config.AzureOpenAIApiKey);

        return hasHuggingFace && (hasGitHubModels || hasAzureOpenAI);
    }
}
