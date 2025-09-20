using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Connectors.InMemory;

// Advanced RAG with Vector Search
// This demo shows how to implement Retrieval-Augmented Generation with embeddings

Console.WriteLine("🔍 Advanced RAG with Vector Search Demo");
Console.WriteLine();

try
{
    // Setup in-memory vector store
    var vectorStore = new InMemoryVectorStore();
    var moviesCollection = vectorStore.GetCollection<int, MovieVector<int>>("movies");
    await moviesCollection.EnsureCollectionExistsAsync();

    Console.WriteLine("📚 Setting up knowledge base with movie information...");

    // Create embeddings generator (using Ollama locally)
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator;
    
    try
    {
        embeddingGenerator = new OllamaEmbeddingGenerator(new Uri("http://localhost:11434/"), "all-minilm");
        Console.WriteLine("✅ Connected to Ollama for embeddings (all-minilm model)");
    }
    catch
    {
        Console.WriteLine("❌ Could not connect to Ollama. Please ensure:");
        Console.WriteLine("  1. Ollama is running: ollama serve");
        Console.WriteLine("  2. The embedding model is available: ollama pull all-minilm");
        return;
    }

    // Load movie data and generate embeddings
    var movieData = MovieFactory<int>.GetMovieVectorList();
    Console.WriteLine($"📽️ Processing {movieData.Count} movies...");

    foreach (var movie in movieData)
    {
        Console.WriteLine($"  Embedding: {movie.Title}");
        movie.Vector = await embeddingGenerator.GenerateVectorAsync(movie.Description);
        await moviesCollection.UpsertAsync(movie);
    }

    Console.WriteLine();
    Console.WriteLine("🔍 Vector store ready! Let's search for movies...");
    Console.WriteLine();

    // Interactive search loop
    var searchQueries = new[]
    {
        "A family friendly movie that includes ogres and dragons",
        "Science fiction with space battles and robots",
        "A romantic comedy with time travel",
        "Action movie with superheroes saving the world",
        "Animated movie about talking toys"
    };

    foreach (var query in searchQueries)
    {
        Console.WriteLine($"🔎 Query: \"{query}\"");
        Console.WriteLine("📊 Top matches:");
        
        // Generate embedding for the search query
        var queryEmbedding = await embeddingGenerator.GenerateVectorAsync(query);
        
        // Search the vector store
        var results = new List<(MovieVector<int> Movie, double Score)>();
        await foreach (var result in moviesCollection.SearchAsync(queryEmbedding, top: 3))
        {
            results.Add((result.Record, result.Score));
        }

        // Display results
        for (int i = 0; i < results.Count; i++)
        {
            var (movie, score) = results[i];
            Console.WriteLine($"  {i + 1}. {movie.Title} (Score: {score:F3})");
            Console.WriteLine($"     {movie.Description}");
            Console.WriteLine();
        }

        Console.WriteLine("---");
        Console.WriteLine();
    }

    Console.WriteLine("✅ RAG Vector Search demo completed!");
    Console.WriteLine();
    Console.WriteLine("💡 Key concepts demonstrated:");
    Console.WriteLine("  • Vector embeddings for semantic search");
    Console.WriteLine("  • In-memory vector storage");
    Console.WriteLine("  • Similarity scoring and ranking");
    Console.WriteLine("  • Knowledge retrieval for AI enhancement");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine();
    Console.WriteLine("🔧 Troubleshooting:");
    Console.WriteLine("• Ensure Ollama is running with the all-minilm model");
    Console.WriteLine("• Check that the semantic kernel packages are properly installed");
}

// Movie data factory (simplified version)
public static class MovieFactory<T>
{
    public static List<MovieVector<T>> GetMovieVectorList()
    {
        return new List<MovieVector<T>>
        {
            new() { Key = (T)(object)1, Title = "Shrek", Description = "An ogre embarks on a quest to rescue a princess from a dragon-guarded tower, discovering friendship and love along the way. Family-friendly animated comedy with fairy tale characters." },
            new() { Key = (T)(object)2, Title = "Star Wars", Description = "Epic space opera featuring rebels fighting against an evil galactic empire, with Jedi knights, lightsabers, and the Force. Classic science fiction adventure." },
            new() { Key = (T)(object)3, Title = "The Matrix", Description = "A hacker discovers reality is a simulation and joins a rebellion against machines that have enslaved humanity. Cyberpunk action with philosophical themes." },
            new() { Key = (T)(object)4, Title = "Toy Story", Description = "Animated tale of toys that come to life when humans aren't around, focusing on friendship and loyalty. Pixar's groundbreaking computer-animated family film." },
            new() { Key = (T)(object)5, Title = "Groundhog Day", Description = "A weatherman gets trapped in a time loop, reliving the same day repeatedly until he learns to become a better person. Romantic comedy with fantasy elements." },
            new() { Key = (T)(object)6, Title = "The Avengers", Description = "Superheroes team up to save Earth from an alien invasion, featuring Iron Man, Captain America, Thor, and Hulk. Action-packed Marvel superhero ensemble." },
            new() { Key = (T)(object)7, Title = "Finding Nemo", Description = "A clownfish searches the ocean for his lost son with the help of a forgetful blue tang fish. Heartwarming Pixar animated adventure about family." },
            new() { Key = (T)(object)8, Title = "Inception", Description = "A thief who steals secrets from dreams is tasked with planting an idea instead. Mind-bending science fiction thriller about reality and dreams." },
            new() { Key = (T)(object)9, Title = "The Lion King", Description = "A young lion prince flees his kingdom after his father's death, later returning to reclaim his throne. Disney animated musical about courage and responsibility." },
            new() { Key = (T)(object)10, Title = "Back to the Future", Description = "A teenager accidentally travels back in time and must ensure his parents fall in love to secure his existence. Science fiction comedy about time travel." }
        };
    }
}

// Movie vector model
public class MovieVector<T>
{
    public T Key { get; set; } = default!;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Embedding<float>? Vector { get; set; }
}