var builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<IResourceWithConnectionString>? aifoundrymodels;

// Configure Azure resources for production
if (builder.ExecutionContext.IsPublishMode)
{
    // See https://learn.microsoft.com/dotnet/aspire/azure/local-provisioning#configuration
    // for instructions providing configuration values
    var aoai = builder.AddAzureOpenAI("aifoundry");

    aoai.AddDeployment(
        name: "gpt-4o-mini",
        modelName: "gpt-4o-mini",
        modelVersion: "2024-07-18");

    aoai.AddDeployment(
        name: "text-embedding-3-small",
        modelName: "text-embedding-3-small",
        modelVersion: "1");

    aifoundrymodels = aoai;
}
else
{
    aifoundrymodels = builder.AddConnectionString("aifoundry");
}

var webApp = builder.AddProject<Projects.MAF_AIWebChatApp_Persisting_Web>("aichatweb-app");
webApp
    .WithReference(aifoundrymodels)
    .WaitFor(aifoundrymodels);

builder.Build().Run();
