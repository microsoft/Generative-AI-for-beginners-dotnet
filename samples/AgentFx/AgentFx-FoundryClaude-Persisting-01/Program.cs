using elbruno.Extensions.AI.Claude;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

// AgentFx Persisting Conversations with Claude via Microsoft Foundry
// Demonstrates:
// 1) Creating an AgentThread, running prompts, and persisting thread state
// 2) Reloading persisted threads and continuing conversations with context preserved
// 3) Using Claude models with the same agent/thread model for stateful interactions

string SavedThreadFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "agent_thread_claude.json");

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpointClaude = config["endpointClaude"] ?? throw new InvalidOperationException("Missing 'endpointClaude' configuration");
var apiKey = config["apikey"] ?? throw new InvalidOperationException("Missing 'apikey' configuration");
var deploymentName = config["deploymentName"] ?? "claude-haiku-4-5";

// Create IChatClient using elbruno.Extensions.AI.Claude package
IChatClient client = new AzureClaudeClient(
    endpoint: new Uri(endpointClaude),
    modelId: deploymentName,
    apiKey: apiKey);

// Create a simple agent
var agent = client.CreateAIAgent(
    name: "agent",
    instructions: "You are a helpful assistant with a good memory");

// ------------------------------------------------------------
// Step 1: Create new thread, ask a question, and persist the thread

StreamConsoleHelper.PrintHeader("Step 1: Create new thread, ask a question, and persist the thread", clearConsole: true);
StreamConsoleHelper.PrintInfo($"Using Claude model: {deploymentName}");
Console.WriteLine();

var thread = agent.GetNewThread();
var question = "My name is Bruno and I live in Madrid";

StreamConsoleHelper.PrintSection("Start answering your question");
StreamConsoleHelper.PrintLabeled("Question", question);
var response = await agent.RunAsync(question, thread);
StreamConsoleHelper.PrintAccumulatedLine(response.Text);
Console.WriteLine();

// Serialize thread state and write to file
var threadRaw = thread.Serialize(JsonSerializerOptions.Web).GetRawText();
await File.WriteAllTextAsync(SavedThreadFilePath, threadRaw);
StreamConsoleHelper.PrintLabeled("Saved thread to", SavedThreadFilePath);
Console.WriteLine();

// ------------------------------------------------------------
// Step 2: Create new thread and ask a question without context

StreamConsoleHelper.PrintHeader("Step 2: Create new thread and ask \"where do I live?\"", clearConsole: false);
thread = agent.GetNewThread();
question = "Where do I live?";

StreamConsoleHelper.PrintSection("Start answering your question (without context)");
StreamConsoleHelper.PrintLabeled("Question", question);
response = await agent.RunAsync(question, thread);
StreamConsoleHelper.PrintAccumulatedLine(response.Text);
Console.WriteLine();

// ------------------------------------------------------------
// Step 3: Load persisted thread and ask the question with context

StreamConsoleHelper.PrintHeader("Step 3: Load persisted thread and ask \"where do I live?\"", clearConsole: false);
var loadedJson = await File.ReadAllTextAsync(SavedThreadFilePath);
JsonElement reloaded = JsonSerializer.Deserialize<JsonElement>(loadedJson, JsonSerializerOptions.Web);

// Rehydrate the thread for this agent
thread = agent.DeserializeThread(reloaded, JsonSerializerOptions.Web);
question = "Where do I live?";

StreamConsoleHelper.PrintSection("Start answering your question (with context)");
StreamConsoleHelper.PrintLabeled("Question", question);
response = await agent.RunAsync(question, thread);
StreamConsoleHelper.PrintAccumulatedLine(response.Text);
Console.WriteLine();

// ------------------------------------------------------------
// Step 4: Continue conversation in the persisted thread

StreamConsoleHelper.PrintHeader("Step 4: Continue conversation with more context", clearConsole: false);
question = "What is my name?";

StreamConsoleHelper.PrintSection("Start answering your question");
StreamConsoleHelper.PrintLabeled("Question", question);
response = await agent.RunAsync(question, thread);
StreamConsoleHelper.PrintAccumulatedLine(response.Text);
Console.WriteLine();

// Save updated thread
threadRaw = thread.Serialize(JsonSerializerOptions.Web).GetRawText();
await File.WriteAllTextAsync(SavedThreadFilePath, threadRaw);
StreamConsoleHelper.PrintLabeled("Updated thread saved to", SavedThreadFilePath);
Console.WriteLine();

// ------------------------------------------------------------
// Step 5: End

StreamConsoleHelper.PrintFooter("End of the example. Press any key to exit.");
Console.ReadKey();
