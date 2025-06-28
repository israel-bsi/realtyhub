using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Web.Components.Common;

namespace RealtyHub.Web.Pages.Customers;

/// <summary>
/// Página responsável por exibir e gerenciar a listagem de clientes na aplicação.
/// </summary>
public partial class ListCustomersPage : ComponentBase
{
    #region Parameters

    /// <summary>
    /// Evento chamado quando um cliente é selecionado.
    /// </summary>
    [Parameter]
    public EventCallback<Cliente> OnCustomerSelected { get; set; }

    /// <summary>
    /// Estilo CSS aplicado às linhas da tabela.
    /// </summary>
    [Parameter]
    public string RowStyle { get; set; } = string.Empty;

    #endregion

    #region Properties

    /// <summary>
    /// Componente de grid do MudBlazor utilizado para exibir a lista de clientes.
    /// </summary>
    public MudDataGrid<Cliente> DataGrid { get; set; } = null!;

    /// <summary>
    /// Lista de clientes a serem exibidos no grid.
    /// </summary>
    public List<Cliente> Customers { get; set; } = [];

    private string _searchTerm = string.Empty;

    /// <summary>
    /// Termo de busca utilizado para filtrar os clientes.
    /// Ao alterar o valor, o grid é recarregado.
    /// </summary>
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

    /// <summary>
    /// Serviço para exibição de notificações na tela.
    /// </summary>
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    /// <summary>
    /// Handler responsável por gerenciar as operações relacionadas aos clientes.
    /// </summary>
    [Inject]
    public ICustomerHandler Handler { get; set; } = null!;

    /// <summary>
    /// Serviço utilizado para exibir diálogos modais.
    /// </summary>
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Exibe um diálogo de confirmação e, se confirmado, executa a exclusão do cliente.
    /// </summary>
    /// <param name="id">Identificador do cliente a ser excluído.</param>
    /// <param name="name">Nome do cliente, para exibição na mensagem de confirmação.</param>
    public async Task OnDeleteButtonClickedAsync(long id, string name)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", $"Ao prosseguir o cliente {name} será excluido. " +
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
    /// Executa a exclusão efetiva do cliente.
    /// </summary>
    /// <param name="id">Identificador do cliente a ser excluído.</param>
    /// <param name="name">Nome do cliente, utilizado para exibir a notificação de sucesso.</param>
    private async Task OnDeleteAsync(long id, string name)
    {
        try
        {
            await Handler.DeleteAsync(new DeleteCustomerRequest { Id = id });
            Customers.RemoveAll(x => x.Id == id);
            await DataGrid.ReloadServerData();
            Snackbar.Add($"Cliente {name} excluído", Severity.Success);
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
        }
    }

    /// <summary>
    /// Carrega os dados dos clientes do servidor de forma paginada para exibição no grid.
    /// </summary>
    /// <param name="state">Estado atual do grid, contendo informações de paginação.</param>
    /// <returns>
    /// Um objeto <see cref="GridData{Customer}"/> contendo a lista de clientes e a contagem total.
    /// Em caso de falha, retorna um grid vazio e exibe uma mensagem de erro.
    /// </returns>
    public async Task<GridData<Cliente>> LoadServerData(GridState<Cliente> state)
    {
        try
        {
            var request = new GetAllCustomersRequest
            {
                PageNumber = state.Page + 1,
                PageSize = state.PageSize,
                SearchTerm = SearchTerm
            };

            var response = await Handler.GetAllAsync(request);
            if (response.IsSuccess)
            {
                return new GridData<Cliente>
                {
                    Items = response.Data ?? [],
                    TotalItems = response.TotalCount
                };
            }

            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            return new GridData<Cliente>
            {
                Items = [],
                TotalItems = 0
            };
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
            return new GridData<Cliente>
            {
                Items = [],
                TotalItems = 0
            };
        }
    }

    /// <summary>
    /// Notifica o componente pai que um cliente foi selecionado.
    /// </summary>
    /// <param name="cliente">Cliente selecionado.</param>
    public async Task SelectCustomer(Cliente cliente)
    {
        if (OnCustomerSelected.HasDelegate)
            await OnCustomerSelected.InvokeAsync(cliente);
    }

    #endregion
}