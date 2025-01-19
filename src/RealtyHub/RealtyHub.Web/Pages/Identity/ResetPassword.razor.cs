using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Requests.Account;

namespace RealtyHub.Web.Pages.Identity;

public class ResetPasswordPage : ComponentBase
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

    public ResetPasswordRequest ResetPasswordModel { get; set; } = null!;
    public ForgotPasswordRequest ForgotPasswordModel { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public bool Success { get; set; }
    #endregion

    #region Services

    [Inject]
    public IAccountHandler AccountHandler { get; set; } = null!;
    
    [Inject] 
    public NavigationManager NavigationManager { get; set; } = null!;
    
    [Inject] 
    public ISnackbar Snackbar { get; set; } = null!;
    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var result = await AccountHandler.ResetPasswordAsync(ResetPasswordModel);
            Success = result.IsSuccess;
            Status = result.Message!;
        }
        catch (Exception e)
        {
            Status = $"Erro ao confirmar email!\n{e.Message}";
        }
    }

    protected async Task OnValidSubmitAsync()
    {
        var result = await AccountHandler.ForgotPasswordAsync(ForgotPasswordModel);
        Snackbar.Add(result.Message ?? string.Empty, Severity.Success);
    }

    #endregion
}