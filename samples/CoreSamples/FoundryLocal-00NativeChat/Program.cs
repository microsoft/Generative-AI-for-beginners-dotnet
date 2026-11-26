using FoundryLocal.Course;
using Microsoft.AI.Foundry.Local;

FoundryCourseChecks.Mode(args, "hello", "hello");
await using var runtime = await FoundryCourseRuntime.StartAsync(args.Contains("--prepare"), server: false);
using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(3));
using var session = new ChatSession(runtime.ChatModel);
using var request = new Request();
request.AddItem(MessageItem.User("What is 2 + 2? Reply with only the number."));
request.SetOptions(new RequestOptions
{
    Search = new SearchOptions { Temperature = 0, MaxOutputTokens = 64 },
    ToolChoice = ToolChoice.None
});
using var response = await session.ProcessRequestAsync(request, timeout.Token);
var parts = new List<string>();
foreach (var item in response)
{
    using (item)
    {
        if (item is MessageItem message)
        {
            parts.Add(message.GetSimpleText());
        }
    }
}
string answer = FoundryCourseChecks.RequireText(string.Join("", parts));
Console.WriteLine($"Native answer: {answer}");
if (answer.Trim() != "4")
{
    throw new InvalidOperationException("Expected 4. The selected model failed the Hello World quality check.");
}
