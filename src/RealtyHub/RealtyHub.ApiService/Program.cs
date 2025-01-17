using System.Globalization;
using RealtyHub.ApiService;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Endpoints;

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

app.ApplyMigrations();

if (app.Environment.IsDevelopment())
    app.ConfigureDevEnvironment();

app.UseStaticFiles();
app.UseExceptionHandler();
app.UseCors(Configuration.CorsPolicyName);
app.UseSecurity();
app.MapEndpoints();

app.Run();

namespace RealtyHub.ApiService
{
    public partial class Program;
}