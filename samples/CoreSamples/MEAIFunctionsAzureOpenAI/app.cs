#:package Azure.AI.OpenAI@2.9.0-beta.1
#:package Microsoft.Extensions.AI@10.8.3
#:package Azure.Identity@1.21.0
#:package Microsoft.Extensions.AI.OpenAI@10.8.3
#:package Microsoft.Extensions.Configuration.UserSecrets@10.0.10
#:property UserSecretsId=genai-beginners-dotnet

﻿using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var endpoint = config["AzureOpenAI:Endpoint"]
    ?? throw new InvalidOperationException("Set AzureOpenAI:Endpoint in User Secrets. See: https://github.com/microsoft/Generative-AI-for-beginners-dotnet/blob/main/01-IntroductionToGenerativeAI/setup-azure-openai.md");
var deploymentName = config["AzureOpenAI:Deployment"] ?? "gpt-5-mini";

IChatClient client = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient()
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

[Description("Get the weather")]
static string GetWeather()
{
    // Uncomment the next line to prove the model really called this function.
    // It prints the moment the tool runs, before any response is written below.
    // Console.WriteLine(">>> [tool] GetWeather() was called by the model");

    var temperature = Random.Shared.Next(5, 20);
    var condition = Random.Shared.Next(0, 2) == 0 ? "sunny" : "rainy";
    return $"The weather is {temperature} degree C and {condition}";
}

var chatOptions = new ChatOptions
{
    Tools = [AIFunctionFactory.Create(GetWeather)],
    ModelId = deploymentName
};

var funcCallingResponseOne = await client.GetResponseAsync("What is today's date?", chatOptions);
var funcCallingResponseTwo = await client.GetResponseAsync("Why don't you tell me about today's temperature?", chatOptions);
var funcCallingResponseThree = await client.GetResponseAsync("Should I bring an umbrella with me today?", chatOptions);

Console.WriteLine($"Response 1: {funcCallingResponseOne}");
Console.WriteLine($"Response 2: {funcCallingResponseTwo}");
Console.WriteLine($"Response 3: {funcCallingResponseThree}");
