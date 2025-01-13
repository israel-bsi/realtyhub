using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models.Account;
namespace RealtyHub.Web.Pages.Identity;

public partial class ConfirmEmailPage : ComponentBase
{
    #region Parameters

    [Parameter] public string UserId { get; set; } = string.Empty;
    [Parameter] public string Token { get; set; } = string.Empty;

    #endregion

    #region Properties

    public UserConfirmEmail InputModel { get; set; } = new();
    public bool IsBusy { get; set; }
    public string Status { get; set; } = string.Empty;
    #endregion

    #region Services

    [Inject]
    public AuthenticationStateProvider AuthenticationState { get; set; } = null!;

    [Inject] public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    [Inject]
    public IAccountHandler AccountHandler { get; set; } = null!;
    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        try
        {
            InputModel.UserId = long.Parse(UserId);
            InputModel.Token = Token;

            var result = await AccountHandler.ConfirmEmail(InputModel);
            if (result.IsSuccess)
            {
                Snackbar.Add("Email confirmado com sucesso!", Severity.Success);
                Status = "Email confirmado com sucesso!";
            }
            else
            {
                Snackbar.Add("Erro ao confirmar email!", Severity.Error);
                Status = "Erro ao confirmar email!";
            }
        }
        catch (Exception e)
        {
            Snackbar.Add($"Erro ao confirmar email!\n{e.Message}", Severity.Error);
            Status = $"Erro ao confirmar email!\n{e.Message}";
        }
    }

    #endregion
}