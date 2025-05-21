using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Requests.Account;
using RealtyHub.Web.Security;

namespace RealtyHub.Web.Pages.Identity;

/// <summary>
/// Página responsável pelo registro de novos usuários na aplicação.
/// </summary>
public partial class RegisterPage : ComponentBase
{
    #region Services

    /// <summary>
    /// Serviço para exibição de notificações na tela.
    /// </summary>
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    /// <summary>
    /// Handler responsável pelas operações de conta, como registro de novos usuários.
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
    /// Modelo utilizado para vincular os dados de registro.
    /// </summary>
    public RegisterRequest InputModel { get; set; } = new();

    /// <summary>
    /// Texto de erro a ser exibido quando as senhas não coincidem.
    /// </summary>
    public string? ErrorText { get; set; }

    /// <summary>
    /// Indica se há mensagem de erro relacionada ao registro.
    /// </summary>
    public bool Error => !string.IsNullOrEmpty(ErrorText);

    /// <summary>
    /// Indica se a página está ocupada realizando uma operação, como o envio do formulário.
    /// </summary>
    public bool IsBusy { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Método de inicialização do componente.
    /// </summary>
    /// <remarks>
    /// Verifica o estado atual de autenticação. Caso o usuário já esteja autenticado, redireciona para a página de dashboard.
    /// </remarks>
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity is { IsAuthenticated: true })
            NavigationManager.NavigateTo("/dashboard");
    }

    #endregion

    #region Methods

    /// <summary>
    /// Manipula o envio válido do formulário de registro.
    /// </summary>
    /// <remarks>
    /// Define o estado de carregamento, envia os dados para o handler de registro e, em caso de sucesso,
    /// exibe uma mensagem e redireciona o usuário para a página de login. Caso contrário, exibe uma mensagem de erro.
    /// </remarks>
    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;
        try
        {
            var result = await Handler.RegisterAsync(InputModel);
            var resultMessage = result.Message ?? string.Empty;
            if (result.IsSuccess)
            {
                Snackbar.Add(resultMessage, Severity.Success);
                NavigationManager.NavigateTo("/login");
            }
            else
            {
                Snackbar.Add(resultMessage, Severity.Error);
            }
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

    /// <summary>
    /// Verifica se as senhas inseridas são iguais.
    /// </summary>
    /// <remarks>
    /// Se as senhas coincidirem, remove a mensagem de erro; caso contrário, define uma mensagem de erro.
    /// </remarks>
    public void IsPasswordEqual()
    {
        if (InputModel.Password == InputModel.ConfirmPassword)
        {
            ErrorText = null;
            return;
        }
        ErrorText = "As senhas não coincidem";
    }

    #endregion
}