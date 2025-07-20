using FluentAssertions;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using RealtyHub.Tests.Common;
using System.Net.Http.Json;

namespace RealtyHub.Tests.Entities.Contracts;

/// <summary>
/// Classe de testes de integração para os endpoints de Contract.
/// Estes testes focam na validação de campos, lógica de negócio e CRUD,
/// sem se preocupar com autenticação (que é bypassada).
/// Cada teste é completamente isolado e limpa o banco antes da execução.
/// </summary>
public class ContractTests : BaseIntegrationTest
{
    public ContractTests(RealtyHubApiTests factory) : base(factory)
    {
    }

    #region GET /v1/contracts - GetAllContractsEndpoint Tests

    [Fact]
    public async Task GetAllContracts_ShouldReturnPagedResponse()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Contract>();
        var (_, dbContext) = CreateDbContext();
        
        for (var i = 1; i <= 5; i++) 
            await CreateCompleteContractScenarioAsync(dbContext);

        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/contracts");
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<Contract>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(5);
        result.TotalCount.Should().Be(5);
    }

    [Fact]
    public async Task GetAllContracts_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Contract>();
        var (_, dbContext) = CreateDbContext();

        for (var i = 1; i <= 15; i++) 
            await CreateCompleteContractScenarioAsync(dbContext);

        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/contracts?pageNumber=1&pageSize=10");
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<Contract>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.TotalCount.Should().Be(15);
    }

    #endregion

    #region GET /v1/contracts/{id} - GetContractByIdEndpoint Tests

    [Fact]
    public async Task GetContractById_WithValidId_ShouldReturnContract()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Contract>();
        var (_, dbContext) = CreateDbContext();

        var (_, seller, buyer, _, offer, contract) = await CreateCompleteContractScenarioAsync(dbContext);

        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/v1/contracts/{contract.Id}");
        var result = await response.Content.ReadFromJsonAsync<Response<Contract>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(contract.Id);
        result.Data.SellerId.Should().Be(seller.Id);
        result.Data.BuyerId.Should().Be(buyer.Id);
        result.Data.OfferId.Should().Be(offer.Id);
    }

    [Fact]
    public async Task GetContractById_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Contract>();
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/contracts/999");
        var message = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        message.Should().Contain("não encontrado");
    }

    #endregion

    #region POST /v1/contracts - CreateContractEndpoint Tests

    [Fact]
    public async Task CreateContract_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Contract>();
        var (_, dbContext) = CreateDbContext();

        var (_, seller, buyer, _, offer) = await CreateBasicScenarioAsync(dbContext);

        var client = Factory.CreateClient();

        var contractRequest = new Contract
        {
            SellerId = seller.Id,
            BuyerId = buyer.Id,
            OfferId = offer.Id,
            IssueDate = DateTime.Now,
            EffectiveDate = DateTime.Now.AddDays(30),
            TermEndDate = DateTime.Now.AddYears(1),
            SignatureDate = DateTime.Now.AddDays(7),
            FileId = Guid.NewGuid().ToString(),
            UserId = RealtyHubApiTests.TestUserId
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/contracts", contractRequest);
        var result = await response.Content.ReadFromJsonAsync<Response<Contract>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.SellerId.Should().Be(seller.Id);
        result.Data.BuyerId.Should().Be(buyer.Id);
        result.Data.OfferId.Should().Be(offer.Id);
        result.Data.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateContract_WithMissingRequiredFields_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Contract>();
        var client = Factory.CreateClient();
        var contract = new Contract
        {
            // Data de emissão ausente
            EffectiveDate = DateTime.Now.AddDays(30),
            TermEndDate = DateTime.Now.AddYears(1)
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/contracts", contract);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateContract_WithInvalidSellerId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Contract>();
        var (_, dbContext) = CreateDbContext();

        var (_, _, buyer, _, offer) = await CreateBasicScenarioAsync(dbContext);

        var client = Factory.CreateClient();
        var contract = new Contract
        {
            SellerId = 999, // ID inválido
            BuyerId = buyer.Id,
            OfferId = offer.Id,
            IssueDate = DateTime.Now,
            EffectiveDate = DateTime.Now.AddDays(30),
            TermEndDate = DateTime.Now.AddYears(1)
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/contracts", contract);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateContract_WithInvalidBuyerId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Contract>();
        var (_, dbContext) = CreateDbContext();
        var (_, seller, _, _, offer) = await CreateBasicScenarioAsync(dbContext);

        var client = Factory.CreateClient();
        var contract = new Contract
        {
            SellerId = seller.Id,
            BuyerId = 999, // ID inválido
            OfferId = offer.Id,
            IssueDate = DateTime.Now,
            EffectiveDate = DateTime.Now.AddDays(30),
            TermEndDate = DateTime.Now.AddYears(1)
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/contracts", contract);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateContract_WithInvalidOfferId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Contract>();
        var (_, dbContext) = CreateDbContext();

        var (_, seller, buyer) = await CreateMinimalScenarioAsync(dbContext);

        var client = Factory.CreateClient();
        var contract = new Contract
        {
            SellerId = seller.Id,
            BuyerId = buyer.Id,
            OfferId = 999, // ID inválido
            IssueDate = DateTime.Now,
            EffectiveDate = DateTime.Now.AddDays(30),
            TermEndDate = DateTime.Now.AddYears(1)
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/contracts", contract);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region PUT /v1/contracts/{id} - UpdateContractEndpoint Tests

    [Fact]
    public async Task UpdateContract_WithValidData_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Contract>();
        var (_, dbContext) = CreateDbContext();

        var (_, seller, buyer, _, offer, contract) = await CreateCompleteContractScenarioAsync(dbContext);

        var client = Factory.CreateClient();

        var updatedContract = new Contract
        {
            Id = contract.Id,
            SellerId = seller.Id,
            BuyerId = buyer.Id,
            OfferId = offer.Id,
            IssueDate = DateTime.Now.AddDays(-10),
            EffectiveDate = DateTime.Now.AddDays(60),
            TermEndDate = DateTime.Now.AddYears(2),
            SignatureDate = DateTime.Now.AddDays(14),
            FileId = Guid.NewGuid().ToString()
        };

        // Act
        var response = await client.PutAsJsonAsync($"/v1/contracts/{contract.Id}", updatedContract);
        var result = await response.Content.ReadFromJsonAsync<Response<Contract>>();

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.EffectiveDate.Should().Be(updatedContract.EffectiveDate);
        result.Data.TermEndDate.Should().Be(updatedContract.TermEndDate);
    }

    [Fact]
    public async Task UpdateContract_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Contract>();
        var (_, dbContext) = CreateDbContext();

        var (_, seller, buyer, _, offer) = await CreateBasicScenarioAsync(dbContext);

        var client = Factory.CreateClient();
        var contract = new Contract
        {
            Id = 999,
            SellerId = seller.Id,
            BuyerId = buyer.Id,
            OfferId = offer.Id,
            IssueDate = DateTime.Now,
            EffectiveDate = DateTime.Now.AddDays(30),
            TermEndDate = DateTime.Now.AddYears(1)
        };

        // Act
        var response = await client.PutAsJsonAsync("/v1/contracts/999", contract);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region DELETE /v1/contracts/{id} - DeleteContractEndpoint Tests

    [Fact]
    public async Task DeleteContract_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Contract>();
        var (_, dbContext) = CreateDbContext();
        var (_, _, _, _, _, contract) = await CreateCompleteContractScenarioAsync(dbContext);
        var client = Factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/v1/contracts/{contract.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verifica se o contrato foi realmente removido
        var getResponse = await client.GetAsync($"/v1/contracts/{contract.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var message = await getResponse.Content.ReadAsStringAsync();
        message.Should().Contain("não encontrado");
    }

    [Fact]
    public async Task DeleteContract_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Contract>();
        var client = Factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/v1/contracts/999");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest);
        var message = await response.Content.ReadAsStringAsync();
        message.Should().Contain("não encontrado");
    }

    #endregion

    #region Field Validation Tests

    [Fact]
    public async Task CreateContract_WithPastEffectiveDate_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Contract>();
        var (_, dbContext) = CreateDbContext();

        var (_, seller, buyer, _, offer) = await CreateBasicScenarioAsync(dbContext);

        var client = Factory.CreateClient();
        var contract = new Contract
        {
            SellerId = seller.Id,
            BuyerId = buyer.Id,
            OfferId = offer.Id,
            IssueDate = DateTime.Now,
            EffectiveDate = DateTime.Now.AddDays(-1), // Data no passado
            TermEndDate = DateTime.Now.AddYears(1)
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/contracts", contract);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateContract_WithTermEndDateBeforeEffectiveDate_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Contract>();
        var (_, dbContext) = CreateDbContext();

        var (_, seller, buyer, _, offer) = await CreateBasicScenarioAsync(dbContext);

        var client = Factory.CreateClient();
        var contract = new Contract
        {
            SellerId = seller.Id,
            BuyerId = buyer.Id,
            OfferId = offer.Id,
            IssueDate = DateTime.Now,
            EffectiveDate = DateTime.Now.AddDays(30),
            TermEndDate = DateTime.Now.AddDays(15) // Data de término antes da vigência
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/contracts", contract);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion
}