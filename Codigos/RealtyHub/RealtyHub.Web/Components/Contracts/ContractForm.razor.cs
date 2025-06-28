using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Web.Components.Common;
using RealtyHub.Web.Components.Customers;
using RealtyHub.Web.Components.Email;
using RealtyHub.Web.Components.Offers;
using RealtyHub.Web.Components.Properties;

namespace RealtyHub.Web.Components.Contracts;

/// <summary>
/// Componente responsável pelo formulário de emissão e edição de contratos.
/// </summary>
public partial class ContractFormComponent : ComponentBase
{
    #region Parameters

    /// <summary>
    /// Identificador do contrato. Se diferente de zero, o formulário entra em modo de edição.
    /// </summary>
    [Parameter]
    public long ContractId { get; set; }

    /// <summary>
    /// Identificador da proposta associada ao contrato.
    /// </summary>
    [Parameter]
    public long OfferId { get; set; }

    /// <summary>
    /// Identificador do imóvel associado ao contrato.
    /// </summary>
    [Parameter]
    public long PropertyId { get; set; }

    /// <summary>
    /// Indica se o formulário está em modo somente Leitura.
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; }

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
    /// Chave para forçar a atualização do formulário.
    /// </summary>
    public int EditFormKey { get; set; }

    /// <summary>
    /// Modelo de entrada utilizado para o binding dos campos do formulário.
    /// </summary>
    public Contrato InputModel { get; set; } = new();

    /// <summary>
    /// Indica a operação atual do formulário ("Emitir" ou "Editar").
    /// </summary>
    public string Operation => ContractId == 0 ? "Emitir" : "Editar";

    #endregion

    #region Services

    /// <summary>
    /// Handler responsável pelas operações de propostas.
    /// </summary>
    [Inject]
    public IOfferHandler OfferHandler { get; set; } = null!;

    /// <summary>
    /// Handler responsável pelas operações de contratos.
    /// </summary>
    [Inject]
    public IContractHandler ContractHandler { get; set; } = null!;

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
    /// Serviço de diálogos para exibição de modais.
    /// </summary>
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Manipula o envio válido do formulário, realizando a criação ou atualização do contrato.
    /// </summary>
    /// <remarks>
    /// Este método verifica se o formulário está em modo de emissão ou edição, chama o handler apropriado para criar ou atualizar o contrato,
    /// exibe mensagens de sucesso ou erro via Snackbar, dispara o evento de submissão e, em caso de sucesso, abre o diálogo para envio do contrato por e-mail.
    /// </remarks>
    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;
        try
        {
            bool success;
            if (ContractId == 0)
            {
                var response = await ContractHandler.CreateAsync(InputModel);
                success = response.IsSuccess;
                if (response.IsSuccess)
                {
                    Snackbar.Add("Contrato emitido com sucesso", Severity.Success);
                    await OnSubmitButtonClickedAsync();
                }
                else
                    Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            }
            else
            {
                var response = await ContractHandler.UpdateAsync(InputModel);
                success = response.IsSuccess;
                if (response.IsSuccess)
                {
                    Snackbar.Add("Contrato alterado com sucesso", Severity.Success);
                    NavigationManager.NavigateTo("/contratos");
                    await OnSubmitButtonClickedAsync();
                }
                else
                    Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            }

            if (success)
                await OpenEmailDialog();
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
    /// Abre o diálogo para confirmação e envio do contrato por e-mail.
    /// </summary>
    /// <remarks>
    /// Exibe um diálogo de confirmação para o envio do contrato por e-mail. Caso o usuário confirme, abre um segundo diálogo para informar os e-mails do comprador e vendedor.
    /// Utiliza o serviço de diálogos (<see cref="IDialogService"/>) para exibir os modais.
    /// </remarks>
    private async Task OpenEmailDialog()
    {
        var parametersConfirm = new DialogParameters
        {
            { "ContentText", "Deseja enviar os contrados via e-mail? " },
            { "ButtonText", "Confirmar" },
            { "ButtonColor", Color.Success }
        };

        var optionsConfirm = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small
        };

        var dialogConfirm = await DialogService
            .ShowAsync<DialogConfirm>("Emails", parametersConfirm, optionsConfirm);
        var confirmResult = await dialogConfirm.Result;

        if (confirmResult is { Canceled: true }) return;

        var parameters = new DialogParameters
        {
            { "ContractId", InputModel.Id },
            { "BuyerEmail", InputModel.Comprador?.Email },
            { "SellerEmail", InputModel.Vendedor?.Email }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true
        };
        var dialog = await DialogService
            .ShowAsync<EmailDialog>("Enviar contrato por e-mail", parameters, options);
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
    /// Abre o diálogo para seleção de imóvel.
    /// </summary>
    /// <remarks>
    /// Exibe um diálogo modal para seleção de um imóvel. Ao selecionar, atualiza o modelo do contrato com o imóvel escolhido.
    /// O diálogo só é aberto se <see cref="LockPropertySearch"/> for falso.
    /// </remarks>
    public async Task OpenPropertyDialog()
    {
        if (LockPropertySearch) return;
        var parameters = new DialogParameters
        {
            { "OnPropertySelected", EventCallback.Factory
                .Create<Imovel>(this, SelectedProperty) }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true
        };
        var dialog = await DialogService
            .ShowAsync<PropertyDialog>("Informe o Imóvel", parameters, options);

        var result = await dialog.Result;

        if (result is { Canceled: false, Data: Imovel selectedProperty })
            SelectedProperty(selectedProperty);
    }

    /// <summary>
    /// Manipula a seleção de um imóvel no diálogo.
    /// </summary>
    /// <remarks>
    /// Atualiza o modelo do contrato com o imóvel selecionado e força a atualização do formulário.
    /// </remarks>
    private void SelectedProperty(Imovel imovel)
    {
        InputModel.Proposta!.Imovel = imovel;
        InputModel.Proposta.ImovelId = imovel.Id;
        EditFormKey++;
        StateHasChanged();
    }

    /// <summary>
    /// Limpa os dados do imóvel selecionado.
    /// </summary>
    /// <remarks>
    /// Remove o imóvel do modelo do contrato e reseta os campos relacionados.
    /// </remarks>
    public void ClearPropertyObjects()
    {
        InputModel.Proposta!.Imovel = new Imovel();
        InputModel.Proposta.ImovelId = 0;
        EditFormKey++;
        StateHasChanged();
    }

    /// <summary>
    /// Abre o diálogo para seleção de proposta.
    /// </summary>
    /// <remarks>
    /// Exibe um diálogo modal para seleção de uma proposta aceita. Ao selecionar, atualiza o modelo do contrato com a proposta escolhida.
    /// O diálogo só é aberto se <see cref="LockPropertySearch"/> for falso.
    /// </remarks>
    public async Task OpenOfferDialog()
    {
        if (LockPropertySearch) return;
        var parameters = new DialogParameters
        {
            { "OnOfferSelected", EventCallback.Factory
                .Create<Proposta>(this, SelectedOffer) },
            { "OnlyAccepted", true }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.ExtraLarge,
            FullWidth = true
        };
        var dialog = await DialogService
            .ShowAsync<OfferListDialog>("Selecione a proposta", parameters, options);
        var result = await dialog.Result;
        if (result is { Canceled: false, Data: Proposta offer })
            SelectedOffer(offer);
    }

    /// <summary>
    /// Manipula a seleção de uma proposta no diálogo.
    /// </summary>
    /// <remarks>
    /// Atualiza o modelo do contrato com a proposta selecionada, preenchendo também os dados do comprador e vendedor.
    /// </remarks>
    private void SelectedOffer(Proposta proposta)
    {
        InputModel.PropostaId = proposta.Id;
        InputModel.Proposta = proposta;
        InputModel.Vendedor = proposta.Imovel!.Vendedor;
        InputModel.VendedorId = proposta.Imovel.VendedorId;
        InputModel.Comprador = proposta.Comprador;
        InputModel.CompradorId = proposta.CompradorId;
        EditFormKey++;
        StateHasChanged();
    }

    /// <summary>
    /// Limpa os dados da proposta selecionada.
    /// </summary>
    /// <remarks>
    /// Remove a proposta do modelo do contrato e reseta os campos de comprador e vendedor.
    /// </remarks>
    public void ClearOfferObjets()
    {
        InputModel.Proposta = new Proposta();
        InputModel.PropostaId = 0;
        InputModel.Vendedor = new Cliente();
        InputModel.VendedorId = 0;
        InputModel.Comprador = new Cliente();
        InputModel.CompradorId = 0;
        EditFormKey++;
        StateHasChanged();
    }

    /// <summary>
    /// Abre o diálogo para seleção do comprador.
    /// </summary>
    /// <remarks>
    /// Exibe um diálogo modal para seleção de um cliente como comprador. O diálogo só é aberto se <see cref="LockCustomerSearch"/> for falso.
    /// </remarks>
    public async Task OpenBuyerDialog()
    {
        if (LockCustomerSearch) return;
        var parameters = new DialogParameters
        {
            { "OnCustomerSelected", EventCallback.Factory
                .Create<Cliente>(this, SelectedBuyer) }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true
        };
        var dialog = await DialogService
            .ShowAsync<CustomerDialog>("Informe o comprador", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: Cliente buyer })
            SelectedBuyer(buyer);
    }

    /// <summary>
    /// Manipula a seleção do comprador no diálogo.
    /// </summary>
    /// <remarks>
    /// Atualiza o modelo do contrato com o comprador selecionado.
    /// </remarks>
    private void SelectedBuyer(Cliente buyer)
    {
        InputModel.Comprador = buyer;
        InputModel.CompradorId = buyer.Id;
        EditFormKey++;
        StateHasChanged();
    }

    /// <summary>
    /// Limpa os dados do comprador selecionado.
    /// </summary>
    /// <remarks>
    /// Remove o comprador do modelo do contrato e reseta os campos relacionados.
    /// </remarks>
    public void ClearBuyerObjects()
    {
        InputModel.Comprador = new Cliente();
        InputModel.CompradorId = 0;
        EditFormKey++;
        StateHasChanged();
    }

    /// <summary>
    /// Abre o diálogo para seleção do vendedor.
    /// </summary>
    /// <remarks>
    /// Exibe um diálogo modal para seleção de um cliente como vendedor. O diálogo só é aberto se <see cref="LockCustomerSearch"/> for falso.
    /// </remarks>
    public async Task OpenSellerDialog()
    {
        if (LockCustomerSearch) return;
        var parameters = new DialogParameters
        {
            { "OnCustomerSelected", EventCallback.Factory
                .Create<Cliente>(this, SelectedSeller) }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true
        };
        var dialog = await DialogService
            .ShowAsync<CustomerDialog>("Informe o vendedor", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: Cliente seller })
            SelectedSeller(seller);
    }

    /// <summary>
    /// Manipula a seleção do vendedor no diálogo.
    /// </summary>
    /// <remarks>
    /// Atualiza o modelo do contrato com o vendedor selecionado.
    /// </remarks>
    private void SelectedSeller(Cliente seller)
    {
        InputModel.Vendedor = seller;
        InputModel.VendedorId = seller.Id;
        EditFormKey++;
        StateHasChanged();
    }

    /// <summary>
    /// Limpa os dados do vendedor selecionado.
    /// </summary>
    /// <remarks>
    /// Remove o vendedor do modelo do contrato e reseta os campos relacionados.
    /// </remarks>
    public void ClearSellerObjects()
    {
        InputModel.Vendedor = new Cliente();
        InputModel.VendedorId = 0;
        EditFormKey++;
        StateHasChanged();
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Inicializa o componente, buscando os dados do contrato, proposta ou imóvel conforme os parâmetros informados.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        try
        {
            if (ContractId != 0)
            {
                var request = new GetContractByIdRequest { Id = ContractId };
                var response = await ContractHandler.GetByIdAsync(request);
                if (response is { IsSuccess: true, Data: not null })
                {
                    InputModel = response.Data;
                    return;
                }
                Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
                return;
            }
            if (OfferId != 0)
            {
                var request = new GetOfferByIdRequest { Id = OfferId };
                var response = await OfferHandler.GetByIdAsync(request);
                if (response is { IsSuccess: true, Data: not null })
                {
                    InputModel.Proposta = response.Data;
                    InputModel.PropostaId = OfferId;
                    InputModel.Vendedor = response.Data.Imovel!.Vendedor;
                    InputModel.VendedorId = response.Data.Imovel.VendedorId;
                    InputModel.Comprador = response.Data.Comprador;
                    InputModel.CompradorId = response.Data.CompradorId;
                    return;
                }
                Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
                return;
            }
            if (PropertyId != 0)
            {
                var request = new GetOfferAcceptedByProperty { PropertyId = PropertyId };
                var response = await OfferHandler.GetAcceptedByProperty(request);
                if (response is { IsSuccess: true, Data: not null })
                {
                    InputModel.Proposta = response.Data;
                    InputModel.PropostaId = response.Data.Id;
                    InputModel.Vendedor = response.Data.Imovel!.Vendedor;
                    InputModel.VendedorId = response.Data.Imovel.VendedorId;
                    InputModel.Comprador = response.Data.Comprador;
                    InputModel.CompradorId = response.Data.CompradorId;
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