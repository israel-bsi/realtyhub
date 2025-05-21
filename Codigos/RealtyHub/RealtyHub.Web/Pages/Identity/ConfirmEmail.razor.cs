using Microsoft.AspNetCore.Components;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models.Account;

namespace RealtyHub.Web.Pages.Identity;

/// <summary>
/// Página responsável por confirmar o e-mail do usuário.
/// </summary>
public partial class ConfirmEmailPage : ComponentBase
{
    #region Parameters

    /// <summary>
    /// Identificador do usuário. O valor é obtido a partir da query string usando o nome "UserId".
    /// </summary>
    [Parameter]
    [SupplyParameterFromQuery(Name = "UserId")]
    public long UserId { get; set; }

    /// <summary>
    /// Token de confirmação de e-mail. O valor é obtido a partir da query string usando o nome "Token".
    /// </summary>
    [Parameter]
    [SupplyParameterFromQuery(Name = "Token")]
    public string Token { get; set; } = string.Empty;

    #endregion

    #region Properties

    /// <summary>
    /// Indica se a página está realizando alguma operação (estado de carregamento).
    /// </summary>
    public bool IsBusy { get; set; }

    /// <summary>
    /// Mensagem de status a ser exibida após a tentativa de confirmação do e-mail.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Indica se a confirmação de e-mail foi realizada com sucesso.
    /// </summary>
    public bool Success { get; set; }

    #endregion

    #region Services

    /// <summary>
    /// Handler responsável pelas operações de conta, incluindo a confirmação de e-mail.
    /// </summary>
    [Inject]
    public IAccountHandler AccountHandler { get; set; } = null!;

    #endregion

    #region Overrides

    /// <summary>
    /// Inicializa o componente e confirma o e-mail do usuário.
    /// </summary>
    /// <remarks>
    /// Durante a inicialização, o componente define o estado de carregamento, envia uma solicitação de confirmação de e-mail
    /// utilizando as informações obtidas da query string, e atualiza o status e o indicador de sucesso com base na resposta do handler.
    /// Caso ocorra um erro, uma mensagem de erro é definida. Ao final, o estado de carregamento é desativado.
    /// </remarks>
    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        try
        {
            var request = new ConfirmEmailRequest
            {
                UserId = UserId,
                Token = Token
            };

            var result = await AccountHandler.ConfirmEmailAsync(request);
            Success = result.IsSuccess;
            Status = result.Message!;
        }
        catch (Exception e)
        {
            Status = $"Erro ao confirmar email!\n{e.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion
}