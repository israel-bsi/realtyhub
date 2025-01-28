using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Extensions;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Web.Components.Common;
using RealtyHub.Web.Components.Offers;

namespace RealtyHub.Web.Pages.Offers;

public partial class ListOffersPage : ComponentBase
{
    #region Parameters

    [Parameter]
    public EventCallback<Offer> OnOfferSelected { get; set; }

    [Parameter]
    public string RowStyle { get; set; } = string.Empty;

    [Parameter] 
    public bool OnlyAccepted { get; set; }

    #endregion

    #region Properties

    public MudDataGrid<Offer> DataGrid { get; set; } = null!;
    public DateRange DateRange { get; set; } = new(DateTime.Now.GetFirstDay(), DateTime.Now.GetLastDay());
    public List<Offer> Items { get; set; } = [];

    #endregion

    #region Services

    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public IOfferHandler Handler { get; set; } = null!;

    #endregion

    #region Methods

    public async Task<GridData<Offer>> LoadServerData(GridState<Offer> state)
    {
        try
        {
            var endDate = DateRange.End?.ToEndOfDay();
            var request = new GetAllOffersRequest
            {
                PageNumber = state.Page + 1,
                PageSize = state.PageSize,
                StartDate = DateRange.Start.ToUtcString(),
                EndDate = endDate.ToUtcString()
            };

            var response = await Handler.GetAllAsync(request);
            if (response.IsSuccess)
            {
                if (OnlyAccepted)
                    Items = response.Data?
                        .Where(o => o.OfferStatus == EOfferStatus.Accepted)
                        .ToList() ?? new List<Offer>();
                else
                    Items = response.Data ?? new List<Offer>();

                return new GridData<Offer>
                {
                    Items = Items.OrderByDescending(o => o.UpdatedAt),
                    TotalItems = response.TotalCount
                };
            }

            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            return new GridData<Offer>();
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
            return new GridData<Offer>();
        }
    }

    public void OnDateRangeChanged(DateRange newDateRange)
    {
        DateRange = newDateRange;
        DataGrid.ReloadServerData();
    }

    public async Task OnAcceptClickedAsync(Offer offer)
    {
        if (IsOfferStatusInvalid(offer)) return;
        var parameters = new DialogParameters
        {
            { "ContentText", "Deseja aceitar a proposta?<br>" +
                             "Após a alteração não será mais possível editar!" },
            { "ButtonColor", Color.Success }
        };
        var dialog = await DialogService.ShowAsync<DialogConfirm>("Confirmação", parameters);
        if (await dialog.Result is { Canceled: true }) return;

        var request = new AcceptOfferRequest { Id = offer.Id };
        var result = await Handler.AcceptAsync(request);
        if (result is { IsSuccess: true, Data: not null })
        {
            Snackbar.Add("Proposta aceita", Severity.Success);
            await DataGrid.ReloadServerData();
        }
        else
            Snackbar.Add(result.Message ?? string.Empty, Severity.Error);
    }

    public async Task OnRejectClickedAsync(Offer offer)
    {
        if (IsOfferStatusInvalid(offer)) return;
        var parameters = new DialogParameters
        {
            { "ContentText", "Deseja recusar a proposta?<br>" +
                             "Após a alteração não será mais possível editar!" },
            { "ButtonColor", Color.Error }
        };
        var dialog = await DialogService.ShowAsync<DialogConfirm>("Confirmação", parameters);
        if (await dialog.Result is { Canceled: true }) return;

        var request = new RejectOfferRequest { Id = offer.Id };
        var result = await Handler.RejectAsync(request);
        if (result is { IsSuccess: true, Data: not null })
        {
            Snackbar.Add("Proposta recusada", Severity.Success);
            await DataGrid.ReloadServerData();
        }
        else
            Snackbar.Add(result.Message ?? string.Empty, Severity.Error);
    }

    public async Task OpenOfferDialog(Offer offer)
    {
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            CloseOnEscapeKey = true,
            FullWidth = true
        };

        var parameters = new DialogParameters
        {
            { "OfferId", offer.Id },
            { "ReadyOnly", offer.OfferStatus != EOfferStatus.Analysis },
            { "ShowPayments", true }
        };

        await DialogService.ShowAsync<OfferDialog>("Visualizar proposta", parameters, options);
    }

    public async Task SelectOffer(Offer offer)
    {
        if (OnOfferSelected.HasDelegate)
            await OnOfferSelected.InvokeAsync(offer);
    }

    private bool IsOfferStatusInvalid(Offer viewing)
    {
        switch (viewing.OfferStatus)
        {
            case EOfferStatus.Accepted:
                Snackbar.Add("Proposta está aceita", Severity.Warning);
                return true;
            case EOfferStatus.Rejected:
                Snackbar.Add("Proposta está cancelada", Severity.Warning);
                return true;
        }
        return false;
    }
    #endregion
}