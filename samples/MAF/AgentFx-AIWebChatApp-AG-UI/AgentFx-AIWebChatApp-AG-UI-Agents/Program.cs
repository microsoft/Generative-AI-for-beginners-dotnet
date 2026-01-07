using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var openai = builder.AddAzureOpenAIClient("openai");
openai.AddChatClient("gpt-4o-mini")
    .UseFunctionInvocation()
    .UseOpenTelemetry(configure: c =>
        c.EnableSensitiveData = builder.Environment.IsDevelopment());

// Register the AI Agent using the Agent Framework
builder.AddAIAgent("ChatAgent", (sp, key) =>
{
    // Get required services
    var logger = sp.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Configuring AI Agent with key '{Key}' for model '{Model}'", key, "gpt-4o-mini");
    var chatClient = sp.GetRequiredService<IChatClient>();

    // Create and configure the AI agent
    var aiAgent = chatClient.CreateAIAgent(
        name: key,
        instructions: "You are a useful agent that helps users with short and funny answers.",
        description: "An AI agent that helps users with short and funny answers."
        )
    .AsBuilder()
    .UseOpenTelemetry(configure: c =>
        c.EnableSensitiveData = builder.Environment.IsDevelopment())
    .Build();

    return aiAgent;
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Map the AG-UI agent endpoint
var aiAgent = app.Services.GetKeyedService<AIAgent>("ChatAgent");
app.MapAGUI("/", aiAgent);

app.Run();
