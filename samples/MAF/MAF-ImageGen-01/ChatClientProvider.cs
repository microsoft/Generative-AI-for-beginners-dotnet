using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

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
        var deploymentName = config["AzureOpenAI:Deployment"] ?? "gpt-5-mini";

        var endpoint = config["AzureOpenAI:Endpoint"];

        var client = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
            .GetChatClient(deploymentName)
            .AsIChatClient()
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();

        return client;
    }
}
