using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace MAF_MultiAgents;

/// <summary>
/// Centralized configuration service for managing application settings.
/// Handles reading from environment variables and user secrets.
/// </summary>
class AppConfigurationService
{
    private static readonly Lazy<AppConfigurationService> _instance = new(() => new AppConfigurationService());
    private readonly IConfiguration _configuration;

    private AppConfigurationService()
    {
        var builder = Host.CreateApplicationBuilder();
        _configuration = builder.Configuration
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>()
            .Build();
    }

    /// <summary>
    /// Gets the singleton instance of the configuration service.
    /// </summary>
    public static AppConfigurationService Instance => _instance.Value;

    /// <summary>
    /// Gets the deployment name for the AI model, defaults to "gpt-5-mini".
    /// </summary>
    public string DeploymentName => _configuration["deploymentName"] ?? "gpt-5-mini";

    /// <summary>
    /// <summary>
    /// Gets the Azure OpenAI endpoint URL.
    /// </summary>
    public string? AzureEndpoint => _configuration["endpoint"];

    /// <summary>
    /// Gets the API key for Azure services.
    /// </summary>
    public string? ApiKey => _configuration["apikey"];

    /// <summary>
    /// Gets the Azure Foundry project endpoint URL.
    /// </summary>
    public string? AzureFoundryProjectEndpoint => _configuration["AZURE_FOUNDRY_PROJECT_ENDPOINT"];

    /// <summary>
    /// Checks if Azure API key is configured and valid.
    /// </summary>
    public bool HasValidApiKey => !string.IsNullOrEmpty(ApiKey);
}
