using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Web.Components;

namespace RealtyHub.Web.Pages.Customers;

public partial class ListCustomersPage : ComponentBase
{
    #region Parameters

    [Parameter] 
    public EventCallback<Customer> OnCustomerSelected { get; set; }

    #endregion

    #region Properties

    public MudDataGrid<Customer> DataGrid { get; set; } = null!;
    public List<Customer> Customers { get; set; } = [];
    private string _searchTerm = string.Empty;
    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (_searchTerm == value) return;
            _searchTerm = value;
            _ = DataGrid.ReloadServerData();
        }
    }

    #endregion

    #region Services
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public ICustomerHandler Handler { get; set; } = null!;

    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    #endregion

    #region Methods

    public async Task OnDeleteButtonClickedAsync(long id, string name)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", $"Ao prosseguir o cliente {name} será excluido. " +
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

        await OnDeleteAsync(id, name);
        StateHasChanged();
    }
    private async Task OnDeleteAsync(long id, string name)
    {
        try
        {
            await Handler.DeleteAsync(new DeleteCustomerRequest { Id = id });
            Customers.RemoveAll(x => x.Id == id);
            await DataGrid.ReloadServerData();
            Snackbar.Add($"Cliente {name} excluído", Severity.Success);
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Success);
        }
    }
    public async Task<GridData<Customer>> LoadServerData(GridState<Customer> state)
    {
        try
        {
            var request = new GetAllCustomersRequest
            {
                PageNumber = state.Page + 1,
                PageSize = state.PageSize,
                SearchTerm = SearchTerm
            };

            var response = await Handler.GetAllAsync(request);
            if (response.IsSuccess)
                return new GridData<Customer>
                {
                    Items = response.Data ?? [],
                    TotalItems = response.TotalCount
                };

            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            return new GridData<Customer>
            {
                Items = [],
                TotalItems = 0
            };
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
            return new GridData<Customer>
            {
                Items = [],
                TotalItems = 0
            };
        }
    }
    public async Task SelectCustomer(Customer customer)
    {
        if (OnCustomerSelected.HasDelegate) 
            await OnCustomerSelected.InvokeAsync(customer);
    }

    #endregion
}