using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Condominiums;
using RealtyHub.Web.Components.Common;

namespace RealtyHub.Web.Pages.Condominiums;

public class ListCondominiumsPage : ComponentBase
{
    #region Parameters

    [Parameter]
    public EventCallback<Condominium> OnCondominiumSelected { get; set; }

    [Parameter]
    public string RowStyle { get; set; } = string.Empty;

    #endregion

    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    [Inject]
    public ICondominiumHandler Handler { get; set; } = null!;

    #endregion

    #region Properties

    public MudDataGrid<Condominium> DataGrid { get; set; } = null!;
    public List<Condominium> Condominiums { get; set; } = [];
    public string SearchTerm { get; set; } = string.Empty;
    public string SelectedFilter { get; set; } = string.Empty;

    public readonly List<FilterOption> FilterOptions =
    [
        new() { DisplayName = "Nome", PropertyName = "Name" },
        new() { DisplayName = "Bairro", PropertyName = "Address.Neighborhood" },
        new() { DisplayName = "Rua", PropertyName = "Address.Street" },
        new() { DisplayName = "Cidade", PropertyName = "Address.City" },
        new() { DisplayName = "Estado", PropertyName = "Address.State" },
        new() { DisplayName = "CEP", PropertyName = "Address.ZipCode" },
        new() { DisplayName = "País", PropertyName = "Address.Country" },
        new() { DisplayName = "Unidades", PropertyName = "Units" },
        new() { DisplayName = "Andares", PropertyName = "Floors" },
        new() { DisplayName = "Elevador", PropertyName = "HasElevator" },
        new() { DisplayName = "Piscina", PropertyName = "HasSwimmingPool" },
        new() { DisplayName = "Salão de Festas", PropertyName = "HasPartyRoom" },
        new() { DisplayName = "Playground", PropertyName = "HasPlayground" },
        new() { DisplayName = "Academia", PropertyName = "HasFitnessRoom" }
    ];

    #endregion

    #region Methods

    public async Task OnDeleteButtonClickedAsync(long id, string name)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", $"Ao prosseguir o condomínio {name} será excluido. " +
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

        await OnDeleteAsync(id, name);
        StateHasChanged();
    }

    private async Task OnDeleteAsync(long id, string name)
    {
        try
        {
            await Handler.DeleteAsync(new DeleteCondominiumRequest { Id = id });
            Condominiums.RemoveAll(x => x.Id == id);
            await DataGrid.ReloadServerData();
            Snackbar.Add($"Condomínio {name} excluído", Severity.Success);
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Success);
        }
    }

    public async Task<GridData<Condominium>> LoadServerData(GridState<Condominium> state)
    {
        try
        {
            var request = new GetAllCondominiumsRequest
            {
                PageNumber = state.Page + 1,
                PageSize = state.PageSize,
                SearchTerm = SearchTerm,
                FilterBy = SelectedFilter
            };

            var response = await Handler.GetAllAsync(request);
            if (response.IsSuccess)
                return new GridData<Condominium>
                {
                    Items = response.Data ?? [],
                    TotalItems = response.TotalCount
                };

            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            return new GridData<Condominium>();
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
            return new GridData<Condominium>();
        }
    }

    public void OnButtonSearchClick() => DataGrid.ReloadServerData();

    public void OnClearSearchClick()
    {
        SearchTerm = string.Empty;
        SelectedFilter = string.Empty;
        DataGrid.ReloadServerData();
    }

    public void OnValueFilterChanged(string newValue)
    {
        SelectedFilter = newValue;
        StateHasChanged();
    }

    public async Task SelectCondominium(Condominium condominium)
    {
        if (OnCondominiumSelected.HasDelegate)
            await OnCondominiumSelected.InvokeAsync(condominium);
    }

    #endregion
}