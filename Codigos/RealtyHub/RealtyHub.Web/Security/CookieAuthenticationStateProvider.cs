using Microsoft.AspNetCore.Components.Authorization;
using RealtyHub.Core.Models.Account;
using System.Net.Http.Json;
using System.Security.Claims;

namespace RealtyHub.Web.Security;

/// <summary>
/// Provedor de estado de autenticação baseado em cookies. Este provedor obtém
/// informações do usuário e seus respectivos claims a partir de uma API backend,
/// atualizando o estado de autenticação da aplicação.
/// </summary>
public class CookieAuthenticationStateProvider :
AuthenticationStateProvider,
ICookieAuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private bool _isAuthenticated;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="CookieAuthenticationStateProvider"/>
    /// utilizando a fábrica de <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="clientFactory">Fábrica para criar instâncias do HttpClient configuradas para o backend.</param>
    public CookieAuthenticationStateProvider(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient(Configuration.HttpClientName);
    }

    /// <summary>
    /// Verifica se o usuário está autenticado, atualizando o estado de autenticação.
    /// </summary>
    /// <returns>Task contendo um booleano indicando se o usuário está autenticado.</returns>
    public async Task<bool> CheckAuthenticatedAsync()
    {
        await GetAuthenticationStateAsync();
        return _isAuthenticated;
    }

    /// <summary>
    /// Notifica a aplicação que o estado de autenticação foi alterado.
    /// </summary>
    public void NotifyAuthenticationStateChanged() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    /// <summary>
    /// Obtém o estado atual de autenticação do usuário. Este método busca as informações do usuário
    /// a partir da API backend e, se bem-sucedido, constrói uma lista de claims para definir o estado autenticado.
    /// </summary>
    /// <returns>
    /// Task contendo um objeto <see cref="AuthenticationState"/> representando o estado do usuário.
    /// </returns>
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

    /// <summary>
    /// Tenta obter as informações do usuário a partir da API backend.
    /// </summary>
    /// <returns>
    /// Task contendo o objeto <see cref="User"/> se obtido com sucesso; caso contrário, null.
    /// </returns>
    private async Task<User?> GetUser()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<User?>("v1/identity/manage-info");
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Constrói uma lista de claims para o usuário com base nas informações obtidas e nos roles retornados pela API.
    /// </summary>
    /// <param name="user">Objeto <see cref="User"/> contendo as informações do usuário.</param>
    /// <returns>
    /// Task contendo uma lista de <see cref="Claim"/> representando as autorizações do usuário.
    /// </returns>
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