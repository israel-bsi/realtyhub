using Microsoft.AspNetCore.Components.Authorization;
using RealtyHub.Core.Models.Account;
using System.Net.Http.Json;
using System.Security.Claims;

namespace RealtyHub.Web.Security;

public class CookieAuthenticationStateProvider :
    AuthenticationStateProvider, ICookieAuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private bool _isAuthenticated;

    public CookieAuthenticationStateProvider(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient(Configuration.HttpClientName);
    }

    public async Task<bool> CheckAuthenticatedAsync()
    {
        await GetAuthenticationStateAsync();
        return _isAuthenticated;
    }
    public void NotifyAuthenticationStateChanged() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _isAuthenticated = false;
        var user = new ClaimsPrincipal(new ClaimsIdentity());

        var userInfo = await GetUser();
        if (userInfo is null)
            return new AuthenticationState(user);

        var claims = await GetClaims(userInfo);

        var id = new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));
        user = new ClaimsPrincipal(id);

        _isAuthenticated = true;
        Configuration.GivenName = userInfo.GivenName;
        return new AuthenticationState(user);
    }

    private async Task<User?> GetUser()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<User?>("v1/identity/manageinfo");
        }
        catch
        {
            return null;
        }
    }

    private async Task<List<Claim>> GetClaims(User user)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.Name, user.Email),
            new (ClaimTypes.Email, user.Email),
            new ("Creci", user.Creci),
            new (ClaimTypes.GivenName, user.GivenName)
        };

        claims.AddRange(
            user.Claims
                .Where(x => x.Key != ClaimTypes.Name && x.Key != ClaimTypes.Email)
                .Select(x => new Claim(x.Key, x.Value))
        );

        RoleClaim[]? roles;
        try
        {
            roles = await _httpClient.GetFromJsonAsync<RoleClaim[]>("v1/identity/manage/roles");
        }
        catch
        {
            return claims;
        }

        foreach (var role in roles ?? [])
            if (!string.IsNullOrEmpty(role.Type) && !string.IsNullOrEmpty(role.Value))
                claims.Add(new Claim(role.Type, role.Value,
                    role.ValueType, role.Issuer, role.OriginalIssuer));

        return claims;
    }
}