using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.Tests.Entities.Condominiums;

/// <summary>
/// Classe de testes de integração para os endpoints de Condominium.
/// Estes testes focam na validação de campos, lógica de negócio e CRUD,
/// sem se preocupar com autenticação (que é bypassada).
/// Cada teste é completamente isolado e limpa o banco antes da execução.
/// </summary>
public class CondominiumTests : IClassFixture<RealtyHubApiTests>
{
    private readonly RealtyHubApiTests _factory;

    public CondominiumTests(RealtyHubApiTests factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Limpa o banco de dados e retorna o número de condomínios encontrados antes da limpeza.
    /// </summary>
    private async Task<int> CleanupDatabaseAndGetPreviousCount()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Conta quantos condomínios existiam antes da limpeza
        var existingCount = await dbContext.Condominiums.CountAsync();
        
        // Remove todos os condomínios existentes
        var existingCondominiums = await dbContext.Condominiums.ToListAsync();
        dbContext.Condominiums.RemoveRange(existingCondominiums);
        await dbContext.SaveChangesAsync();
        
        return existingCount;
    }

    /// <summary>
    /// Cria um condomínio válido para uso nos testes.
    /// </summary>
    private static Condominium CreateValidCondominium(string name = "Condomínio Teste")
    {
        return new Condominium
        {
            Name = name,
            Address = new Address
            {
                Street = "Rua do Condomínio",
                Number = "123",
                Neighborhood = "Centro",
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
            IsActive = true,
            UserId = RealtyHubApiTests.TestUserId
        };
    }

    #region GET /v1/condominiums - GetAllCondominiumsEndpoint Tests

    [Fact]
    public async Task GetAllCondominiums_ShouldReturnPagedResponse()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        
        // Cria 5 condomínios
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var condominiums = new List<Condominium>();
        
        for (int i = 1; i <= 5; i++)
        {
            var condominium = CreateValidCondominium($"Condomínio {i}");
            condominiums.Add(condominium);
        }
        
        await dbContext.Condominiums.AddRangeAsync(condominiums);
        await dbContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount();
        
        // Cria 15 condomínios
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var condominiums = new List<Condominium>();
        
        for (int i = 1; i <= 15; i++)
        {
            var condominium = CreateValidCondominium($"Condomínio {i}");
            condominiums.Add(condominium);
        }
        
        await dbContext.Condominiums.AddRangeAsync(condominiums);
        await dbContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount();
        
        // Cria condomínios com nomes específicos
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var condominium1 = CreateValidCondominium("Residencial Luxo");
        var condominium2 = CreateValidCondominium("Edifício Simples");
        var condominium3 = CreateValidCondominium("Condomínio Centro");
        
        await dbContext.Condominiums.AddRangeAsync(condominium1, condominium2, condominium3);
        await dbContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount();
        
        // Cria um condomínio diretamente no banco
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var condominium = CreateValidCondominium("Condomínio Busca");
        await dbContext.Condominiums.AddAsync(condominium);
        await dbContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();

        var condominium = new Condominium
        {
            Name = "Residencial Novo",
            Address = new Address
            {
                Street = "Av. Paulista",
                Number = "1000",
                Neighborhood = "Bela Vista",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01310-100"
            },
            Units = 80,
            Floors = 15,
            HasElevator = true,
            HasSwimmingPool = true,
            HasPartyRoom = true,
            HasPlayground = true,
            HasFitnessRoom = true,
            CondominiumValue = 500.00m
        };

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
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var condominium = new Condominium
        {
            // Name is missing - required field
            Units = 50,
            Floors = 10
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/condominiums", condominium);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCondominium_WithMaxLengthExceeded_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var condominium = new Condominium
        {
            Name = new string('A', 121), // Nome com mais de 120 caracteres (limite)
            Address = new Address
            {
                Street = "Rua Teste",
                Number = "123",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234567"
            },
            Units = 50,
            Floors = 10
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/condominiums", condominium);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCondominium_WithMinimalData_ShouldReturnCreated()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var condominium = new Condominium
        {
            Name = "Edifício Simples",
            Address = new Address
            {
                Street = "Rua Simples",
                Number = "200",
                Neighborhood = "Bairro Simples",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234567"
            },
            Units = 20,
            Floors = 4,
            HasElevator = false,
            HasSwimmingPool = false,
            HasPartyRoom = false,
            HasPlayground = false,
            HasFitnessRoom = false,
            CondominiumValue = 150.00m
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/condominiums", condominium);
        var result = await response.Content.ReadFromJsonAsync<Response<Condominium>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Edifício Simples");
        result.Data.HasElevator.Should().BeFalse();
        result.Data.HasSwimmingPool.Should().BeFalse();
        result.Data.CondominiumValue.Should().Be(150.00m);
    }

    #endregion

    #region PUT /v1/condominiums/{id} - UpdateCondominiumEndpoint Tests

    [Fact]
    public async Task UpdateCondominium_WithValidData_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        
        // Cria um condomínio diretamente no banco para atualizar
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var condominium = CreateValidCondominium("Condomínio Original");
        await dbContext.Condominiums.AddAsync(condominium);
        await dbContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

        var updatedCondominium = new Condominium
        {
            Id = condominium.Id,
            Name = "Condomínio Atualizado",
            Address = new Address
            {
                Street = "Rua Atualizada",
                Number = "999",
                Neighborhood = "Bairro Novo",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234-567"
            },
            Units = 100,
            Floors = 20,
            HasElevator = true,
            HasSwimmingPool = true,
            HasPartyRoom = true,
            HasPlayground = true,
            HasFitnessRoom = true,
            CondominiumValue = 750.00m
        };

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
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var condominium = new Condominium
        {
            Id = 999,
            Name = "Condomínio Teste",
            Address = new Address
            {
                Street = "Rua Teste",
                Number = "123",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234567"
            },
            Units = 50,
            Floors = 10
        };

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
        await CleanupDatabaseAndGetPreviousCount();
        
        // Cria um condomínio diretamente no banco para deletar
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var condominium = CreateValidCondominium("Condomínio Para Deletar");
        await dbContext.Condominiums.AddAsync(condominium);
        await dbContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var condominium = new Condominium
        {
            Name = "Condomínio Teste",
            Address = new Address
            {
                Street = "Rua Teste",
                Number = "123",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234567"
            },
            Units = -1, // Número negativo de unidades
            Floors = 10
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/condominiums", condominium);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCondominium_WithInvalidFloors_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var condominium = new Condominium
        {
            Name = "Condomínio Teste",
            Address = new Address
            {
                Street = "Rua Teste",
                Number = "123",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234567"
            },
            Units = 50,
            Floors = 0 // Número zero de andares
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/condominiums", condominium);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

}