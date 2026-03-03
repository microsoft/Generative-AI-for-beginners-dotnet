using MAF_AIWebChatApp_Persisting.Web.Components;
using MAF_AIWebChatApp_Persisting.Web.Services;
using MAF_AIWebChatApp_Persisting.Web.Services.Ingestion;
using ElBruno.Connectors.SqliteVec;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;

const string systemPrompt = @"You are an assistant who answers general questions and also help the user with information you retrieve.
        
        Use only simple markdown to format your responses.

        Use the search tool to find relevant information. When you do this, end your
        reply with citations in the special XML format:

        <citation filename='string' page_number='number'>exact quote here</citation>

        Always include the citation in your response if there are results.

        The quote must be max5 words, taken word-for-word from the search result, and is the basis for why the citation is relevant.
        Don't refer to the presence of citations; just emit these tags right at the end, with no surrounding text.";

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Configure Azure OpenAI (name matches the Aspire resource key; keep existing deployment name "aifoundry")
var openai = builder.AddAzureOpenAIClient("aifoundry");
openai.AddChatClient("gpt-5-mini")
 .UseFunctionInvocation()
 .UseOpenTelemetry(configure: c =>
 c.EnableSensitiveData = builder.Environment.IsDevelopment());
openai.AddEmbeddingGenerator("text-embedding-3-small");

// Register vector storage & ingestion services
var vectorStorePath = Path.Combine(AppContext.BaseDirectory, "vector-store.db");
var vectorStoreConnectionString = $"Data Source={vectorStorePath}";
builder.Services.AddSqliteVecCollection<string, IngestedChunk>("data-chatapp21-chunks", vectorStoreConnectionString);
builder.Services.AddSqliteVecCollection<string, IngestedDocument>("data-chatapp21-documents", vectorStoreConnectionString);
builder.Services.AddScoped<DataIngestor>();
builder.Services.AddSingleton<SemanticSearch>();

// Register search functions for DI
builder.Services.AddSingleton<SearchFunctions>();
// Register thread persistence helper
builder.Services.AddSingleton<ChatThreadStore>();

// Register AI Agent using Agent Framework
builder.AddAIAgent("ChatAgent", (sp, key) =>
{
    var searchFunctions = sp.GetRequiredService<SearchFunctions>();
    var chatClient = sp.GetRequiredService<IChatClient>();

    var agent = chatClient.CreateAIAgent(
    name: key,
    instructions: systemPrompt,
    description: "Helpful agent",
    tools: [AIFunctionFactory.Create(searchFunctions.SearchAsync)]
    )
    .AsBuilder()
    .UseOpenTelemetry(configure: c => c.EnableSensitiveData = builder.Environment.IsDevelopment())
    .Build();

    return agent;
});

var app = builder.Build();

app.MapDefaultEndpoints();

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

// Ingest PDF files on startup
await DataIngestor.IngestDataAsync(
 app.Services,
 new PDFDirectorySource(Path.Combine(builder.Environment.WebRootPath, "Data")));

app.Run();
