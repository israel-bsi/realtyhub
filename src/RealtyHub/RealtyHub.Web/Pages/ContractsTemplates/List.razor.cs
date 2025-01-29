using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;

namespace RealtyHub.Web.Pages.ContractsTemplates;

public partial class ListContractTemplatesPage : ComponentBase
{
    #region Properties

    public List<ContractTemplate> ContractTemplates { get; set; } = [];

    #endregion

    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public IContractTemplateHandler Handler { get; set; } = null!;

    [Inject]
    public IJSRuntime JsRuntime { get; set; } = null!;

    #endregion

    #region Methods

    public async Task<GridData<ContractTemplate>> LoadServerData(
        GridState<ContractTemplate> state)
    {
        try
        {
            var response = await Handler.GetAllAsync();
            if (response.IsSuccess)
            {
                ContractTemplates = response.Data ?? [];
                return new GridData<ContractTemplate>
                {
                    Items = ContractTemplates
                        .Where(c => c.ShowInPage),
                    TotalItems = ContractTemplates.Count
                };
            }

            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            return new GridData<ContractTemplate>();
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
            return new GridData<ContractTemplate>();
        }
    }

    public async Task ShowInNewPage(ContractTemplate template)
    {
        await JsRuntime.InvokeVoidAsync("openContractPdfInNewTab", template.Path);
    }

    #endregion

}