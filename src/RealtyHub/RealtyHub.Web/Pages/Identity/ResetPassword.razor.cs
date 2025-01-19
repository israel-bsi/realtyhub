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

    public bool IsBusy { get; set; }
    public ResetPasswordRequest ResetPasswordModel { get; set; } = new();
    public ForgotPasswordRequest ForgotPasswordModel { get; set; } = new();
    
    #endregion

    #region Services

    [Inject]
    public IAccountHandler AccountHandler { get; set; } = null!;
    
    [Inject] 
    public NavigationManager NavigationManager { get; set; } = null!;
    
    [Inject] 
    public ISnackbar Snackbar { get; set; } = null!;
    
    #endregion

    #region Methods

    protected async Task OnValidSubmitForgotFormAsync()
    {
        IsBusy = true;
        try
        {
            var result = await AccountHandler.ForgotPasswordAsync(ForgotPasswordModel);
            Snackbar.Add(result.Message ?? string.Empty, Severity.Success);
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
    public async Task OnValidSubmitResetFormAsync()
    {
        IsBusy = true;
        try
        {
            ResetPasswordModel.UserId = UserId;
            ResetPasswordModel.Token = Token;
            var result = await AccountHandler.ResetPasswordAsync(ResetPasswordModel);
            if (result.IsSuccess)
            {
                Snackbar.Add(result.Message ?? string.Empty, Severity.Success);
                NavigationManager.NavigateTo("/login");
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