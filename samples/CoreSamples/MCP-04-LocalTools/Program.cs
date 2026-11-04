using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Server;
using OllamaSharp;
using System.ComponentModel;
using System.Reflection;

if (args.SequenceEqual(["--server"]))
{
    // stdout is reserved for MCP JSON-RPC messages, not application logging.
    var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
    {
        Args = [],
        DisableDefaults = true
    });
    builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
    builder.Services.AddMcpServer().WithStdioServerTransport().WithTools<LibraryTools>();
    await builder.Build().RunAsync();
    return;
}
if (args.Length != 0)
{
    throw new ArgumentException("Usage: dotnet run (the --server mode is started by the client)");
}
using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(2));
var transport = new StdioClientTransport(new StdioClientTransportOptions
{
    Name = "Local library",
    Command = "dotnet",
    Arguments = [Assembly.GetExecutingAssembly().Location, "--server"],
    StandardErrorLines = line => Console.Error.WriteLine(line)
});
await using var mcp = await McpClient.CreateAsync(transport, cancellationToken: timeout.Token);
var tools = await mcp.ListToolsAsync(cancellationToken: timeout.Token);
Console.WriteLine($"Local MCP tools: {string.Join(", ", tools.Select(tool => tool.Name))}");

using var httpClient = new HttpClient
{
    BaseAddress = new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434"),
    Timeout = TimeSpan.FromMinutes(2)
};
using IChatClient chat = ((IChatClient)new OllamaApiClient(
        httpClient, Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "llama3.2"))
    .AsBuilder().UseFunctionInvocation().Build();
var result = await chat.GetResponseAsync(
    "Call GetLibraryHours and tell me when the library opens on weekdays.",
    new ChatOptions { Tools = [.. tools], Temperature = 0 }, timeout.Token);
Console.WriteLine($"Answer: {result.Text}");
int calls = result.Messages.SelectMany(message => message.Contents)
    .OfType<FunctionCallContent>().Count(call => call.Name == tools.Single().Name);
Console.WriteLine($"MCP tool calls: {calls}");
if (calls == 0)
{
    throw new InvalidOperationException("The model did not invoke the local MCP tool. Use a tool-capable model such as llama3.2.");
}
if (string.IsNullOrWhiteSpace(result.Text))
{
    throw new InvalidOperationException("The model returned no answer.");
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
