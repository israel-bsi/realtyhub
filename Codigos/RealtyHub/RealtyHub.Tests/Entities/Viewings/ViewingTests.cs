using System.Net;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using RealtyHub.Tests.Common;

namespace RealtyHub.Tests.Entities.Viewings;

/// <summary>
/// Classe de testes de integração para os endpoints de Viewing.
/// Estes testes focam na validação de campos, lógica de negócio e CRUD,
/// sem se preocupar com autenticação (que é bypassada).
/// Cada teste é completamente isolado e limpa o banco antes da execução.
/// </summary>
public class ViewingTests : BaseIntegrationTest
{
    public ViewingTests(RealtyHubApiTests factory) : base(factory)
    {
    }

    #region GET /v1/viewings - GetAllViewingsEndpoint Tests

    [Fact]
    public async Task GetAllViewings_ShouldReturnPagedResponse()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Viewing>();
        var (_, dbContext) = CreateDbContext();
        
        var (_, _, buyer, property, _) = await CreateBasicScenarioAsync(dbContext);
        
        for (var i = 1; i <= 5; i++)
        {
            var viewing = MockData.GetValidViewing(buyer, property);
            viewing.ViewingDate = DateTime.Now.AddDays(i);
            await dbContext.Viewing.AddAsync(viewing);
        }
        
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount<Viewing>();
        var (_, dbContext) = CreateDbContext();
        
        var (_, _, buyer, property, _) = await CreateBasicScenarioAsync(dbContext);
        
        for (var i = 1; i <= 15; i++)
        {
            var viewing = MockData.GetValidViewing(buyer, property);
            viewing.ViewingDate = DateTime.Now.AddDays(i);
            await dbContext.Viewing.AddAsync(viewing);
        }
        
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount<Viewing>();
        var (_, dbContext) = CreateDbContext();
        
        var (_, _, buyer, property, _) = await CreateBasicScenarioAsync(dbContext);
        
        var viewing = MockData.GetValidViewing(buyer, property);
        viewing.ViewingDate = DateTime.Now.AddDays(1);
        await dbContext.Viewing.AddAsync(viewing);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount<Viewing>();
        var client = Factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount<Viewing>();
        var (_, dbContext) = CreateDbContext();
        
        var (condominium, seller, buyer) = await CreateMinimalScenarioAsync(dbContext);
        
        var property = MockData.GetValidProperty(seller, condominium);
        await dbContext.Properties.AddAsync(property);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();
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
        await CleanupDatabaseAndGetPreviousCount<Viewing>();
        var client = Factory.CreateClient();
        var viewing = new Viewing
        {
            // ViewingDate ausente
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
        await CleanupDatabaseAndGetPreviousCount<Viewing>();
        var (_, dbContext) = CreateDbContext();
        
        var (condominium, seller, buyer) = await CreateMinimalScenarioAsync(dbContext);
        
        var property = MockData.GetValidProperty(seller, condominium);
        await dbContext.Properties.AddAsync(property);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();
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
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    #endregion

    #region PUT /v1/viewings/{id}/reschedule - RescheduleViewingEndpoint Tests

    [Fact]
    public async Task RescheduleViewing_WithValidData_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Viewing>();
        var (_, dbContext) = CreateDbContext();
        
        var (_, _, buyer, property, _) = await CreateBasicScenarioAsync(dbContext);
        
        var viewing = MockData.GetValidViewing(buyer, property);
        viewing.ViewingDate = DateTime.Now.AddDays(1);
        await dbContext.Viewing.AddAsync(viewing);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount<Viewing>();
        var (_, dbContext) = CreateDbContext();
        
        var (condominium, seller, buyer) = await CreateMinimalScenarioAsync(dbContext);
        
        var property = MockData.GetValidProperty(seller, condominium);
        await dbContext.Properties.AddAsync(property);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();
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
        await CleanupDatabaseAndGetPreviousCount<Viewing>();
        var (_, dbContext) = CreateDbContext();
        
        var (_, _, buyer, property, _) = await CreateBasicScenarioAsync(dbContext);
        
        var viewing = MockData.GetValidViewing(buyer, property);
        viewing.ViewingDate = DateTime.Now.AddDays(1);
        await dbContext.Viewing.AddAsync(viewing);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount<Viewing>();
        var client = Factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount<Viewing>();
        var (_, dbContext) = CreateDbContext();
        
        var (_, _, buyer, property, _) = await CreateBasicScenarioAsync(dbContext);
        
        var viewing = MockData.GetValidViewing(buyer, property);
        viewing.ViewingDate = DateTime.Now.AddDays(1);
        await dbContext.Viewing.AddAsync(viewing);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount<Viewing>();
        var client = Factory.CreateClient();

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
        await CleanupDatabaseAndGetPreviousCount<Viewing>();
        var (_, dbContext) = CreateDbContext();
        
        var (condominium, seller, _) = await CreateMinimalScenarioAsync(dbContext);
        
        var property = MockData.GetValidProperty(seller, condominium);
        await dbContext.Properties.AddAsync(property);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();
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
        await CleanupDatabaseAndGetPreviousCount<Viewing>();
        var (_, dbContext) = CreateDbContext();
        
        var buyer = MockData.GetValidCustomer(ECustomerType.Buyer);
        await dbContext.Customers.AddAsync(buyer);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();
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
}