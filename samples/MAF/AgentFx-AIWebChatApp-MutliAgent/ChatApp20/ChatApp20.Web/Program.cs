using ChatApp20.Web.Components;
using ChatApp20.Web.Services;
using ChatApp20.Web.Services.Ingestion;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;

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
    .UseOpenTelemetry(configure: c =>
        c.EnableSensitiveData = builder.Environment.IsDevelopment())
    .Build();
    return aiAgent;
});

// Register a research agent
builder.AddAIAgent("ResearchAgent", (sp, key) =>
{
    var chatClient = sp.GetRequiredService<IChatClient>();
    var searchFunctions = sp.GetRequiredService<SearchFunctions>();
    
    return chatClient.CreateAIAgent(
        name: "ResearchAgent",
        instructions: @"You are a research specialist. Find and summarize information from documents.
        Use the search tool to find relevant information. When you do this, end your
        reply with citations in the special XML format:

        <citation filename='string' page_number='number'>exact quote here</citation>
        Always include the citation in your response if there are results.

        The quote must be max 5 words, taken word-for-word from the search result, and is the basis for why the citation is relevant.
        Don't refer to the presence of citations; just emit these tags right at the end, with no surrounding text.",
        tools: [AIFunctionFactory.Create(searchFunctions.SearchAsync)]
    )
    .AsBuilder()
    .UseOpenTelemetry(configure: c =>
        c.EnableSensitiveData = builder.Environment.IsDevelopment())
    .Build();
});

// Register a writing agent
builder.AddAIAgent("WritingAgent", (sp, key) =>
{
    var chatClient = sp.GetRequiredService<IChatClient>();
    
    return chatClient.CreateAIAgent(
        name: "WritingAgent",
        instructions: @"You are a writing specialist. Take information and create well-structured, engaging content.
        When you find citations, validate that they follow this special XML format:

        <citation filename='string' page_number='number'>exact quote here</citation>
        Fix the format if it's invalid"
    )
    .AsBuilder()
    .UseOpenTelemetry(configure: c =>
        c.EnableSensitiveData = builder.Environment.IsDevelopment())
    .Build();
});

// Register a coordinator agent that uses both
builder.AddAIAgent("CoordinatorAgent", (sp, key) =>
{
    var chatClient = sp.GetRequiredService<IChatClient>();
    var researchAgent = sp.GetRequiredKeyedService<AIAgent>("ResearchAgent");
    var writingAgent = sp.GetRequiredKeyedService<AIAgent>("WritingAgent");
    
    // Create functions that delegate to other agents
    async Task<string> ResearchAsync(string topic)
    {
        var messages = new[] { new ChatMessage(ChatRole.User, topic) };
        var result = await researchAgent.RunAsync(messages);
        return result.Text ?? "";
    }
    
    async Task<string> WriteAsync(string content)
    {
        var messages = new[] { new ChatMessage(ChatRole.User, $"Write an article based on: {content}") };
        var result = await writingAgent.RunAsync(messages);
        return result.Text ?? "";
    }
    
    return chatClient.CreateAIAgent(
        name: "CoordinatorAgent",
        instructions: @"You coordinate research and writing to create comprehensive articles.
        Do not answer questions about anything else.
        Use only simple markdown to format your responses.
        If the agents response include citations, include them in the special XML format:

        <citation filename='string' page_number='number'>exact quote here</citation>
        Always include the citation in your response if there are results.",
        tools: [
            AIFunctionFactory.Create(ResearchAsync),
            AIFunctionFactory.Create(WriteAsync)
        ]
    )
    .AsBuilder()
    .UseOpenTelemetry(configure: c =>
        c.EnableSensitiveData = builder.Environment.IsDevelopment())
    .Build();
});

openai.AddEmbeddingGenerator("text-embedding-3-small");

var vectorStorePath = Path.Combine(AppContext.BaseDirectory, "vector-store.db");
var vectorStoreConnectionString = $"Data Source={vectorStorePath}";
builder.Services.AddSqliteCollection<string, IngestedChunk>("data-chatapp20-chunks", vectorStoreConnectionString);
builder.Services.AddSqliteCollection<string, IngestedDocument>("data-chatapp20-documents", vectorStoreConnectionString);
builder.Services.AddScoped<DataIngestor>();
builder.Services.AddSingleton<SemanticSearch>();

// Added DI registration for SearchFunctions to expose AI callable search tool via injected SemanticSearch
builder.Services.AddSingleton<SearchFunctions>();

// Register services for OpenAI responses and conversations (also required for DevUI)
builder.Services.AddOpenAIResponses();
builder.Services.AddOpenAIConversations();

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

// Map endpoints for OpenAI responses and conversations (also required for DevUI)
app.MapOpenAIResponses();
app.MapOpenAIConversations();

if (builder.Environment.IsDevelopment())
{
    // Map DevUI endpoint to /devui
    app.MapDevUI();
}

app.Run();