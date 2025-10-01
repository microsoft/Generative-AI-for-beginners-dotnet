using ModelContextProtocol;

var builder = WebApplication.CreateBuilder(args);

// Add default services
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();

// add MCP server
//builder.Services.AddMcpServer().WithTools();
builder.Services.AddMcpServer().WithToolsFromAssembly();
var app = builder.Build();

// Initialize default endpoints
app.MapDefaultEndpoints();
app.UseHttpsRedirection();

// map endpoints
app.MapGet("/", () => $"Hello World! {DateTime.Now}");

app.Run();
