using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Requests.Properties;

namespace RealtyHub.Web.Components.Offers;

public partial class OfferFormComponent : ComponentBase
{
    #region Parameters

    [Parameter]
    public long PropertyId { get; set; }

    [Parameter]
    public long OfferId { get; set; }

    [Parameter] 
    public bool ReadyOnly { get; set; }

    [Parameter]
    public EventCallback OnSubmitButtonClicked { get; set; }

    #endregion

    #region Properties

    public bool IsBusy { get; set; }
    public Offer InputModel { get; set; } = new();
    public string Operation => OfferId == 0 ? "Enviar" : "Editar";

    #endregion

    #region Services

    [Inject]
    public IOfferHandler OfferHandler { get; set; } = null!;

    [Inject]
    public IPropertyHandler PropertyHandler { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    #endregion

    #region Methods

    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;
        try
        {
            string message;
            if (OfferId == 0)
            {
                var response = await OfferHandler.CreateAsync(InputModel);
                if (response.IsSuccess)
                {
                    Snackbar.Add("Proposta cadastrada com sucesso", Severity.Success);
                    await OnSubmitButtonClickedAsync();
                    NavigationManager.NavigateTo("/home");
                    return;
                }
                message = response.Message ?? string.Empty;
            }
            else
            {
                var response = await OfferHandler.UpdateAsync(InputModel);
                if (response.IsSuccess)
                {
                    Snackbar.Add("Proposta alterada com sucesso", Severity.Success);
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

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        try
        {
            if (OfferId != 0)
            {
                var request = new GetOfferByIdRequest { Id = OfferId };
                var response = await OfferHandler.GetByIdAsync(request);
                if (response is { IsSuccess: true, Data: not null })
                {
                    InputModel = response.Data;
                    return;
                }
                Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            }
            else
            {
                var request = new GetPropertyByIdRequest { Id = PropertyId };
                var response = await PropertyHandler.GetByIdAsync(request);
                if (response is { IsSuccess: true, Data: not null })
                {
                    InputModel.Property = response.Data;
                    InputModel.PropertyId = response.Data.Id;
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