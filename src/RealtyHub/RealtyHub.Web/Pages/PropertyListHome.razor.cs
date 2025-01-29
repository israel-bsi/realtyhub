using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Web.Components.Offers;

namespace RealtyHub.Web.Pages;

public partial class PropertyListHomePage : ComponentBase
{
    #region Properties

    public bool IsBusy { get; set; }
    public MudDataGrid<Property> DataGrid { get; set; } = null!;
    public List<Property> Properties { get; set; } = [];
    public string SearchTerm { get; set; } = string.Empty;
    public string SelectedFilter { get; set; } = string.Empty;

    public readonly List<FilterOption> FilterOptions = new()
    {
        new FilterOption { DisplayName = "Título", PropertyName = "Title" },
        new FilterOption { DisplayName = "Descrição", PropertyName = "Description" },
        new FilterOption { DisplayName = "Bairro", PropertyName = "Address.Neighborhood" },
        new FilterOption { DisplayName = "Garagens", PropertyName = "Garage" },
        new FilterOption { DisplayName = "Quartos", PropertyName = "Bedroom" },
        new FilterOption { DisplayName = "Banheiros", PropertyName = "Bathroom" },
        new FilterOption { DisplayName = "Área", PropertyName = "Area" },
        new FilterOption { DisplayName = "Preço", PropertyName = "Price" },
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

    public string GetSrcThumbnailPhoto(Property property)
    {
        var photo = property
            .PropertyPhotos
            .FirstOrDefault(p => p.IsThumbnail);

        return $"{Configuration.BackendUrl}/photos/{photo?.Id}{photo?.Extension}";
    }

    public async Task OnSendOfferClickedAsync(Property property)
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
            { "PropertyId", property.Id }
        };

        await DialogService.ShowAsync<OfferDialog>("Enviar proposta", parameters, options);
    }

    #endregion
}