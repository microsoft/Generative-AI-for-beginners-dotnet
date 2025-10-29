using AgentFx_BackgroundResponses_01_Simple;
using Microsoft.Extensions.AI;
using OpenAI;
using System.Text.Json;
using System.Threading.Tasks;

// More information: https://learn.microsoft.com/en-us/agent-framework/tutorials/agents/persisted-conversation?pivots=programming-language-csharp

string SavedThreadFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "agent_thread.json");

// create a simple agent
var client = ChatClientProvider.GetChatClient();
var agent = client.CreateAIAgent(
    name: "agent",
    instructions: "You are a helpful assistant");

// ------------------------------------------------------------
// Step 1, create new thread, ask a question and persist the thread

var thread = agent.GetNewThread();
var question = "My name is Bruno";

StreamConsoleHelper.PrintHeader("Step 1, create new thread, ask a question and persist the thread", false);
StreamConsoleHelper.PrintSection("Start answering your question");
StreamConsoleHelper.PrintLabeled("Question", question);
var response = await agent.RunAsync(question, thread);
StreamConsoleHelper.PrintAccumulatedLine(response.Text);

// Serialize thread state and write to file
var threadRaw = thread.Serialize(JsonSerializerOptions.Web).GetRawText();
await File.WriteAllTextAsync(SavedThreadFilePath, threadRaw);
StreamConsoleHelper.PrintLabeled("Saved thread to", SavedThreadFilePath);

// ------------------------------------------------------------
// Step 2, create new thread and ask the question "what is my name"

StreamConsoleHelper.PrintHeader("Step 2, create new thread and ask the question \"what is my name\"", false);
thread = agent.GetNewThread();
question = "What is my name?";

StreamConsoleHelper.PrintSection("Start answering your question");
StreamConsoleHelper.PrintLabeled("Question", question);
response = await agent.RunAsync(question, thread);
StreamConsoleHelper.PrintAccumulatedLine(response.Text);


// ------------------------------------------------------------
// Step 3, load persisted thread and ask the question "what is my name"

StreamConsoleHelper.PrintHeader("Step 3, load persisted thread and ask the question \"what is my name\"", false);
var loadedJson = await File.ReadAllTextAsync(SavedThreadFilePath);
JsonElement reloaded = JsonSerializer.Deserialize<JsonElement>(loadedJson, JsonSerializerOptions.Web);

// Rehydrate the thread for this agent
thread = agent.DeserializeThread(reloaded, JsonSerializerOptions.Web);
question = "What is my name?";

StreamConsoleHelper.PrintSection("Start answering your question");
StreamConsoleHelper.PrintLabeled("Question", question);
response = await agent.RunAsync(question, thread);
StreamConsoleHelper.PrintAccumulatedLine(response.Text);


// ------------------------------------------------------------
// Step 4, end

StreamConsoleHelper.PrintFooter("End of the example, press any key to exit");
Console.ReadKey();