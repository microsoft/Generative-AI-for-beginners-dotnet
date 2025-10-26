using HFMCP.GenImage.Web;
using HFMCP.GenImage.Web.Components;
using HFMCP.GenImage.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Data Protection for secure configuration storage
builder.Services.AddDataProtection();

// Register AI services
builder.Services.AddScoped<IAIConfigurationService, AIConfigurationService>();
builder.Services.AddScoped<IMCPService, MCPService>();
builder.Services.AddHttpClient<IAIService, AIService>();

var app = builder.Build();

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

app.MapDefaultEndpoints();

app.Run();
