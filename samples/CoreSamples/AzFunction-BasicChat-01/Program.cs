using Azure.AI.OpenAI;
using Azure.Core;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using AzFunction_BasicChat_01;
using Microsoft.Extensions.AI;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var openTelemetry = builder.Services.AddOpenTelemetry()
    .UseFunctionsWorkerDefaults();

if (!string.IsNullOrWhiteSpace(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
{
    openTelemetry.UseAzureMonitorExporter();
}

builder.Services
    .AddOptions<FoundryOptions>()
    .BindConfiguration(FoundryOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<TokenCredential, DefaultAzureCredential>();
builder.Services.AddSingleton<IChatClient>(services =>
{
    var options = services.GetRequiredService<IOptions<FoundryOptions>>().Value;
    var credential = services.GetRequiredService<TokenCredential>();

    return new AzureOpenAIClient(new Uri(options.Endpoint), credential)
        .GetChatClient(options.DeploymentName)
        .AsIChatClient();
});
builder.Services.AddSingleton<IChatResponder, FoundryChatResponder>();

builder.Build().Run();
