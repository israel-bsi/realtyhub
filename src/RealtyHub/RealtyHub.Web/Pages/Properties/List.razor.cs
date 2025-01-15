using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Web.Components;

namespace RealtyHub.Web.Pages.Properties;

public partial class ListPropertiesPage : ComponentBase
{
    #region Properties

    public MudDataGrid<Property> DataGrid { get; set; } = null!;
    public List<Property> Properties { get; set; } = [];
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
    public IPropertyHandler Handler { get; set; } = null!;

    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    #endregion

    #region Methods

    public async Task OnDeleteButtonClickedAsync(long id, string title)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", $"Ao prosseguir o imóvel {title} será excluido. " +
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

        await OnDeleteAsync(id, title);
        StateHasChanged();
    }

    private async Task OnDeleteAsync(long id, string name)
    {
        try
        {
            await Handler.DeleteAsync(new DeletePropertyRequest { Id = id });
            Properties.RemoveAll(x => x.Id == id);
            await DataGrid.ReloadServerData();
            Snackbar.Add($"Imóvel {name} excluído", Severity.Success);
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Success);
        }
    }

    public async Task<GridData<Property>> LoadServerData(GridState<Property> state)
    {
        try
        {
            var request = new GetAllPropertiesRequest
            {
                PageNumber = state.Page + 1,
                PageSize = state.PageSize,
                SearchTerm = SearchTerm
            };

            var response = await Handler.GetAllAsync(request);
            if (response.IsSuccess)
                return new GridData<Property>
                {
                    Items = response.Data ?? [],
                    TotalItems = response.TotalCount
                };

            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            return new GridData<Property>
            {
                Items = [],
                TotalItems = 0
            };
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
            return new GridData<Property>
            {
                Items = [],
                TotalItems = 0
            };
        }
    }

    #endregion
}