using FoundryLocal.Course;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.ComponentModel;

string mode = FoundryCourseChecks.Mode(args, "story", "story", "tools", "workflow");
await using var runtime = await FoundryCourseRuntime.StartAsync(args.Contains("--prepare"), tools: mode == "tools");
using var chat = runtime.CreateChatClient();
using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(4));
if (mode == "tools")
{
    int calls = 0;
    [Description("Get the library's weekday opening hours from fixed demo data.")]
    string GetLibraryHours()
    {
        calls++;
        Console.WriteLine("Agent GetLibraryHours invoked.");
        return "The library opens at 9 AM on weekdays.";
    }
    using var bounded = chat.AsBuilder()
        .UseFunctionInvocation(configure: c => c.MaximumIterationsPerRequest = 3).Build();
    AIAgent librarian = bounded.AsAIAgent(new ChatClientAgentOptions
    {
        Name = "Librarian",
        UseProvidedChatClientAsIs = true,
        ChatOptions = new ChatOptions
        {
            Instructions = "Use the supplied tool for library hours. Never invent opening times.",
            Temperature = 0,
            MaxOutputTokens = 256,
            ToolMode = ChatToolMode.RequireAny,
            Tools = [AIFunctionFactory.Create(GetLibraryHours, "GetLibraryHours")]
        }
    });
    var answer = await librarian.RunAsync("When does the library open on weekdays?", cancellationToken: timeout.Token);
    Console.WriteLine($"Answer: {FoundryCourseChecks.RequireText(answer.Text)}\nAgent tool calls: {calls}");
    if (calls != 1 || !FoundryCourseChecks.ContainsOpeningTime(answer.Text))
    {
        throw new InvalidOperationException("Expected one real agent tool call and a grounded opening time.");
    }
}
else
{
    AIAgent writer = chat.AsAIAgent(new ChatClientAgentOptions
    {
        Name = "Writer",
        ChatOptions = new ChatOptions
        {
            Instructions = "Write a short story in Spanish, under 80 words.",
            Temperature = 0,
            MaxOutputTokens = 256,
            ToolMode = ChatToolMode.None
        }
    });
    var draft = await writer.RunAsync("Write a story about a friendly robot visiting Lima.", cancellationToken: timeout.Token);
    Console.WriteLine($"Writer: {FoundryCourseChecks.RequireText(draft.Text)}");
    if (mode == "workflow")
    {
        AIAgent editor = chat.AsAIAgent(new ChatClientAgentOptions
        {
            Name = "Editor",
            ChatOptions = new ChatOptions
            {
                Instructions = "Edit the supplied story for clarity. Keep Spanish and stay under 80 words.",
                Temperature = 0,
                MaxOutputTokens = 256,
                ToolMode = ChatToolMode.None
            }
        });
        var edited = await editor.RunAsync(draft.Text, cancellationToken: timeout.Token);
        Console.WriteLine($"Editor: {FoundryCourseChecks.RequireText(edited.Text)}");
    }
}
