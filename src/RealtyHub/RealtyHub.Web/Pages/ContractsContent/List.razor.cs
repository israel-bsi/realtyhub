using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.ContractsContent;

namespace RealtyHub.Web.Pages.ContractsContent;

public partial class ListContractsContentPage : ComponentBase
{
    #region Properties

    public MudDataGrid<ContractContent> DataGrid { get; set; } = null!;
    public List<ContractContent> ContractContents { get; set; } = [];

    #endregion

    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public IContractContentHandler Handler { get; set; } = null!;

    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    [Inject] 
    public IJSRuntime JsRuntime { get; set; } = null!;

    #endregion

    #region Methods

    public async Task<GridData<ContractContent>> LoadServerData(
        GridState<ContractContent> state)
    {
        try
        {
            var request = new GetAllContractContentByUserRequest();

            var response = await Handler.GetAllByUserAsync(request);
            if (response.IsSuccess)
            {
                ContractContents = response.Data ?? [];
                return new GridData<ContractContent>
                {
                    Items = ContractContents.OrderByDescending(c => c.UpdatedAt),
                    TotalItems = ContractContents.Count
                };
            }

            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            return new GridData<ContractContent>();
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
            return new GridData<ContractContent>();
        }
    }

    public async Task ShowInNewPage(ContractContent content)
    {
        await JsRuntime.InvokeVoidAsync("openContractPdfInNewTab", content.Path);
    }

    #endregion

}