// Structured Output Demo  
// This demo shows how to get structured JSON responses from AI models
// 
// Prerequisites: Set GITHUB_TOKEN environment variable or user secret
// Run with: dotnet run 05-structured-output.cs (requires .NET 10)

using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;

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

// Create chat client
IChatClient client = new ChatCompletionsClient(
        endpoint: new Uri("https://models.inference.ai.azure.com"),
        new AzureKeyCredential(githubToken))
        .AsIChatClient("gpt-4o-mini");

Console.WriteLine("📊 Structured Output Demo");
Console.WriteLine("=========================");
Console.WriteLine("This demo shows how to get structured JSON responses from AI models.\n");

// Demo 1: Product Analysis
await DemoProductAnalysis(client);

// Demo 2: Recipe Generation  
await DemoRecipeGeneration(client);

// Demo 3: Meeting Summary
await DemoMeetingSummary(client);

Console.WriteLine("✨ Demo completed! All responses were structured JSON objects.");

static async Task DemoProductAnalysis(IChatClient client)
{
    Console.WriteLine("🛍️  Demo 1: Product Review Analysis");
    Console.WriteLine("──────────────────────────────────");
    
    var productReviews = """
        "This laptop is amazing! Super fast processor and great battery life. Worth every penny."
        "The screen is a bit dim and the keyboard feels cheap. Not impressed."
        "Excellent build quality and performance. Highly recommended for developers."
        "Overpriced for what you get. There are better alternatives."
        """;

    var prompt = $@"Analyze these product reviews and return the results in the following JSON format:
        {{{{
            ""overall_sentiment"": ""positive|negative|neutral"",
            ""sentiment_score"": 0.0-1.0,
            ""key_themes"": [""theme1"", ""theme2""],
            ""summary"": ""brief summary"",
            ""recommendation"": ""buy|avoid|consider""
        }}}}
        
        Reviews:
        {productReviews}
        
        Respond only with valid JSON, no additional text.";

    try
    {
        var response = await client.CompleteAsync(prompt);
        var analysis = JsonSerializer.Deserialize<ProductAnalysis>(response.Message.Text);
        
        Console.WriteLine("📈 Analysis Result:");
        Console.WriteLine($"   Overall Sentiment: {analysis.OverallSentiment}");
        Console.WriteLine($"   Sentiment Score: {analysis.SentimentScore:F2}");
        Console.WriteLine($"   Key Themes: {string.Join(", ", analysis.KeyThemes)}");
        Console.WriteLine($"   Summary: {analysis.Summary}");
        Console.WriteLine($"   Recommendation: {analysis.Recommendation}");
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error parsing product analysis: {ex.Message}");
        Console.WriteLine();
    }
}

static async Task DemoRecipeGeneration(IChatClient client)
{
    Console.WriteLine("👨‍🍳 Demo 2: Recipe Generation");
    Console.WriteLine("─────────────────────────────");
    
    var prompt = @"Create a simple pasta recipe and return it in the following JSON format:
        {
            ""name"": ""recipe name"",
            ""prep_time_minutes"": 15,
            ""cook_time_minutes"": 20,
            ""servings"": 4,
            ""difficulty"": ""easy|medium|hard"",
            ""ingredients"": [
                {""item"": ""ingredient name"", ""amount"": ""2 cups"", ""notes"": ""optional notes""}
            ],
            ""steps"": [
                {""step_number"": 1, ""instruction"": ""detailed instruction"", ""time_minutes"": 5}
            ],
            ""tips"": [""tip1"", ""tip2""]
        }
        
        Respond only with valid JSON, no additional text.";

    try
    {
        var response = await client.CompleteAsync(prompt);
        var recipe = JsonSerializer.Deserialize<Recipe>(response.Message.Text);
        
        Console.WriteLine("🍝 Generated Recipe:");
        Console.WriteLine($"   Name: {recipe.Name}");
        Console.WriteLine($"   Prep: {recipe.PrepTimeMinutes}min | Cook: {recipe.CookTimeMinutes}min | Serves: {recipe.Servings}");
        Console.WriteLine($"   Difficulty: {recipe.Difficulty}");
        Console.WriteLine("   Ingredients:");
        foreach (var ingredient in recipe.Ingredients)
        {
            Console.WriteLine($"     • {ingredient.Amount} {ingredient.Item}");
        }
        Console.WriteLine("   Steps:");
        foreach (var step in recipe.Steps)
        {
            Console.WriteLine($"     {step.StepNumber}. {step.Instruction} ({step.TimeMinutes}min)");
        }
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error parsing recipe: {ex.Message}");
        Console.WriteLine();
    }
}

static async Task DemoMeetingSummary(IChatClient client)
{
    Console.WriteLine("📝 Demo 3: Meeting Summary");
    Console.WriteLine("─────────────────────────");
    
    var meetingTranscript = """
        John: Thanks everyone for joining. Let's start with the quarterly review.
        Sarah: The sales numbers look good. We're up 15% from last quarter.
        Mike: That's great! The new marketing campaign is really paying off.
        John: What about the development timeline for the new features?
        Sarah: We're slightly behind on the mobile app, but the web version is on track.
        Mike: I think we need to hire two more developers to meet our Q4 goals.
        John: Agreed. I'll work on the budget approval. Any other concerns?
        Sarah: The customer support team needs better tools for handling tickets.
        """;

    var prompt = $@"Summarize this meeting transcript and return the results in the following JSON format:
        {{{{
            ""meeting_type"": ""type of meeting"",
            ""key_topics"": [""topic1"", ""topic2""],
            ""decisions_made"": [""decision1"", ""decision2""],
            ""action_items"": [
                {{{{""assignee"": ""person"", ""task"": ""task description"", ""priority"": ""high|medium|low""}}}}
            ],
            ""metrics_mentioned"": [
                {{{{""metric"": ""metric name"", ""value"": ""value"", ""context"": ""context""}}}}
            ],
            ""next_steps"": [""step1"", ""step2""]
        }}}}
        
        Meeting Transcript:
        {meetingTranscript}
        
        Respond only with valid JSON, no additional text.";

    try
    {
        var response = await client.CompleteAsync(prompt);
        var summary = JsonSerializer.Deserialize<MeetingSummary>(response.Message.Text);
        
        Console.WriteLine("📋 Meeting Summary:");
        Console.WriteLine($"   Type: {summary.MeetingType}");
        Console.WriteLine($"   Key Topics: {string.Join(", ", summary.KeyTopics)}");
        Console.WriteLine("   Decisions Made:");
        foreach (var decision in summary.DecisionsMade)
        {
            Console.WriteLine($"     • {decision}");
        }
        Console.WriteLine("   Action Items:");
        foreach (var item in summary.ActionItems)
        {
            Console.WriteLine($"     • {item.Assignee}: {item.Task} ({item.Priority} priority)");
        }
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error parsing meeting summary: {ex.Message}");
        Console.WriteLine();
    }
}

// Data classes for structured outputs
public class ProductAnalysis
{
    [JsonPropertyName("overall_sentiment")]
    public string OverallSentiment { get; set; } = "";
    
    [JsonPropertyName("sentiment_score")]
    public double SentimentScore { get; set; }
    
    [JsonPropertyName("key_themes")]
    public string[] KeyThemes { get; set; } = [];
    
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = "";
    
    [JsonPropertyName("recommendation")]
    public string Recommendation { get; set; } = "";
}

public class Recipe
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    
    [JsonPropertyName("prep_time_minutes")]
    public int PrepTimeMinutes { get; set; }
    
    [JsonPropertyName("cook_time_minutes")]
    public int CookTimeMinutes { get; set; }
    
    [JsonPropertyName("servings")]
    public int Servings { get; set; }
    
    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; } = "";
    
    [JsonPropertyName("ingredients")]
    public Ingredient[] Ingredients { get; set; } = [];
    
    [JsonPropertyName("steps")]
    public RecipeStep[] Steps { get; set; } = [];
    
    [JsonPropertyName("tips")]
    public string[] Tips { get; set; } = [];
}

public class Ingredient
{
    [JsonPropertyName("item")]
    public string Item { get; set; } = "";
    
    [JsonPropertyName("amount")]
    public string Amount { get; set; } = "";
    
    [JsonPropertyName("notes")]
    public string Notes { get; set; } = "";
}

public class RecipeStep
{
    [JsonPropertyName("step_number")]
    public int StepNumber { get; set; }
    
    [JsonPropertyName("instruction")]
    public string Instruction { get; set; } = "";
    
    [JsonPropertyName("time_minutes")]
    public int TimeMinutes { get; set; }
}

public class MeetingSummary
{
    [JsonPropertyName("meeting_type")]
    public string MeetingType { get; set; } = "";
    
    [JsonPropertyName("key_topics")]
    public string[] KeyTopics { get; set; } = [];
    
    [JsonPropertyName("decisions_made")]
    public string[] DecisionsMade { get; set; } = [];
    
    [JsonPropertyName("action_items")]
    public ActionItem[] ActionItems { get; set; } = [];
    
    [JsonPropertyName("metrics_mentioned")]
    public Metric[] MetricsMentioned { get; set; } = [];
    
    [JsonPropertyName("next_steps")]
    public string[] NextSteps { get; set; } = [];
}

public class ActionItem
{
    [JsonPropertyName("assignee")]
    public string Assignee { get; set; } = "";
    
    [JsonPropertyName("task")]
    public string Task { get; set; } = "";
    
    [JsonPropertyName("priority")]
    public string Priority { get; set; } = "";
}

public class Metric
{
    [JsonPropertyName("metric")]
    public string MetricName { get; set; } = "";
    
    [JsonPropertyName("value")]
    public string Value { get; set; } = "";
    
    [JsonPropertyName("context")]
    public string Context { get; set; } = "";
}