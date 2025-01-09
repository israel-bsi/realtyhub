var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.RealtyHub_ApiService>("apiservice");

builder.AddProject<Projects.RealtyHub_Frontend>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
