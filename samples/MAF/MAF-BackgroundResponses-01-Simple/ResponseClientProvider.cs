#pragma warning disable OPENAI001

using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.AI;
using OpenAI.Responses;

namespace MAF_BackgroundResponses_01_Simple;

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
        var deploymentName = config["AzureOpenAI:Deployment"] ?? "gpt-5-mini";
        var endpoint = config["AzureOpenAI:Endpoint"];

        var azureClient = new AzureOpenAIClient(
            new Uri(endpoint),
            new AzureCliCredential());

        // Background responses and continuation tokens are a Responses API feature, so this
        // must be the response client — the chat-completions client never emits a
        // ContinuationToken, which makes the continuation phase of this sample impossible.
        return azureClient.GetResponsesClient().AsIChatClient(deploymentName);
    }
}
