/*
 * Purpose: Make typed text decisions with a model served by Llama App.
 * 1. Connect to the local Llama App decision endpoint and select an installed model.
 * 2. Send one synthetic support request to choice, score, and assessment operations.
 * 3. Display probabilities instead of asking a chat model to explain its answer.
 */
using ElBruno.AI.Decisions.Llama;

const string defaultModelId = "LiquidAI/d1-omni-600M-GGUF:Q8_0";

if (args.Length > 1)
{
    throw new ArgumentException("Usage: Decision-14-LlamaText [model-id]");
}

string modelId = args.Length == 1 ? args[0] : defaultModelId;
if (string.IsNullOrWhiteSpace(modelId))
{
    throw new ArgumentException("The model ID must not be empty.", nameof(args));
}

// 1. Llama App serves locally installed decision models on its default endpoint.
using var client = new LlamaDecisionClient(new LlamaDecisionOptions
{
    Model = modelId,
    Timeout = TimeSpan.FromMinutes(3)
});
using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(3));

// 2. Use fixed demo text so no customer data or credentials are required.
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

// 3. Show the selected outcome and the model's separate probability estimates.
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
