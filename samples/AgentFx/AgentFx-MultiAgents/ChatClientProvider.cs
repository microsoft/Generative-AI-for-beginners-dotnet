using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System.ClientModel;

namespace AgentFx_MultiAgents;

/// <summary>
/// Provides factory methods for creating chat clients for different AI services.
/// Supports Azure OpenAI and Ollama.
/// </summary>
class ChatClientProvider
{
    private const string DefaultOllamaEndpoint = "http://localhost:11434/";
    private const string DefaultOllamaModel = "llama3.2";

    private static readonly AppConfigurationService _config = AppConfigurationService.Instance;

    /// <summary>
    /// Creates an Ollama chat client for local AI inference.
    /// </summary>
    /// <param name="model">The Ollama model to use (default: llama3.2).</param>
    /// <param name="endpoint">The Ollama endpoint URL (default: http://localhost:11434/).</param>
    /// <returns>An IChatClient configured for Ollama.</returns>
    public static IChatClient GetChatClientOllama(
        string model = DefaultOllamaModel, 
        string endpoint = DefaultOllamaEndpoint)
    {
        return new OllamaApiClient(new Uri(endpoint), model);
    }

    /// <summary>
    /// Creates a chat client based on available configuration.
    /// Priority: Azure OpenAI with API Key > Azure OpenAI with Default Credentials.
    /// </summary>
    /// <returns>An IChatClient configured with function invocation support.</returns>
    public static IChatClient GetChatClient()
    {
        return CreateAzureOpenAIClient();
    }

    /// <summary>
    /// Creates a chat client for Azure OpenAI with either API key or default credentials.
    /// </summary>
    private static IChatClient CreateAzureOpenAIClient()
    {
        var endpointUri = new Uri(_config.AzureEndpoint!);

        if (_config.HasValidApiKey)
        {
            return CreateAzureOpenAIClientWithApiKey(endpointUri);
        }

        return CreateAzureOpenAIClientWithDefaultCredentials(endpointUri);
    }

    /// <summary>
    /// Creates an Azure OpenAI client using API key authentication.
    /// </summary>
    private static IChatClient CreateAzureOpenAIClientWithApiKey(Uri endpoint)
    {
        var apiKey = new ApiKeyCredential(_config.ApiKey!);
        return new AzureOpenAIClient(endpoint, apiKey)
            .GetChatClient(_config.DeploymentName)
            .AsIChatClient()
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();
    }

    /// <summary>
    /// Creates an Azure OpenAI client using DefaultAzureCredential for authentication.
    /// </summary>
    private static IChatClient CreateAzureOpenAIClientWithDefaultCredentials(Uri endpoint)
    {
        return new AzureOpenAIClient(endpoint, new DefaultAzureCredential())
            .GetChatClient(_config.DeploymentName)
            .AsIChatClient()
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();
    }
}
