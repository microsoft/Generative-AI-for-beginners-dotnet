/*
 * Purpose: Run typed decisions against a cached Foundry Local decision model.
 * 1. Connect the SDK to the model cache used by the Foundry Local CLI.
 * 2. Resolve a model that supports native choice, score, and assessment decisions.
 * 3. Print the typed results and release the model without deleting its cache.
 */
using ElBruno.AI.Decisions.Foundry.Local;
using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.Logging.Abstractions;

const string defaultModelId = "kev-0.8b";

if (args.Length > 1)
{
    throw new ArgumentException("Usage: Decision-13-FoundryLocal [model-id]");
}

string modelId = args.Length == 1 ? args[0] : defaultModelId;
if (string.IsNullOrWhiteSpace(modelId))
{
    throw new ArgumentException("The model ID must not be empty.", nameof(args));
}

// 1. Reuse the Foundry Local CLI cache. This does not download a missing model.
await FoundryLocalManager.CreateAsync(
    new Configuration { AppName = "foundry" },
    NullLogger.Instance);
FoundryLocalManager manager = FoundryLocalManager.Instance;
IModel? model = null;

try
{
    // 2. Resolve a cached model whose metadata advertises typed decision support.
    model = await FoundryLocalDecisionModelResolver.ResolveAsync(modelId);
    var client = new FoundryLocalDecisionClient(model);
    using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(5));

    // 3. Make three different decisions from the same synthetic support request.
    const string request = "The kitchen tap is leaking. Please contact me.";

    var route = await client.ChooseAsync(
        request,
        "Which team should handle this request?",
        new Dictionary<string, string?>
        {
            ["plumbing"] = "Water leaks and drains",
            ["electrical"] = "Lights and wiring",
            ["other"] = "Other problems or unclear requests"
        },
        timeout.Token);

    string[] urgencyLevels = ["Routine", "Soon", "Immediate"];
    var urgency = await client.ScoreAsync(
        request,
        "How urgent is this request?",
        urgencyLevels,
        timeout.Token);

    var contactRequested = await client.AssessAsync(
        request,
        "The customer asks to be contacted.",
        timeout.Token);

    Console.WriteLine($"Model: {modelId}");
    Console.WriteLine($"Route: {route.Choice} (confidence: {route.Confidence:P1})");
    foreach ((string option, double probability) in route.Probabilities)
    {
        Console.WriteLine($"  {option}: {probability:P1}");
    }

    Console.WriteLine($"Urgency score (expected rubric position): {urgency.Score:F2}");
    for (int level = 0; level < urgency.LevelProbabilities.Count; level++)
    {
        Console.WriteLine($"  {urgencyLevels[level]}: {urgency.LevelProbabilities[level]:P1}");
    }

    Console.WriteLine($"Contact requested probability: {contactRequested.Probability:P1}");
    Console.WriteLine("Probabilities and confidence are model estimates, not guarantees of correctness.");
}
finally
{
    try
    {
        if (model is not null)
        {
            await model.UnloadAsync();
        }
    }
    finally
    {
        manager.Dispose();
    }
}
