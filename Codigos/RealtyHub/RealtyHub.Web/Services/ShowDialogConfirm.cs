using MudBlazor;
using RealtyHub.Web.Components.Common;

namespace RealtyHub.Web.Services;

public class ShowDialogConfirm
{
    private readonly IDialogService _dialogService;

    public ShowDialogConfirm(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public async Task<DialogResult?> ShowDialogAsync(DialogParameters parameters)
    {
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small
        };

        var dialog = await _dialogService.ShowAsync<DialogConfirm>("Confirmação", parameters, options);
        return await dialog.Result;
    }
}