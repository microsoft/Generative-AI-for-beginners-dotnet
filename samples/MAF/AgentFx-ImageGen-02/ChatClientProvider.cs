using Azure;
using Azure.AI.Inference;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.ClientModel;

namespace AgentFx_ImageGen_01;

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
            var apiKey = new ApiKeyCredential(config["apikey"]);

            client = new AzureOpenAIClient(new Uri(endpoint), apiKey)
                .GetChatClient(deploymentName)
                .AsIChatClient()
                .AsBuilder()
                .UseFunctionInvocation()
                .Build();
        }
        return client;
    }
}
