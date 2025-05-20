using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Requests.Properties;
using System.Text.RegularExpressions;

namespace RealtyHub.Web.Components.Offers;

/// <summary>
/// Componente responsável pelo formulário de envio e edição de propostas de compra de imóvel.
/// </summary>
public partial class OfferFormComponent : ComponentBase
{
    #region Parameters

    /// <summary>
    /// Identificador do imóvel para o qual a proposta será enviada.
    /// </summary>
    [Parameter]
    public long PropertyId { get; set; }

    /// <summary>
    /// Identificador da proposta. Se diferente de zero, o formulário entra em modo de edição.
    /// </summary>
    [Parameter]
    public long OfferId { get; set; }

    /// <summary>
    /// Indica se o formulário está em modo somente leitura.
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Indica se a seção de pagamentos deve ser exibida.
    /// </summary>
    [Parameter]
    public bool ShowPayments { get; set; }

    /// <summary>
    /// Evento disparado ao clicar no botão de submissão do formulário.
    /// </summary>
    [Parameter]
    public EventCallback OnSubmitButtonClicked { get; set; }

    #endregion

    #region Properties

    /// <summary>
    /// Indica se o formulário está em estado de carregamento ou processamento.
    /// </summary>
    public bool IsBusy { get; set; }

    /// <summary>
    /// Modelo de entrada utilizado para o binding dos campos do formulário.
    /// </summary>
    public Offer InputModel { get; set; } = new();

    /// <summary>
    /// Indica a operação atual do formulário ("Enviar" ou "Editar").
    /// </summary>
    public string Operation => OfferId == 0 ? "Enviar" : "Editar";

    /// <summary>
    /// Indica se o botão de adicionar pagamento deve estar desabilitado (máximo de 5 pagamentos).
    /// </summary>
    public bool DisableAddPayment => InputModel.Payments.Count >= 5;

    /// <summary>
    /// Expressão regular utilizada para remover caracteres não numéricos dos campos relevantes.
    /// </summary>
    public string Pattern = @"\D";

    #endregion

    #region Services

    /// <summary>
    /// Handler responsável pelas operações de propostas.
    /// </summary>
    [Inject]
    public IOfferHandler OfferHandler { get; set; } = null!;

    /// <summary>
    /// Handler responsável pelas operações de imóveis.
    /// </summary>
    [Inject]
    public IPropertyHandler PropertyHandler { get; set; } = null!;

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

    #endregion

    #region Methods

    /// <summary>
    /// Manipula o envio válido do formulário, realizando a criação ou atualização da proposta.
    /// </summary>
    /// <remarks>
    /// Remove caracteres não numéricos do telefone do comprador, chama o handler para criar ou atualizar a proposta,
    /// exibe mensagens de sucesso ou erro via Snackbar, dispara o evento de submissão e redireciona para a listagem de imóveis em caso de sucesso.
    /// </remarks>
    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;
        try
        {
            string message;
            InputModel.Buyer!.Phone = Regex.Replace(InputModel.Buyer.Phone, Pattern, "");
            if (OfferId == 0)
            {
                var response = await OfferHandler.CreateAsync(InputModel);
                if (response.IsSuccess)
                {
                    Snackbar.Add("Proposta enviada com sucesso", Severity.Success);
                    Snackbar.Add("Aguarde nosso contato", Severity.Success);
                    await OnSubmitButtonClickedAsync();
                    NavigationManager.NavigateTo("/listar-imoveis");
                    return;
                }
                message = response.Message ?? string.Empty;
            }
            else
            {
                var response = await OfferHandler.UpdateAsync(InputModel);
                if (response.IsSuccess)
                {
                    Snackbar.Add("Proposta alterada com sucesso", Severity.Success);
                    await OnSubmitButtonClickedAsync();
                    return;
                }
                message = response.Message ?? string.Empty;
            }
            Snackbar.Add(message, Severity.Error);
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

    /// <summary>
    /// Adiciona um novo pagamento à lista de pagamentos da proposta, limitado a 5 pagamentos.
    /// </summary>
    /// <remarks>
    /// Cada novo pagamento é criado com o tipo padrão <see cref="EPaymentType.BankSlip"/>.
    /// </remarks>
    public void AddPayment()
    {
        if (InputModel.Payments.Count < 5)
        {
            InputModel.Payments.Add(new Payment
            {
                PaymentType = EPaymentType.BankSlip
            });
        }
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Inicializa o componente, carregando os dados da proposta para edição ou preparando para envio de nova proposta.
    /// </summary>
    /// <remarks>
    /// Se <see cref="OfferId"/> for diferente de zero, busca os dados da proposta para edição.
    /// Caso contrário, busca os dados do imóvel e prepara o modelo para envio de nova proposta.
    /// Exibe mensagens de erro via Snackbar em caso de falha.
    /// </remarks>
    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        try
        {
            if (OfferId != 0)
            {
                var request = new GetOfferByIdRequest { Id = OfferId };
                var response = await OfferHandler.GetByIdAsync(request);
                if (response is { IsSuccess: true, Data: not null })
                {
                    InputModel = response.Data;
                    return;
                }
                Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            }
            else
            {
                var request = new GetPropertyByIdRequest { Id = PropertyId };
                var response = await PropertyHandler.GetByIdAsync(request);
                if (response is { IsSuccess: true, Data: not null })
                {
                    InputModel.Property = response.Data;
                    InputModel.PropertyId = response.Data.Id;
                    InputModel.Buyer = new Customer();
                    InputModel.BuyerId = 0;
                    return;
                }

                Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
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