using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Requests.Account;
using RealtyHub.Web.Security;

namespace RealtyHub.Web.Pages.Identity;

/// <summary>
/// Página responsável por gerenciar o login do usuário na aplicação.
/// </summary>
public partial class LoginPage : ComponentBase
{
    #region Services

    /// <summary>
    /// Serviço para exibição de notificações na tela.
    /// </summary>
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    /// <summary>
    /// Handler responsável pelas operações de conta, como login.
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

    #region Properties

    /// <summary>
    /// Modelo utilizado para vincular os dados de login.
    /// </summary>
    public LoginRequest InputModel { get; set; } = new();

    /// <summary>
    /// Indica se a página está ocupada realizando uma operação (ex: envio do formulário).
    /// </summary>
    public bool IsBusy { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Método de inicialização do componente.
    /// </summary>
    /// <remarks>
    /// Verifica o estado atual de autenticação e, se o usuário já estiver autenticado, redireciona para a página inicial.
    /// </remarks>
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity is { IsAuthenticated: true })
            NavigationManager.NavigateTo("/");
    }

    #endregion

    #region Methods

    /// <summary>
    /// Manipula o envio válido do formulário de login.
    /// </summary>
    /// <remarks>
    /// Define o estado de carregamento, envia os dados de login para o handler e, em caso de sucesso,
    /// atualiza o estado de autenticação e redireciona o usuário para a página de listagem de imóveis.
    /// Em caso de falha, exibe uma mensagem de erro utilizando o serviço de Snackbar.
    /// </remarks>
    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;
        try
        {
            var result = await Handler.LoginAsync(InputModel);
            if (result.IsSuccess)
            {
                await AuthenticationStateProvider.GetAuthenticationStateAsync();
                AuthenticationStateProvider.NotifyAuthenticationStateChanged();
                NavigationManager.NavigateTo("/listar-imoveis");
            }
            else
                Snackbar.Add(result.Message ?? string.Empty, Severity.Error);
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion
}