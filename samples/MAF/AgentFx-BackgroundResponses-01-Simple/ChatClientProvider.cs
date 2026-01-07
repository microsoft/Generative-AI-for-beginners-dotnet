using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.ClientModel;

namespace AgentFx_BackgroundResponses_01_Simple;

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
        var apiKey = config["apikey"];

        var azureClient = new AzureOpenAIClient(
            new Uri(endpoint),
            new AzureCliCredential());
        if (!string.IsNullOrEmpty(apiKey))
        {
            azureClient = new AzureOpenAIClient(
                new Uri(endpoint), 
                new ApiKeyCredential(apiKey));
        }
        
        return azureClient
            .GetChatClient(deploymentName)
            .AsIChatClient()
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();
    }
}
