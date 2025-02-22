using MudBlazor;
using RealtyHub.Web.Components.Common;

namespace RealtyHub.Web.Services;

public class ShowDialogConfirm(IDialogService dialogService)
{
    public async Task<DialogResult?> ShowDialogAsync(DialogParameters parameters)
    {
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small
        };

        var dialog = await dialogService.ShowAsync<DialogConfirm>("Confirmação", parameters, options);
        return await dialog.Result;
    }
}