var builder = DistributedApplication.CreateBuilder(args);

var aspnetserver = builder
    .AddProject<Projects.McpSample_AspNetCoreServer>("aspnetserver")
    .WithExternalHttpEndpoints();
var blazorchat = builder
    .AddProject<Projects.McpSample_BlazorChat>("blazorchat")
    .WithReference(aspnetserver)
    .WithExternalHttpEndpoints();

builder.Build().Run();