using ChatApp20.Web.Components;
using ChatApp20.Web.Services;
using ChatApp20.Web.Services.Ingestion;
using ElBruno.Connectors.SqliteVec;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var openai = builder.AddAzureOpenAIClient("openai");
openai.AddChatClient("gpt-5-mini")
    .UseFunctionInvocation()
    .UseOpenTelemetry(configure: c =>
        c.EnableSensitiveData = builder.Environment.IsDevelopment());

builder.AddAIAgent("ChatAgent", (sp, key) =>
{
    // get logger
    var logger = sp.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Configuring AI Agent with key '{Key}' for model '{Model}'", key, "gpt-5-mini");

    // get tools
    var searchFunctions = sp.GetRequiredService<SearchFunctions>();

    // create agent
    var chatClient = sp.GetRequiredService<IChatClient>();
    var aiAgent = chatClient.CreateAIAgent(
        name: key,
        instructions: "You are an useful agent that helps users with short and funny answers.",
        description: "An AI agent that helps users with short and funny answers.",
        tools: [AIFunctionFactory.Create(searchFunctions.SearchAsync)]
        )
    .AsBuilder()
    .Use((agent, context, next, cancellationToken) =>
        CustomFunctionCallingMiddleware(agent, context, next, cancellationToken, sp.GetRequiredService<ILogger<Program>>()))
    .UseOpenTelemetry(configure: c =>
        c.EnableSensitiveData = builder.Environment.IsDevelopment())
    .Build();
    return aiAgent;
});

openai.AddEmbeddingGenerator("text-embedding-3-small");

var vectorStorePath = Path.Combine(AppContext.BaseDirectory, "vector-store.db");
var vectorStoreConnectionString = $"Data Source={vectorStorePath}";
builder.Services.AddSqliteVecCollection<string, IngestedChunk>("data-chatapp20-chunks", vectorStoreConnectionString);
builder.Services.AddSqliteVecCollection<string, IngestedDocument>("data-chatapp20-documents", vectorStoreConnectionString);
builder.Services.AddScoped<DataIngestor>();
builder.Services.AddSingleton<SemanticSearch>();

// Added DI registration for SearchFunctions to expose AI callable search tool via injected SemanticSearch
builder.Services.AddSingleton<SearchFunctions>();

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

app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// By default, we ingest PDF files from the /wwwroot/Data directory. You can ingest from
// other sources by implementing IIngestionSource.
// Important: ensure that any content you ingest is trusted, as it may be reflected back
// to users or could be a source of prompt injection risk.
await DataIngestor.IngestDataAsync(
    app.Services,
    new PDFDirectorySource(Path.Combine(builder.Environment.WebRootPath, "Data")));

app.Run();

static async ValueTask<object?> CustomFunctionCallingMiddleware(
    AIAgent agent,
    FunctionInvocationContext context,
    Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next,
    CancellationToken cancellationToken,
    ILogger logger)
{
    logger.LogInformation("Function Name: {FunctionName}", context!.Function.Name);
    var result = await next(context, cancellationToken);
    logger.LogInformation("Function Call Result: {Result}", result);

    return result;
}