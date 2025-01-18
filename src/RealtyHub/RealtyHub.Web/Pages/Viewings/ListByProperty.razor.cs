using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Extensions;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
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
    public List<Viewing> InputModel { get; set; } = [];
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
    }
    public async Task OnRescheduleButtonClickedAsync(Viewing viewing)
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
            { "RedirectToPageList", false },
            { "ViewingId", viewing.Id}
        };
        await DialogService.ShowAsync<ViewingDialog>(null, parameters, options);
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
                InputModel = response.Data;
                Property = InputModel.FirstOrDefault()?.Property ?? new Property();
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