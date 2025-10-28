#pragma warning disable OPENAI001

using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenAI.Responses;
using System.ClientModel;

namespace AgentFx_BackgroundResponses_01_Simple;

class ResponseClientProvider
{
    public static OpenAIResponseClient GetResponseClient()
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

        return azureClient.GetOpenAIResponseClient(deploymentName);
    }
}
