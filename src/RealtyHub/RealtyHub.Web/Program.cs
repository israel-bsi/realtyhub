using RealtyHub.ServiceDefaults;
using RealtyHub.Web.Components;
using System.Globalization;
using System.Net;
using RealtyHub.Core.Services;
using RealtyHub.Web;
using RealtyHub.Web.Security;
using RealtyHub.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

builder.Services.AddTransient<IViaCepService, ViaCepService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();
builder.Services.AddScoped<CookieHandler>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
builder.Services.AddScoped(x => (ICookieAuthenticationStateProvider)x.GetRequiredService<AuthenticationStateProvider>());
builder.Services.AddMudServices();
builder.Services.AddLocalization();

builder.Services.AddHttpClient(Configuration.HttpClientName, client =>
{
    client.BaseAddress = new("http://apiservice");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    UseDefaultCredentials = true,
    CookieContainer = new CookieContainer(),
    UseCookies = true
})
.AddHttpMessageHandler<CookieHandler>();


var app = builder.Build();
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

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