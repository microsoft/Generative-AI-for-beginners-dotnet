using Microsoft.Extensions.AI;
using System.ComponentModel;

var ollamaEndpoint = "http://localhost:11434";
var chatModel = "llama3.2";

IChatClient client = new OllamaChatClient(
    endpoint: ollamaEndpoint,
    modelId: chatModel)
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

ChatOptions options = new ChatOptions
{
    Tools = [
        AIFunctionFactory.Create(GetTheWeather)
    ]    
};

var question = "Solve 2+2. Provide an accurate and short answer";
Console.WriteLine($"question: {question}");
var response = await client.GetResponseAsync(question, options);
Console.WriteLine($"response: {response}");

Console.WriteLine();

question = "Do I need an umbrella today?. Provide an accurate and short answer";
Console.WriteLine($"question: {question}");
response = await client.GetResponseAsync(question, options);
Console.WriteLine($"response: {response}");



[Description("Get the weather")]
static string GetTheWeather()
{
    Console.WriteLine("\tGetTheWeather function invoked.");

    var temperature = Random.Shared.Next(5, 20);
    var conditions = Random.Shared.Next(0, 1) == 0 ? "sunny" : "rainy";
    var weather = $"The weather is {temperature} degrees C and {conditions}.";
    Console.WriteLine($"\tGetTheWeather result: {weather}.");
    return weather;
}