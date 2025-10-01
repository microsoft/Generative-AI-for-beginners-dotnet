using Azure.AI.Inference;
using Azure;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;

var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
if (string.IsNullOrEmpty(githubToken))
{
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    githubToken = config["GITHUB_TOKEN"];
}

ChatOptions options = new ChatOptions
{
    Tools = [
        AIFunctionFactory.Create(GetTheWeather)
    ]
};


IChatClient client = new ChatCompletionsClient(
    endpoint: new Uri("https://models.github.ai/inference"),
    new AzureKeyCredential(githubToken))
    .AsIChatClient("gpt-4o-mini")
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

var question = "Do I need an umbrella today?";
Console.WriteLine($"question: {question}");
var response = await client.GetResponseAsync(question, options);
Console.WriteLine($"response: {response}");


[Description("Get the weather")]
static string GetTheWeather()
{
    var temperature = Random.Shared.Next(5, 20);
    var conditions = Random.Shared.Next(0, 1) == 0 ? "sunny" : "rainy";
    var weatherInfo = $"The weather is {temperature} degrees C and {conditions}.";
    Console.WriteLine($"\tFunction Call - Returning weather info: {weatherInfo}");
    return weatherInfo;
}