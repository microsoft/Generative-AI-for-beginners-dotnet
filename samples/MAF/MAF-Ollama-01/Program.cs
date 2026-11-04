using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System.ComponentModel;

string mode = args.FirstOrDefault() ?? "story";
if (args.Length > 1 || mode is not ("story" or "tools" or "workflow"))
{
    throw new ArgumentException("Usage: dotnet run -- [story|tools|workflow]");
}
using var httpClient = new HttpClient
{
    BaseAddress = new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434"),
    Timeout = TimeSpan.FromMinutes(2)
};
using IChatClient chatClient =
    new OllamaApiClient(httpClient, Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "llama3.2");
using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(3));

AIAgent writer = chatClient.AsAIAgent(
    name: "Writer",
    instructions: "Write short stories that are engaging and creative, and always add bad jokes to them.");

if (mode == "tools")
{
    int calls = 0;
    AIAgent librarian = chatClient.AsAIAgent(
        name: "Librarian",
        instructions: "Use GetLibraryHours for library opening hours. Do not invent opening times.",
        tools: [AIFunctionFactory.Create(GetLibraryHours, nameof(GetLibraryHours))]);
    var answer = await librarian.RunAsync("Call GetLibraryHours and tell me when the library opens.", cancellationToken: timeout.Token);
    if (string.IsNullOrWhiteSpace(answer.Text))
    {
        throw new InvalidOperationException("The agent returned no answer.");
    }
    Console.WriteLine(answer.Text);
    Console.WriteLine($"Library tool calls: {calls}");
    if (calls == 0)
    {
        throw new InvalidOperationException("The model did not invoke the library tool. Use a tool-capable model such as llama3.2.");
    }

    [Description("Get the library's weekday opening hours from fixed demo data.")]
    string GetLibraryHours()
    {
        calls++;
        Console.WriteLine("GetLibraryHours function invoked.");
        return "The library opens at 9 AM on weekdays.";
    }
}
else
{
    AgentResponse response = await writer.RunAsync(
        "Write a short story about Lima, Peru in Spanish. Keep it under 100 words.", cancellationToken: timeout.Token);
    if (string.IsNullOrWhiteSpace(response.Text))
    {
        throw new InvalidOperationException("The writer returned no story.");
    }
    Console.WriteLine($"Writer: {response.Text}");
    if (mode == "workflow")
    {
        // Explicit sequential orchestration: only the writer's output goes to the editor.
        AIAgent editor = chatClient.AsAIAgent(
            name: "Editor", instructions: "Edit the supplied story for clarity. Keep it in Spanish and under 100 words.");
        var edited = await editor.RunAsync(response.Text, cancellationToken: timeout.Token);
        if (string.IsNullOrWhiteSpace(edited.Text))
        {
            throw new InvalidOperationException("The editor returned no story.");
        }
        Console.WriteLine($"Editor: {edited.Text}");
    }
}