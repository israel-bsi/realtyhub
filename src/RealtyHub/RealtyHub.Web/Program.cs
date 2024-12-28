using RealtyHub.ServiceDefaults;
using RealtyHub.Web;
using RealtyHub.Web.Components;
using System.Globalization;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

builder.Services.AddHttpClient("", client => client.BaseAddress = new("http://apiservice"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
