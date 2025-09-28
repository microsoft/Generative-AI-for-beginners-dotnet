using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ClientModel;
using System.ComponentModel;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var endpoint = config["endpoint"];
var apiKey = new ApiKeyCredential(config["apikey"]);
var deploymentName = "gpt-4.1-mini";

IChatClient client = new AzureOpenAIClient(new Uri(endpoint), apiKey)
    .GetChatClient(deploymentName)
    .AsIChatClient()
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

[Description("Get the weather")]
static string GetWeather()
{
    var temperature = Random.Shared.Next(5, 20);
    var condition = Random.Shared.Next(0, 1) == 0 ? "sunny" : "rainy";
    return $"The weather is {temperature} degree C and {condition}";
}

var chatOptions = new ChatOptions
{
    Tools = [AIFunctionFactory.Create(GetWeather)],
    ModelId = deploymentName
};

client.AsBuilder()
.UseFunctionInvocation()
.Build();

var funcCallingResponseOne = await client.GetResponseAsync("What is today's date?", chatOptions);
var funcCallingResponseTwo = await client.GetResponseAsync("Why don't you tell me about today's temperature?", chatOptions);
var funcCallingResponseThree = await client.GetResponseAsync("Should I bring an umbrella with me today?", chatOptions);

Console.WriteLine($"Response 1: {funcCallingResponseOne}");
Console.WriteLine($"Response 2: {funcCallingResponseTwo}");
Console.WriteLine($"Response 3: {funcCallingResponseThree}");