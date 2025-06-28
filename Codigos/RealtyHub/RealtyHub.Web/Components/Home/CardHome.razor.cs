using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Models;
using RealtyHub.Web.Components.Offers;

namespace RealtyHub.Web.Components.Home;

/// <summary>
/// Componente responsável por exibir o card de um imóvel na página inicial, incluindo foto, informações e ação para envio de proposta.
/// </summary>
public partial class CardHomeComponent : ComponentBase
{
    #region Parameters

    /// <summary>
    /// Classe CSS personalizada para o card.
    /// </summary>
    [Parameter]
    public string Class { get; set; } = string.Empty;

    /// <summary>
    /// Imóvel a ser exibido no card.
    /// </summary>
    [Parameter]
    public Imovel Imovel { get; set; } = new();

    #endregion

    #region Services

    /// <summary>
    /// Serviço de diálogo para exibição de modais.
    /// </summary>
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Obtém a URL da foto principal (thumbnail) do imóvel, ou a primeira foto disponível.
    /// </summary>
    /// <remarks>
    /// Busca a foto marcada como thumbnail em <see cref="Core.Models.Imovel.FotosImovel"/>. Se não houver, retorna a primeira foto.
    /// Monta a URL completa para exibição da imagem no card.
    /// </remarks>
    /// <returns>URL da foto do imóvel.</returns>
    public string GetSrcPhoto()
    {
        var photo = Imovel
                        .FotosImovel
                        .FirstOrDefault(p => p.Miniatura)
                    ?? Imovel.FotosImovel.FirstOrDefault();

        var fullName = $"{photo?.Id}{photo?.Extensao}";

        return $"{Configuration.BackendUrl}/photos/{fullName}";
    }

    /// <summary>
    /// Abre o diálogo para envio de proposta para o imóvel exibido no card.
    /// </summary>
    /// <remarks>
    /// Exibe um modal utilizando <see cref="OfferDialog"/>, passando o ID do imóvel como parâmetro.
    /// </remarks>
    public async Task OnSendOfferClickedAsync()
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
            { "PropertyId", Imovel.Id }
        };

        await DialogService.ShowAsync<OfferDialog>("Enviar proposta", parameters, options);
    }

    #endregion
}