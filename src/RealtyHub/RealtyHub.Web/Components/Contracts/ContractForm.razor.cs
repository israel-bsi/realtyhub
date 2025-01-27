using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Web.Components.Customers;
using RealtyHub.Web.Components.Offers;
using RealtyHub.Web.Components.Properties;

namespace RealtyHub.Web.Components.Contracts;

public partial class ContractFormComponent : ComponentBase
{
    #region Parameters

    [Parameter]
    [SupplyParameterFromQuery(Name = "Id")]
    public long ContractId { get; set; }

    [Parameter]
    public long OfferId { get; set; }

    [Parameter]
    public long PropertyId { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public bool LockPropertySearch { get; set; }

    [Parameter]
    public bool LockCustomerSearch { get; set; }

    [Parameter]
    public EventCallback OnSubmitButtonClicked { get; set; }

    #endregion

    #region Properties

    public bool IsBusy { get; set; }
    public int EditFormKey { get; set; }
    public Contract InputModel { get; set; } = new();
    public string Operation => ContractId == 0 ? "Emitir" : "Editar";

    #endregion

    #region Services

    [Inject]
    public IOfferHandler OfferHandler { get; set; } = null!;

    [Inject]
    public IContractHandler ContractHandler { get; set; } = null!;

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
        IsBusy = true;
        try
        {
            string message;
            if (ContractId == 0)
            {
                var response = await ContractHandler.CreateAsync(InputModel);
                if (response.IsSuccess)
                {
                    Snackbar.Add("Contrato emitido com sucesso", Severity.Success);
                    await OnSubmitButtonClickedAsync();
                    return;
                }
                message = response.Message ?? string.Empty;
            }
            else
            {
                var response = await ContractHandler.UpdateAsync(InputModel);
                if (response.IsSuccess)
                {
                    Snackbar.Add("Contrato alterado com sucesso", Severity.Success);
                    await OnSubmitButtonClickedAsync();
                    return;
                }
                message = response.Message ?? string.Empty;
            }
            Snackbar.Add(message, Severity.Error);
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

    private async Task OnSubmitButtonClickedAsync()
    {
        if (OnSubmitButtonClicked.HasDelegate)
            await OnSubmitButtonClicked.InvokeAsync();
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
        InputModel.Offer!.Property = property;
        InputModel.Offer.PropertyId = property.Id;
        EditFormKey++;
        StateHasChanged();
    }

    public async Task OpenOfferDialog()
    {
        if (LockPropertySearch) return;
        var parameters = new DialogParameters
        {
            { "OnOfferSelected", EventCallback.Factory
                .Create<Offer>(this, SelectedOffer) }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true
        };
        var dialog = await DialogService
            .ShowAsync<OfferDialog>("Informe a proposta", parameters, options);
        var result = await dialog.Result;
        if (result is { Canceled: false, Data: Offer offer })
            SelectedOffer(offer);
    }
    private void SelectedOffer(Offer offer)
    {
        InputModel.OfferId = offer.Id;
        InputModel.Offer = offer;
        InputModel.Seller = offer.Property!.Seller;
        InputModel.SellerId = offer.Property.SellerId;
        InputModel.Buyer = offer.Buyer;
        InputModel.BuyerId = offer.BuyerId;
        EditFormKey++;
        StateHasChanged();
    }

    public async Task OpenBuyerDialog()
    {
        if (LockCustomerSearch) return;
        var parameters = new DialogParameters
        {
            { "OnCustomerSelected", EventCallback.Factory
                .Create<Customer>(this, SelectedBuyer) }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true
        };
        var dialog = await DialogService
            .ShowAsync<CustomerDialog>("Informe o comprador", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: Customer buyer })
            SelectedBuyer(buyer);
    }
    private void SelectedBuyer(Customer buyer)
    {
        InputModel.Buyer = buyer;
        InputModel.BuyerId = buyer.Id;
        EditFormKey++;
        StateHasChanged();
    }

    public async Task OpenSellerDialog()
    {
        if (LockCustomerSearch) return;
        var parameters = new DialogParameters
        {
            { "OnCustomerSelected", EventCallback.Factory
                .Create<Customer>(this, SelectedSeller) }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true
        };
        var dialog = await DialogService
            .ShowAsync<CustomerDialog>("Informe o vendedor", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: Customer seller })
            SelectedSeller(seller);
    }
    private void SelectedSeller(Customer seller)
    {
        InputModel.Seller = seller;
        InputModel.SellerId = seller.Id;
        EditFormKey++;
        StateHasChanged();
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        try
        {
            if (ContractId != 0)
            {
                var request = new GetContractByIdRequest { Id = ContractId };
                var response = await ContractHandler.GetByIdAsync(request);
                if (response is { IsSuccess: true, Data: not null })
                {
                    InputModel = response.Data;
                    return;
                }
                Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
                return;
            }
            if (OfferId != 0)
            {
                var request = new GetOfferByIdRequest { Id = OfferId };
                var response = await OfferHandler.GetByIdAsync(request);
                if (response is { IsSuccess: true, Data: not null })
                {
                    InputModel.Offer = response.Data;
                    InputModel.OfferId = OfferId;
                    return;
                }
                Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
                return;
            }
            if (PropertyId != 0)
            {
                var request = new GetOfferAcceptedByProperty { PropertyId = PropertyId };
                var response = await OfferHandler.GetAcceptedByProperty(request);
                if (response is { IsSuccess: true, Data: not null })
                {
                    InputModel.Offer = response.Data;
                    InputModel.OfferId = response.Data.Id;
                    return;
                }
                Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
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