using FoundryLocal.Course;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Reflection;

if (args.SequenceEqual(["--server"]))
{
    // stdout belongs to MCP JSON-RPC, including when a parent captures the process.
    var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings { Args = [], DisableDefaults = true });
    builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
    builder.Services.AddMcpServer().WithStdioServerTransport().WithTools<LibraryTools>();
    await builder.Build().RunAsync();
    return;
}
string mode = FoundryCourseChecks.Mode(args, "run", "run", "protocol");
using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(3));
var transport = new StdioClientTransport(new StdioClientTransportOptions
{
    Name = "Local Foundry library",
    Command = "dotnet",
    Arguments = [Assembly.GetExecutingAssembly().Location, "--server"],
    StandardErrorLines = Console.Error.WriteLine
});
await using var mcp = await McpClient.CreateAsync(transport, cancellationToken: timeout.Token);
var tools = await mcp.ListToolsAsync(cancellationToken: timeout.Token);
Console.WriteLine($"Local MCP tools: {string.Join(", ", tools.Select(tool => tool.Name))}");
if (mode == "protocol")
{
    await mcp.CallToolAsync(tools.Single().Name, cancellationToken: timeout.Token);
    Console.WriteLine("Local MCP protocol round trip complete.");
    return;
}
await using var runtime = await FoundryCourseRuntime.StartAsync(args.Contains("--prepare"), tools: true);
using var client = runtime.CreateChatClient();
using var chat = client.AsBuilder()
    .UseFunctionInvocation(configure: c => c.MaximumIterationsPerRequest = 3).Build();
var result = await chat.GetResponseAsync(
    "Call GetLibraryHours and tell me when the library opens on weekdays.",
    new ChatOptions { Tools = [.. tools], ToolMode = ChatToolMode.RequireAny, Temperature = 0, MaxOutputTokens = 256 }, timeout.Token);
int calls = result.Messages.SelectMany(m => m.Contents).OfType<FunctionCallContent>()
    .Count(call => call.Name == tools.Single().Name);
Console.WriteLine($"Answer: {FoundryCourseChecks.RequireText(result.Text)}\nMCP tool calls: {calls}");
if (calls != 1 || !FoundryCourseChecks.ContainsOpeningTime(result.Text))
{
    throw new InvalidOperationException("Expected a real local MCP tool call and grounded opening time.");
}

[McpServerToolType]
internal sealed class LibraryTools
{
    [McpServerTool, Description("Get the library's weekday opening hours from fixed demo data.")]
    public static string GetLibraryHours()
    {
        Console.Error.WriteLine("Local MCP GetLibraryHours invoked.");
        return "The library opens at 9 AM on weekdays.";
    }
}
