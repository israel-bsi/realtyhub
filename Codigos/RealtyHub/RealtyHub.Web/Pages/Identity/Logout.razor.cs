using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Web.Security;

namespace RealtyHub.Web.Pages.Identity;

/// <summary>
/// Página responsável por gerenciar o logout do usuário na aplicação.
/// </summary>
public partial class LogoutPage : ComponentBase
{
    #region Services

    /// <summary>
    /// Serviço para exibição de notificações na tela.
    /// </summary>
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    /// <summary>
    /// Handler responsável pelas operações de conta, como logout.
    /// </summary>
    [Inject]
    public IAccountHandler Handler { get; set; } = null!;

    /// <summary>
    /// Serviço para gerenciamento de navegação entre páginas.
    /// </summary>
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    /// <summary>
    /// Provedor do estado de autenticação baseado em cookies.
    /// </summary>
    [Inject]
    public ICookieAuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    #endregion

    #region Overrides

    /// <summary>
    /// Método chamado durante a inicialização do componente.
    /// </summary>
    /// <remarks>
    /// Verifica se o usuário está autenticado. Se sim, executa o logout 
    /// e atualiza o estado de autenticação, notificando a aplicação da mudança.
    /// </remarks>
    protected override async Task OnInitializedAsync()
    {
        if (await AuthenticationStateProvider.CheckAuthenticatedAsync())
        {
            await Handler.LogoutAsync();
            await AuthenticationStateProvider.GetAuthenticationStateAsync();
            AuthenticationStateProvider.NotifyAuthenticationStateChanged();
        }
        await base.OnInitializedAsync();
    }

    #endregion
}