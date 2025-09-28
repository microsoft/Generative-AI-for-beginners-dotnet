using System.ComponentModel.DataAnnotations;

namespace HFMCP.GenImage.Web.Services;

public interface IAIConfigurationService
{
    Task<AIConfiguration> GetConfigurationAsync();
    Task SaveConfigurationAsync(AIConfiguration configuration);
    Task<bool> IsConfigurationValidAsync();
}

public class AIConfiguration
{
    [Required(ErrorMessage = "Hugging Face Access Token is required")]
    public string HuggingFaceToken { get; set; } = string.Empty;

    public string HuggingFaceMCPServer { get; set; } = "https://huggingface.co/mcp";

    public string ModelName { get; set; } = "gpt-4.1-mini";

    // Optional GitHub Models settings - not required since user can choose GitHub Models OR Azure OpenAI
    public string? GitHubToken { get; set; } = string.Empty;

    // Optional Azure AI Foundry settings
    public string? AzureOpenAIEndpoint { get; set; }
    public string? AzureOpenAIApiKey { get; set; }
}
