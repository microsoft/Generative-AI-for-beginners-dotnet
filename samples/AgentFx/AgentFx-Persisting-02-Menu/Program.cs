using AgentFx_BackgroundResponses_01_Simple;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System.Text.Json;

// Persisting Conversation — Menu Sample
//1) Provides an interactive menu to create, resume, and persist AgentThreads.
//2) Demonstrates running questions on new threads or resumed threads and persisting state between runs.
//3) Centralizes console I/O and printing helpers to make the sample easier to extend.
// More information: https://learn.microsoft.com/en-us/agent-framework/tutorials/agents/persisted-conversation?pivots=programming-language-csharp

await BackgroundResponsesDemo.RunAsync();

internal static class BackgroundResponsesDemo
{
    private static readonly string SavedThreadFilePath = Path.Combine(Path.GetTempPath(), "agent_thread.json");

    public static async Task RunAsync()
    {
        // get chat response client
        var client = ChatClientProvider.GetChatClient();

        // create a simple agent with basic instructions
        var agent = client.CreateAIAgent(
            name: "agent",
            instructions: "You are a helpful assistant");

        while (true)
        {
            PersistingUI.Clear();
            StreamConsoleHelper.PrintHeader("AgentFx - Background Responses (Persisting Demo)");
            PersistingUI.PrintMenu();

            var sel = PersistingUI.ReadSelection();

            if (sel == "0") break;

            switch (sel)
            {
                case "1":
                    await OptionStartNewThread(agent);
                    break;
                case "2":
                    await OptionSimpleSession(agent);
                    break;
                case "3":
                    await OptionLoadAndContinue(agent);
                    break;
                default:
                    PersistingUI.PrintMessage("Invalid selection. Press any key to continue...");
                    PersistingUI.WaitForKey();
                    break;
            }
        }
    }

    private static async Task OptionStartNewThread(AIAgent agent)
    {
        // create a brand new thread, pass it to the unified handler, persist after
        var thread = agent.GetNewThread();
        thread = await RunQuestionWithThread(agent, thread, persistAfter: true);

        PersistingUI.PrintMessage(string.Empty);
        PersistingUI.PrintMessage("[press a key to go back to main menu]");
        PersistingUI.WaitForKey();
    }

    private static async Task OptionSimpleSession(AIAgent agent)
    {
        // create a temporary thread, do not persist after
        var tempThread = agent.GetNewThread();
        tempThread = await RunQuestionWithThread(agent, tempThread, persistAfter: false);

        PersistingUI.PrintMessage(string.Empty);
        PersistingUI.PrintMessage("[press a key to go back to main menu]");
        PersistingUI.WaitForKey();
    }

    private static async Task OptionLoadAndContinue(AIAgent agent)
    {
        if (!File.Exists(SavedThreadFilePath))
        {
            StreamConsoleHelper.PrintError($"No saved thread found at: {SavedThreadFilePath}");
            PersistingUI.PrintMessage("[press a key to go back to main menu]");
            PersistingUI.WaitForKey();
            return;
        }

        var loadedJson = await File.ReadAllTextAsync(SavedThreadFilePath);
        JsonElement reloaded = JsonSerializer.Deserialize<JsonElement>(loadedJson, JsonSerializerOptions.Web);

        // Rehydrate the thread for this agent
        var resumedThread = agent.DeserializeThread(reloaded, JsonSerializerOptions.Web);

        // Use the unified handler and persist after execution
        resumedThread = await RunQuestionWithThread(agent, resumedThread, persistAfter: true);

        PersistingUI.PrintMessage(string.Empty);
        PersistingUI.PrintMessage("[press a key to go back to main menu]");
        PersistingUI.WaitForKey();
    }

    /// <summary>
    /// Unified interaction function: prompts the user for a question, runs the agent using the provided thread,
    /// prints the response using StreamConsoleHelper, and optionally persists the thread to disk.
    /// The function returns the (possibly updated) AgentThread so callers can continue working with it.
    /// </summary>
    private static async Task<AgentThread> RunQuestionWithThread(AIAgent agent, AgentThread thread, bool persistAfter)
    {
        var question = PersistingUI.PromptInput("Enter your question: ");

        StreamConsoleHelper.PrintSection("Start answering your question");
        StreamConsoleHelper.StartAccumulatedStream();

        // Use the RunAsync convenience method to get a single response object tied to the thread
        var response = await agent.RunAsync(question, thread);

        // Print assembled text from the response
        StreamConsoleHelper.AccumulateAndPrint(response.Text);
        StreamConsoleHelper.FlushAccumulated();

        if (persistAfter)
        {
            // Serialize thread state and write to file
            var threadRaw = thread.Serialize(JsonSerializerOptions.Web).GetRawText();
            await File.WriteAllTextAsync(SavedThreadFilePath, threadRaw);
            StreamConsoleHelper.PrintLabeled("Saved thread to", SavedThreadFilePath);
        }

        return thread;
    }
}
