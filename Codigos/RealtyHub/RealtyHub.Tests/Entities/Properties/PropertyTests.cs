using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using RealtyHub.Core.Utilities.FakeEntities;
using RealtyHub.Tests.Common;

namespace RealtyHub.Tests.Entities.Properties;

/// <summary>
/// Classe de testes de integração para os endpoints de Property.
/// Estes testes focam na validação de campos, lógica de negócio e CRUD,
/// sem se preocupar com autenticação (que é bypassada).
/// Cada teste é completamente isolado e limpa o banco antes da execução.
/// </summary>
public class PropertyTests : BaseIntegrationTest
{
    public PropertyTests(RealtyHubApiTests factory) : base(factory)
    {
    }

    #region GET /v1/properties - GetAllPropertiesEndpoint Tests

    [Fact]
    public async Task GetAllProperties_ShouldReturnPagedResponse()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Property>();
        var (_, dbContext) = CreateDbContext();
        
        for (var i = 1; i <= 5; i++)
        {
            var (_, _, _, property, _) = await CreateBasicScenarioAsync(dbContext);
            property.Title = $"Imóvel {i}";
            dbContext.Properties.Update(property);
            await dbContext.SaveChangesAsync();
        }
        
        var client = Factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount<Property>();
        var (_, dbContext) = CreateDbContext();
        
        for (var i = 1; i <= 15; i++)
        {
            var (_, _, _, property, _) = await CreateBasicScenarioAsync(dbContext);
            property.Title = $"Imóvel {i}";
            dbContext.Properties.Update(property);
            await dbContext.SaveChangesAsync();
        }
        
        var client = Factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount<Property>();
        var (_, dbContext) = CreateDbContext();
        
        var (condominium, seller, _) = await CreateMinimalScenarioAsync(dbContext);
        
        var property1 = MockData.GetValidProperty(seller, condominium);
        property1.Title = "Apartamento Luxo";
        await dbContext.Properties.AddAsync(property1);
        
        var property2 = MockData.GetValidProperty(seller, condominium);
        property2.Title = "Casa Simples";
        await dbContext.Properties.AddAsync(property2);
        
        var property3 = MockData.GetValidProperty(seller, condominium);
        property3.Title = "Kitnet Centro";
        await dbContext.Properties.AddAsync(property3);
        
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount<Property>();
        var (_, dbContext) = CreateDbContext();
        
        var (_, _, _, property, _) = await CreateBasicScenarioAsync(dbContext);
        property.Title = "Imóvel Busca";
        dbContext.Properties.Update(property);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount<Property>();
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/properties/999");
        var message = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        message.Should().Contain("não encontrado");
    }

    #endregion

    #region POST /v1/properties - CreatePropertyEndpoint Tests

    [Fact]
    public async Task CreateProperty_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Property>();
        var (_, dbContext) = CreateDbContext();
        
        var (condominium, seller, _) = await CreateMinimalScenarioAsync(dbContext);
        
        var client = Factory.CreateClient();
        var property = MockData.GetValidProperty(seller, condominium);
        property.Title = "Apartamento Novo";
        property.Price = 750000.00m;
        property.PropertyType = EPropertyType.Apartment;

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
        await CleanupDatabaseAndGetPreviousCount<Property>();
        var client = Factory.CreateClient();
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
        await CleanupDatabaseAndGetPreviousCount<Property>();
        var (_, dbContext) = CreateDbContext();
        
        var (condominium, seller, _) = await CreateMinimalScenarioAsync(dbContext);
        
        var client = Factory.CreateClient();
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
        await CleanupDatabaseAndGetPreviousCount<Property>();
        var (_, dbContext) = CreateDbContext();
        
        var (condominium, seller, _) = await CreateMinimalScenarioAsync(dbContext);
        
        var client = Factory.CreateClient();
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
        await CleanupDatabaseAndGetPreviousCount<Property>();
        var (_, dbContext) = CreateDbContext();
        
        var (condominium, seller, _, property, _) = await CreateBasicScenarioAsync(dbContext);
        property.Title = "Imóvel Original";
        dbContext.Properties.Update(property);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();
        
        var updatedProperty = MockData.GetValidProperty(seller, condominium);
        updatedProperty.Id = property.Id; // Mantem o ID do imóvel existente
        updatedProperty.Title = "Imóvel Atualizado";
        updatedProperty.Price = 600000.00m; // Atualiza o preço
        updatedProperty.PropertyType = EPropertyType.House; // Atualiza o tipo de imóvel

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
        await CleanupDatabaseAndGetPreviousCount<Property>();
        var (_, dbContext) = CreateDbContext();
        
        var (condominium, seller, _) = await CreateMinimalScenarioAsync(dbContext);
        
        var client = Factory.CreateClient();
       
        var property = MockData.GetValidProperty(seller, condominium);
        property.Id = 999; // ID inválido

        // Act
        var response = await client.PutAsJsonAsync($"/v1/properties/{property.Id}", property);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region DELETE /v1/properties/{id} - DeletePropertyEndpoint Tests

    [Fact]
    public async Task DeleteProperty_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Property>();
        var (_, dbContext) = CreateDbContext();
        
        var (_, _, _, property, _) = await CreateBasicScenarioAsync(dbContext);
        property.Title = "Imóvel Para Deletar";
        dbContext.Properties.Update(property);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/v1/properties/{property.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verificando se o imóvel foi realmente removido
        var getResponse = await client.GetAsync($"/v1/properties/{property.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteProperty_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Property>();
        var client = Factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount<Property>();
        var (_, dbContext) = CreateDbContext();
        
        var (condominium, seller, _) = await CreateMinimalScenarioAsync(dbContext);
        
        var client = Factory.CreateClient();

        var property = MockData.GetValidProperty(seller, condominium);
        property.Bedroom = -1; // Contagem de quartos inválida

        // Act
        var response = await client.PostAsJsonAsync("/v1/properties", property);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProperty_WithInvalidArea_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Property>();
        var (_, dbContext) = CreateDbContext();
        
        var (condominium, seller, _) = await CreateMinimalScenarioAsync(dbContext);
        
        var client = Factory.CreateClient();

        var property = MockData.GetValidProperty(seller, condominium);
        property.Area = 0; // Área invalida

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
        await CleanupDatabaseAndGetPreviousCount<Property>();
        var (_, dbContext) = CreateDbContext();
        
        var (condominium, seller, _) = await CreateMinimalScenarioAsync(dbContext);
        
        // Act
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

    #endregion
}
