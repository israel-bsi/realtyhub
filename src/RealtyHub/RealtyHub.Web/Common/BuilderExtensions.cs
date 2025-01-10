using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Services;
using RealtyHub.ServiceDefaults;
using RealtyHub.Web.Handlers;
using RealtyHub.Web.Security;
using RealtyHub.Web.Services;

namespace RealtyHub.Web.Common;

public static class BuilderExtensions
{

    public static void ConfigureSecurity(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<CookieHandler>();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
        builder.Services.AddScoped(x => (ICookieAuthenticationStateProvider)x.GetRequiredService<AuthenticationStateProvider>());
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<UserIdentityService>();
    }

    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.AddServiceDefaults();
        builder.Services.AddRazorPages();
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddMudServices();

        builder.Services.AddTransient<IAccountHandler, AccountHandler>();
        builder.Services.AddTransient<ICustomerHandler, CustomerHandler>();
        builder.Services.AddTransient<IViaCepService, ViaCepService>();
        builder.Services.AddLocalization();
    }

    public static void ConfigureHttpClient(this WebApplicationBuilder builder)
    {
        //Configuration.BackendUrl = builder.Configuration.GetValue<string>("BackendUrl") ?? string.Empty;
        Configuration.BackendUrl = "http://apiservice";
        builder.Services.AddHttpClient(Configuration.HttpClientName, client =>
        {
            client.BaseAddress = new Uri(Configuration.BackendUrl);
        }).ConfigurePrimaryHttpMessageHandler(() =>
            new HttpClientHandler
            {
                UseCookies = true,
                AllowAutoRedirect = true
            }
        );
    }
}