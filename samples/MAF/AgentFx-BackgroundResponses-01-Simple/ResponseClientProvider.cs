#pragma warning disable OPENAI001

using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.AI;
using OpenAI.Responses;
using System.ClientModel;

namespace AgentFx_BackgroundResponses_01_Simple;

class ResponseClientProvider
{
    // Return an IChatClient constructed from the AzureOpenAIClient so samples can create agents
    public static IChatClient GetResponseClient()
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

        // Create a Chat client for the target deployment and return the IChatClient wrapper
        return azureClient.GetChatClient(deploymentName).AsIChatClient();
    }
}
