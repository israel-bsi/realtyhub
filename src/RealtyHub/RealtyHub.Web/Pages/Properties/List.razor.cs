using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Web.Components.Common;

namespace RealtyHub.Web.Pages.Properties;

public partial class ListPropertiesPage : ComponentBase
{
    #region Parameters

    [Parameter]
    public EventCallback<Property> OnPropertySelected { get; set; }

    [Parameter]
    public string RowStyle { get; set; } = string.Empty;

    #endregion

    #region Properties

    public MudDataGrid<Property> DataGrid { get; set; } = null!;
    public List<Property> Properties { get; set; } = [];
    public string SearchTerm { get; set; } = string.Empty;
    public string SelectedFilter { get; set; } = string.Empty;

    public readonly List<FilterOption> FilterOptions = new()
    {
        new FilterOption { DisplayName = "Título", PropertyName = "Title" },
        new FilterOption { DisplayName = "Descrição", PropertyName = "Description" },
        new FilterOption { DisplayName = "Bairro", PropertyName = "Address.Neighborhood" },
        new FilterOption { DisplayName = "Rua", PropertyName = "Address.Street" },
        new FilterOption { DisplayName = "Cidade", PropertyName = "Address.City" },
        new FilterOption { DisplayName = "Estado", PropertyName = "Address.State" },
        new FilterOption { DisplayName = "CEP", PropertyName = "Address.ZipCode" },
        new FilterOption { DisplayName = "País", PropertyName = "Address.Country" }
    };

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
                SearchTerm = SearchTerm,
                FilterBy = SelectedFilter
            };

            var response = await Handler.GetAllAsync(request);
            if (response.IsSuccess)
                return new GridData<Property>
                {
                    Items = response.Data ?? [],
                    TotalItems = response.TotalCount
                };

            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            return new GridData<Property>();
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
            return new GridData<Property>();
        }
    }

    public void OnButtonSearchClick() => DataGrid.ReloadServerData();

    public void OnClearSearchClick()
    {
        SearchTerm = string.Empty;
        DataGrid.ReloadServerData();
    }

    public void OnValueFilterChanged(string newValue)
    {
        SelectedFilter = newValue;
        StateHasChanged();
    }

    public async Task SelectProperty(Property property)
    {
        if (OnPropertySelected.HasDelegate)
            await OnPropertySelected.InvokeAsync(property);
    }
    
    public string GetSrcThumbnailPhoto(Property property)
    {
        var photo = property
            .PropertyPhotos
            .FirstOrDefault(p=>p.IsThumbnail);

        return $"{Configuration.BackendUrl}/photos/{photo?.Id}{photo?.Extension}";
    }

    #endregion
}