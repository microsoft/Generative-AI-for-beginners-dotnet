using Microsoft.Extensions.AI;
using OllamaSharp;
using System.ComponentModel;

using var httpClient = new HttpClient
{
    BaseAddress = new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434"),
    Timeout = TimeSpan.FromMinutes(2)
};
using IChatClient client = ((IChatClient)new OllamaApiClient(
        httpClient, Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "llama3.2"))
    .AsBuilder()
    .UseFunctionInvocation(configure: invocation => invocation.MaximumIterationsPerRequest = 3)
    .Build();
using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(2));

// No tools are offered for arithmetic. Tool availability is an application boundary.
var question = "Solve 2+2. Provide an accurate and short answer.";
Console.WriteLine($"question: {question}");
var response = await client.GetResponseAsync(question, cancellationToken: timeout.Token);
if (string.IsNullOrWhiteSpace(response.Text))
{
    throw new InvalidOperationException("The model returned no arithmetic answer.");
}
Console.WriteLine($"response: {response.Text}");

int toolCalls = 0;
var options = new ChatOptions
{
    Tools = [AIFunctionFactory.Create(GetTheWeather, nameof(GetTheWeather))],
    Temperature = 0,
    MaxOutputTokens = 128
};
question = "Call GetTheWeather once to check the demo weather, then tell me whether I need an umbrella. Do not call it again after receiving the result.";
Console.WriteLine($"question: {question}");
response = await client.GetResponseAsync(question, options, timeout.Token);
if (string.IsNullOrWhiteSpace(response.Text))
{
    throw new InvalidOperationException("The model returned no weather answer.");
}
Console.WriteLine($"response: {response.Text}");
Console.WriteLine($"Weather tool calls: {toolCalls}");
if (toolCalls == 0)
{
    throw new InvalidOperationException("The model did not call the weather tool. Use a tool-capable model such as llama3.2.");
}

[Description("Get the current demo weather. This is fixed sample data, not a live forecast.")]
string GetTheWeather()
{
    toolCalls++;
    const string weather = "The demo weather is 12 degrees C and rainy. An umbrella is recommended.";
    Console.WriteLine($"GetTheWeather function invoked: {weather}");
    return weather;
}
