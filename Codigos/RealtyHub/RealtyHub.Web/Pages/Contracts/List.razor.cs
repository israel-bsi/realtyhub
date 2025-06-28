using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using RealtyHub.Core.Extensions;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Web.Components.Common;

namespace RealtyHub.Web.Pages.Contracts;

/// <summary>
/// Página responsável pela listagem e gerenciamento de contratos na aplicação.
/// </summary>
public partial class ListContractsPage : ComponentBase
{
    #region Properties

    /// <summary>
    /// Componente de grid para exibir a lista de contratos.
    /// </summary>
    public MudDataGrid<Contrato> DataGrid { get; set; } = null!;

    /// <summary>
    /// Lista de contratos a serem exibidos no grid.
    /// </summary>
    public List<Contrato> Contracts { get; set; } = [];

    /// <summary>
    /// Intervalo de datas utilizado para filtrar os contratos.
    /// O intervalo é definido com a data inicial sendo o primeiro dia do mês atual
    /// e a data final sendo o último dia do mês atual.
    /// </summary>
    public DateRange DateRange { get; set; } = new(DateTime.Now.GetFirstDay(),
        DateTime.Now.GetLastDay());

    #endregion

    #region Services

    /// <summary>
    /// Serviço para exibição de mensagens de notificação.
    /// </summary>
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    /// <summary>
    /// Handler responsável pelas operações relacionadas a contratos.
    /// </summary>
    [Inject]
    public IContractHandler Handler { get; set; } = null!;

    /// <summary>
    /// Serviço para exibição de diálogos modais.
    /// </summary>
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    /// <summary>
    /// Serviço JavaScript para interações com o navegador (ex.: abrir nova aba).
    /// </summary>
    [Inject]
    public IJSRuntime JsRuntime { get; set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Carrega os dados do servidor de forma paginada para exibição no grid.
    /// </summary>
    /// <param name="state">Estado atual do grid contendo informações sobre paginação.</param>
    /// <returns>
    /// Um objeto <see cref="GridData{Contract}"/> contendo a lista de contratos e a contagem total.
    /// Em caso de falha, exibe uma mensagem de erro e retorna um grid vazio.
    /// </returns>
    public async Task<GridData<Contrato>> LoadServerData(GridState<Contrato> state)
    {
        try
        {
            var endDate = DateRange.End?.ToEndOfDay();
            var request = new GetAllContractsRequest
            {
                PageNumber = state.Page + 1,
                PageSize = state.PageSize,
                StartDate = DateRange.Start.ToUtcString(),
                EndDate = endDate.ToUtcString()
            };

            var response = await Handler.GetAllAsync(request);
            if (response.IsSuccess)
            {
                Contracts = response.Data ?? [];
                return new GridData<Contrato>
                {
                    Items = Contracts.OrderByDescending(c => c.AtualizadoEm),
                    TotalItems = response.TotalCount
                };
            }

            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            return new GridData<Contrato>();
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
            return new GridData<Contrato>();
        }
    }

    /// <summary>
    /// Atualiza o intervalo de datas utilizado para filtrar os contratos e recarrega os dados do grid.
    /// </summary>
    /// <param name="newDateRange">Novo intervalo de datas selecionado.</param>
    public void OnDateRangeChanged(DateRange newDateRange)
    {
        DateRange = newDateRange;
        DataGrid.ReloadServerData();
    }

    /// <summary>
    /// Abre o arquivo PDF do contrato em uma nova aba do navegador.
    /// </summary>
    /// <param name="contrato">Contrato cuja visualização em PDF será aberta.</param>
    /// <returns>Task representando a operação assíncrona.</returns>
    public async Task ShowInNewPage(Contrato contrato)
    {
        await JsRuntime.InvokeVoidAsync("openContractPdfInNewTab", contrato.CaminhoArquivo);
    }

    /// <summary>
    /// Abre um diálogo de confirmação para exclusão do contrato e, se confirmado, executa a exclusão.
    /// </summary>
    /// <param name="id">Identificador do contrato a ser excluído.</param>
    /// <returns>Task representando a operação assíncrona.</returns>
    public async Task OnDeleteButtonClickedAsync(long id)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", $"Ao prosseguir o contrato com identificador <b>{id}</b> será excluido. " +
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

        await OnDeleteAsync(id);
        StateHasChanged();
    }

    /// <summary>
    /// Executa a exclusão efetiva do contrato.
    /// </summary>
    /// <param name="id">Identificador do contrato que será deletado.</param>
    /// <returns>Task representando a operação assíncrona.</returns>
    private async Task OnDeleteAsync(long id)
    {
        try
        {
            await Handler.DeleteAsync(new DeleteContractRequest { Id = id });
            Contracts.RemoveAll(x => x.Id == id);
            await DataGrid.ReloadServerData();
            Snackbar.Add($"Contrato {id} excluído", Severity.Success);
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
        }
    }

    #endregion
}