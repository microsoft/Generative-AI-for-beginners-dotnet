// Retrieval-Augmented Generation (RAG) Demo
// This demo shows how to implement basic RAG using Microsoft.Extensions.AI with in-memory vector storage
// 
// Prerequisites: Set GITHUB_TOKEN environment variable or user secret
// Run with: dotnet run 04-retrieval-augmented-generation.cs (requires .NET 10)

using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.Text;

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

Console.WriteLine("🔍 Retrieval-Augmented Generation (RAG) Demo");
Console.WriteLine("===========================================");
Console.WriteLine("Setting up movie database with simple similarity search...\n");

// Sample movie data with embedded descriptions
var movieData = new List<Movie>
{
    new() { Id = 1, Title = "The Lion King", Year = 1994, Category = "Animation, Family, Adventure", 
           Description = "A young lion named Simba embarks on a journey to reclaim his throne as the king of the Pride Lands after his father's death.",
           Keywords = new[] { "lion", "family", "adventure", "animals", "kingdom", "young", "journey" } },
    new() { Id = 2, Title = "Inception", Year = 2010, Category = "Science Fiction, Action, Thriller",
           Description = "A group of thieves enter the dreams of their targets to steal information in this mind-bending sci-fi thriller.",
           Keywords = new[] { "dreams", "sci-fi", "thriller", "mind-bending", "thieves", "technology" } },
    new() { Id = 3, Title = "Shrek", Year = 2001, Category = "Animation, Comedy, Family",
           Description = "An ogre named Shrek goes on a quest to rescue Princess Fiona from a dragon-guarded castle.",
           Keywords = new[] { "ogre", "dragon", "princess", "quest", "comedy", "family", "fairy tale" } },
    new() { Id = 4, Title = "Finding Nemo", Year = 2003, Category = "Animation, Family, Adventure",
           Description = "A clownfish named Marlin searches for his lost son Nemo across the ocean with help from a forgetful fish named Dory.",
           Keywords = new[] { "fish", "ocean", "family", "adventure", "lost", "search", "underwater" } },
    new() { Id = 5, Title = "The Matrix", Year = 1999, Category = "Science Fiction, Action",
           Description = "A computer hacker discovers that reality is a simulation and joins a rebellion against the machines.",
           Keywords = new[] { "hacker", "simulation", "reality", "machines", "rebellion", "technology" } }
};

// Create chat client
IChatClient chatClient = new ChatCompletionsClient(
        endpoint: new Uri("https://models.inference.ai.azure.com"),
        new AzureKeyCredential(githubToken))
        .AsIChatClient("gpt-4o-mini");

Console.WriteLine("📚 Movie database loaded:");
foreach (var movie in movieData)
{
    Console.WriteLine($"   ✓ {movie.Title} ({movie.Year})");
}

Console.WriteLine("\n🎬 Movie Recommendation System Ready!");
Console.WriteLine("Ask for movie recommendations and I'll search our database!");
Console.WriteLine("Type 'quit' to exit.\n");

try
{
    while (true)
    {
        Console.Write("👤 What kind of movie are you looking for? ");
        var query = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(query) || query.ToLower() == "quit")
        {
            Console.WriteLine("👋 Goodbye! Enjoy your movies!");
            break;
        }

        Console.WriteLine($"\n🔍 Searching for movies matching: '{query}'");
        
        // Simple keyword-based similarity search
        var searchResults = FindSimilarMovies(query.ToLower(), movieData);

        if (searchResults.Any())
        {
            // Create context from search results
            var context = new StringBuilder();
            context.AppendLine("Based on your request, here are some relevant movies from our database:");
            foreach (var movie in searchResults.Take(3))
            {
                context.AppendLine($"- {movie.Title} ({movie.Year}): {movie.Description}");
            }
            
            // Generate AI response with context
            var prompt = $@"User Query: {query}
                
Movie Database Context:
{context}
                
Please provide a helpful recommendation based on the user's request and the movies found in our database. 
Explain why these movies match what they're looking for.";

            Console.WriteLine("🤖 AI Recommendation:");
            Console.WriteLine(new string('─', 50));
            var response = await chatClient.CompleteAsync(prompt);
            Console.WriteLine(response.Message.Text);
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("❌ No matching movies found in our database.");
            Console.WriteLine();
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine("Make sure your GITHUB_TOKEN is valid and has access to GitHub Models.");
}

// Simple similarity search based on keywords and description
static List<Movie> FindSimilarMovies(string query, List<Movie> movies)
{
    var queryWords = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var results = new List<(Movie movie, int score)>();
    
    foreach (var movie in movies)
    {
        int score = 0;
        
        // Check keywords
        foreach (var word in queryWords)
        {
            if (movie.Keywords.Any(k => k.Contains(word, StringComparison.OrdinalIgnoreCase)))
                score += 3;
            if (movie.Description.Contains(word, StringComparison.OrdinalIgnoreCase))
                score += 2;
            if (movie.Title.Contains(word, StringComparison.OrdinalIgnoreCase))
                score += 4;
            if (movie.Category.Contains(word, StringComparison.OrdinalIgnoreCase))
                score += 2;
        }
        
        if (score > 0)
            results.Add((movie, score));
    }
    
    return results.OrderByDescending(r => r.score).Select(r => r.movie).ToList();
}

// Simplified movie data class
public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int Year { get; set; }
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public string[] Keywords { get; set; } = [];
}