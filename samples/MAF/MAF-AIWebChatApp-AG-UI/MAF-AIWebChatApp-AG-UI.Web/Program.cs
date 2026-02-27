using MAF_AIWebChatApp_AG_UI.Web.Components;
using MAF_AIWebChatApp_AG_UI.Web.Services;
using MAF_AIWebChatApp_AG_UI.Web.Services.Ingestion;
using ElBruno.Connectors.SqliteVec;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.AGUI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// register chat and embedding clients with OpenAI
var openai = builder.AddAzureOpenAIClient("openai");
openai.AddChatClient("gpt-5-mini")
    .UseFunctionInvocation()
    .UseOpenTelemetry(configure: c =>
        c.EnableSensitiveData = builder.Environment.IsDevelopment());
openai.AddEmbeddingGenerator("text-embedding-3-small");


// create the Agent Service
builder.Services.AddHttpClient<AgentsService>(
    static client => client.BaseAddress = new("https+http://aichatweb-agents"));

// Register the AI Agent using the Agent Framework and AG-UI
//builder.AddAIAgent("ChatAgent", (sp, key) =>
builder.Services.AddActivatedKeyedSingleton<AIAgent>("ChatAgent", (sp, key) =>
{
    var logger = sp.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Configuring AI Agent with key '{Key}' for model '{Model}'", key, "gpt-4o-mini");

    // Create frontend tools
    var searchFunctions = sp.GetRequiredService<SearchFunctions>();
    AITool[] frontendTools = [AIFunctionFactory.Create(searchFunctions.SearchAsync)];


    var agentService = sp.GetRequiredService<AgentsService>();
    AGUIChatClient chatClient = new(agentService.HttpClient, "https+http://aichatweb-agents");

    // Create and configure the AI agent
    var aiAgent = chatClient.CreateAIAgent(
        name: key.ToString(),
        instructions: "You are a useful agent that helps users with short and funny answers.",
        description: "An AI agent that helps users with short and funny answers.",
        tools: frontendTools)
    .AsBuilder()
    .UseOpenTelemetry(configure: c =>
        c.EnableSensitiveData = builder.Environment.IsDevelopment())
    .Build();

    return aiAgent;
});



var vectorStorePath = Path.Combine(AppContext.BaseDirectory, "vector-store.db");
var vectorStoreConnectionString = $"Data Source={vectorStorePath}";
builder.Services.AddSqliteVecCollection<string, IngestedChunk>("data-MAF-aiwebchatapp-ag-ui-chunks", vectorStoreConnectionString);
builder.Services.AddSqliteVecCollection<string, IngestedDocument>("data-MAF-aiwebchatapp-ag-ui-documents", vectorStoreConnectionString);
builder.Services.AddScoped<DataIngestor>();
builder.Services.AddSingleton<SemanticSearch>();

// Register SearchFunctions for DI injection into the agent
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
