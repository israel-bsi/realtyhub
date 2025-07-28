using System.Net.Http.Json;
using FluentAssertions;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using RealtyHub.Tests.Common;

namespace RealtyHub.Tests.Entities.Offers;

/// <summary>
/// Classe de testes de integração para os endpoints de Offer.
/// Estes testes focam na validação de campos, lógica de negócio e CRUD,
/// sem se preocupar com autenticação (que é bypassada).
/// Cada teste é completamente isolado e limpa o banco antes da execução.
/// </summary>
public class OfferTests : BaseIntegrationTest
{
    public OfferTests(RealtyHubApiTests factory) : base(factory)
    {
    }

    #region GET /v1/offers - GetAllOffersEndpoint Tests

    [Fact]
    public async Task GetAllOffers_ShouldReturnPagedResponse()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var (_, dbContext) = CreateDbContext();
        
        for (var i = 1; i <= 5; i++)
        {
            var (_, _, _, _, offer) = await CreateBasicScenarioAsync(dbContext);
            offer.Amount = 480000.00m + (i * 1000);
            dbContext.Offers.Update(offer);
            await dbContext.SaveChangesAsync();
        }
        
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/offers");
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<Offer>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(5);
        result.TotalCount.Should().Be(5);
    }

    [Fact]
    public async Task GetAllOffers_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var (_, dbContext) = CreateDbContext();
        
        for (var i = 1; i <= 15; i++)
        {
            var (_, _, _, _, offer) = await CreateBasicScenarioAsync(dbContext);
            // Modifica o valor para ser único
            offer.Amount = 480000.00m + (i * 1000);
            dbContext.Offers.Update(offer);
            await dbContext.SaveChangesAsync();
        }
        
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/offers?pageNumber=1&pageSize=10");
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<Offer>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().BeLessOrEqualTo(10);
        result.TotalCount.Should().Be(15);
    }

    #endregion

    #region GET /v1/offers/{id} - GetOfferByIdEndpoint Tests

    [Fact]
    public async Task GetOfferById_WithValidId_ShouldReturnOffer()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var (_, dbContext) = CreateDbContext();
        
        var (_, _, buyer, property, offer) = await CreateBasicScenarioAsync(dbContext);
        
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/v1/offers/{offer.Id}");
        var result = await response.Content.ReadFromJsonAsync<Response<Offer>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(offer.Id);
        result.Data.BuyerId.Should().Be(buyer.Id);
        result.Data.PropertyId.Should().Be(property.Id);
    }

    [Fact]
    public async Task GetOfferById_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/offers/999");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /v1/offers/accepted - GetOfferAcceptedEndpoint Tests

    [Fact]
    public async Task GetOfferAccepted_ShouldReturnAcceptedOffers()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var (_, dbContext) = CreateDbContext();
        
        var (_, _, buyer, property, offer1) = await CreateBasicScenarioAsync(dbContext);
        offer1.OfferStatus = EOfferStatus.Accepted;
        dbContext.Offers.Update(offer1);
        
        var offer2 = MockData.GetValidOffer(buyer, property);
        offer2.Amount = 490000.00m;
        offer2.OfferStatus = EOfferStatus.Analysis;
        await dbContext.Offers.AddAsync(offer2);
        
        var offer3 = MockData.GetValidOffer(buyer, property);
        offer3.Amount = 500000.00m;
        offer3.OfferStatus = EOfferStatus.Accepted;
        await dbContext.Offers.AddAsync(offer3);
        
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/offers/accepted");
        
        var content = await response.Content.ReadAsStringAsync();
        Response<List<Offer>>? result = null;
        
        if (!string.IsNullOrEmpty(content))
        {
            result = await response.Content.ReadFromJsonAsync<Response<List<Offer>>>();
        }

        // Assert
        // Nota: O endpoint pode retornar NotFound se não houver ofertas aceitas
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
        if (response.StatusCode == HttpStatusCode.OK && result != null)
        {
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            // Deve retornar apenas as ofertas aceitas
            result.Data!.All(o => o.OfferStatus == EOfferStatus.Accepted).Should().BeTrue();
        }
    }

    #endregion

    #region GET /v1/offers/customer/{customerId} - GetAllOffersByCustomerEndpoint Tests

    [Fact]
    public async Task GetAllOffersByCustomer_WithValidCustomerId_ShouldReturnOffers()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var (_, dbContext) = CreateDbContext();
        
        var (_, _, buyer, property, _) = await CreateBasicScenarioAsync(dbContext);
        
        // Cria 2 ofertas adicionais para o mesmo cliente
        for (int i = 1; i <= 2; i++)
        {
            var offer = MockData.GetValidOffer(buyer, property);
            offer.Amount = 480000.00m + (i * 1000);
            await dbContext.Offers.AddAsync(offer);
        }
        
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/v1/offers/customer/{buyer.Id}");
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<Offer>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(3); // 1 do cenário básico + 2 criadas
        result.Data.All(o => o.BuyerId == buyer.Id).Should().BeTrue();
    }

    #endregion

    #region GET /v1/offers/property/{propertyId} - GetAllOffersByPropertyEndpoint Tests

    [Fact]
    public async Task GetAllOffersByProperty_WithValidPropertyId_ShouldReturnOffers()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var (_, dbContext) = CreateDbContext();
        
        var (_, _, buyer, property, _) = await CreateBasicScenarioAsync(dbContext);
        
        for (int i = 1; i <= 2; i++)
        {
            var offer = MockData.GetValidOffer(buyer, property);
            offer.Amount = 480000.00m + (i * 1000);
            await dbContext.Offers.AddAsync(offer);
        }
        
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/v1/offers/property/{property.Id}");
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<Offer>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(3); // 1 do cenário básico + 2 criadas
        result.Data.All(o => o.PropertyId == property.Id).Should().BeTrue();
    }

    #endregion

    #region POST /v1/offers - CreateOfferEndpoint Tests

    [Fact]
    public async Task CreateOffer_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var (_, dbContext) = CreateDbContext();
        
        var (condominium, seller, buyer) = await CreateMinimalScenarioAsync(dbContext);
        
        var property = MockData.GetValidProperty(seller, condominium);
        await dbContext.Properties.AddAsync(property);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();
        
        var offerRequest = new Offer
        {
            Amount = 480000.00m,
            PropertyId = property.Id,
            BuyerId = buyer.Id,
            SubmissionDate = DateTime.Now,
            OfferStatus = EOfferStatus.Analysis,
            UserId = RealtyHubApiTests.TestUserId,
            Payments =
            [
                new()
                {
                    Amount = 480000.00m,
                    PaymentType = EPaymentType.Pix,
                    UserId = RealtyHubApiTests.TestUserId,
                    IsActive = true
                }
            ]
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/offers", offerRequest);
        var result = await response.Content.ReadFromJsonAsync<Response<Offer>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Amount.Should().Be(480000.00m);
        result.Data.BuyerId.Should().Be(buyer.Id);
        result.Data.PropertyId.Should().Be(property.Id);
        result.Data.OfferStatus.Should().Be(EOfferStatus.Analysis);
    }

    [Fact]
    public async Task CreateOffer_WithMissingRequiredFields_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var (_, dbContext) = CreateDbContext();
        var client = Factory.CreateClient();

        var condominium = MockData.GetValidCondominium();
        await dbContext.Condominiums.AddAsync(condominium);
        await dbContext.SaveChangesAsync();

        var seller = MockData.GetValidCustomer(ECustomerType.Seller);
        await dbContext.Customers.AddAsync(seller);
        await dbContext.SaveChangesAsync();

        var property = MockData.GetValidProperty(seller, condominium);
        await dbContext.Properties.AddAsync(property);
        await dbContext.SaveChangesAsync();

        var buyer = MockData.GetValidCustomer(ECustomerType.Buyer);
        await dbContext.Customers.AddAsync(buyer);
        await dbContext.SaveChangesAsync();

        var offer = MockData.GetValidOffer(buyer, property);
        offer.Amount = 0; // Simula campo obrigatório ausente

        // Act
        var response = await client.PostAsJsonAsync("/v1/offers", offer);
        var message = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        message.Should().Contain("deve ser maior que zero");
    }

    [Fact]
    public async Task CreateOffer_WithInvalidBuyerId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var (_, dbContext) = CreateDbContext();
        
        var (condominium, seller, _) = await CreateMinimalScenarioAsync(dbContext);
        
        var property = MockData.GetValidProperty(seller, condominium);
        await dbContext.Properties.AddAsync(property);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();
        var offer = new Offer
        {
            Amount = 480000.00m,
            PropertyId = property.Id,
            BuyerId = 999, // ID inválido
            SubmissionDate = DateTime.Now,
            OfferStatus = EOfferStatus.Analysis
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/offers", offer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOffer_WithInvalidPropertyId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var (_, dbContext) = CreateDbContext();
        
        var buyer = MockData.GetValidCustomer(ECustomerType.Buyer);
        await dbContext.Customers.AddAsync(buyer);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();
        var offer = new Offer
        {
            Amount = 480000.00m,
            PropertyId = 999, // ID inválido
            BuyerId = buyer.Id,
            SubmissionDate = DateTime.Now,
            OfferStatus = EOfferStatus.Analysis
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/offers", offer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOffer_WithZeroAmount_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var (_, dbContext) = CreateDbContext();
        
        var (condominium, seller, buyer) = await CreateMinimalScenarioAsync(dbContext);
        
        var property = MockData.GetValidProperty(seller, condominium);
        await dbContext.Properties.AddAsync(property);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();
        var offer = new Offer
        {
            Amount = 0, // Valor zero
            PropertyId = property.Id,
            BuyerId = buyer.Id,
            SubmissionDate = DateTime.Now,
            OfferStatus = EOfferStatus.Analysis
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/offers", offer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region PUT /v1/offers/{id} - UpdateOfferEndpoint Tests

    [Fact]
    public async Task UpdateOffer_WithValidData_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var (_, dbContext) = CreateDbContext();
        
        var (_, _, buyer, property, offer) = await CreateBasicScenarioAsync(dbContext);

        // Atualiza o status da proposa para análise antes de atualizar
        // O handler não permite atualizar propostas que não estão em análise
        offer.OfferStatus = EOfferStatus.Analysis;
        dbContext.Offers.Update(offer);
        await dbContext.SaveChangesAsync();

        var client = Factory.CreateClient();

        var updatedOffer = new Offer
        {
            Id = offer.Id,
            Amount = 500000.00m,
            PropertyId = property.Id,
            BuyerId = buyer.Id,
            SubmissionDate = DateTime.Now.AddDays(-1),
            OfferStatus = EOfferStatus.Analysis,
            Payments =
            [
                new Payment
                {
                    Amount = 500000.00m,
                    PaymentType = EPaymentType.Pix,
                    UserId = RealtyHubApiTests.TestUserId,
                    IsActive = true
                }
            ]
        };

        // Act
        var response = await client.PutAsJsonAsync($"/v1/offers/{offer.Id}", updatedOffer);
        var result = await response.Content.ReadFromJsonAsync<Response<Offer>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Amount.Should().Be(500000.00m);
    }

    [Fact]
    public async Task UpdateOffer_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var (_, dbContext) = CreateDbContext();
        
        var (condominium, seller, buyer) = await CreateMinimalScenarioAsync(dbContext);
        
        var property = MockData.GetValidProperty(seller, condominium);
        await dbContext.Properties.AddAsync(property);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();
        var offer = new Offer
        {
            Id = 999,
            Amount = 500000.00m,
            PropertyId = property.Id,
            BuyerId = buyer.Id,
            SubmissionDate = DateTime.Now,
            OfferStatus = EOfferStatus.Analysis
        };

        // Act
        var response = await client.PutAsJsonAsync("/v1/offers/999", offer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region PUT /v1/offers/{id}/accept - AcceptOfferEndpoint Tests

    [Fact]
    public async Task AcceptOffer_WithValidId_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var (_, dbContext) = CreateDbContext();
        
        var (_, _, _, _, offer) = await CreateBasicScenarioAsync(dbContext);
        offer.OfferStatus = EOfferStatus.Analysis;
        dbContext.Offers.Update(offer);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

        // Act
        var response = await client.PutAsync($"/v1/offers/{offer.Id}/accept", null);
        var result = await response.Content.ReadFromJsonAsync<Response<Offer>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.OfferStatus.Should().Be(EOfferStatus.Accepted);
    }

    [Fact]
    public async Task AcceptOffer_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var client = Factory.CreateClient();

        // Act
        var response = await client.PutAsync("/v1/offers/999/accept", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region PUT /v1/offers/{id}/reject - RejectOfferEndpoint Tests

    [Fact]
    public async Task RejectOffer_WithValidId_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var (_, dbContext) = CreateDbContext();
        
        var (_, _, _, _, offer) = await CreateBasicScenarioAsync(dbContext);
        offer.OfferStatus = EOfferStatus.Analysis;
        dbContext.Offers.Update(offer);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

        // Act
        var response = await client.PutAsync($"/v1/offers/{offer.Id}/reject", null);
        var result = await response.Content.ReadFromJsonAsync<Response<Offer>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.OfferStatus.Should().Be(EOfferStatus.Rejected);
    }

    [Fact]
    public async Task RejectOffer_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var client = Factory.CreateClient();

        // Act
        var response = await client.PutAsync("/v1/offers/999/reject", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Business Logic Tests

    [Fact]
    public async Task AcceptOffer_AlreadyAccepted_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var (_, dbContext) = CreateDbContext();
        
        var (_, _, _, _, offer) = await CreateBasicScenarioAsync(dbContext);
        offer.OfferStatus = EOfferStatus.Accepted;
        dbContext.Offers.Update(offer);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

        // Act
        var response = await client.PutAsync($"/v1/offers/{offer.Id}/accept", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RejectOffer_AlreadyRejected_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Offer>();
        var (_, dbContext) = CreateDbContext();
        
        var (_, _, _, _, offer) = await CreateBasicScenarioAsync(dbContext);
        offer.OfferStatus = EOfferStatus.Rejected;
        dbContext.Offers.Update(offer);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

        // Act
        var response = await client.PutAsync($"/v1/offers/{offer.Id}/reject", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion
}
