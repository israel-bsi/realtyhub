using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Web.Components.Offers;

namespace RealtyHub.Web.Pages;

/// <summary>
/// Página responsável por exibir e gerenciar a listagem de imóveis na página inicial da aplicação.
/// </summary>
public partial class PropertyListHomePage : ComponentBase
{
    #region Properties

    /// <summary>
    /// Indica se a página está ocupada realizando alguma operação (ex.: carregamento de dados).
    /// </summary>
    public bool IsBusy { get; set; }

    /// <summary>
    /// Componente de grid do MudBlazor utilizado para exibir a lista de imóveis.
    /// </summary>
    public MudDataGrid<Imovel> DataGrid { get; set; } = null!;

    /// <summary>
    /// Lista de imóveis a serem exibidos no grid.
    /// </summary>
    public List<Imovel> Properties { get; set; } = [];

    /// <summary>
    /// Termo de busca para filtrar os imóveis.
    /// </summary>
    public string SearchTerm { get; set; } = string.Empty;

    /// <summary>
    /// Filtro selecionado para refinar a busca.
    /// </summary>
    public string SelectedFilter { get; set; } = string.Empty;

    /// <summary>
    /// Lista de opções de filtro disponíveis para a busca de imóveis.
    /// </summary>
    public readonly List<FilterOption> FilterOptions = new()
    {
        new FilterOption { DisplayName = "Descrição", PropertyName = "Description" },
        new FilterOption { DisplayName = "Bairro", PropertyName = "Address.Neighborhood" },
        new FilterOption { DisplayName = "Garagens", PropertyName = "Garage" },
        new FilterOption { DisplayName = "Quartos", PropertyName = "Bedroom" },
        new FilterOption { DisplayName = "Banheiros", PropertyName = "Bathroom" },
        new FilterOption { DisplayName = "Área", PropertyName = "Area" },
        new FilterOption { DisplayName = "Preço", PropertyName = "Price" },
    };

    #endregion

    #region Services

    /// <summary>
    /// Serviço para exibição de notificações (snackbars) na tela.
    /// </summary>
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    /// <summary>
    /// Handler responsável pelas operações relacionadas a imóveis, como listar propriedades.
    /// </summary>
    [Inject]
    public IPropertyHandler Handler { get; set; } = null!;

    /// <summary>
    /// Serviço para exibição de diálogos modais.
    /// </summary>
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Carrega os dados dos imóveis do servidor de forma paginada para exibição no grid.
    /// </summary>
    /// <param name="state">Estado atual do grid, contendo informações de paginação.</param>
    /// <returns>
    /// Um objeto <see cref="GridData{Property}"/> contendo a lista de imóveis e a contagem total.
    /// Em caso de falha, retorna um grid vazio e exibe uma mensagem de erro.
    /// </returns>
    public async Task<GridData<Imovel>> LoadServerData(GridState<Imovel> state)
    {
        try
        {
            var request = new GetAllPropertiesRequest
            {
                PageNumber = state.Page + 1,
                PageSize = state.PageSize,
                SearchTerm = SearchTerm,
                FilterBy = SelectedFilter
            };

            var response = await Handler.GetAllAsync(request);
            if (response.IsSuccess)
            {
                return new GridData<Imovel>
                {
                    Items = response.Data ?? [],
                    TotalItems = response.TotalCount
                };
            }

            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            return new GridData<Imovel>();
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
            return new GridData<Imovel>();
        }
    }

    /// <summary>
    /// Aciona a recarga dos dados do grid com base no termo de busca e nos filtros atuais.
    /// </summary>
    public void OnButtonSearchClick() => DataGrid.ReloadServerData();

    /// <summary>
    /// Limpa o termo de busca e recarrega os dados do grid.
    /// </summary>
    public void OnClearSearchClick()
    {
        SearchTerm = string.Empty;
        DataGrid.ReloadServerData();
    }

    /// <summary>
    /// Atualiza o filtro selecionado e solicita a atualização da interface.
    /// </summary>
    /// <param name="newValue">Novo valor do filtro.</param>
    public void OnValueFilterChanged(string newValue)
    {
        SelectedFilter = newValue;
        StateHasChanged();
    }

    /// <summary>
    /// Obtém a URL da foto thumbnail do imóvel.
    /// Se houver uma foto marcada como thumbnail, essa URL é utilizada; caso contrário, utiliza a primeira foto disponível.
    /// </summary>
    /// <param name="imovel">Imóvel cujo thumbnail será obtido.</param>
    /// <returns>String com a URL da foto.</returns>
    public string GetSrcThumbnailPhoto(Imovel imovel)
    {
        var photo = imovel
            .FotosImovel
            .FirstOrDefault(p => p.Miniatura)
            ?? imovel.FotosImovel.FirstOrDefault();

        return $"{Configuration.BackendUrl}/photos/{photo?.Id}{photo?.Extensao}";
    }

    /// <summary>
    /// Abre um diálogo modal para o envio de uma proposta de compra para o imóvel selecionado.
    /// </summary>
    /// <param name="imovel">Imóvel para o qual a proposta será enviada.</param>
    /// <returns>Task representando a operação assíncrona.</returns>
    public async Task OnSendOfferClickedAsync(Imovel imovel)
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
            { "PropertyId", imovel.Id }
        };

        await DialogService.ShowAsync<OfferDialog>("Enviar proposta", parameters, options);
    }

    #endregion
}