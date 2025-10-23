using Microsoft.Extensions.Configuration;

public class GameConfigManager
{
    public string AzureOpenAIEndpoint { get; }
    public string AzureOpenAIModel { get; }
    public string AzureOpenAIApiKey { get; }

    public GameConfigManager()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets(typeof(Program).Assembly)
            .Build();
        AzureOpenAIEndpoint = config["AZURE_OPENAI_ENDPOINT"] ?? string.Empty;
        AzureOpenAIModel = config["AZURE_OPENAI_MODEL"] ?? string.Empty;
        AzureOpenAIApiKey = config["AZURE_OPENAI_APIKEY"] ?? string.Empty;
    }
}
