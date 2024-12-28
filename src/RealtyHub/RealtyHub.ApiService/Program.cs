using RealtyHub.ApiService;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Endpoints;
using RealtyHub.ServiceDefaults;
using System.Globalization;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
builder.AddConfiguration();
builder.AddSecurity();
builder.AddDataContexts();
builder.AddCrossOrigin();
builder.AddDocumentation();
builder.AddServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.ConfigureDevEnvironment();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseCors(ApiConfiguration.CorsPolicyName);
app.UseSecurity();
app.MapEndpoints();
app.MapDefaultEndpoints();

app.Run();

public partial class Program {}