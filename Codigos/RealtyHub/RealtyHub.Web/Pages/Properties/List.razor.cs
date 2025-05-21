using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Web.Components.Common;
using RealtyHub.Web.Services;

namespace RealtyHub.Web.Pages.Properties;

/// <summary>
/// Página responsável por exibir e gerenciar a listagem de imóveis na aplicação.
/// </summary>
public partial class ListPropertiesPage : ComponentBase
{
    #region Parameters

    /// <summary>
    /// Evento chamado quando um imóvel é selecionado.
    /// </summary>
    [Parameter]
    public EventCallback<Property> OnPropertySelected { get; set; }

    /// <summary>
    /// Estilo CSS aplicado às linhas da tabela.
    /// </summary>
    [Parameter]
    public string RowStyle { get; set; } = string.Empty;

    #endregion

    #region Properties

    /// <summary>
    /// Componente de grid do MudBlazor utilizado para exibir os imóveis.
    /// </summary>
    public MudDataGrid<Property> DataGrid { get; set; } = null!;

    /// <summary>
    /// Lista de imóveis a serem exibidos no grid.
    /// </summary>
    public List<Property> Properties { get; set; } = [];

    /// <summary>
    /// Termo de busca para filtrar os imóveis.
    /// </summary>
    public string SearchTerm { get; set; } = string.Empty;

    /// <summary>
    /// Filtro selecionado para busca, aplicável às propriedades.
    /// </summary>
    public string SelectedFilter { get; set; } = string.Empty;

    /// <summary>
    /// Lista de opções de filtro para a busca de imóveis.
    /// </summary>
    public readonly List<FilterOption> FilterOptions = new()
    {
        new FilterOption { DisplayName = "Título", PropertyName = "Title" },
        new FilterOption { DisplayName = "Descrição", PropertyName = "Description" },
        new FilterOption { DisplayName = "Nome do condomínio", PropertyName = "Condominium.Name" },
        new FilterOption { DisplayName = "Bairro", PropertyName = "Address.Neighborhood" },
        new FilterOption { DisplayName = "Rua", PropertyName = "Address.Street" },
        new FilterOption { DisplayName = "Cidade", PropertyName = "Address.City" },
        new FilterOption { DisplayName = "Estado", PropertyName = "Address.State" },
        new FilterOption { DisplayName = "CEP", PropertyName = "Address.ZipCode" },
        new FilterOption { DisplayName = "País", PropertyName = "Address.Country" },
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
    /// Handler responsável pelas operações de imóveis, como recuperar e excluir propriedades.
    /// </summary>
    [Inject]
    public IPropertyHandler Handler { get; set; } = null!;

    /// <summary>
    /// Serviço para exibição de diálogos modais.
    /// </summary>
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    /// <summary>
    /// Serviço JavaScript para interagir com o navegador (ex.: abrir novo aba).
    /// </summary>
    [Inject]
    public IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// Serviço responsável por gerar relatórios de propriedades.
    /// </summary>
    [Inject]
    public PropertyReport PropertyReport { get; set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Manipula o clique do botão para gerar o relatório do imóvel.
    /// Caso o relatório seja gerado com sucesso, abre o PDF em uma nova aba.
    /// </summary>
    /// <returns>Task representando a operação assíncrona.</returns>
    public async Task OnReportClickedAsync()
    {
        var result = await PropertyReport.GetPropertyAsync();
        if (result.IsSuccess)
        {
            await JsRuntime.InvokeVoidAsync("openContractPdfInNewTab", result.Data?.Url);
            return;
        }
        Snackbar.Add(result.Message ?? string.Empty, Severity.Error);
    }

    /// <summary>
    /// Abre um diálogo de confirmação para a exclusão de um imóvel e, se confirmado, executa a exclusão.
    /// Após a exclusão, recarrega os dados no grid e exibe uma notificação.
    /// </summary>
    /// <param name="id">Identificador do imóvel a ser excluído.</param>
    /// <param name="title">Título do imóvel, usado na mensagem de confirmação.</param>
    /// <returns>Task representando a operação assíncrona.</returns>
    public async Task OnDeleteButtonClickedAsync(long id, string title)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", $"Ao prosseguir o imóvel {title} será excluido. " +
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

        await OnDeleteAsync(id, title);
        StateHasChanged();
    }

    /// <summary>
    /// Executa a exclusão efetiva do imóvel.
    /// Remove o imóvel da lista local, recarrega os dados do grid e exibe uma notificação.
    /// </summary>
    /// <param name="id">Identificador do imóvel a ser excluído.</param>
    /// <param name="name">Título do imóvel para a mensagem de sucesso.</param>
    /// <returns>Task representando a operação assíncrona.</returns>
    private async Task OnDeleteAsync(long id, string name)
    {
        try
        {
            await Handler.DeleteAsync(new DeletePropertyRequest { Id = id });
            Properties.RemoveAll(x => x.Id == id);
            await DataGrid.ReloadServerData();
            Snackbar.Add($"Imóvel {name} excluído", Severity.Success);
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
        }
    }

    /// <summary>
    /// Carrega os dados dos imóveis do servidor de forma paginada para exibição no grid.
    /// </summary>
    /// <param name="state">Estado atual do grid contendo informações de paginação.</param>
    /// <returns>
    /// Um objeto <see cref="GridData{Property}"/> contendo a lista de imóveis e a contagem total.
    /// Em caso de falha, retorna um grid vazio e exibe uma mensagem de erro.
    /// </returns>
    public async Task<GridData<Property>> LoadServerData(GridState<Property> state)
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
                return new GridData<Property>
                {
                    Items = response.Data ?? [],
                    TotalItems = response.TotalCount
                };

            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            return new GridData<Property>();
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
            return new GridData<Property>();
        }
    }

    /// <summary>
    /// Aciona a recarga dos dados do grid com base no termo de busca atual.
    /// </summary>
    public void OnButtonSearchClick() => DataGrid.ReloadServerData();

    /// <summary>
    /// Limpa o termo de busca e recarrega o grid.
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
    /// Notifica o componente pai que um imóvel foi selecionado.
    /// </summary>
    /// <param name="property">Imóvel selecionado.</param>
    /// <returns>Task representando a operação assíncrona.</returns>
    public async Task SelectProperty(Property property)
    {
        if (OnPropertySelected.HasDelegate)
            await OnPropertySelected.InvokeAsync(property);
    }

    /// <summary>
    /// Obtém a URL da foto thumbnail do imóvel.
    /// Se uma foto marcada como thumbnail existir, utiliza-a; caso contrário, utiliza a primeira foto disponível.
    /// </summary>
    /// <param name="property">Imóvel cujo thumbnail será obtido.</param>
    /// <returns>String com a URL da foto.</returns>
    public string GetSrcThumbnailPhoto(Property property)
    {
        var photo = property
                        .PropertyPhotos
                        .FirstOrDefault(p => p.IsThumbnail)
                    ?? property.PropertyPhotos.FirstOrDefault();

        return $"{Configuration.BackendUrl}/photos/{photo?.Id}{photo?.Extension}";
    }

    #endregion
}