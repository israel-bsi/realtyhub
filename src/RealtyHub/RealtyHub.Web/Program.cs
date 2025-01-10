using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using RealtyHub.Core.Handlers;
using RealtyHub.ServiceDefaults;
using System.Globalization;
using System.Net;
using Blazored.LocalStorage;
using RealtyHub.Core.Services;
using RealtyHub.Web;
using RealtyHub.Web.Handlers;
using RealtyHub.Web.Security;
using RealtyHub.Web.Services;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

var builder = WebApplication.CreateBuilder(args);

Configuration.BackendUrl = builder.Configuration.GetValue<string>("BackendUrl") ?? string.Empty;
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddServerSideBlazor();
builder.Services.AddOutputCache();
builder.Services.AddScoped<CookieHandler>();
builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
builder.Services.AddScoped(x => (ICookieAuthenticationStateProvider)x.GetRequiredService<AuthenticationStateProvider>());
builder.Services.AddAuthorizationCore();
builder.Services.AddMudServices();
builder.Services.AddLocalization();
builder.Services.AddTransient<IAccountHandler, AccountHandler>();
builder.Services.AddTransient<ICustomerHandler, CustomerHandler>();
builder.Services.AddTransient<IViaCepService, ViaCepService>();

builder.Services.AddHttpClient(Configuration.HttpClientName, client =>
    {
        //client.BaseAddress = new Uri("http://apiservice");
        client.BaseAddress = new Uri(Configuration.BackendUrl);
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseDefaultCredentials = true,
        CookieContainer = new CookieContainer(),
        UseCookies = true,
    })
    .AddHttpMessageHandler<CookieHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseOutputCache();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapDefaultEndpoints();

app.Run();