using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Services;
using System.Globalization;
using Blazored.LocalStorage;
using RealtyHub.Web;
using RealtyHub.Web.Handlers;
using RealtyHub.Web.Security;
using RealtyHub.Web.Services;
using MudBlazor.Translations;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<CookieHandler>();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
builder.Services.AddScoped(x =>
    (ICookieAuthenticationStateProvider)x.GetRequiredService<AuthenticationStateProvider>());
builder.Services.AddMudServices();
builder.Services.AddMudTranslations();
builder.Services.AddBlazoredLocalStorage();

builder.Services
    .AddHttpClient(Configuration.HttpClientName, opt => { opt.BaseAddress = new Uri(Configuration.BackendUrl); })
    .AddHttpMessageHandler<CookieHandler>();

builder.Services.AddTransient<IAccountHandler, AccountHandler>();
builder.Services.AddTransient<ICustomerHandler, CustomerHandler>();
builder.Services.AddTransient<IPropertyHandler, PropertyHandler>();
builder.Services.AddTransient<IPropertyImageHandler, PropertyImageHandler>();
builder.Services.AddTransient<IViaCepService, ViaCepService>();
builder.Services.AddTransient<DocumentValidator>();

builder.Services.AddLocalization();
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentUICulture;

await builder.Build().RunAsync();