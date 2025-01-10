using System.Globalization;
using RealtyHub.Web.Common;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();
builder.ConfigureSecurity();
builder.ConfigureHttpClient();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

app.Run();