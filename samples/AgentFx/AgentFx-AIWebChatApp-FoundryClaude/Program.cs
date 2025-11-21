using elbruno.Extensions.AI.Claude;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using AgentFx_AIWebChatApp_FoundryClaude.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Load configuration
var config = builder.Configuration;
var endpointClaude = config["endpointClaude"] ?? throw new InvalidOperationException("Missing 'endpointClaude' configuration");
var apiKey = config["apikey"] ?? throw new InvalidOperationException("Missing 'apikey' configuration");
var deploymentName = config["deploymentName"] ?? "claude-haiku-4-5";

// Register IChatClient with Claude integration using elbruno.Extensions.AI.Claude
builder.Services.AddSingleton<IChatClient>(_ =>
{
    return new AzureClaudeClient(
        endpoint: new Uri(endpointClaude),
        modelId: deploymentName,
        apiKey: apiKey);
});

// Register AI Agent with dependency injection
builder.Services.AddSingleton<AIAgent>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Configuring AI Agent with Claude model '{Model}'", deploymentName);

    var chatClient = sp.GetRequiredService<IChatClient>();
    return chatClient.CreateAIAgent(
        name: "ClaudeChat",
        instructions: "You are a helpful and friendly AI assistant."
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
