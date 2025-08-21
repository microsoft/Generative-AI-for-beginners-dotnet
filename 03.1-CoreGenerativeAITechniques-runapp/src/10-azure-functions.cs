using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ClientModel;
using System.ComponentModel;

// Azure OpenAI Function Calling Demo
// This shows how to use AI function calling with Azure OpenAI

Console.WriteLine("🔧 Azure OpenAI Function Calling Demo");
Console.WriteLine();

try
{
    // Load Azure OpenAI configuration
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    
    var endpoint = config["AZURE_OPENAI_ENDPOINT"];
    var apiKey = config["AZURE_OPENAI_APIKEY"];
    var deploymentName = config["AZURE_OPENAI_MODEL"] ?? "gpt-4o-mini";

    if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
    {
        Console.WriteLine("❌ Missing Azure OpenAI configuration");
        Console.WriteLine("Please set the following user secrets:");
        Console.WriteLine("  dotnet user-secrets set 'AZURE_OPENAI_ENDPOINT' 'https://your-resource.openai.azure.com/'");
        Console.WriteLine("  dotnet user-secrets set 'AZURE_OPENAI_APIKEY' 'your-api-key'");
        Console.WriteLine("  dotnet user-secrets set 'AZURE_OPENAI_MODEL' 'gpt-4o-mini' (optional)");
        return;
    }

    // Create Azure OpenAI client with function calling
    var client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey))
        .GetChatClient(deploymentName)
        .AsIChatClient()
        .AsBuilder()
        .UseFunctionInvocation()
        .Build();

    // Define chat options with functions
    var chatOptions = new ChatOptions
    {
        Tools = [
            AIFunctionFactory.Create(GetWeather),
            AIFunctionFactory.Create(GetCurrentTime),
            AIFunctionFactory.Create(CalculateDistance)
        ]
    };

    Console.WriteLine($"🤖 Using Azure OpenAI model: {deploymentName}");
    Console.WriteLine("🔧 Available functions: GetWeather, GetCurrentTime, CalculateDistance");
    Console.WriteLine();

    // Test various function calling scenarios
    var questions = new[]
    {
        "What time is it right now?",
        "What's the weather like today?", 
        "Should I bring an umbrella?",
        "How far is it between New York and Los Angeles?",
        "What's the current time and weather? Should I go for a walk?"
    };

    foreach (var question in questions)
    {
        Console.WriteLine($"❓ Question: {question}");
        Console.Write("🤖 Response: ");
        
        try
        {
            var response = await client.GetResponseAsync(question, chatOptions);
            Console.WriteLine(response.Text);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
        
        Console.WriteLine();
    }

    Console.WriteLine("✅ Function calling demo completed!");
    Console.WriteLine("💡 Notice how the AI automatically called the appropriate functions to answer your questions");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine();
    Console.WriteLine("🔧 Troubleshooting:");
    Console.WriteLine("• Verify your Azure OpenAI endpoint and API key");
    Console.WriteLine("• Ensure your model deployment supports function calling");
    Console.WriteLine("• Check that your Azure resource is properly configured");
}

// Function implementations

[Description("Get the current weather conditions")]
static string GetWeather()
{
    // Simulate weather data
    var conditions = new[] { "sunny", "cloudy", "rainy", "partly cloudy", "windy" };
    var temperature = Random.Shared.Next(-5, 35);
    var condition = conditions[Random.Shared.Next(conditions.Length)];
    var humidity = Random.Shared.Next(30, 90);
    
    var weather = $"Temperature: {temperature}°C, Conditions: {condition}, Humidity: {humidity}%";
    Console.WriteLine($"  🌤️ [Function Call] Getting weather: {weather}");
    return weather;
}

[Description("Get the current date and time")]
static string GetCurrentTime()
{
    var currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    Console.WriteLine($"  🕐 [Function Call] Getting current time: {currentTime}");
    return $"Current date and time: {currentTime}";
}

[Description("Calculate the distance between two cities")]
static string CalculateDistance(
    [Description("The first city")] string city1 = "New York",
    [Description("The second city")] string city2 = "Los Angeles")
{
    // Simulate distance calculation (in reality, you'd use a mapping service)
    var distances = new Dictionary<(string, string), int>
    {
        { ("New York", "Los Angeles"), 2445 },
        { ("New York", "Chicago"), 790 },
        { ("Los Angeles", "San Francisco"), 380 },
        { ("Chicago", "Detroit"), 280 },
        { ("Miami", "New York"), 1090 }
    };

    var key1 = (city1, city2);
    var key2 = (city2, city1);
    
    if (distances.TryGetValue(key1, out var distance) || distances.TryGetValue(key2, out distance))
    {
        var result = $"Distance between {city1} and {city2}: {distance} miles";
        Console.WriteLine($"  🗺️ [Function Call] Calculating distance: {result}");
        return result;
    }
    
    // Default calculation for unknown cities
    var estimatedDistance = Random.Shared.Next(100, 3000);
    var result2 = $"Estimated distance between {city1} and {city2}: {estimatedDistance} miles";
    Console.WriteLine($"  🗺️ [Function Call] Estimating distance: {result2}");
    return result2;
}