using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Customers;

namespace RealtyHub.Web.Pages.Customers;

public partial class ListCustomersPage : ComponentBase
{
    #region Properties

    public bool IsBusy { get; set; }
    public List<Customer> Customers { get; set; } = new();
    public string SearchTerm { get; set; } = string.Empty;
    #endregion

    #region Services
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public ICustomerHandler Handler { get; set; } = null!;

    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        try
        {
            var request = new GetAllCustomersRequest();
            var result = await Handler.GetAllAsync(request);
            if (result.IsSuccess)
                Customers = result.Data ?? new List<Customer>();
            else
                Snackbar.Add(result.Message, Severity.Error);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }
        IsBusy = false;
        await base.OnInitializedAsync();
    }

    #endregion

    #region Methods

    public async void OnDeleteButtonClickedAsync(long id, string name)
    {
        var result = await DialogService.ShowMessageBox(
            "ATENÇÃO",
            $"Ao prosseguir o cliente {name} será excluido. Esta é uma ação irreversível! Deseja continuar?",
            yesText: "Excluir",
            cancelText: "Cancelar");

        if (result is true)
        {
            await OnDeleteAsync(id, name);
        }

        StateHasChanged();
    }

    public async Task OnDeleteAsync(long id, string name)
    {
        try
        {
            await Handler.DeleteAsync(new DeleteCustomerRequest { Id = id });
            Customers.RemoveAll(x => x.Id == id);
            Snackbar.Add($"Cliente {name} excluído", Severity.Success);
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Success);
        }
    }

    public Func<Customer, bool> Filter => customer =>
    {
        if (string.IsNullOrEmpty(SearchTerm))
            return true;

        if (customer.Id.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        if (customer.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        if (customer.BusinessName != null && customer.BusinessName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        if (customer.DocumentNumber.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        if (customer.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        if (customer.Phone.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        if (customer.Address.ZipCode.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    };

    #endregion
}