using Azure;
using Azure.AI.Inference;
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

        IChatClient client = null;
        var githubToken = config["GITHUB_TOKEN"];

        // create a client if githubToken is valid string
        if (!string.IsNullOrEmpty(githubToken))
        {
            client = new ChatCompletionsClient(
                endpoint: new Uri("https://models.github.ai/inference"),
                new AzureKeyCredential(githubToken))
                .AsIChatClient(deploymentName)
                .AsBuilder()
                .UseFunctionInvocation()
                .Build();
        }
        else
        {
            // create an Azure OpenAI client if githubToken is not valid
            var endpoint = config["endpoint"];

            if (!string.IsNullOrEmpty(config["apikey"]))
            {
                var apiKey = new ApiKeyCredential(config["apikey"]);
                client = new AzureOpenAIClient(new Uri(endpoint), apiKey)
                    .GetChatClient(deploymentName)
                    .AsIChatClient()
                    .AsBuilder()
                    .UseFunctionInvocation()
                    .Build();
            }
            else
            {
                // use default azure credentials if no apiKey is provided
                client = new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential())
                    .GetChatClient(deploymentName)
                    .AsIChatClient()
                    .AsBuilder()
                    .UseFunctionInvocation()
                    .Build();
            }
        }
        return client;
    }
}
