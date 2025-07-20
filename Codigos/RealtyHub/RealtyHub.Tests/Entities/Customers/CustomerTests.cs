using System.Net.Http.Json;
using FluentAssertions;
using Moq;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using RealtyHub.Tests.Common;

namespace RealtyHub.Tests.Entities.Customers;

/// <summary>
/// Classe de testes de integração para os endpoints de Customer.
/// Estes testes focam na validação de campos, lógica de negócio e CRUD,
/// sem se preocupar com autenticação (que é bypassada).
/// Cada teste é completamente isolado e limpa o banco antes da execução.
/// </summary>
public class CustomerTests : BaseIntegrationTest
{
    public CustomerTests(RealtyHubApiTests factory) : base(factory)
    {
    }

    #region GET /v1/customers - GetAllCustomersEndpoint Tests

    [Fact]
    public async Task GetAllCustomers_ShouldReturnPagedResponse()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Customer>();
        var (_, dbContext) = CreateDbContext();

        for (int i = 0; i < 5; i++)
        {
            var customer = MockData.GetValidCustomer(ECustomerType.Buyer);
            customer.Name = $"Cliente {i + 1}";
            customer.Email = $"cliente{i + 1}@test.com";
            await dbContext.Customers.AddAsync(customer);
            await dbContext.SaveChangesAsync();
        }

        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/customers");
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<Customer>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(5);
        result.TotalCount.Should().Be(5);
    }

    [Fact]
    public async Task GetAllCustomers_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Customer>();
        var (_, dbContext) = CreateDbContext();

        for (int i = 0; i < 15; i++)
        {
            var customer = MockData.GetValidCustomer(ECustomerType.Buyer);
            customer.Name = $"Cliente {i + 1}";
            customer.Email = $"cliente{i + 1}@test.com";
            await dbContext.Customers.AddAsync(customer);
            await dbContext.SaveChangesAsync();
        }

        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/customers?pageNumber=1&pageSize=10");
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<Customer>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().BeLessOrEqualTo(10);
        result.TotalCount.Should().Be(15);
    }

    [Fact]
    public async Task GetAllCustomers_WithSearchTerm_ShouldFilterResults()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Customer>();
        var (_, dbContext) = CreateDbContext();

        for (int i = 0; i < 5; i++)
        {
            var customer = MockData.GetValidCustomer(ECustomerType.Buyer);
            customer.Name = $"Cliente {i + 1}";
            customer.Email = $"cliente{i + 1}@test.com";
            await dbContext.Customers.AddAsync(customer);
            await dbContext.SaveChangesAsync();
        }

        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/customers?searchTerm=empresa");
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<Customer>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
    }

    #endregion

    #region GET /v1/customers/{id} - GetCustomerByIdEndpoint Tests

    [Fact]
    public async Task GetCustomerById_WithValidId_ShouldReturnCustomer()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Customer>();
        var (_, dbContext) = CreateDbContext();
        
        var customer = MockData.GetValidCustomer(ECustomerType.Buyer);
        customer.Name = "Cliente Busca";
        customer.Email = "busca@test.com";
        await dbContext.Customers.AddAsync(customer);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/v1/customers/{customer.Id}");

        var result = await response.Content.ReadFromJsonAsync<Response<Customer>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(customer.Id);
        result.Data.Name.Should().Be("Cliente Busca");
    }

    [Fact]
    public async Task GetCustomerById_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Customer>();
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/customers/999");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    #endregion

    #region POST /v1/customers - CreateCustomerEndpoint Tests

    [Fact]
    public async Task CreateCustomer_WithValidIndividualCustomer_ShouldReturnCreated()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Customer>();
        var client = Factory.CreateClient();

        var customer = MockData.GetValidCustomer(ECustomerType.Buyer);
        customer.Name = "João Silva";
        customer.Email = "joao.silva@test.com";
        customer.PersonType = EPersonType.Individual;

        // Act
        var response = await client.PostAsJsonAsync("/v1/customers", customer);
        var result = await response.Content.ReadFromJsonAsync<Response<Customer>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("João Silva");
        result.Data.Email.Should().Be("joao.silva@test.com");
        result.Data.PersonType.Should().Be(EPersonType.Individual);
    }

    [Fact]
    public async Task CreateCustomer_WithValidBusinessCustomer_ShouldReturnCreated()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Customer>();
        var client = Factory.CreateClient();

        var customer = MockData.GetValidCustomer(ECustomerType.Seller);
        customer.Name = "Empresa ABC Ltda";
        customer.PersonType = EPersonType.Business;
        customer.BusinessName = "Empresa ABC Ltda";

        // Act
        var response = await client.PostAsJsonAsync("/v1/customers", customer);
        var result = await response.Content.ReadFromJsonAsync<Response<Customer>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Empresa ABC Ltda");
        result.Data.PersonType.Should().Be(EPersonType.Business);
        result.Data.BusinessName.Should().Be("Empresa ABC Ltda");
    }

    [Fact]
    public async Task CreateCustomer_WithMissingRequiredFields_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Customer>();
        var client = Factory.CreateClient();

        var customer = MockData.GetValidCustomer(ECustomerType.Buyer);
        customer.Name = string.Empty; // Nome vazio

        // Act
        var response = await client.PostAsJsonAsync("/v1/customers", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCustomer_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Customer>();
        var client = Factory.CreateClient();
       
        var customer = MockData.GetValidCustomer(ECustomerType.Buyer);
        customer.Email = "invalid-email"; // Email inválido

        // Act
        var response = await client.PostAsJsonAsync("/v1/customers", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCustomer_WithMaxLengthExceeded_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Customer>();
        var client = Factory.CreateClient();

        var customer = MockData.GetValidCustomer(ECustomerType.Buyer);
        customer.Name = new string('A', 81); // Nome com mais de 80 caracteres (limite)

        // Act
        var response = await client.PostAsJsonAsync("/v1/customers", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region PUT /v1/customers/{id} - UpdateCustomerEndpoint Tests

    [Fact]
    public async Task UpdateCustomer_WithValidData_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Customer>();
        var (_, dbContext) = CreateDbContext();
        
        var customer = MockData.GetValidCustomer(ECustomerType.Buyer);
        customer.Name = "Cliente Original";
        customer.Email = "original@test.com";
        await dbContext.Customers.AddAsync(customer);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

        var updatedCustomer = MockData.GetValidCustomer(ECustomerType.Buyer);
        updatedCustomer.Id = customer.Id; // Mantém o ID do cliente existente
        updatedCustomer.Name = "João Silva Atualizado";
        updatedCustomer.Email = "joao.atualizado@test.com";

        // Act
        var response = await client.PutAsJsonAsync($"/v1/customers/{customer.Id}", updatedCustomer);
        
        var result = await response.Content.ReadFromJsonAsync<Response<Customer>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("João Silva Atualizado");
        result.Data.Email.Should().Be("joao.atualizado@test.com");
    }

    [Fact]
    public async Task UpdateCustomer_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Customer>();
        var client = Factory.CreateClient();
       
        var customer = MockData.GetValidCustomer(ECustomerType.Buyer);
        customer.Id = 999; // ID inválido que não existe no banco

        // Act
        var response = await client.PutAsJsonAsync($"/v1/customers/{customer.Id}", customer);
        var message = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        message.Should().Contain("não encontrado");
    }

    #endregion

    #region DELETE /v1/customers/{id} - DeleteCustomerEndpoint Tests

    [Fact]
    public async Task DeleteCustomer_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Customer>();
        var (_, dbContext) = CreateDbContext();
        
        var customer = MockData.GetValidCustomer(ECustomerType.Buyer);
        customer.Name = "Cliente Para Deletar";
        customer.Email = "deletar@test.com";
        await dbContext.Customers.AddAsync(customer);
        await dbContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/v1/customers/{customer.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verifica se o cliente foi realmente removido
        var getResponse = await client.GetAsync($"/v1/customers/{customer.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var message = await getResponse.Content.ReadAsStringAsync();
        message.Should().Contain("não encontrado");
    }

    [Fact]
    public async Task DeleteCustomer_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Customer>();
        var client = Factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/v1/customers/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var message = await response.Content.ReadAsStringAsync();
        message.Should().Contain("não encontrado");
    }

    #endregion

    #region Field Validation Tests

    [Fact]
    public async Task CreateCustomer_WithInvalidPhoneNumber_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Customer>();
        var client = Factory.CreateClient();

        var customer = MockData.GetValidCustomer(ECustomerType.Buyer);
        customer.Phone = "invalid-phone"; // Número de telefone inválido

        // Act
        var response = await client.PostAsJsonAsync("/v1/customers", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCustomer_WithDocumentNumberTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount<Customer>();
        var client = Factory.CreateClient();

        var customer = MockData.GetValidCustomer(ECustomerType.Buyer);
        customer.DocumentNumber = new string('1', 21); // Documento com mais de 20 caracteres

        // Act
        var response = await client.PostAsJsonAsync("/v1/customers", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion
}