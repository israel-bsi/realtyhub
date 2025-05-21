using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Extensions;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Web.Components.Common;
using RealtyHub.Web.Components.Viewings;

namespace RealtyHub.Web.Pages.Viewings;

/// <summary>
/// Página responsável por exibir e gerenciar a listagem de visitas de um imóvel ou de todas as visitas.
/// </summary>
public partial class ListViewingsPage : ComponentBase
{
    #region Parameters

    /// <summary>
    /// Identificador do imóvel para filtrar as visitas.
    /// Se 0, a listagem exibirá todas as visitas.
    /// </summary>
    [Parameter]
    public long PropertyId { get; set; }

    #endregion

    #region Properties

    /// <summary>
    /// Componente de grid do MudBlazor utilizado para exibir as visitas.
    /// </summary>
    public MudDataGrid<Viewing> DataGrid { get; set; } = null!;

    /// <summary>
    /// Intervalo de datas utilizado para filtrar as visitas.
    /// Inicializado do primeiro ao último dia do mês atual.
    /// </summary>
    public DateRange DateRange { get; set; } = new(DateTime.Now.GetFirstDay(), DateTime.Now.GetLastDay());

    /// <summary>
    /// Lista de visitas a serem exibidas no grid.
    /// </summary>
    public List<Viewing> Items { get; set; } = [];

    /// <summary>
    /// Imóvel associado às visitas. Pode ser nulo se <see cref="PropertyId"/> for 0.
    /// </summary>
    public Property? Property { get; set; }

    /// <summary>
    /// Título do cabeçalho da página.
    /// Exibe "Visitas do imóvel {Property.Title}" se um imóvel estiver definido; caso contrário, exibe "Todas as visitas".
    /// </summary>
    public string HeaderTitle => Property is null ? "Todas as visitas" : $"Visitas do imóvel {Property.Title}";

    #endregion

    #region Services

    /// <summary>
    /// Serviço para exibição de notificações (snackbars) na tela.
    /// </summary>
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    /// <summary>
    /// Handler responsável pelas operações relacionadas a visitas.
    /// </summary>
    [Inject]
    public IViewingHandler ViewingHandler { get; set; } = null!;

    /// <summary>
    /// Handler responsável pelas operações relacionadas aos imóveis.
    /// Utilizado para recuperar informações do imóvel e suas visitas.
    /// </summary>
    [Inject]
    public IPropertyHandler PropertyHandler { get; set; } = null!;

    /// <summary>
    /// Serviço para exibição de diálogos modais.
    /// </summary>
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Carrega os dados das visitas do servidor de forma paginada para exibição no grid.
    /// </summary>
    /// <param name="state">Estado atual do grid, contendo informações de paginação.</param>
    /// <returns>
    /// Um objeto <see cref="GridData{Viewing}"/> contendo a lista de visitas e a contagem total.
    /// Caso ocorra erro, emite uma notificação e retorna um grid vazio.
    /// </returns>
    public async Task<GridData<Viewing>> LoadServerData(GridState<Viewing> state)
    {
        try
        {
            // Ajusta o final do dia para incluir todas as visitas do dia.
            var endDate = DateRange.End?.AddHours(23).AddMinutes(59).AddSeconds(59);
            string message;
            if (PropertyId != 0)
            {
                // Se um imóvel específico foi informado, busca as visitas associadas.
                var request = new GetAllViewingsByPropertyRequest
                {
                    PropertyId = PropertyId,
                    PageNumber = state.Page + 1,
                    PageSize = state.PageSize,
                    StartDate = DateRange.Start?.ToUniversalTime().ToString("o"),
                    EndDate = endDate?.ToUniversalTime().ToString("o")
                };

                var response = await PropertyHandler.GetAllViewingsAsync(request);
                if (response.IsSuccess)
                    return new GridData<Viewing>
                    {
                        Items = response.Data ?? [],
                        TotalItems = response.TotalCount
                    };
                message = response.Message ?? string.Empty;
            }
            else
            {
                // Caso contrário, busca todas as visitas.
                var request = new GetAllViewingsRequest
                {
                    PageNumber = state.Page + 1,
                    PageSize = state.PageSize,
                    StartDate = DateRange.Start?.ToUniversalTime().ToString("o"),
                    EndDate = endDate?.ToUniversalTime().ToString("o")
                };

                var response = await ViewingHandler.GetAllAsync(request);
                if (response.IsSuccess)
                    return new GridData<Viewing>
                    {
                        Items = response.Data ?? [],
                        TotalItems = response.TotalCount
                    };
                message = response.Message ?? string.Empty;
            }

            Snackbar.Add(message, Severity.Error);
            return new GridData<Viewing>();
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
            return new GridData<Viewing>();
        }
    }

    /// <summary>
    /// Dispara a atualização do grid de visitas.
    /// </summary>
    public async Task ReloadDataGridAsync()
        => await DataGrid.ReloadServerData();

    /// <summary>
    /// Abre o diálogo para agendamento de uma nova visita.
    /// Após o agendamento, o grid é recarregado para refletir as mudanças.
    /// </summary>
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
            { "RedirectToPageList", false }
        };
        var dialog = await DialogService.ShowAsync<ViewingDialog>("Agendar visita", parameters, options);
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadDataGridAsync();
    }

    /// <summary>
    /// Abre o diálogo para reagendamento de uma visita.
    /// </summary>
    /// <param name="viewing">A visita a ser reagendada.</param>
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
            { "Id", viewing.Id }
        };
        await DialogService.ShowAsync<ViewingDialog>("Reagendar visita", parameters, options);
        await DataGrid.ReloadServerData();
    }

    /// <summary>
    /// Finaliza a visita, marcando-a como concluída.
    /// </summary>
    /// <param name="viewing">A visita a ser finalizada.</param>
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

    /// <summary>
    /// Cancela a visita.
    /// </summary>
    /// <param name="viewing">A visita a ser cancelada.</param>
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

    /// <summary>
    /// Verifica se o status da visita é inválido para alterações (finalização ou cancelamento).
    /// Exibe uma notificação de alerta caso o status seja "Done" ou "Canceled".
    /// </summary>
    /// <param name="viewing">A visita a ser verificada.</param>
    /// <returns>True se a visita não puder ser alterada; caso contrário, false.</returns>
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
            default:
                return false;
        }
    }

    /// <summary>
    /// Atualiza o intervalo de datas utilizado para filtrar as visitas e recarrega o grid.
    /// </summary>
    /// <param name="newDateRange">Novo intervalo de datas selecionado.</param>
    public void OnDateRangeChanged(DateRange newDateRange)
    {
        DateRange = newDateRange;
        DataGrid.ReloadServerData();
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Inicializa o componente.
    /// Se um <see cref="PropertyId"/> for informado, busca os detalhes do imóvel correspondente.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (PropertyId != 0)
            {
                var request = new GetPropertyByIdRequest { Id = PropertyId };
                var response = await PropertyHandler.GetByIdAsync(request);
                if (response is { IsSuccess: true, Data: not null })
                    Property = response.Data;
                else
                    Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            }
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
        }
    }

    #endregion
}