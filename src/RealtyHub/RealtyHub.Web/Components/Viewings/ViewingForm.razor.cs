using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Core.Responses;
using RealtyHub.Web.Components.Customers;
using RealtyHub.Web.Components.Properties;

namespace RealtyHub.Web.Components.Viewings;

public partial class ViewingFormComponent : ComponentBase
{
    #region Parameters

    [Parameter]
    public long Id { get; set; }

    [Parameter] 
    public Customer? Customer { get; set; }

    [Parameter]
    public Property? Property { get; set; }

    [Parameter] 
    public bool LockPropertySearch { get; set; }

    [Parameter]
    public bool LockCustomerSearch { get; set; }

    [Parameter]
    public bool RedirectToPageList { get; set; }

    [Parameter]
    public EventCallback OnSubmitButtonClicked { get; set; }

    #endregion

    #region Properties
    public string Operation => Id != 0
        ? "Reagendar" : "Agendar";
    public Viewing InputModel { get; set; } = new();
    public TimeSpan? ViewingTime { get; set; }
    public bool IsBusy { get; set; }
    public int EditFormKey { get; set; }
    
    #endregion

    #region Services

    [Inject]
    public IViewingHandler ViewingHandler { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    #endregion

    #region Methods

    public async Task OnValidSubmitAsync()
    {
        if (ViewingTime is null)
        {
            Snackbar.Add("Informe o horário da visita", Severity.Error);
            return;
        }
        if (InputModel.Customer is null)
        {
            Snackbar.Add("Informe o cliente", Severity.Error);
            return;
        }
        if (InputModel.Property is null)
        {
            Snackbar.Add("Informe o imóvel", Severity.Error);
            return;
        }
        IsBusy = true;
        try
        {
            if (InputModel.ViewingDate.HasValue && ViewingTime.HasValue)
            {
                InputModel.ViewingDate = InputModel
                    .ViewingDate.Value.Date.Add(ViewingTime.Value)
                    .ToUniversalTime();
            }

            Response<Viewing?> result;
            if (Id == 0)
                result = await ViewingHandler.ScheduleAsync(InputModel);
            else
                result = await ViewingHandler.RescheduleAsync(InputModel);

            var resultMessage = result.Message ?? string.Empty;
            if (!result.IsSuccess)
            {
                Snackbar.Add(resultMessage, Severity.Error);
                return;
            }

            await OnSubmitButtonClickedAsync();
            Snackbar.Add(resultMessage, Severity.Success);
            if (RedirectToPageList)
                NavigationManager.NavigateTo("/visitas");
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
    private async Task LoadViewingAsync()
    {
        GetViewingByIdRequest? request = null;
        try
        {
            request = new GetViewingByIdRequest { Id = Id };
        }
        catch
        {
            Snackbar.Add("Parâmetro inválido", Severity.Error);
        }

        if (request is null) return;

        var response = await ViewingHandler.GetByIdAsync(request);
        if (response is { IsSuccess: true, Data: not null })
        {
            InputModel.Id = response.Data.Id;
            InputModel.ViewingDate = response.Data.ViewingDate;
            InputModel.ViewingStatus = response.Data.ViewingStatus;
            InputModel.CustomerId = response.Data.CustomerId;
            InputModel.PropertyId = response.Data.PropertyId;
            InputModel.Property = response.Data.Property;
            InputModel.Customer = response.Data.Customer;
            InputModel.UserId = response.Data.UserId;
            ViewingTime = InputModel.ViewingDate!.Value.TimeOfDay;
        }
        else
        {
            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            NavigationManager.NavigateTo("/visitas");
        }
    }
    public async Task OnClickDoneViewing()
    {
        var request = new DoneViewingRequest { Id = InputModel.Id };
        var result = await ViewingHandler.DoneAsync(request);
        var resultMessage = result.Message ?? string.Empty;
        if (result is { IsSuccess: true, Data: not null })
            InputModel.ViewingStatus = result.Data.ViewingStatus;

        Snackbar.Add(resultMessage, Severity.Info);
        StateHasChanged();
    }
    public async Task OnClickCancelViewing()
    {
        var request = new CancelViewingRequest { Id = InputModel.Id };
        var result = await ViewingHandler.CancelAsync(request);
        var resultMessage = result.Message ?? string.Empty;
        if (result is { IsSuccess: true, Data: not null })
            InputModel.ViewingStatus = result.Data.ViewingStatus;

        Snackbar.Add(resultMessage, Severity.Info);
        StateHasChanged();
    }
    public async Task OpenCustomerDialog()
    {
        if (LockCustomerSearch) return;
        var parameters = new DialogParameters
        {
            { "OnCustomerSelected", EventCallback.Factory
                .Create<Customer>(this, SelectedCustomer) }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true
        };
        var dialog = await DialogService
            .ShowAsync<CustomerDialog>("Informe o Cliente", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: Customer selectedCustomer }) 
            SelectedCustomer(selectedCustomer);
    }
    private void SelectedCustomer(Customer customer)
    {
        InputModel.Customer = customer;
        InputModel.CustomerId = customer.Id;
        EditFormKey++;
        StateHasChanged();
    }
    public async Task OpenPropertyDialog()
    {
        if (LockPropertySearch) return;
        var parameters = new DialogParameters
        {
            { "OnPropertySelected", EventCallback.Factory
                .Create<Property>(this, SelectedProperty) }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true
        };
        var dialog = await DialogService
            .ShowAsync<PropertyDialog>("Informe o Imóvel", parameters, options);

        var result = await dialog.Result;

        if (result is { Canceled: false, Data: Property selectedProperty })
            SelectedProperty(selectedProperty);
    }
    private void SelectedProperty(Property property)
    {
        InputModel.Property = property;
        InputModel.PropertyId = property.Id;
        EditFormKey++;
        StateHasChanged();
    }
    private async Task OnSubmitButtonClickedAsync()
    {
        if (OnSubmitButtonClicked.HasDelegate)
            await OnSubmitButtonClicked.InvokeAsync();
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        try
        {
            if (Id != 0)
                await LoadViewingAsync();
            else
            {
                InputModel.Customer = Customer;
                InputModel.CustomerId = Customer?.Id ?? 0;
                InputModel.Property = Property;
                InputModel.PropertyId = Property?.Id ?? 0;
            }
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