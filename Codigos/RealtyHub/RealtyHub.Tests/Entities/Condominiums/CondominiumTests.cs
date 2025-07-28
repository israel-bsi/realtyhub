using System.Net.Http.Json;
using FluentAssertions;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using RealtyHub.Tests.Common;

namespace RealtyHub.Tests.Entities.Condominiums;

/// <summary>
/// Classe de testes de integração para os endpoints de Condominium.
/// Estes testes focam na validação de campos, lógica de negócio e CRUD,
/// sem se preocupar com autenticação (que é bypassada).
/// Cada teste é completamente isolado e limpa o banco antes da execução.
/// </summary>
public class CondominiumTests : BaseIntegrationTest
{
    public CondominiumTests(RealtyHubApiTests factory) : base(factory)
    {
    }

    #region GET /v1/condominiums - GetAllCondominiumsEndpoint Tests

    [Fact]
    public async Task GetAllCondominiums_ShouldReturnPagedResponse()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Condominium>();
        var (_, dbContext) = CreateDbContext();
        
        for (var i = 1; i <= 5; i++)
        {
            var condominium = MockData.GetValidCondominium();
            condominium.Name = $"Condomínio {i}";
            await dbContext.Condominiums.AddAsync(condominium);
            await dbContext.SaveChangesAsync();
        }

        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/condominiums");
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<Condominium>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(5);
        result.TotalCount.Should().Be(5);
    }

    [Fact]
    public async Task GetAllCondominiums_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Condominium>();
        var (_, dbContext) = CreateDbContext();
        
        for (var i = 1; i <= 15; i++)
        {
            var condominium = MockData.GetValidCondominium();
            condominium.Name = $"Condomínio {i}";
            await dbContext.Condominiums.AddAsync(condominium);
            await dbContext.SaveChangesAsync();
        }
        
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/condominiums?pageNumber=1&pageSize=10");
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<Condominium>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().BeLessOrEqualTo(10);
        result.TotalCount.Should().Be(15);
    }

    [Fact]
    public async Task GetAllCondominiums_WithSearchTerm_ShouldFilterResults()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Condominium>();
        var (_, dbContext) = CreateDbContext();
        
        var condominium1 = MockData.GetValidCondominium();
        condominium1.Name = "Residencial Luxo";
        await dbContext.Condominiums.AddAsync(condominium1);
        
        var condominium2 = MockData.GetValidCondominium();
        condominium2.Name = "Edifício Simples";
        await dbContext.Condominiums.AddAsync(condominium2);
        
        var condominium3 = MockData.GetValidCondominium();
        condominium3.Name = "Condomínio Centro";
        await dbContext.Condominiums.AddAsync(condominium3);
        
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/condominiums?searchTerm=residencial");
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<Condominium>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        // Deve encontrar pelo menos o "Residencial Luxo"
        result.Data!.Any(c => c.Name.Contains("Residencial", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    #endregion

    #region GET /v1/condominiums/{id} - GetCondominiumByIdEndpoint Tests

    [Fact]
    public async Task GetCondominiumById_WithValidId_ShouldReturnCondominium()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Condominium>();
        var (_, dbContext) = CreateDbContext();
        
        var condominium = MockData.GetValidCondominium();
        condominium.Name = "Condomínio Busca";
        await dbContext.Condominiums.AddAsync(condominium);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/v1/condominiums/{condominium.Id}");
        var result = await response.Content.ReadFromJsonAsync<Response<Condominium>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(condominium.Id);
        result.Data.Name.Should().Be("Condomínio Busca");
    }

    [Fact]
    public async Task GetCondominiumById_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Condominium>();
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/condominiums/999");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    #endregion

    #region POST /v1/condominiums - CreateCondominiumEndpoint Tests

    [Fact]
    public async Task CreateCondominium_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Condominium>();
        var client = Factory.CreateClient();

        var condominium = MockData.GetValidCondominium();
        condominium.Name = "Residencial Novo";
        condominium.Units = 80;
        condominium.HasElevator = true;
        condominium.CondominiumValue = 500.00m;

        // Act
        var response = await client.PostAsJsonAsync("/v1/condominiums", condominium);
        var result = await response.Content.ReadFromJsonAsync<Response<Condominium>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Residencial Novo");
        result.Data.Units.Should().Be(80);
        result.Data.HasElevator.Should().BeTrue();
        result.Data.CondominiumValue.Should().Be(500.00m);
    }

    [Fact]
    public async Task CreateCondominium_WithMissingRequiredFields_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Condominium>();
        var client = Factory.CreateClient();
        
        var condominium = MockData.GetValidCondominium();
        condominium.Name = string.Empty; // Nome ausente

        // Act
        var response = await client.PostAsJsonAsync("/v1/condominiums", condominium);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCondominium_WithMaxLengthExceeded_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Condominium>();
        var client = Factory.CreateClient();

        var condominium = MockData.GetValidCondominium();
        condominium.Name = new string('A', 121); // Nome com mais de 120 caracteres (limite)

        // Act
        var response = await client.PostAsJsonAsync("/v1/condominiums", condominium);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region PUT /v1/condominiums/{id} - UpdateCondominiumEndpoint Tests

    [Fact]
    public async Task UpdateCondominium_WithValidData_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Condominium>();
        var (_, dbContext) = CreateDbContext();
        
        var condominium = MockData.GetValidCondominium();
        condominium.Name = "Condomínio Original";
        await dbContext.Condominiums.AddAsync(condominium);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

        var updatedCondominium = MockData.GetValidCondominium();
        updatedCondominium.Id = condominium.Id; // Manter o ID original
        updatedCondominium.Name = "Condomínio Atualizado";
        updatedCondominium.Units = 100;
        updatedCondominium.CondominiumValue = 750.00m;

        // Act
        var response = await client.PutAsJsonAsync($"/v1/condominiums/{condominium.Id}", updatedCondominium);
        var result = await response.Content.ReadFromJsonAsync<Response<Condominium>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Condomínio Atualizado");
        result.Data.Units.Should().Be(100);
        result.Data.CondominiumValue.Should().Be(750.00m);
    }

    [Fact]
    public async Task UpdateCondominium_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Condominium>();
        var client = Factory.CreateClient();
       
        var condominium = MockData.GetValidCondominium();
        condominium.Id = 999; // ID inválido

        // Act
        var response = await client.PutAsJsonAsync("/v1/condominiums/999", condominium);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region DELETE /v1/condominiums/{id} - DeleteCondominiumEndpoint Tests

    [Fact]
    public async Task DeleteCondominium_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Condominium>();
        var (_, dbContext) = CreateDbContext();
        
        var condominium = MockData.GetValidCondominium();
        condominium.Name = "Condomínio Para Deletar";
        await dbContext.Condominiums.AddAsync(condominium);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/v1/condominiums/{condominium.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify condominium is soft deleted - should return NotFound when trying to access
        var getResponse = await client.GetAsync($"/v1/condominiums/{condominium.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCondominium_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Condominium>();
        var client = Factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/v1/condominiums/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Field Validation Tests

    [Fact]
    public async Task CreateCondominium_WithInvalidUnits_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Condominium>();
        var client = Factory.CreateClient();
      
        var condominium = MockData.GetValidCondominium();
        condominium.Units = -10; // Número negativo de unidades

        // Act
        var response = await client.PostAsJsonAsync("/v1/condominiums", condominium);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCondominium_WithInvalidFloors_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Condominium>();
        var client = Factory.CreateClient();
    
        var condominium = MockData.GetValidCondominium();
        condominium.Floors = 0; // Número de andares inválido (0)

        // Act
        var response = await client.PostAsJsonAsync("/v1/condominiums", condominium);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion
}