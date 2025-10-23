using McpSample.BlazorChat;
using McpSample.BlazorChat.Components;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

var builder = WebApplication.CreateBuilder(args);

// add aspire service defaults
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add configuration service
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Add logging service
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

builder.Services.AddSingleton<ILogger>(sp => sp.GetRequiredService<ILogger<Program>>());

// add MCP client
builder.Services.AddSingleton<IMcpClient>(sp =>
{
    var clientTransport = new SseClientTransport(
        new()
        {
            Name = "AspNetCore Server",
            Endpoint = new Uri("https://localhost:7133"), // MCP server endpoint
            TransportMode = HttpTransportMode.StreamableHttp
        });

    var mcpClient = McpClientFactory.CreateAsync(clientTransport).GetAwaiter().GetResult();
    return mcpClient;
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
