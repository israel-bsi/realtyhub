using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Condominiums;
using RealtyHub.Web.Components.Common;

namespace RealtyHub.Web.Pages.Condominiums;

/// <summary>
/// Página responsável pela listagem e gerenciamento de condomínios na aplicação.
/// </summary>
public class ListCondominiumsPage : ComponentBase
{
    #region Parameters

    /// <summary>
    /// Delegate para notificar a seleção de um condomínio.
    /// </summary>
    [Parameter]
    public EventCallback<Condominio> OnCondominiumSelected { get; set; }

    /// <summary>
    /// Estilo CSS aplicado às linhas da tabela.
    /// </summary>
    [Parameter]
    public string RowStyle { get; set; } = string.Empty;

    #endregion

    #region Services

    /// <summary>
    /// Serviço para exibição de mensagens de notificação.
    /// </summary>
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    /// <summary>
    /// Serviço para exibição de diálogos modais.
    /// </summary>
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    /// <summary>
    /// Handler para operações relacionadas aos condomínios.
    /// </summary>
    [Inject]
    public ICondominiumHandler Handler { get; set; } = null!;

    #endregion

    #region Properties

    /// <summary>
    /// Componente de grid para exibir a lista de condomínios.
    /// </summary>
    public MudDataGrid<Condominio> DataGrid { get; set; } = null!;

    /// <summary>
    /// Lista de condomínios carregados do servidor.
    /// </summary>
    public List<Condominio> Condominiums { get; set; } = [];

    /// <summary>
    /// Termo de busca utilizado para filtrar a lista de condomínios.
    /// </summary>
    public string SearchTerm { get; set; } = string.Empty;

    /// <summary>
    /// Filtro selecionado para a busca.
    /// </summary>
    public string SelectedFilter { get; set; } = string.Empty;

    /// <summary>
    /// Lista de opções de filtro disponíveis para a busca.
    /// </summary>
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

    /// <summary>
    /// Método chamado quando o botão de exclusão é clicado.
    /// </summary>
    /// <param name="id">Identificador do condomínio a ser excluído.</param>
    /// <param name="name">Nome do condomínio (para exibição na mensagem de confirmação).</param>
    /// <returns>Task representando a operação assíncrona.</returns>
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

    /// <summary>
    /// Executa a operação de exclusão efetiva de um condomínio.
    /// </summary>
    /// <param name="id">Identificador do condomínio a ser excluído.</param>
    /// <param name="name">Nome do condomínio (utilizado para exibir a mensagem de sucesso).</param>
    /// <returns>Task representando a operação assíncrona.</returns>
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
            Snackbar.Add(e.Message, Severity.Error);
        }
    }

    /// <summary>
    /// Carrega os dados do servidor de forma paginada para exibição no grid.
    /// </summary>
    /// <param name="state">Estado atual do grid contendo paginação e filtros.</param>
    /// <returns>Um objeto <see cref="GridData{Condominium}"/> contendo a lista de condomínios e a contagem total.</returns>
    public async Task<GridData<Condominio>> LoadServerData(GridState<Condominio> state)
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
                return new GridData<Condominio>
                {
                    Items = response.Data ?? [],
                    TotalItems = response.TotalCount
                };

            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            return new GridData<Condominio>();
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
            return new GridData<Condominio>();
        }
    }

    /// <summary>
    /// Aciona a recarga dos dados do grid quando o botão de busca é clicado.
    /// </summary>
    public void OnButtonSearchClick() => DataGrid.ReloadServerData();

    /// <summary>
    /// Limpa o termo de busca e o filtro, e recarrega os dados do grid.
    /// </summary>
    public void OnClearSearchClick()
    {
        SearchTerm = string.Empty;
        SelectedFilter = string.Empty;
        DataGrid.ReloadServerData();
    }

    /// <summary>
    /// Manipula a alteração do valor do filtro selecionado.
    /// </summary>
    /// <param name="newValue">Novo valor do filtro.</param>
    public void OnValueFilterChanged(string newValue)
    {
        SelectedFilter = newValue;
        StateHasChanged();
    }

    /// <summary>
    /// Notifica ao componente pai que um condomínio foi selecionado.
    /// </summary>
    /// <param name="condominio">O condomínio selecionado.</param>
    /// <returns>Task representando a operação assíncrona.</returns>
    public async Task SelectCondominium(Condominio condominio)
    {
        if (OnCondominiumSelected.HasDelegate)
            await OnCondominiumSelected.InvokeAsync(condominio);
    }

    #endregion
}