using RealtyHub.ApiService;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Endpoints;
using System.Globalization;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

QuestPDF.Settings.License = LicenseType.Community;

builder.AddDirectories();
builder.Logging.AddConsole();
builder.AddConfiguration();
builder.AddSecurity();
builder.AddDataContexts();
builder.AddCrossOrigin();
builder.AddDocumentation();
builder.AddServices();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

app.ApplyMigrations();

app.UseSwagger();
app.UseSwaggerUI();
app.MapSwagger().RequireAuthorization();

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
    /// <summary>
    /// Representa o ponto de entrada principal para a aplicação.
    /// </summary>
    public partial class Program;
}