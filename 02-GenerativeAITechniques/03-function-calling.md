# Function Calling

In this lesson, you'll learn how to extend AI capabilities by letting the model call your .NET functions. This is one of the most powerful patterns in AI development, enabling your AI to access real-time data, perform calculations, and interact with external systems.

---

[![Function Calling](./images/LIM_GAN_04_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson2-functions)

_Click the image to watch the video_

---

## Why Function Calling?

AI models are trained on data up to a certain date and can only work with information in their context. They can't:

- Check current weather or stock prices
- Query your database
- Send emails or create calendar events
- Perform precise calculations

**Function calling** solves this by letting the model request that your code run a specific function, then use the result in its response.

---

## Part 1: How Function Calling Works

The function calling flow has these steps:

1. **You define functions** the AI can call
2. **You tell the AI** about these functions (their names, descriptions, parameters)
3. **When the AI needs information**, it requests a function call instead of making up an answer
4. **Your code runs** the function and returns the result
5. **The AI uses** the result to formulate its final response

### The Magic: AI Chooses When to Call

The AI automatically decides whether to call a function based on:

- The user's question
- The function descriptions you provide
- What information it needs to answer well

---

## Part 2: Your First Function Call

Let's give the AI the ability to check the weather.

### Step 1: Define the Function

```csharp
[Description("Get the current weather for a location")]
static string GetWeather(string location)
{
    // In a real app, you'd call a weather API here
    var temperature = Random.Shared.Next(15, 30);
    var conditions = Random.Shared.Next(0, 2) == 0 ? "sunny" : "cloudy";
    return $"{location}: {temperature}°C and {conditions}";
}
```

**Key points:**

- The `[Description]` attribute tells the AI what this function does
- Parameters are automatically inferred from the method signature
- The AI will learn that this function takes a `location` string

### Step 2: Configure the Client

```csharp
var chatOptions = new ChatOptions
{
    Tools = [AIFunctionFactory.Create(GetWeather)]
};

IChatClient client = new ChatCompletionsClient(
        endpoint: new Uri("https://models.github.ai/inference"),
        new AzureKeyCredential(githubToken))
    .AsIChatClient("gpt-4o-mini")
    .AsBuilder()
    .UseFunctionInvocation()  // Enable automatic function calling
    .Build();
```

**Key points:**

- `ChatOptions.Tools` defines available functions
- `AIFunctionFactory.Create` wraps your method as an AI-callable function
- `UseFunctionInvocation()` enables automatic function execution

> **Learn more:** [Tool calling with IChatClient](https://learn.microsoft.com/dotnet/ai/ichatclient#tool-calling) explains how function invocation works under the hood.

### Step 3: Use It Naturally

```csharp
var response = await client.GetResponseAsync(
    "Should I bring an umbrella to Seattle today?", 
    chatOptions);

Console.WriteLine(response.Text);
```

**What happens:**

1. AI recognizes it needs weather data to answer
2. AI requests to call `GetWeather("Seattle")`
3. Your code runs the function, returns "Seattle: 22°C and sunny"
4. AI receives the result and formulates its answer

**Output:**

```
Based on the current weather in Seattle (22°C and sunny), 
you probably won't need an umbrella today!
```

> **Try it yourself:** [MEAIFunctions sample](../samples/CoreSamples/MEAIFunctions/)

---

## Part 3: Functions with Parameters

Functions can have multiple parameters with different types:

```csharp
[Description("Convert a temperature between Celsius and Fahrenheit")]
static string ConvertTemperature(
    [Description("The temperature value to convert")] double value,
    [Description("The unit to convert from: 'C' for Celsius, 'F' for Fahrenheit")] string fromUnit)
{
    if (fromUnit.ToUpper() == "C")
    {
        double fahrenheit = (value * 9 / 5) + 32;
        return $"{value}°C = {fahrenheit:F1}°F";
    }
    else
    {
        double celsius = (value - 32) * 5 / 9;
        return $"{value}°F = {celsius:F1}°C";
    }
}
```

**Notice:**

- Parameters can have their own `[Description]` attributes
- The AI learns the parameter types and purposes
- Complex logic works just like any other C# method

### Using Parameter Descriptions

Good descriptions help the AI understand when and how to call your function:

```csharp
[Description("Search for products in the catalog")]
static string SearchProducts(
    [Description("The search query (product name, category, or keywords)")] string query,
    [Description("Maximum number of results to return (1-50)")] int maxResults = 10,
    [Description("Filter by price range: 'low', 'medium', 'high', or 'all'")] string priceRange = "all")
{
    // Implementation
}
```

---

## Part 4: Multiple Functions

You can provide multiple functions, and the AI will choose which ones to call:

```csharp
[Description("Get the current weather for a location")]
static string GetWeather(string location) { /* ... */ }

[Description("Get the current stock price for a ticker symbol")]
static string GetStockPrice(string symbol) { /* ... */ }

[Description("Search for nearby restaurants")]
static string SearchRestaurants(string location, string cuisine) { /* ... */ }

var chatOptions = new ChatOptions
{
    Tools = [
        AIFunctionFactory.Create(GetWeather),
        AIFunctionFactory.Create(GetStockPrice),
        AIFunctionFactory.Create(SearchRestaurants)
    ]
};
```

Now you can ask natural questions:

```csharp
// Calls GetWeather
await client.GetResponseAsync("What's the weather in Tokyo?", chatOptions);

// Calls GetStockPrice
await client.GetResponseAsync("How is Microsoft stock doing?", chatOptions);

// Calls SearchRestaurants
await client.GetResponseAsync("Find Italian restaurants near downtown Seattle", chatOptions);

// Might call multiple functions
await client.GetResponseAsync(
    "I'm visiting Paris tomorrow. What's the weather like, and can you suggest some good cafes?", 
    chatOptions);
```

---

## Part 5: Functions in Conversations

Functions work seamlessly with conversation history:

```csharp
List<ChatMessage> history = [
    new ChatMessage(ChatRole.System, 
        "You are a helpful travel assistant with access to weather and restaurant data.")
];

var chatOptions = new ChatOptions
{
    Tools = [
        AIFunctionFactory.Create(GetWeather),
        AIFunctionFactory.Create(SearchRestaurants)
    ]
};

// First turn
history.Add(new ChatMessage(ChatRole.User, "I'm planning a trip to Rome"));
var response1 = await client.GetResponseAsync(history, chatOptions);
history.AddMessages(response1);

// Follow-up that triggers function call
history.Add(new ChatMessage(ChatRole.User, "What's the weather like there?"));
var response2 = await client.GetResponseAsync(history, chatOptions);
// AI calls GetWeather("Rome") and incorporates the result
```

---

## Part 6: Async Functions

Functions can be asynchronous for calling external APIs:

```csharp
[Description("Get the current weather from a weather service")]
static async Task<string> GetWeatherAsync(string location)
{
    using var http = new HttpClient();
    var response = await http.GetStringAsync(
        $"https://api.weather.example/current?location={location}");
    return response;
}
```

The AI automatically awaits async functions.

---

## Part 7: Function Calling with Streaming

Function calling works with streaming too:

```csharp
await foreach (var update in client.GetStreamingResponseAsync(
    "What's the weather in all major European capitals?", 
    chatOptions))
{
    Console.Write(update.Text);
}
```

The stream pauses while functions execute, then continues with the results incorporated.

---

## Part 8: Best Practices

### Write Good Descriptions

The description is how the AI understands when to use your function:

```csharp
// Good: Specific and clear
[Description("Get the current balance for a bank account by account number")]

// Bad: Vague
[Description("Get balance")]
```

### Handle Errors Gracefully

Return helpful messages when things go wrong:

```csharp
[Description("Get stock price for a ticker symbol")]
static string GetStockPrice(string symbol)
{
    try
    {
        // ... call stock API
        return $"{symbol}: $142.50";
    }
    catch (Exception)
    {
        return $"Unable to retrieve stock price for {symbol}. The ticker may be invalid.";
    }
}
```

### Keep Functions Focused

Each function should do one thing well:

```csharp
// Good: Single responsibility
[Description("Get current weather")]
static string GetWeather(string location) { }

[Description("Get weather forecast for the next 5 days")]
static string GetWeatherForecast(string location) { }

// Avoid: Too many responsibilities
[Description("Get weather, forecast, and historical data")]
static string GetAllWeatherData(string location, string dataType) { }
```

---

## Part 9: Real-World Example

Here's a more complete example with multiple practical functions:

```csharp
using Microsoft.Extensions.AI;

// Define useful functions
[Description("Get the current date and time in a specific timezone")]
static string GetCurrentTime(string timezone = "UTC")
{
    var tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);
    var time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
    return $"Current time in {timezone}: {time:f}";
}

[Description("Calculate the tip amount for a bill")]
static string CalculateTip(double billAmount, double tipPercentage = 18)
{
    var tip = billAmount * (tipPercentage / 100);
    var total = billAmount + tip;
    return $"Bill: ${billAmount:F2}, Tip ({tipPercentage}%): ${tip:F2}, Total: ${total:F2}";
}

[Description("Convert between currencies using current exchange rates")]
static string ConvertCurrency(double amount, string from, string to)
{
    // Simplified - in reality, you'd call an exchange rate API
    var rates = new Dictionary<string, double>
    {
        ["USD"] = 1.0,
        ["EUR"] = 0.85,
        ["GBP"] = 0.73,
        ["JPY"] = 110.0
    };
    
    var usdAmount = amount / rates[from];
    var result = usdAmount * rates[to];
    return $"{amount} {from} = {result:F2} {to}";
}

// Configure and use
var chatOptions = new ChatOptions
{
    Tools = [
        AIFunctionFactory.Create(GetCurrentTime),
        AIFunctionFactory.Create(CalculateTip),
        AIFunctionFactory.Create(ConvertCurrency)
    ]
};

IChatClient client = new ChatCompletionsClient(...)
    .AsIChatClient("gpt-4o-mini")
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

// Natural conversations that trigger functions
Console.WriteLine(await client.GetResponseAsync(
    "My dinner bill is $85. What's a good tip?", chatOptions));

Console.WriteLine(await client.GetResponseAsync(
    "How much is 500 euros in Japanese yen?", chatOptions));

Console.WriteLine(await client.GetResponseAsync(
    "What time is it in Tokyo right now?", chatOptions));
```

---

## Let's Review: What You Learned

| Concept | Key Takeaway |
|---------|-------------|
| **Function Calling** | Let AI call your code when it needs external data |
| **AIFunctionFactory** | Wraps .NET methods as AI-callable functions |
| **UseFunctionInvocation** | Enables automatic function execution |
| **Descriptions** | Tell the AI when and how to use functions |
| **Multiple Functions** | AI chooses which function(s) to call based on the question |

### Quick Self-Check

1. When does the AI decide to call a function?
2. Why are good `[Description]` attributes important?
3. How do you handle errors in functions gracefully?

---

## Sample Code Reference

| Sample | Description |
|--------|-------------|
| [MEAIFunctions](../samples/CoreSamples/MEAIFunctions/) | Basic function calling |
| [MEAIFunctionsAzureOpenAI](../samples/CoreSamples/MEAIFunctionsAzureOpenAI/) | Function calling with Azure OpenAI |
| [MEAIFunctionsOllama](../samples/CoreSamples/MEAIFunctionsOllama/) | Function calling with Ollama |

---

## Additional Resources

- [IChatClient Tool Calling](https://learn.microsoft.com/dotnet/ai/ichatclient#tool-calling): Complete guide to function calling with IChatClient
- [Access Data in AI Functions](https://learn.microsoft.com/dotnet/ai/how-to/access-data-in-functions): Best practices for database and API access in tools
- [Function calling concepts](https://learn.microsoft.com/dotnet/ai/conceptual/function-calling): How AI models decide when to call functions

---

## Up Next

Now that you can extend AI with your code, let's learn how to build production-ready AI applications with middleware pipelines:

[Continue to Part 4: Middleware Pipelines →](./04-middleware-pipeline.md)
