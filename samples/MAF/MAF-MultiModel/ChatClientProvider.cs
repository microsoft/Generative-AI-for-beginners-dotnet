using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OllamaSharp;

namespace MAF_MultiModel;

class ChatClientProvider
{
    public static IChatClient GetChatClientOllama(string model = "llama3.2")
    {
        return new OllamaApiClient(new Uri("http://localhost:11434/"), model);
    }

    public static IChatClient GetChatClient()
    {
        var builder = Host.CreateApplicationBuilder();
        var config = builder.Configuration
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>()
            .Build();
        var deploymentName = config["AzureOpenAI:Deployment"] ?? "gpt-5-mini";

        var endpoint = config["AzureOpenAI:Endpoint"];

        return new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential())
            .GetChatClient(deploymentName)
            .AsIChatClient()
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();
    }
}
