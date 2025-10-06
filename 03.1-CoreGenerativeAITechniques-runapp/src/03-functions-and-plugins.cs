// Functions and Plugins Demo
// This demo shows how to use AI functions/tools with Microsoft.Extensions.AI
// 
// Prerequisites: Set GITHUB_TOKEN environment variable or user secret
// Run with: dotnet run 03-functions-and-plugins.cs (requires .NET 10)

using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;

// Get GitHub token from environment or user secrets
var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
if (string.IsNullOrEmpty(githubToken))
{
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    githubToken = config["GITHUB_TOKEN"];
}

if (string.IsNullOrEmpty(githubToken))
{
    Console.WriteLine("Error: GITHUB_TOKEN not found. Please set it as an environment variable or user secret.");
    return;
}

// Create chat options with function tools
ChatOptions options = new ChatOptions
{
    Tools = [
        AIFunctionFactory.Create(GetWeather),
        AIFunctionFactory.Create(GetDateTime),
        AIFunctionFactory.Create(CalculateSum)
    ]
};

// Create chat client with function invocation capability
IChatClient client = new ChatCompletionsClient(
    endpoint: new Uri("https://models.inference.ai.azure.com"),
    new AzureKeyCredential(githubToken))
    .AsIChatClient("gpt-4o-mini")
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

Console.WriteLine("🔧 AI Functions & Plugins Demo");
Console.WriteLine("===============================");
Console.WriteLine("Ask questions that might require tool usage!");
Console.WriteLine("Try: 'What's the weather like?' or 'What time is it?' or 'What's 15 + 27?'");
Console.WriteLine("Type 'quit' to exit.\n");

try
{
    while (true)
    {
        Console.Write("👤 You: ");
        var question = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(question) || question.ToLower() == "quit")
        {
            Console.WriteLine("👋 Goodbye!");
            break;
        }

        Console.WriteLine($"\n🤖 Processing: {question}");
        Console.WriteLine("─" + new string('─', 49));
        
        var response = await client.CompleteAsync(question, options);
        Console.WriteLine($"💬 Response: {response.Message.Text}\n");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine("Make sure your GITHUB_TOKEN is valid and has access to GitHub Models.");
}

// Function definitions
[Description("Get the current weather information")]
static string GetWeather([Description("The location to get weather for")] string location = "your area")
{
    var temperature = Random.Shared.Next(-5, 35);
    var conditions = Random.Shared.Next(0, 3) switch
    {
        0 => "sunny",
        1 => "rainy", 
        _ => "cloudy"
    };
    var weatherInfo = $"The weather in {location} is {temperature}°C and {conditions}.";
    Console.WriteLine($"   🌤️  Function Call - Weather: {weatherInfo}");
    return weatherInfo;
}

[Description("Get the current date and time")]
static string GetDateTime()
{
    var now = DateTime.Now;
    var dateTimeInfo = $"Current date and time: {now:yyyy-MM-dd HH:mm:ss}";
    Console.WriteLine($"   🕒 Function Call - DateTime: {dateTimeInfo}");
    return dateTimeInfo;
}

[Description("Calculate the sum of two numbers")]
static int CalculateSum([Description("First number")] int a, [Description("Second number")] int b)
{
    var result = a + b;
    Console.WriteLine($"   🧮 Function Call - Math: {a} + {b} = {result}");
    return result;
}