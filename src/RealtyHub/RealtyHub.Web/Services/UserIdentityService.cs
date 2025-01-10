using System.Text.Json;
using RealtyHub.Core.Models.Account;

namespace RealtyHub.Web.Services;

public class UserIdentityService(IHttpContextAccessor httpContextAccessor)
{
    public async Task<User?> GetUserAsync()
    {
        const string authCookieName = ".AspNetCore.Identity.Application";

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
            return null;

        if (!httpContext.Request.Cookies.TryGetValue(authCookieName, out var cookieValue))
        {
            return null;
        }

        var handler = new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = new System.Net.CookieContainer()
        };

        var baseUri = new Uri(Configuration.BackendUrl);

        handler.CookieContainer.SetCookies(baseUri, $"{authCookieName}={cookieValue}");

        using var client = new HttpClient(handler);
        client.BaseAddress = baseUri;

        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync("v1/identity/manage/info");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        var content = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<User>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return user;
    }
}
