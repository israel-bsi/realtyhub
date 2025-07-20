using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using RealtyHub.Core.Utilities.FakeEntities;

namespace RealtyHub.Tests.Entities.Properties;

/// <summary>
/// Classe de testes de integração para os endpoints de Property.
/// Estes testes focam na validação de campos, lógica de negócio e CRUD,
/// sem se preocupar com autenticação (que é bypassada).
/// Cada teste é completamente isolado e limpa o banco antes da execução.
/// </summary>
public class PropertyTests : IClassFixture<RealtyHubApiTests>
{
    private readonly RealtyHubApiTests _factory;

    public PropertyTests(RealtyHubApiTests factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Limpa o banco de dados e retorna o número de propriedades encontradas antes da limpeza.
    /// </summary>
    private async Task<int> CleanupDatabaseAndGetPreviousCount()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Conta quantas propriedades existiam antes da limpeza
        var existingCount = await dbContext.Properties.CountAsync();
        
        // Remove todas as propriedades existentes
        var existingProperties = await dbContext.Properties.ToListAsync();
        dbContext.Properties.RemoveRange(existingProperties);
        await dbContext.SaveChangesAsync();
        
        return existingCount;
    }

    /// <summary>
    /// Cria um customer válido para ser usado como vendedor nos testes.
    /// </summary>
    private async Task<Customer> CreateValidSeller()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
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
        await dbContext.SaveChangesAsync();
        
        return seller;
    }

    /// <summary>
    /// Cria um condomínio válido para ser usado nos testes.
    /// </summary>
    private async Task<Condominium> CreateValidCondominium()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
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
            UserId = RealtyHubApiTests.TestUserId,
            IsActive = true
        };
        
        await dbContext.Condominiums.AddAsync(condominium);
        await dbContext.SaveChangesAsync();
        
        return condominium;
    }

    /// <summary>
    /// Cria uma propriedade válida para uso nos testes.
    /// </summary>
    private static Property CreateValidProperty(long sellerId, long condominiumId, string title = "Imóvel Teste")
    {
        return new Property
        {
            Title = title,
            Description = "Descrição do imóvel teste",
            Price = 500000.00m,
            PropertyType = EPropertyType.Apartment,
            Bedroom = 3,
            Bathroom = 2,
            Garage = 1,
            Area = 120.5,
            TransactionsDetails = "Detalhes da transação",
            SellerId = sellerId,
            CondominiumId = condominiumId,
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
    }

    #region GET /v1/properties - GetAllPropertiesEndpoint Tests

    [Fact]
    public async Task GetAllProperties_ShouldReturnPagedResponse()
    {
        // Arrange
        var previousCount = await CleanupDatabaseAndGetPreviousCount();
        var seller = await CreateValidSeller();
        var condominium = await CreateValidCondominium();
        
        // Cria 5 propriedades
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var properties = new List<Property>();
        
        for (int i = 1; i <= 5; i++)
        {
            var property = CreateValidProperty(seller.Id, condominium.Id, $"Imóvel {i}");
            properties.Add(property);
        }
        
        await dbContext.Properties.AddRangeAsync(properties);
        await dbContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/properties");
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<Property>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(5);
        result.TotalCount.Should().Be(5);
    }

    [Fact]
    public async Task GetAllProperties_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var seller = await CreateValidSeller();
        var condominium = await CreateValidCondominium();
        
        // Cria 15 propriedades
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var properties = new List<Property>();
        
        for (int i = 1; i <= 15; i++)
        {
            var property = CreateValidProperty(seller.Id, condominium.Id, $"Imóvel {i}");
            properties.Add(property);
        }
        
        await dbContext.Properties.AddRangeAsync(properties);
        await dbContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/properties?pageNumber=1&pageSize=10");
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<Property>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().BeLessOrEqualTo(10);
        result.TotalCount.Should().Be(15);
    }

    [Fact]
    public async Task GetAllProperties_WithSearchTerm_ShouldFilterResults()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var seller = await CreateValidSeller();
        var condominium = await CreateValidCondominium();
        
        // Cria propriedades com títulos específicos
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var property1 = CreateValidProperty(seller.Id, condominium.Id, "Apartamento Luxo");
        var property2 = CreateValidProperty(seller.Id, condominium.Id, "Casa Simples");
        var property3 = CreateValidProperty(seller.Id, condominium.Id, "Kitnet Centro");
        
        await dbContext.Properties.AddRangeAsync(property1, property2, property3);
        await dbContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/properties?searchTerm=apartamento");
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<Property>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        // Deve encontrar pelo menos o "Apartamento Luxo"
        result.Data!.Any(p => p.Title.Contains("Apartamento", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    #endregion

    #region GET /v1/properties/{id} - GetPropertyByIdEndpoint Tests

    [Fact]
    public async Task GetPropertyById_WithValidId_ShouldReturnProperty()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var seller = await CreateValidSeller();
        var condominium = await CreateValidCondominium();
        
        // Cria uma propriedade diretamente no banco
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var property = CreateValidProperty(seller.Id, condominium.Id, "Imóvel Busca");
        await dbContext.Properties.AddAsync(property);
        await dbContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/v1/properties/{property.Id}");
        var result = await response.Content.ReadFromJsonAsync<Response<Property>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(property.Id);
        result.Data.Title.Should().Be("Imóvel Busca");
    }

    [Fact]
    public async Task GetPropertyById_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/properties/999");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    #endregion

    #region POST /v1/properties - CreatePropertyEndpoint Tests

    [Fact]
    public async Task CreateProperty_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var seller = await CreateValidSeller();
        var condominium = await CreateValidCondominium();
        
        var client = _factory.CreateClient();
        var property = new Property
        {
            Title = "Apartamento Novo",
            Description = "Apartamento recém-construído com acabamento de luxo",
            Price = 750000.00m,
            PropertyType = EPropertyType.Apartment,
            Bedroom = 4,
            Bathroom = 3,
            Garage = 2,
            Area = 150.0,
            TransactionsDetails = "Financiamento disponível",
            SellerId = seller.Id,
            CondominiumId = condominium.Id,
            RegistryNumber = "987654321",
            RegistryRecord = "123456789",
            IsNew = true,
            ShowInHome = true,
            Address = new Address
            {
                Street = "Av. Paulista",
                Number = "1000",
                Complement = "Apto 1501",
                Neighborhood = "Bela Vista",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01310-100"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/properties", property);
        var result = await response.Content.ReadFromJsonAsync<Response<Property>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be("Apartamento Novo");
        result.Data.Price.Should().Be(750000.00m);
        result.Data.PropertyType.Should().Be(EPropertyType.Apartment);
    }

    [Fact]
    public async Task CreateProperty_WithMissingRequiredFields_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var property = new Property
        {
            // Title is missing - required field
            Description = "Descrição teste",
            Price = 500000.00m
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/properties", property);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProperty_WithInvalidPrice_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var seller = await CreateValidSeller();
        var condominium = await CreateValidCondominium();
        
        var client = _factory.CreateClient();
        var property = new Property
        {
            Title = "Imóvel Teste",
            Description = "Descrição teste",
            Price = -1000.00m, // Preço negativo
            PropertyType = EPropertyType.Apartment,
            Bedroom = 2,
            Bathroom = 1,
            Garage = 1,
            Area = 80.0,
            TransactionsDetails = "Detalhes",
            SellerId = seller.Id,
            CondominiumId = condominium.Id,
            RegistryNumber = "123456789",
            RegistryRecord = "987654321"
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/properties", property);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProperty_WithMaxLengthExceeded_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var seller = await CreateValidSeller();
        var condominium = await CreateValidCondominium();
        
        var client = _factory.CreateClient();
        var property = new Property
        {
            Title = new string('A', 121), // Título com mais de 120 caracteres (limite)
            Description = "Descrição teste",
            Price = 500000.00m,
            PropertyType = EPropertyType.Apartment,
            Bedroom = 2,
            Bathroom = 1,
            Garage = 1,
            Area = 80.0,
            TransactionsDetails = "Detalhes",
            SellerId = seller.Id,
            CondominiumId = condominium.Id,
            RegistryNumber = "123456789",
            RegistryRecord = "987654321"
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/properties", property);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region PUT /v1/properties/{id} - UpdatePropertyEndpoint Tests

    [Fact]
    public async Task UpdateProperty_WithValidData_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var seller = await CreateValidSeller();
        var condominium = await CreateValidCondominium();
        
        // Cria uma propriedade diretamente no banco para atualizar
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var property = CreateValidProperty(seller.Id, condominium.Id, "Imóvel Original");
        await dbContext.Properties.AddAsync(property);
        await dbContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

        var updatedProperty = new Property
        {
            Id = property.Id,
            Title = "Imóvel Atualizado",
            Description = "Descrição atualizada",
            Price = 600000.00m,
            PropertyType = EPropertyType.House,
            Bedroom = 4,
            Bathroom = 3,
            Garage = 2,
            Area = 180.0,
            TransactionsDetails = "Detalhes atualizados",
            SellerId = seller.Id,
            CondominiumId = condominium.Id,
            RegistryNumber = "111222333",
            RegistryRecord = "333222111",
            Address = new Address
            {
                Street = "Rua Atualizada",
                Number = "999",
                Neighborhood = "Bairro Novo",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234-567"
            }
        };

        // Act
        var response = await client.PutAsJsonAsync($"/v1/properties/{property.Id}", updatedProperty);
        var result = await response.Content.ReadFromJsonAsync<Response<Property>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be("Imóvel Atualizado");
        result.Data.Price.Should().Be(600000.00m);
        result.Data.PropertyType.Should().Be(EPropertyType.House);
    }

    [Fact]
    public async Task UpdateProperty_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var seller = await CreateValidSeller();
        var condominium = await CreateValidCondominium();
        
        var client = _factory.CreateClient();
        var property = new Property
        {
            Id = 999,
            Title = "Imóvel Teste",
            Description = "Descrição teste",
            Price = 500000.00m,
            PropertyType = EPropertyType.Apartment,
            Bedroom = 2,
            Bathroom = 1,
            Garage = 1,
            Area = 80.0,
            TransactionsDetails = "Detalhes",
            SellerId = seller.Id,
            CondominiumId = condominium.Id,
            RegistryNumber = "123456789",
            RegistryRecord = "987654321"
        };

        // Act
        var response = await client.PutAsJsonAsync("/v1/properties/999", property);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region DELETE /v1/properties/{id} - DeletePropertyEndpoint Tests

    [Fact]
    public async Task DeleteProperty_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var seller = await CreateValidSeller();
        var condominium = await CreateValidCondominium();
        
        // Cria uma propriedade diretamente no banco para deletar
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var property = CreateValidProperty(seller.Id, condominium.Id, "Imóvel Para Deletar");
        await dbContext.Properties.AddAsync(property);
        await dbContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/v1/properties/{property.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify property is soft deleted - should return BadRequest when trying to access
        var getResponse = await client.GetAsync($"/v1/properties/{property.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteProperty_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/v1/properties/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Field Validation Tests

    [Fact]
    public async Task CreateProperty_WithInvalidBedroomCount_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var seller = await CreateValidSeller();
        var condominium = await CreateValidCondominium();
        
        var client = _factory.CreateClient();
        var property = new Property
        {
            Title = "Imóvel Teste",
            Description = "Descrição teste",
            Price = 500000.00m,
            PropertyType = EPropertyType.Apartment,
            Bedroom = -1, // Número negativo de quartos
            Bathroom = 1,
            Garage = 1,
            Area = 80.0,
            TransactionsDetails = "Detalhes",
            SellerId = seller.Id,
            CondominiumId = condominium.Id,
            RegistryNumber = "123456789",
            RegistryRecord = "987654321"
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/properties", property);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProperty_WithInvalidArea_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var seller = await CreateValidSeller();
        var condominium = await CreateValidCondominium();
        
        var client = _factory.CreateClient();
        var property = new Property
        {
            Title = "Imóvel Teste",
            Description = "Descrição teste",
            Price = 500000.00m,
            PropertyType = EPropertyType.Apartment,
            Bedroom = 2,
            Bathroom = 1,
            Garage = 1,
            Area = 0, // Área zero
            TransactionsDetails = "Detalhes",
            SellerId = seller.Id,
            CondominiumId = condominium.Id,
            RegistryNumber = "123456789",
            RegistryRecord = "987654321"
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/properties", property);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Infrastructure Tests

    [Fact]
    public async Task PropertyFake_GetFakeProperties_ShouldCreateCorrectQuantity()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var seller = await CreateValidSeller();
        var condominium = await CreateValidCondominium();
        
        // Act
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var fakeProperties = PropertyFake.GetFakeProperties(3, (int)seller.Id, (int)condominium.Id);
        
        // Atribui o UserId correto
        foreach (var property in fakeProperties)
        {
            property.UserId = RealtyHubApiTests.TestUserId;
        }
        
        await dbContext.Properties.AddRangeAsync(fakeProperties);
        await dbContext.SaveChangesAsync();
        
        var propertyCount = await dbContext.Properties.CountAsync();

        // Assert
        propertyCount.Should().Be(3);
    }

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
