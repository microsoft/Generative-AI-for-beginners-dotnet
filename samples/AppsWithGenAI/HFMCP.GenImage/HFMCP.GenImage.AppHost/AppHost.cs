var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.HFMCP_GenImage_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.HFMCP_GenImage_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
