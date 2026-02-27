using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.ClientModel;

namespace MAF_ImageGen_01;

class ChatClientProvider
{
    public static IChatClient GetChatClient()
    {
        var builder = Host.CreateApplicationBuilder();
        var config = builder.Configuration
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>()
            .Build();
        var deploymentName = config["deploymentName"] ?? "gpt-5-mini";

        var endpoint = config["endpoint"];
        var apiKey = new ApiKeyCredential(config["apikey"]);

        var client = new AzureOpenAIClient(new Uri(endpoint), apiKey)
            .GetChatClient(deploymentName)
            .AsIChatClient()
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();

        return client;
    }
}
