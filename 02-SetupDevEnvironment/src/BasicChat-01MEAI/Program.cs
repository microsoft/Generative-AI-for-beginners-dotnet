using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ClientModel;

var deploymentName = "gpt-4o-mini"; // e.g. "gpt-4o-mini"
var endpoint = new Uri("https://Bt-Az-OpenAi.openai.azure.com/"); // e.g. "https://< your hub name >.openai.azure.com/"

// Build configuration to access user secrets
var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var apiKeyValue = Environment.GetEnvironmentVariable("AZURE_AI_KEY");
if (string.IsNullOrEmpty(apiKeyValue))
{
    apiKeyValue = config["AZURE_AI_KEY"];
}

var apiKey = new ApiKeyCredential(apiKeyValue);

IChatClient client = new AzureOpenAIClient(
    endpoint,
    apiKey).GetChatClient(deploymentName).AsIChatClient();



var response = await client.GetResponseAsync("What is AI?");

Console.WriteLine(response.Text);