# Agents with Tools

In this lesson, you'll give your agents the ability to take actions in the real world—calling APIs, querying databases, and executing code.

---

[![Agents with Tools](./images/LIM_GAN_08_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson4-tools)

_Click the image to watch the video_

---

## Why Tools Matter

An agent without tools is just a fancy chatbot. It can only reason about what it knows. But with tools, an agent can:

- **Retrieve current information** (weather, stock prices, database records)
- **Take actions** (send emails, create files, update systems)
- **Execute code** (run calculations, process data)
- **Interact with external systems** (APIs, services, hardware)

Tools transform agents from responders into actors.

---

## Part 1: What Are Agent Tools?

In Agent Framework, tools are functions that the agent can call during its reasoning process. The agent:

1. **Reads** the user's request
2. **Decides** if it needs to call a tool
3. **Calls** the tool with appropriate parameters
4. **Uses** the tool's response to continue reasoning
5. **Generates** the final response

```
User: "What's the weather in Seattle?"
         │
         ▼
┌─────────────────────────────────────────┐
│ Agent Reasoning                         │
│                                         │
│ "I need to get weather data.            │
│  I'll call the GetWeather tool..."      │
│                                         │
│        ┌─────────────────────┐          │
│        │ GetWeather("Seattle")          │
│        │ → "Cloudy, 15°C"    │          │
│        └─────────────────────┘          │
│                                         │
│ "Now I can answer the user."            │
└─────────────────────────────────────────┘
         │
         ▼
Response: "The weather in Seattle is cloudy with a temperature of 15°C."
```

---

## Part 2: Creating Agent Tools

Tools are created using `AIFunctionFactory.Create()`, the same pattern you learned in Lesson 2.

### Define a Tool Function

```csharp
using System.ComponentModel;
using Microsoft.Extensions.AI;

[Description("Get the current weather for a given location")]
static string GetWeather(
    [Description("The city name to get weather for")] string location)
{
    // In a real app, this would call a weather API
    return $"The weather in {location} is cloudy with a high of 15°C.";
}
```

The `[Description]` attributes are critical—they tell the AI what the tool does and what each parameter means.

### Attach Tools to an Agent

```csharp
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

// Create tools from functions
var weatherTool = AIFunctionFactory.Create(GetWeather);

// Create an agent with tools
AIAgent assistant = chatClient.CreateAIAgent(
    name: "WeatherAssistant",
    instructions: "You are a helpful assistant that can check the weather.",
    tools: [weatherTool]);

// The agent will automatically call GetWeather when needed
var response = await assistant.RunAsync("What's the weather like in Paris?");
Console.WriteLine(response.Text);
```

**Learn more:** [Agent Tools](https://learn.microsoft.com/agent-framework/user-guide/agents/agent-tools) on Microsoft Learn.

---

## Part 3: Multiple Tools

Agents can have multiple tools and will choose the right one based on the user's request:

```csharp
[Description("Get the current weather for a location")]
static string GetWeather(string location)
    => $"The weather in {location} is sunny with a high of 22°C.";

[Description("Get the current time in a timezone")]
static string GetTime(string timezone)
    => $"The current time in {timezone} is {DateTime.UtcNow:HH:mm} UTC.";

[Description("Convert currency from one type to another")]
static string ConvertCurrency(string from, string to, decimal amount)
    => $"{amount} {from} is approximately {amount * 0.85m} {to}.";

// Create agent with multiple tools
AIAgent assistant = chatClient.CreateAIAgent(
    name: "MultiToolAssistant",
    instructions: "You are a helpful assistant with access to weather, time, and currency tools.",
    tools: [
        AIFunctionFactory.Create(GetWeather),
        AIFunctionFactory.Create(GetTime),
        AIFunctionFactory.Create(ConvertCurrency)
    ]);

// The agent figures out which tool to use
await assistant.RunAsync("What's the weather in Tokyo?");      // Uses GetWeather
await assistant.RunAsync("What time is it in London?");        // Uses GetTime
await assistant.RunAsync("Convert 100 USD to EUR");            // Uses ConvertCurrency
```

---

## Part 4: Tools with Complex Parameters

Tools can accept complex objects as parameters:

```csharp
public class FlightSearchRequest
{
    [Description("Departure airport code (e.g., SEA)")]
    public string From { get; set; } = "";
    
    [Description("Arrival airport code (e.g., JFK)")]
    public string To { get; set; } = "";
    
    [Description("Departure date in YYYY-MM-DD format")]
    public string Date { get; set; } = "";
    
    [Description("Number of passengers")]
    public int Passengers { get; set; } = 1;
}

[Description("Search for available flights")]
static string SearchFlights(FlightSearchRequest request)
{
    return $"""
        Found 3 flights from {request.From} to {request.To} on {request.Date}:
        1. UA123 - Departs 8:00 AM - $350/person
        2. AA456 - Departs 12:30 PM - $420/person
        3. DL789 - Departs 6:15 PM - $380/person
        """;
}

AIAgent travelAgent = chatClient.CreateAIAgent(
    name: "TravelAgent",
    instructions: "You are a travel booking assistant.",
    tools: [AIFunctionFactory.Create(SearchFlights)]);

var response = await travelAgent.RunAsync(
    "Find me a flight from Seattle to New York for 2 people on March 15th, 2026");
```

The agent will parse the natural language request and construct the `FlightSearchRequest` object automatically.

---

## Part 5: Async Tools

Tools can be asynchronous for operations that involve I/O:

```csharp
[Description("Search for products in the database")]
static async Task<string> SearchProducts(
    [Description("Search query")] string query)
{
    // Simulate database query
    await Task.Delay(100);
    
    return $"""
        Found 3 products matching "{query}":
        1. Product A - $29.99
        2. Product B - $49.99
        3. Product C - $19.99
        """;
}

[Description("Place an order for a product")]
static async Task<string> PlaceOrder(
    [Description("Product name")] string productName,
    [Description("Quantity to order")] int quantity)
{
    // Simulate order processing
    await Task.Delay(200);
    
    var orderId = Guid.NewGuid().ToString()[..8].ToUpper();
    return $"Order {orderId} placed for {quantity}x {productName}. Estimated delivery: 3-5 days.";
}

AIAgent shopAgent = chatClient.CreateAIAgent(
    name: "ShoppingAssistant",
    instructions: "Help users find and order products.",
    tools: [
        AIFunctionFactory.Create(SearchProducts),
        AIFunctionFactory.Create(PlaceOrder)
    ]);
```

---

## Part 6: Tools That Call External APIs

Here's a more realistic example calling an external API:

```csharp
using System.Net.Http.Json;

public class WeatherService
{
    private readonly HttpClient _http = new();
    
    [Description("Get real weather data for a city")]
    public async Task<string> GetRealWeather(
        [Description("City name")] string city)
    {
        try
        {
            // Using a free weather API
            var url = $"https://wttr.in/{city}?format=j1";
            var data = await _http.GetFromJsonAsync<JsonElement>(url);
            
            var temp = data.GetProperty("current_condition")[0]
                .GetProperty("temp_C").GetString();
            var desc = data.GetProperty("current_condition")[0]
                .GetProperty("weatherDesc")[0]
                .GetProperty("value").GetString();
            
            return $"Current weather in {city}: {desc}, {temp}°C";
        }
        catch
        {
            return $"Sorry, couldn't get weather data for {city}.";
        }
    }
}

// Create agent with instance method as tool
var weatherService = new WeatherService();

AIAgent agent = chatClient.CreateAIAgent(
    name: "WeatherBot",
    instructions: "You provide accurate weather information.",
    tools: [AIFunctionFactory.Create(weatherService.GetRealWeather)]);
```

---

## Part 7: Per-Run Tools

You can also provide tools at runtime, not just at construction:

```csharp
// Agent created without tools
AIAgent agent = chatClient.CreateAIAgent(
    name: "FlexibleAgent",
    instructions: "You are a flexible assistant.");

// Add tools for a specific run
var chatOptions = new ChatOptions
{
    Tools = [AIFunctionFactory.Create(GetWeather)]
};

var response = await agent.RunAsync(
    "What's the weather in Berlin?",
    options: new ChatClientAgentRunOptions(chatOptions));
```

This is useful when different conversations need different tools.

---

## Part 8: Best Practices for Tools

### Clear Descriptions
```csharp
// ❌ Bad - vague description
[Description("Gets data")]
static string GetData(string input) { ... }

// ✅ Good - specific description
[Description("Retrieves the current stock price for a given ticker symbol")]
static string GetStockPrice(
    [Description("Stock ticker symbol (e.g., MSFT, AAPL)")] string ticker) { ... }
```

### Error Handling
```csharp
[Description("Get user account balance")]
static async Task<string> GetBalance(string accountId)
{
    try
    {
        var balance = await _database.GetBalanceAsync(accountId);
        return $"Account {accountId} has a balance of ${balance:F2}";
    }
    catch (AccountNotFoundException)
    {
        return $"Account {accountId} was not found. Please verify the account ID.";
    }
    catch (Exception ex)
    {
        return "Unable to retrieve balance at this time. Please try again later.";
    }
}
```

### Keep Tools Focused
```csharp
// ❌ Bad - tool does too much
[Description("Manage the entire order lifecycle")]
static string ManageOrder(string action, string orderId, ...) { ... }

// ✅ Good - focused, single-purpose tools
[Description("Create a new order")]
static string CreateOrder(...) { ... }

[Description("Cancel an existing order")]
static string CancelOrder(string orderId) { ... }

[Description("Get order status")]
static string GetOrderStatus(string orderId) { ... }
```

---

## Let's Review: What You Learned

| Concept | Key Takeaway |
|---------|-------------|
| **Tools** | Functions that agents can call during reasoning |
| **AIFunctionFactory** | Creates tools from C# methods |
| **Descriptions** | Critical for the agent to understand tool purpose |
| **Multiple Tools** | Agent automatically chooses the right tool |
| **Async Tools** | Support for I/O operations and API calls |

### Quick Self-Check

1. What happens if you don't provide `[Description]` attributes?
2. How does the agent decide which tool to use?
3. When would you use per-run tools vs construction-time tools?

---

## Sample Code Reference

| Sample | Description |
|--------|-------------|
| [AgentFx-BackgroundResponses-02-Tools](../samples/AgentFx/AgentFx-BackgroundResponses-02-Tools/) | Agent with function tools |
| [AgentFx-BackgroundResponses-03-Complex](../samples/AgentFx/AgentFx-BackgroundResponses-03-Complex/) | Complex tool scenarios |

---

## Additional Resources

- [Agent Tools Documentation](https://learn.microsoft.com/agent-framework/user-guide/agents/agent-tools): Complete guide to creating and managing agent tools
- [Function Calling Concepts](https://learn.microsoft.com/dotnet/ai/conceptual/function-calling): How AI models decide when and how to call functions
- [AIFunctionFactory reference](https://learn.microsoft.com/dotnet/api/microsoft.extensions.ai.aifunctionfactory): API for wrapping .NET methods as AI tools

---

## Up Next

A single agent with tools is powerful, but some tasks need multiple specialists working together. Let's explore multi-agent workflows:

[Continue to Part 3: Multi-Agent Workflows →](./03-multi-agent-workflows.md)
