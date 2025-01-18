using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Web.Components;
using RealtyHub.Web.Components.Viewings;

namespace RealtyHub.Web.Pages.Viewings;

public partial class ListViewingsByPropertyPage : ComponentBase
{
    #region Parameters

    [Parameter]
    public long PropertyId { get; set; }

    #endregion

    #region Properties

    public MudDataGrid<Viewing> DataGrid { get; set; } = null!;
    public bool IsBusy { get; set; }
    public DateRange DateRange { get; set; } = new();
    public List<Viewing> Items { get; set; } = [];
    public Property Property { get; set; } = new();

    #endregion

    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public IViewingHandler ViewingHandler { get; set; } = null!;

    [Inject]
    public IPropertyHandler PropertyHandler { get; set; } = null!;

    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    #endregion

    #region Methods

    public async Task<GridData<Viewing>> LoadServerData(GridState<Viewing> state)
    {
        try
        {
            var request = new GetAllViewingsByPropertyRequest
            {
                PropertyId = PropertyId,
                PageNumber = state.Page + 1,
                PageSize = state.PageSize,
            };

            var response = await PropertyHandler.GetAllViewingsAsync(request);
            if (response.IsSuccess)
                return new GridData<Viewing>()
                {
                    Items = response.Data ?? [],
                    TotalItems = response.TotalCount
                };

            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            return new GridData<Viewing>();
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
            return new GridData<Viewing>();
        }
    }
    public async Task OnScheduleButtonClickedAsync()
    {
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true
        };
        var parameters = new DialogParameters
        {
            { "Property", Property },
            { "LockPropertySearch", true },
            { "RedirectToPageList", false }
        };
        await DialogService.ShowAsync<ViewingDialog>(null, parameters, options);
        await DataGrid.ReloadServerData();
    }
    public async Task OnRescheduleButtonClickedAsync(Viewing viewing)
    {
        if (IsViewingStatusInValid(viewing)) return;

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true
        };
        var parameters = new DialogParameters
        {
            { "Property", Property },
            { "LockPropertySearch", true },
            { "RedirectToPageList", false },
            { "Id", viewing.Id}
        };
        await DialogService.ShowAsync<ViewingDialog>(null, parameters, options);
        await DataGrid.ReloadServerData();
    }
    public async Task OnDoneButtonClickedAsync(Viewing viewing)
    {
        if (IsViewingStatusInValid(viewing)) return;

        var parameters = new DialogParameters
        {
            { "ContentText", "Deseja finalizar a visita?" },
            { "ButtonColor", Color.Warning }
        };

        var dialog = await DialogService.ShowAsync<DialogConfirm>("Confirmação", parameters);
        if (await dialog.Result is { Canceled: true }) return;

        var request = new DoneViewingRequest { Id = viewing.Id };
        var result = await ViewingHandler.DoneAsync(request);
        if (result is { IsSuccess: true, Data: not null })
        {
            var viewingExisting = Items.FirstOrDefault(x => x.Id == viewing.Id);
            if (viewingExisting is not null)
                viewingExisting.ViewingStatus = result.Data.ViewingStatus;
        }
        Snackbar.Add(result.Message ?? string.Empty, Severity.Info);
        await DataGrid.ReloadServerData();
    }
    public async Task OnCancelButtonClickedAsync(Viewing viewing)
    {
        if (IsViewingStatusInValid(viewing)) return;
        var parameters = new DialogParameters
        {
            { "ContentText", "Deseja cancelar a visita?" },
            { "ButtonColor", Color.Error }
        };
        var dialog = await DialogService.ShowAsync<DialogConfirm>("Confirmação", parameters);
        if (await dialog.Result is { Canceled: true }) return;

        var request = new CancelViewingRequest { Id = viewing.Id };
        var result = await ViewingHandler.CancelAsync(request);
        if (result is { IsSuccess: true, Data: not null })
        {
            var viewingExisting = Items.FirstOrDefault(x => x.Id == viewing.Id);
            if (viewingExisting is not null)
                viewingExisting.ViewingStatus = result.Data.ViewingStatus;
        }
        Snackbar.Add(result.Message ?? string.Empty, Severity.Info);
        await DataGrid.ReloadServerData();
    }
    private bool IsViewingStatusInValid(Viewing viewing)
    {
        switch (viewing.ViewingStatus)
        {
            case EViewingStatus.Done:
                Snackbar.Add("Visita está finalizada", Severity.Warning);
                return true;
            case EViewingStatus.Canceled:
                Snackbar.Add("Visita está cancelada", Severity.Warning);
                return true;
        }
        return false;
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        GetAllViewingsByPropertyRequest? request = null;
        try
        {
            request = new GetAllViewingsByPropertyRequest { PropertyId = PropertyId };
        }
        catch
        {
            Snackbar.Add("Parâmetro inválido", Severity.Error);
        }

        if (request is null) return;

        try
        {
            var response = await PropertyHandler.GetAllViewingsAsync(request);
            if (response is { IsSuccess: true, Data: not null })
            {
                Items = response.Data;
                Property = Items.FirstOrDefault()?.Property ?? new Property();
            }
            else
                Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
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