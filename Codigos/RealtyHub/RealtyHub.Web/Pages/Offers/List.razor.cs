using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Extensions;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Web.Components.Common;
using RealtyHub.Web.Components.Offers;
using RealtyHub.Web.Services;

namespace RealtyHub.Web.Pages.Offers;

/// <summary>
/// Página responsável por exibir e gerenciar a listagem de propostas de compra de imóvel na aplicação.
/// </summary>
public partial class ListOffersPage : ComponentBase
{
    #region Parameters

    /// <summary>
    /// Evento chamado quando uma proposta é selecionada.
    /// </summary>
    [Parameter]
    public EventCallback<Proposta> OnOfferSelected { get; set; }

    /// <summary>
    /// Estilo CSS aplicado às linhas da tabela.
    /// </summary>
    [Parameter]
    public string RowStyle { get; set; } = string.Empty;

    /// <summary>
    /// Indica se somente as propostas aceitas devem ser exibidas.
    /// </summary>
    [Parameter]
    public bool OnlyAccepted { get; set; }

    #endregion

    #region Properties

    /// <summary>
    /// Componente de grid do MudBlazor utilizado para exibir a lista de propostas.
    /// </summary>
    public MudDataGrid<Proposta> DataGrid { get; set; } = null!;

    /// <summary>
    /// Intervalo de datas utilizado para filtrar as propostas.
    /// Inicia no primeiro dia do mês atual e termina no último dia do mês atual.
    /// </summary>
    public DateRange DateRange { get; set; } = new(DateTime.Now.GetFirstDay(), DateTime.Now.GetLastDay());

    /// <summary>
    /// Lista de propostas a serem exibidas no grid.
    /// </summary>
    public List<Proposta> Items { get; set; } = [];

    #endregion

    #region Services

    /// <summary>
    /// Serviço para exibição de diálogos modais.
    /// </summary>
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    /// <summary>
    /// Serviço para exibição de notificações na tela.
    /// </summary>
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    /// <summary>
    /// Handler responsável pelas operações relacionadas a propostas.
    /// </summary>
    [Inject]
    public IOfferHandler Handler { get; set; } = null!;

    /// <summary>
    /// Serviço responsável por gerar relatórios de propostas.
    /// </summary>
    [Inject]
    public OfferReport OfferReport { get; set; } = null!;

    /// <summary>
    /// Serviço JavaScript para interações com o navegador, como abrir um PDF em nova aba.
    /// </summary>
    [Inject]
    public IJSRuntime JsRuntime { get; set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Manipula o clique do botão de gerar relatório da proposta.
    /// Caso o relatório seja gerado com sucesso, abre o PDF em uma nova aba.
    /// </summary>
    /// <returns>Task representando a operação assíncrona.</returns>
    public async Task OnReportClickedAsync()
    {
        var result = await OfferReport.GetOfferAsync();
        if (result.IsSuccess)
        {
            await JsRuntime.InvokeVoidAsync("openContractPdfInNewTab", result.Data?.Url);
            return;
        }
        Snackbar.Add(result.Message ?? string.Empty, Severity.Error);
    }

    /// <summary>
    /// Carrega os dados das propostas do servidor de forma paginada para exibição no grid.
    /// </summary>
    /// <param name="state">Estado atual do grid contendo informações de paginação.</param>
    /// <returns>
    /// Um objeto <see cref="GridData{Offer}"/> contendo a lista de propostas e a contagem total.
    /// Caso ocorra uma falha, retorna um grid vazio e exibe uma mensagem de erro.
    /// </returns>
    public async Task<GridData<Proposta>> LoadServerData(GridState<Proposta> state)
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
                        .Where(o => o.StatusProposta == EStatusProposta.Aceita)
                        .ToList() ?? new List<Proposta>();
                else
                    Items = response.Data ?? new List<Proposta>();

                return new GridData<Proposta>
                {
                    Items = Items.OrderByDescending(o => o.AtualizadoEm),
                    TotalItems = response.TotalCount
                };
            }

            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            return new GridData<Proposta>();
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
            return new GridData<Proposta>();
        }
    }

    /// <summary>
    /// Atualiza o intervalo de datas utilizado para filtrar as propostas e recarrega os dados do grid.
    /// </summary>
    /// <param name="newDateRange">Novo intervalo de datas selecionado.</param>
    public void OnDateRangeChanged(DateRange newDateRange)
    {
        DateRange = newDateRange;
        DataGrid.ReloadServerData();
    }

    /// <summary>
    /// Manipula o clique para aceitar uma proposta.
    /// Exibe um diálogo de confirmação e, se confirmado, envia a solicitação para aceitar a proposta.
    /// Após a operação, exibe uma notificação e recarrega o grid.
    /// </summary>
    /// <param name="proposta">Proposta a ser aceita.</param>
    public async Task OnAcceptClickedAsync(Proposta proposta)
    {
        if (IsOfferStatusInvalid(proposta)) return;
        var parameters = new DialogParameters
        {
            { "ContentText", "Deseja aceitar a proposta?<br>" +
                             "Após a alteração não será mais possível editar!" },
            { "ButtonColor", Color.Success }
        };
        var dialog = await DialogService.ShowAsync<DialogConfirm>("Confirmação", parameters);
        if (await dialog.Result is { Canceled: true }) return;

        var request = new AcceptOfferRequest { Id = proposta.Id };
        var result = await Handler.AcceptAsync(request);
        if (result is { IsSuccess: true, Data: not null })
        {
            Snackbar.Add("Proposta aceita", Severity.Success);
            await DataGrid.ReloadServerData();
        }
        else
        {
            Snackbar.Add(result.Message ?? string.Empty, Severity.Error);
        }
    }

    /// <summary>
    /// Manipula o clique para rejeitar uma proposta.
    /// Exibe um diálogo de confirmação e, se confirmado, envia a solicitação para rejeitar a proposta.
    /// Após a operação, exibe uma notificação e recarrega o grid.
    /// </summary>
    /// <param name="proposta">Proposta a ser rejeitada.</param>
    public async Task OnRejectClickedAsync(Proposta proposta)
    {
        if (IsOfferStatusInvalid(proposta)) return;
        var parameters = new DialogParameters
        {
            { "ContentText", "Deseja recusar a proposta?<br>" +
                             "Após a alteração não será mais possível editar!" },
            { "ButtonColor", Color.Error }
        };
        var dialog = await DialogService.ShowAsync<DialogConfirm>("Confirmação", parameters);
        if (await dialog.Result is { Canceled: true }) return;

        var request = new RejectOfferRequest { Id = proposta.Id };
        var result = await Handler.RejectAsync(request);
        if (result is { IsSuccess: true, Data: not null })
        {
            Snackbar.Add("Proposta recusada", Severity.Success);
            await DataGrid.ReloadServerData();
        }
        else
        {
            Snackbar.Add(result.Message ?? string.Empty, Severity.Error);
        }
    }

    /// <summary>
    /// Abre uma caixa de diálogo para visualizar os detalhes completos de uma proposta.
    /// </summary>
    /// <param name="proposta">Proposta a ser visualizada.</param>
    public async Task OpenOfferDialog(Proposta proposta)
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
            { "OfferId", proposta.Id },
            { "ReadyOnly", proposta.StatusProposta != EStatusProposta.EmAnalise },
            { "ShowPayments", true }
        };

        await DialogService.ShowAsync<OfferDialog>("Visualizar proposta", parameters, options);
    }

    /// <summary>
    /// Notifica o componente pai que uma proposta foi selecionada.
    /// </summary>
    /// <param name="proposta">Proposta selecionada.</param>
    public async Task SelectOffer(Proposta proposta)
    {
        if (OnOfferSelected.HasDelegate)
            await OnOfferSelected.InvokeAsync(proposta);
    }

    /// <summary>
    /// Verifica se o status da proposta é inválido para alteração.
    /// Se a proposta estiver em status "Accepted" ou "Rejected", exibe uma mensagem de alerta.
    /// </summary>
    /// <param name="proposta">Proposta a ser verificada.</param>
    /// <returns>
    /// True se o status da proposta for inválido para alteração; caso contrário, false.
    /// </returns>
    private bool IsOfferStatusInvalid(Proposta proposta)
    {
        switch (proposta.StatusProposta)
        {
            case EStatusProposta.Aceita:
                Snackbar.Add("Proposta está aceita", Severity.Warning);
                return true;
            case EStatusProposta.Rejeitada:
                Snackbar.Add("Proposta está cancelada", Severity.Warning);
                return true;
            default:
                return false;
        }
    }

    #endregion
}