using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Extensions;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Web.Components.Common;

namespace RealtyHub.Web.Pages.Contracts;

public partial class ListContractsPage : ComponentBase
{
    #region Properties

    public MudDataGrid<Contract> DataGrid { get; set; } = null!;
    public List<Contract> Contracts { get; set; } = [];
    public DateRange DateRange { get; set; } = new(DateTime.Now.GetFirstDay(), 
        DateTime.Now.GetLastDay());

    #endregion

    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public IContractHandler Handler { get; set; } = null!;

    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    #endregion

    #region Methods

    public async Task<GridData<Contract>> LoadServerData(GridState<Contract> state)
    {
        try
        {
            var endDate = DateRange.End?.ToEndOfDay();
            var request = new GetAllContractsRequest
            {
                PageNumber = state.Page + 1,
                PageSize = state.PageSize,
                StartDate = DateRange.Start.ToUtcString(),
                EndDate = endDate.ToUtcString()
            };

            var response = await Handler.GetAllAsync(request);
            if (response.IsSuccess)
            {
                Contracts = response.Data ?? [];
                return new GridData<Contract>
                {
                    Items = Contracts.OrderByDescending(c=>c.UpdatedAt),
                    TotalItems = response.TotalCount
                };
            }

            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            return new GridData<Contract>();
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
            return new GridData<Contract>();
        }
    }

    public void OnDateRangeChanged(DateRange newDateRange)
    {
        DateRange = newDateRange;
        DataGrid.ReloadServerData();
    }

    public async Task DownloadContract()
    {
        Snackbar.Add("Contrato gerado com sucesso", Severity.Success);
    }

    public async Task OnDeleteButtonClickedAsync(long id)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", $"Ao prosseguir o contrato com identificador <b>{id}</b> será excluido. " +
                             "Esta é uma ação irreversível! Deseja continuar?" },
            { "ButtonText", "Confirmar" },
            { "ButtonColor", Color.Error }
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small
        };

        var dialog = await DialogService.ShowAsync<DialogConfirm>("Atenção", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: true }) return;

        await OnDeleteAsync(id);
        StateHasChanged();
    }
    private async Task OnDeleteAsync(long id)
    {
        try
        {
            await Handler.DeleteAsync(new DeleteContractRequest { Id = id });
            Contracts.RemoveAll(x => x.Id == id);
            await DataGrid.ReloadServerData();
            Snackbar.Add($"Contrato {id} excluído", Severity.Success);
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Success);
        }
    }

    #endregion
}