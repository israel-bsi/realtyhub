using MudBlazor;
using RealtyHub.Web.Components.Common;

namespace RealtyHub.Web.Services;

/// <summary>
/// Serviço responsável por exibir um diálogo de confirmação utilizando os componentes do MudBlazor.
/// </summary>
public class ShowDialogConfirm
{
    private readonly IDialogService _dialogService;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ShowDialogConfirm"/>.
    /// </summary>
    /// <param name="dialogService">Serviço de diálogo utilizado para exibir o componente de confirmação.</param>
    public ShowDialogConfirm(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    /// <summary>
    /// Exibe um diálogo de confirmação com as configurações especificadas.
    /// </summary>
    /// <param name="parameters">Parâmetros do diálogo que determinam o conteúdo e as ações disponíveis.</param>
    /// <returns>
    /// Task contendo o resultado do diálogo (<see cref="DialogResult"/>) ou null caso a operação seja cancelada.
    /// </returns>
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