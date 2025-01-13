using Microsoft.AspNetCore.Components;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models.Account;

namespace RealtyHub.Web.Pages.Identity;

public partial class ConfirmEmailPage : ComponentBase
{
    #region Parameters

    [Parameter]
    [SupplyParameterFromQuery(Name = "UserId")]
    public long UserId { get; set; }
    [Parameter]
    [SupplyParameterFromQuery(Name = "Token")]
    public string Token { get; set; } = string.Empty;

    #endregion

    #region Properties
    public string Status { get; set; } = string.Empty;
    #endregion

    #region Services

    [Inject]
    public IAccountHandler AccountHandler { get; set; } = null!;
    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var request = new UserConfirmEmail
            {
                UserId = UserId,
                Token = Token
            };

            var result = await AccountHandler.ConfirmEmail(request);
            Status = result.Message!;
        }
        catch (Exception e)
        {
            Status = $"Erro ao confirmar email!\n{e.Message}";
        }
    }

    #endregion
}