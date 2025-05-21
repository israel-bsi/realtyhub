using Microsoft.AspNetCore.Components.Authorization;

namespace RealtyHub.Web.Security;

/// <summary>
/// Interface que define o contrato para um provedor de estado de autenticação baseado em cookies.
/// </summary>
public interface ICookieAuthenticationStateProvider
{
    /// <summary>
    /// Verifica se o usuário está autenticado.
    /// </summary>
    /// <returns>Task contendo um booleano indicando se o usuário está autenticado.</returns>
    Task<bool> CheckAuthenticatedAsync();

    /// <summary>
    /// Obtém o estado atual de autenticação do usuário.
    /// </summary>
    /// <returns>Task contendo o objeto <see cref="AuthenticationState"/> com o estado atual.</returns>
    Task<AuthenticationState> GetAuthenticationStateAsync();

    /// <summary>
    /// Notifica a aplicação que houve alteração no estado de autenticação.
    /// </summary>
    void NotifyAuthenticationStateChanged();
}