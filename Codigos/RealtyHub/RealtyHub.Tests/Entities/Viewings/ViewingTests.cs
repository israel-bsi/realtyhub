using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.Tests.Entities.Viewings;

/// <summary>
/// Classe de testes de integração para os endpoints de Viewing.
/// Estes testes focam na validação de campos, lógica de negócio e CRUD,
/// sem se preocupar com autenticação (que é bypassada).
/// Cada teste é completamente isolado e limpa o banco antes da execução.
/// </summary>
public class ViewingTests : IClassFixture<RealtyHubApiTests>
{
    private readonly RealtyHubApiTests _factory;

    public ViewingTests(RealtyHubApiTests factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Limpa o banco de dados e retorna o número de visitas encontradas antes da limpeza.
    /// </summary>
    private async Task<int> CleanupDatabaseAndGetPreviousCount()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Conta quantas visitas existiam antes da limpeza
        var existingCount = await dbContext.Viewing.CountAsync();
        
        // Remove todas as visitas existentes
        var existingViewings = await dbContext.Viewing.ToListAsync();
        dbContext.Viewing.RemoveRange(existingViewings);
        await dbContext.SaveChangesAsync();
        
        return existingCount;
    }

    /// <summary>
    /// Cria um customer válido para ser usado como comprador nos testes.
    /// </summary>
    private async Task<Customer> CreateValidBuyer()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var buyer = new Customer
        {
            Name = "Comprador Teste",
            Email = "comprador@test.com",
            Phone = "11999999999",
            DocumentNumber = "12345678901",
            CustomerType = ECustomerType.Buyer,
            PersonType = EPersonType.Individual,
            Occupation = "Comprador",
            Nationality = "Brasileira",
            MaritalStatus = EMaritalStatus.Single,
            UserId = RealtyHubApiTests.TestUserId,
            Address = new Address
            {
                Street = "Rua do Comprador",
                Number = "123",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234567"
            },
            IsActive = true
        };
        
        await dbContext.Customers.AddAsync(buyer);
        await dbContext.SaveChangesAsync();
        
        return buyer;
    }

    /// <summary>
    /// Cria uma propriedade válida para ser usada nos testes.
    /// </summary>
    private async Task<Property> CreateValidProperty()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Cria um vendedor primeiro
        var seller = new Customer
        {
            Name = "Vendedor Teste",
            Email = "vendedor@test.com",
            Phone = "11999999999",
            DocumentNumber = "12345678901",
            CustomerType = ECustomerType.Seller,
            PersonType = EPersonType.Individual,
            Occupation = "Vendedor",
            Nationality = "Brasileira",
            MaritalStatus = EMaritalStatus.Single,
            UserId = RealtyHubApiTests.TestUserId,
            Address = new Address
            {
                Street = "Rua do Vendedor",
                Number = "123",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234567"
            },
            IsActive = true
        };
        
        await dbContext.Customers.AddAsync(seller);
        
        // Cria um condomínio
        var condominium = new Condominium
        {
            Name = "Condomínio Teste",
            Address = new Address
            {
                Street = "Rua do Condomínio",
                Number = "456",
                Neighborhood = "Jardins",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234567"
            },
            Units = 50,
            Floors = 10,
            HasElevator = true,
            HasSwimmingPool = true,
            HasPartyRoom = false,
            HasPlayground = true,
            HasFitnessRoom = true,
            CondominiumValue = 350.50m,
            UserId = RealtyHubApiTests.TestUserId,
            IsActive = true
        };
        
        await dbContext.Condominiums.AddAsync(condominium);
        await dbContext.SaveChangesAsync();
        
        // Cria a propriedade
        var property = new Property
        {
            Title = "Imóvel Teste",
            Description = "Descrição do imóvel teste",
            Price = 500000.00m,
            PropertyType = EPropertyType.Apartment,
            Bedroom = 3,
            Bathroom = 2,
            Garage = 1,
            Area = 120.5,
            TransactionsDetails = "Detalhes da transação",
            SellerId = seller.Id,
            CondominiumId = condominium.Id,
            RegistryNumber = "123456789",
            RegistryRecord = "987654321",
            IsNew = true,
            ShowInHome = true,
            IsActive = true,
            UserId = RealtyHubApiTests.TestUserId,
            Address = new Address
            {
                Street = "Rua do Imóvel",
                Number = "789",
                Neighborhood = "Vila Nova",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234567"
            }
        };
        
        await dbContext.Properties.AddAsync(property);
        await dbContext.SaveChangesAsync();
        
        return property;
    }

    /// <summary>
    /// Cria uma visita válida para uso nos testes.
    /// </summary>
    private static Viewing CreateValidViewing(long buyerId, long propertyId, DateTime? viewingDate = null)
    {
        return new Viewing
        {
            ViewingDate = viewingDate ?? DateTime.Now.AddDays(1),
            ViewingStatus = EViewingStatus.Scheduled,
            BuyerId = buyerId,
            PropertyId = propertyId,
            UserId = RealtyHubApiTests.TestUserId
        };
    }

    #region GET /v1/viewings - GetAllViewingsEndpoint Tests

    [Fact]
    public async Task GetAllViewings_ShouldReturnPagedResponse()
    {
        // Arrange
        var previousCount = await CleanupDatabaseAndGetPreviousCount();
        var buyer = await CreateValidBuyer();
        var property = await CreateValidProperty();
        
        // Cria 5 visitas
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var viewings = new List<Viewing>();
        
        for (int i = 1; i <= 5; i++)
        {
            var viewing = CreateValidViewing(buyer.Id, property.Id, DateTime.Now.AddDays(i));
            viewings.Add(viewing);
        }
        
        await dbContext.Viewing.AddRangeAsync(viewings);
        await dbContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/viewings");
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<Viewing>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(5);
        result.TotalCount.Should().Be(5);
    }

    [Fact]
    public async Task GetAllViewings_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var buyer = await CreateValidBuyer();
        var property = await CreateValidProperty();
        
        // Cria 15 visitas
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var viewings = new List<Viewing>();
        
        for (int i = 1; i <= 15; i++)
        {
            var viewing = CreateValidViewing(buyer.Id, property.Id, DateTime.Now.AddDays(i));
            viewings.Add(viewing);
        }
        
        await dbContext.Viewing.AddRangeAsync(viewings);
        await dbContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/viewings?pageNumber=1&pageSize=10");
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<Viewing>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().BeLessOrEqualTo(10);
        result.TotalCount.Should().Be(15);
    }

    #endregion

    #region GET /v1/viewings/{id} - GetViewingByIdEndpoint Tests

    [Fact]
    public async Task GetViewingById_WithValidId_ShouldReturnViewing()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var buyer = await CreateValidBuyer();
        var property = await CreateValidProperty();
        
        // Cria uma visita diretamente no banco
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var viewing = CreateValidViewing(buyer.Id, property.Id, DateTime.Now.AddDays(1));
        await dbContext.Viewing.AddAsync(viewing);
        await dbContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/v1/viewings/{viewing.Id}");
        var result = await response.Content.ReadFromJsonAsync<Response<Viewing>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(viewing.Id);
        result.Data.ViewingStatus.Should().Be(EViewingStatus.Scheduled);
    }

    [Fact]
    public async Task GetViewingById_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/viewings/999");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    #endregion

    #region POST /v1/viewings - ScheduleViewingEndpoint Tests

    [Fact]
    public async Task ScheduleViewing_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var buyer = await CreateValidBuyer();
        var property = await CreateValidProperty();
        
        var client = _factory.CreateClient();
        var viewing = new Viewing
        {
            ViewingDate = DateTime.Now.AddDays(2),
            ViewingStatus = EViewingStatus.Scheduled,
            BuyerId = buyer.Id,
            PropertyId = property.Id
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/viewings", viewing);
        var result = await response.Content.ReadFromJsonAsync<Response<Viewing>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.ViewingStatus.Should().Be(EViewingStatus.Scheduled);
        result.Data.BuyerId.Should().Be(buyer.Id);
        result.Data.PropertyId.Should().Be(property.Id);
    }

    [Fact]
    public async Task ScheduleViewing_WithMissingRequiredFields_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var viewing = new Viewing
        {
            // ViewingDate is missing - required field
            ViewingStatus = EViewingStatus.Scheduled
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/viewings", viewing);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ScheduleViewing_WithPastDate_ShouldReturnCreated()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var buyer = await CreateValidBuyer();
        var property = await CreateValidProperty();
        
        var client = _factory.CreateClient();
        var viewing = new Viewing
        {
            ViewingDate = DateTime.Now.AddDays(-1), // Data no passado
            ViewingStatus = EViewingStatus.Scheduled,
            BuyerId = buyer.Id,
            PropertyId = property.Id
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/viewings", viewing);

        // Assert
        // Nota: Atualmente não há validação de data no passado implementada
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    #endregion

    #region PUT /v1/viewings/{id}/reschedule - RescheduleViewingEndpoint Tests

    [Fact]
    public async Task RescheduleViewing_WithValidData_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var buyer = await CreateValidBuyer();
        var property = await CreateValidProperty();
        
        // Cria uma visita diretamente no banco para reagendar
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var viewing = CreateValidViewing(buyer.Id, property.Id, DateTime.Now.AddDays(1));
        await dbContext.Viewing.AddAsync(viewing);
        await dbContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

        var rescheduleRequest = new Viewing
        {
            Id = viewing.Id,
            ViewingDate = DateTime.Now.AddDays(3), // Nova data
            ViewingStatus = EViewingStatus.Scheduled,
            BuyerId = buyer.Id,
            PropertyId = property.Id
        };

        // Act
        var response = await client.PutAsJsonAsync($"/v1/viewings/{viewing.Id}/reschedule", rescheduleRequest);
        var result = await response.Content.ReadFromJsonAsync<Response<Viewing>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.ViewingDate.Should().Be(rescheduleRequest.ViewingDate);
    }

    [Fact]
    public async Task RescheduleViewing_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var buyer = await CreateValidBuyer();
        var property = await CreateValidProperty();
        
        var client = _factory.CreateClient();
        var viewing = new Viewing
        {
            Id = 999,
            ViewingDate = DateTime.Now.AddDays(3),
            ViewingStatus = EViewingStatus.Scheduled,
            BuyerId = buyer.Id,
            PropertyId = property.Id
        };

        // Act
        var response = await client.PutAsJsonAsync("/v1/viewings/999/reschedule", viewing);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region PUT /v1/viewings/{id}/done - DoneViewingEndpoint Tests

    [Fact]
    public async Task DoneViewing_WithValidId_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var buyer = await CreateValidBuyer();
        var property = await CreateValidProperty();
        
        // Cria uma visita diretamente no banco para marcar como finalizada
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var viewing = CreateValidViewing(buyer.Id, property.Id, DateTime.Now.AddDays(1));
        await dbContext.Viewing.AddAsync(viewing);
        await dbContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

        // Act
        var response = await client.PutAsync($"/v1/viewings/{viewing.Id}/done", new StringContent("{}", Encoding.UTF8, "application/json"));
        var result = await response.Content.ReadFromJsonAsync<Response<Viewing>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.ViewingStatus.Should().Be(EViewingStatus.Done);
    }

    [Fact]
    public async Task DoneViewing_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();

        // Act
        var response = await client.PutAsync("/v1/viewings/999/done", new StringContent("{}", Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region PUT /v1/viewings/{id}/cancel - CancelViewingEndpoint Tests

    [Fact]
    public async Task CancelViewing_WithValidId_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var buyer = await CreateValidBuyer();
        var property = await CreateValidProperty();
        
        // Cria uma visita diretamente no banco para cancelar
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var viewing = CreateValidViewing(buyer.Id, property.Id, DateTime.Now.AddDays(1));
        await dbContext.Viewing.AddAsync(viewing);
        await dbContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

        // Act
        var response = await client.PutAsync($"/v1/viewings/{viewing.Id}/cancel", new StringContent("{}", Encoding.UTF8, "application/json"));
        var result = await response.Content.ReadFromJsonAsync<Response<Viewing>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.ViewingStatus.Should().Be(EViewingStatus.Canceled);
    }

    [Fact]
    public async Task CancelViewing_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();

        // Act
        var response = await client.PutAsync("/v1/viewings/999/cancel", new StringContent("{}", Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Field Validation Tests

    [Fact]
    public async Task ScheduleViewing_WithInvalidBuyerId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var property = await CreateValidProperty();
        
        var client = _factory.CreateClient();
        var viewing = new Viewing
        {
            ViewingDate = DateTime.Now.AddDays(1),
            ViewingStatus = EViewingStatus.Scheduled,
            BuyerId = 999, // ID inválido
            PropertyId = property.Id
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/viewings", viewing);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ScheduleViewing_WithInvalidPropertyId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var buyer = await CreateValidBuyer();
        
        var client = _factory.CreateClient();
        var viewing = new Viewing
        {
            ViewingDate = DateTime.Now.AddDays(1),
            ViewingStatus = EViewingStatus.Scheduled,
            BuyerId = buyer.Id,
            PropertyId = 999 // ID inválido
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/viewings", viewing);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Infrastructure Tests

    [Fact]
    public void MockData_CreateSimpleClient_ShouldReturnClient()
    {
        // Arrange & Act
        var client = _factory.CreateClient();

        // Assert
        client.Should().NotBeNull();
    }

    #endregion
} 
