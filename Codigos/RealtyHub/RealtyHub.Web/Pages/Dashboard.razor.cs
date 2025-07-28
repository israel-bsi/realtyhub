using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Core.Requests.Offers;

namespace RealtyHub.Web.Pages;

/// <summary>
/// Dashboard principal do sistema que exibe informações resumidas para corretores
/// </summary>
public partial class DashboardPage : ComponentBase
{
    #region Properties

    /// <summary>
    /// Indica se a página está carregando dados
    /// </summary>
    public bool IsBusy { get; set; } = true;

    /// <summary>
    /// Total de imóveis cadastrados
    /// </summary>
    public int TotalProperties { get; set; }

    /// <summary>
    /// Total de clientes cadastrados
    /// </summary>
    public int TotalCustomers { get; set; }

    /// <summary>
    /// Total de propostas recebidas
    /// </summary>
    public int TotalOffers { get; set; }

    /// <summary>
    /// Total de propostas aceitas
    /// </summary>
    public int AcceptedOffers { get; set; }

    /// <summary>
    /// Lista dos últimos imóveis cadastrados
    /// </summary>
    public List<Property> RecentProperties { get; set; } = [];

    /// <summary>
    /// Lista das últimas propostas recebidas
    /// </summary>
    public List<Offer> RecentOffers { get; set; } = [];

    /// <summary>
    /// Lista dos últimos clientes cadastrados
    /// </summary>
    public List<Customer> RecentCustomers { get; set; } = [];

    #endregion

    #region Services

    /// <summary>
    /// Handler para operações com imóveis
    /// </summary>
    [Inject]
    public IPropertyHandler PropertyHandler { get; set; } = null!;

    /// <summary>
    /// Handler para operações com clientes
    /// </summary>
    [Inject]
    public ICustomerHandler CustomerHandler { get; set; } = null!;

    /// <summary>
    /// Handler para operações com propostas
    /// </summary>
    [Inject]
    public IOfferHandler OfferHandler { get; set; } = null!;

    /// <summary>
    /// Serviço para navegação
    /// </summary>
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    /// <summary>
    /// Serviço para notificações
    /// </summary>
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    #endregion

    #region Overrides

    /// <summary>
    /// Carrega os dados do dashboard ao inicializar o componente
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await LoadDashboardDataAsync();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Carrega todos os dados necessários para o dashboard
    /// </summary>
    private async Task LoadDashboardDataAsync()
    {
        IsBusy = true;
        
        try
        {
            // Carrega estatísticas e dados recentes em paralelo
            var propertiesTask = LoadPropertiesDataAsync();
            var customersTask = LoadCustomersDataAsync();
            var offersTask = LoadOffersDataAsync();

            await Task.WhenAll(propertiesTask, customersTask, offersTask);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao carregar dados do dashboard: {ex.Message}", Severity.Error);
        }
        finally
        {
            IsBusy = false;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Carrega dados relacionados aos imóveis
    /// </summary>
    private async Task LoadPropertiesDataAsync()
    {
        try
        {
            var request = new GetAllPropertiesRequest
            {
                PageNumber = 1,
                PageSize = 5 // Últimos 5 imóveis
            };

            var response = await PropertyHandler.GetAllAsync(request);
            if (response.IsSuccess)
            {
                TotalProperties = response.TotalCount;
                RecentProperties = response.Data ?? [];
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao carregar dados de imóveis: {ex.Message}", Severity.Warning);
        }
    }

    /// <summary>
    /// Carrega dados relacionados aos clientes
    /// </summary>
    private async Task LoadCustomersDataAsync()
    {
        try
        {
            var request = new GetAllCustomersRequest
            {
                PageNumber = 1,
                PageSize = 5 // Últimos 5 clientes
            };

            var response = await CustomerHandler.GetAllAsync(request);
            if (response.IsSuccess)
            {
                TotalCustomers = response.TotalCount;
                RecentCustomers = response.Data ?? [];
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao carregar dados de clientes: {ex.Message}", Severity.Warning);
        }
    }

    /// <summary>
    /// Carrega dados relacionados às propostas
    /// </summary>
    private async Task LoadOffersDataAsync()
    {
        try
        {
            var request = new GetAllOffersRequest
            {
                PageNumber = 1,
                PageSize = 5 // Últimas 5 propostas
            };

            var response = await OfferHandler.GetAllAsync(request);
            if (response.IsSuccess)
            {
                TotalOffers = response.TotalCount;
                RecentOffers = response.Data ?? [];
                
                // Conta propostas aceitas
                AcceptedOffers = RecentOffers.Count(o => o.OfferStatus == Core.Enums.EOfferStatus.Accepted);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao carregar dados de propostas: {ex.Message}", Severity.Warning);
        }
    }

    /// <summary>
    /// Obtém a URL da foto principal do imóvel
    /// </summary>
    public string GetPropertyPhotoUrl(Property property)
    {
        var photo = property.PropertyPhotos.FirstOrDefault(p => p.IsThumbnail) 
                   ?? property.PropertyPhotos.FirstOrDefault();

        return photo == null 
            ? "/src/img/no-image.png" // Imagem padrão caso não tenha foto
            : $"{Configuration.BackendUrl}/photos/{photo.Id}{photo.Extension}";
    }

    /// <summary>
    /// Navega para a página de listagem de imóveis
    /// </summary>
    public void NavigateToProperties() => NavigationManager.NavigateTo("/imoveis");

    /// <summary>
    /// Navega para a página de listagem de clientes
    /// </summary>
    public void NavigateToCustomers() => NavigationManager.NavigateTo("/clientes");

    /// <summary>
    /// Navega para a página de listagem de propostas
    /// </summary>
    public void NavigateToOffers() => NavigationManager.NavigateTo("/propostas");

    /// <summary>
    /// Navega para os detalhes de um imóvel específico
    /// </summary>
    public void NavigateToPropertyDetails(long propertyId) => NavigationManager.NavigateTo($"/imoveis/detalhes/{propertyId}");

    /// <summary>
    /// Recarrega os dados do dashboard
    /// </summary>
    public async Task RefreshDashboardAsync()
    {
        await LoadDashboardDataAsync();
    }

    #endregion
}