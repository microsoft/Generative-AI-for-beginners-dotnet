using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System.ClientModel;
using System.ComponentModel;

// Configure the chat client from environment variables
IChatClient chatClient = CreateChatClient();

// Create the AI agent with the GetCurrentTime tool
AIAgent timeAgent = chatClient.AsAIAgent(
    name: "TimeZoneAgent",
    instructions: "You are a helpful assistant that provides the current date and time in any timezone. " +
                  "When a user asks for the time, use the GetCurrentTime tool to get accurate results. " +
                  "Always include the timezone name and the formatted date/time in your response.",
    tools: [AIFunctionFactory.Create(GetCurrentTime)]);

// Run an interactive chat loop
Console.WriteLine("TimeZone Agent — Ask me for the current time in any timezone!");
Console.WriteLine("Type 'exit' to quit.\n");

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    AgentResponse response = await timeAgent.RunAsync(input);
    Console.WriteLine($"Agent: {response.Text}\n");
}

Console.WriteLine("Goodbye!");

// --- Tool function ---

[Description("Gets the current date and time for the specified timezone.")]
static string GetCurrentTime(
    [Description("The timezone name, e.g. 'Eastern Standard Time', 'UTC', 'Pacific Standard Time'.")] string timezoneName)
{
    try
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneName);
        var now = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, tz);
        return $"Current time in {tz.DisplayName}: {now:yyyy-MM-dd HH:mm:ss} ({tz.StandardName})";
    }
    catch (TimeZoneNotFoundException)
    {
        return $"Timezone '{timezoneName}' was not found. Please use a valid timezone name such as 'UTC', 'Eastern Standard Time', or 'Pacific Standard Time'.";
    }
}

// --- Chat client factory ---

static IChatClient CreateChatClient()
{
    // Try Azure OpenAI first
    var azureEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
    var azureModel = Environment.GetEnvironmentVariable("AZURE_OPENAI_MODEL") ?? "gpt-5-mini";
    var azureApiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_APIKEY");

    if (!string.IsNullOrEmpty(azureEndpoint) && !string.IsNullOrEmpty(azureApiKey))
    {
        Console.WriteLine($"Using Azure OpenAI: {azureEndpoint} / {azureModel}");
        return new AzureOpenAIClient(new Uri(azureEndpoint), new ApiKeyCredential(azureApiKey))
            .GetChatClient(azureModel)
            .AsIChatClient();
    }

    // Fall back to Ollama
    var ollamaEndpoint = Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434/";
    var ollamaModel = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "phi4-mini";

    Console.WriteLine($"Using Ollama: {ollamaEndpoint} / {ollamaModel}");
    return new OllamaApiClient(new Uri(ollamaEndpoint), ollamaModel);
}
