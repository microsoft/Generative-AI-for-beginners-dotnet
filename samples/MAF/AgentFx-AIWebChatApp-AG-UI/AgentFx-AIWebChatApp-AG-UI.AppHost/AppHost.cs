var builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<IResourceWithConnectionString>? openai;

// Configure Azure resources for production
if (builder.ExecutionContext.IsPublishMode)
{
    // See https://learn.microsoft.com/dotnet/aspire/azure/local-provisioning#configuration
    // for instructions providing configuration values
    var aoai = builder.AddAzureOpenAI("openai");

    aoai.AddDeployment(
        name: "gpt-4o-mini",
        modelName: "gpt-4o-mini",
        modelVersion: "2024-07-18");

    aoai.AddDeployment(
        name: "text-embedding-3-small",
        modelName: "text-embedding-3-small",
        modelVersion: "1");

    openai = aoai;
}
else
{
    openai = builder.AddConnectionString("openai");
}

var agents = builder.AddProject<Projects.AgentFx_AIWebChatApp_AG_UI_Agents>("aichatweb-agents")
    .WithReference(openai)
    .WaitFor(openai);

var webApp = builder.AddProject<Projects.AgentFx_AIWebChatApp_AG_UI_Web>("aichatweb-app");
webApp
    .WithReference(openai)
    .WaitFor(openai)
    .WithReference(agents)
    .WaitFor(agents);

builder.Build().Run();
