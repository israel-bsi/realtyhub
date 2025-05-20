using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Core.Responses;
using RealtyHub.Web.Components.Customers;
using RealtyHub.Web.Components.Properties;
using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Web.Components.Viewings;

/// <summary>
/// Componente responsável pelo formulário de agendamento e reagendamento de visitas.
/// </summary>
public partial class ViewingFormComponent : ComponentBase
{
    #region Parameters

    /// <summary>
    /// Identificador da visita. Se diferente de zero, o formulário entra em modo de edição/reagendamento.
    /// </summary>
    [Parameter]
    public long Id { get; set; }

    /// <summary>
    /// Cliente associado à visita (opcional).
    /// </summary>
    [Parameter]
    public Customer? Customer { get; set; }

    /// <summary>
    /// Imóvel associado à visita (opcional).
    /// </summary>
    [Parameter]
    public Property? Property { get; set; }

    /// <summary>
    /// Indica se a busca de imóveis está bloqueada.
    /// </summary>
    [Parameter]
    public bool LockPropertySearch { get; set; }

    /// <summary>
    /// Indica se a busca de clientes está bloqueada.
    /// </summary>
    [Parameter]
    public bool LockCustomerSearch { get; set; }

    /// <summary>
    /// Indica se deve redirecionar para a listagem após submissão.
    /// </summary>
    [Parameter]
    public bool RedirectToPageList { get; set; } = true;

    /// <summary>
    /// Evento disparado ao clicar no botão de submissão do formulário.
    /// </summary>
    [Parameter]
    public EventCallback OnSubmitButtonClicked { get; set; }

    #endregion

    #region Properties

    /// <summary>
    /// Indica a operação atual do formulário ("Reagendar" ou "Agendar").
    /// </summary>
    public string Operation => Id != 0
        ? "Reagendar" : "Agendar";

    /// <summary>
    /// Modelo de entrada utilizado para o binding dos campos do formulário.
    /// </summary>
    public Viewing InputModel { get; set; } = new();

    /// <summary>
    /// Horário selecionado para a visita.
    /// </summary>
    [DataType(DataType.Time)]
    public TimeSpan? ViewingTime { get; set; } = DateTime.Now.TimeOfDay;

    /// <summary>
    /// Indica se o formulário está em estado de carregamento ou processamento.
    /// </summary>
    public bool IsBusy { get; set; }

    /// <summary>
    /// Chave para forçar atualização do formulário.
    /// </summary>
    public int EditFormKey { get; set; }

    #endregion

    #region Services

    /// <summary>
    /// Handler responsável pelas operações de visitas.
    /// </summary>
    [Inject]
    public IViewingHandler ViewingHandler { get; set; } = null!;

    /// <summary>
    /// Serviço de navegação para redirecionamento de páginas.
    /// </summary>
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    /// <summary>
    /// Serviço para exibição de mensagens e notificações.
    /// </summary>
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    /// <summary>
    /// Serviço de diálogo para exibição de modais.
    /// </summary>
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Manipula o envio válido do formulário, realizando o agendamento ou reagendamento da visita.
    /// </summary>
    /// <remarks>
    /// Valida os campos obrigatórios, ajusta a data/hora da visita, chama o handler para agendar ou reagendar,
    /// exibe mensagens de sucesso ou erro via Snackbar, dispara o evento de submissão e redireciona para a listagem se necessário.
    /// </remarks>
    public async Task OnValidSubmitAsync()
    {
        if (IsFormInvalid()) return;
        IsBusy = true;
        try
        {
            if (InputModel.ViewingDate.HasValue && ViewingTime.HasValue)
            {
                InputModel.ViewingDate = InputModel
                    .ViewingDate.Value.Date.Add(ViewingTime.Value)
                    .ToUniversalTime();
            }

            Response<Viewing?> result;
            if (Id == 0)
                result = await ViewingHandler.ScheduleAsync(InputModel);
            else
                result = await ViewingHandler.RescheduleAsync(InputModel);

            var resultMessage = result.Message ?? string.Empty;
            if (!result.IsSuccess)
            {
                Snackbar.Add(resultMessage, Severity.Error);
                return;
            }

            await OnSubmitButtonClickedAsync();
            Snackbar.Add(resultMessage, Severity.Success);
            if (RedirectToPageList)
                NavigationManager.NavigateTo("/visitas");
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

    /// <summary>
    /// Valida se os campos obrigatórios do formulário foram preenchidos.
    /// </summary>
    /// <remarks>
    /// Exibe mensagens de erro via Snackbar caso cliente ou imóvel não estejam informados.
    /// </remarks>
    /// <returns>True se o formulário for inválido, caso contrário false.</returns>
    private bool IsFormInvalid()
    {
        if (InputModel.Buyer is null && InputModel.Property is null)
        {
            Snackbar.Add("Informe o cliente e o imóvel", Severity.Error);
            return true;
        }
        if (InputModel.Buyer is null)
        {
            Snackbar.Add("Informe o cliente", Severity.Error);
            return true;
        }
        if (InputModel.Property is null)
        {
            Snackbar.Add("Informe o imóvel", Severity.Error);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Carrega os dados da visita para edição a partir do ID informado.
    /// </summary>
    /// <remarks>
    /// Busca os dados da visita via handler, preenche o modelo de entrada e ajusta o horário.
    /// Em caso de erro, exibe mensagem e redireciona para a listagem de visitas.
    /// </remarks>
    private async Task LoadViewingAsync()
    {
        GetViewingByIdRequest? request = null;
        try
        {
            request = new GetViewingByIdRequest { Id = Id };
        }
        catch
        {
            Snackbar.Add("Parâmetro inválido", Severity.Error);
        }

        if (request is null) return;

        var response = await ViewingHandler.GetByIdAsync(request);
        if (response is { IsSuccess: true, Data: not null })
        {
            InputModel.Id = response.Data.Id;
            InputModel.ViewingDate = response.Data.ViewingDate;
            InputModel.ViewingStatus = response.Data.ViewingStatus;
            InputModel.Buyer = response.Data.Buyer;
            InputModel.BuyerId = response.Data.BuyerId;
            InputModel.Property = response.Data.Property;
            InputModel.PropertyId = response.Data.PropertyId;
            InputModel.UserId = response.Data.UserId;
            ViewingTime = InputModel.ViewingDate!.Value.TimeOfDay;
        }
        else
        {
            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            NavigationManager.NavigateTo("/visitas");
        }
    }

    /// <summary>
    /// Marca a visita como realizada.
    /// </summary>
    /// <remarks>
    /// Chama o handler para finalizar a visita e atualiza o status no modelo.
    /// Exibe mensagem informativa via Snackbar.
    /// </remarks>
    public async Task OnClickDoneViewing()
    {
        var request = new DoneViewingRequest { Id = InputModel.Id };
        var result = await ViewingHandler.DoneAsync(request);
        var resultMessage = result.Message ?? string.Empty;
        if (result is { IsSuccess: true, Data: not null })
            InputModel.ViewingStatus = result.Data.ViewingStatus;

        Snackbar.Add(resultMessage, Severity.Info);
        StateHasChanged();
    }

    /// <summary>
    /// Cancela a visita.
    /// </summary>
    /// <remarks>
    /// Chama o handler para cancelar a visita e atualiza o status no modelo.
    /// Exibe mensagem informativa via Snackbar.
    /// </remarks>
    public async Task OnClickCancelViewing()
    {
        var request = new CancelViewingRequest { Id = InputModel.Id };
        var result = await ViewingHandler.CancelAsync(request);
        var resultMessage = result.Message ?? string.Empty;
        if (result is { IsSuccess: true, Data: not null })
            InputModel.ViewingStatus = result.Data.ViewingStatus;

        Snackbar.Add(resultMessage, Severity.Info);
        StateHasChanged();
    }

    /// <summary>
    /// Abre o diálogo para seleção do cliente.
    /// </summary>
    /// <remarks>
    /// Exibe um diálogo modal para seleção de um cliente. O diálogo só é aberto se <see cref="LockCustomerSearch"/> for falso.
    /// </remarks>
    public async Task OpenCustomerDialog()
    {
        if (LockCustomerSearch) return;
        var parameters = new DialogParameters
        {
            { "OnCustomerSelected", EventCallback.Factory
                .Create<Customer>(this, SelectedCustomer) }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true
        };
        var dialog = await DialogService
            .ShowAsync<CustomerDialog>("Informe o Cliente", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: Customer selectedCustomer })
            SelectedCustomer(selectedCustomer);
    }

    /// <summary>
    /// Manipula a seleção do cliente no diálogo, atualizando o modelo da visita.
    /// </summary>
    /// <param name="customer">Cliente selecionado.</param>
    private void SelectedCustomer(Customer customer)
    {
        InputModel.Buyer = customer;
        InputModel.BuyerId = customer.Id;
        EditFormKey++;
        StateHasChanged();
    }

    /// <summary>
    /// Abre o diálogo para seleção do imóvel.
    /// </summary>
    /// <remarks>
    /// Exibe um diálogo modal para seleção de um imóvel. O diálogo só é aberto se <see cref="LockPropertySearch"/> for falso.
    /// </remarks>
    public async Task OpenPropertyDialog()
    {
        if (LockPropertySearch) return;
        var parameters = new DialogParameters
        {
            { "OnPropertySelected", EventCallback.Factory
                .Create<Property>(this, SelectedProperty) }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.ExtraLarge,
            FullWidth = true
        };
        var dialog = await DialogService
            .ShowAsync<PropertyDialog>("Informe o Imóvel", parameters, options);

        var result = await dialog.Result;

        if (result is { Canceled: false, Data: Property selectedProperty })
            SelectedProperty(selectedProperty);
    }

    /// <summary>
    /// Manipula a seleção do imóvel no diálogo, atualizando o modelo da visita.
    /// </summary>
    /// <param name="property">Imóvel selecionado.</param>
    private void SelectedProperty(Property property)
    {
        InputModel.Property = property;
        InputModel.PropertyId = property.Id;
        EditFormKey++;
        StateHasChanged();
    }

    /// <summary>
    /// Dispara o evento de clique no botão de submissão, se houver delegate.
    /// </summary>
    /// <remarks>
    /// Verifica se existe um delegate associado ao evento <see cref="OnSubmitButtonClicked"/> e o executa de forma assíncrona.
    /// Permite que componentes pais sejam notificados após a submissão do formulário.
    /// </remarks>
    private async Task OnSubmitButtonClickedAsync()
    {
        if (OnSubmitButtonClicked.HasDelegate)
            await OnSubmitButtonClicked.InvokeAsync();
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Inicializa o componente, carregando os dados da visita para edição ou preparando para agendamento.
    /// </summary>
    /// <remarks>
    /// Se <see cref="Id"/> for diferente de zero, busca os dados da visita para edição.
    /// Caso contrário, preenche os dados iniciais com o cliente e imóvel informados por parâmetro.
    /// Exibe mensagens de erro via Snackbar em caso de falha.
    /// </remarks>
    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        try
        {
            if (Id != 0)
                await LoadViewingAsync();
            else
            {
                InputModel.Buyer = Customer;
                InputModel.BuyerId = Customer?.Id ?? 0;
                InputModel.Property = Property;
                InputModel.PropertyId = Property?.Id ?? 0;
            }
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