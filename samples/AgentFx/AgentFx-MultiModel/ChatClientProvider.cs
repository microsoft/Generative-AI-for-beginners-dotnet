using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OllamaSharp;
using System.ClientModel;

namespace AgentFx_MultiModel;

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
        var deploymentName = config["deploymentName"] ?? "gpt-5-mini";

        var endpoint = config["endpoint"];

        if (!string.IsNullOrEmpty(config["apikey"]))
        {
            var apiKey = new ApiKeyCredential(config["apikey"]);
            return new AzureOpenAIClient(new Uri(endpoint), apiKey)
                .GetChatClient(deploymentName)
                .AsIChatClient()
                .AsBuilder()
                .UseFunctionInvocation()
                .Build();
        }
        else
        {
            // use default azure credentials if no apiKey is provided
            return new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential())
                .GetChatClient(deploymentName)
                .AsIChatClient()
                .AsBuilder()
                .UseFunctionInvocation()
                .Build();
        }
    }
}
