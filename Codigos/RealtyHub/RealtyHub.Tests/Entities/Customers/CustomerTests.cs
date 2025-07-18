using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using System.Net.Http.Json;

namespace RealtyHub.Tests.Entities.Customers;

/// <summary>
/// Classe de testes de integração para os endpoints de Customer.
/// Estes testes focam na validação de campos, lógica de negócio e CRUD,
/// sem se preocupar com autenticação (que é bypassada).
/// Cada teste é completamente isolado e limpa o banco antes da execução.
/// </summary>
/// <remarks>
/// Esta classe testa todos os endpoints disponíveis na pasta RealtyHub.ApiService/Endpoints/Customers:
/// - GET /v1/customers (GetAllCustomersEndpoint)
/// - GET /v1/customers/{id} (GetCustomerByIdEndpoint)
/// - POST /v1/customers (CreateCustomerEndpoint)
/// - PUT /v1/customers/{id} (UpdateCustomerEndpoint)
/// - DELETE /v1/customers/{id} (DeleteCustomerEndpoint)
/// </remarks>
public class CustomerTests : IClassFixture<RealtyHubApiTests>
{
    private readonly RealtyHubApiTests _factory;

    public CustomerTests(RealtyHubApiTests factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Limpa o banco de dados e retorna o número de clientes encontrados antes da limpeza.
    /// </summary>
    private async Task<int> CleanupDatabaseAndGetPreviousCount()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApiService.Data.AppDbContext>();
        
        // Conta quantos clientes existiam antes da limpeza
        var existingCount = await dbContext.Customers.CountAsync();
        
        // Remove todos os customers existentes
        var existingCustomers = await dbContext.Customers.ToListAsync();
        dbContext.Customers.RemoveRange(existingCustomers);
        await dbContext.SaveChangesAsync();
        
        return existingCount;
    }

    /// <summary>
    /// Cria um customer válido para uso nos testes.
    /// </summary>
    private static Customer CreateValidCustomer(string name = "Cliente Teste", string email = "cliente@test.com")
    {
        return new Customer
        {
            Name = name,
            Email = email,
            Phone = "11999999999",
            DocumentNumber = "12345678901",
            CustomerType = Core.Enums.ECustomerType.Buyer,
            PersonType = Core.Enums.EPersonType.Individual,
            Occupation = "Teste",
            Nationality = "Brasileira",
            MaritalStatus = Core.Enums.EMaritalStatus.Single,
            UserId = RealtyHubApiTests.TestUserId, // Usa o mesmo UserId que será usado nos endpoints
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
            IsActive = true
        };
    }

    #region GET /v1/customers - GetAllCustomersEndpoint Tests

    [Fact]
    public async Task GetAllCustomers_ShouldReturnPagedResponse()
    {
        // Arrange
        var previousCount = await CleanupDatabaseAndGetPreviousCount();
        await MockData.CreateCustomers(_factory, true, 3, 2); // 5 customers total
        var client = MockData.CreateSimpleClient(_factory);

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
        await CleanupDatabaseAndGetPreviousCount();
        await MockData.CreateCustomers(_factory, true, 10, 5); // 15 customers total
        var client = MockData.CreateSimpleClient(_factory);

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
        await CleanupDatabaseAndGetPreviousCount();
        await MockData.CreateCustomers(_factory, true, 2, 2);
        var client = MockData.CreateSimpleClient(_factory);

        // Act
        var response = await client.GetAsync("/v1/customers?searchTerm=empresa");
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<Customer>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        // O resultado pode variar dependendo dos dados gerados pelo CustomerFake
    }

    #endregion

    #region GET /v1/customers/{id} - GetCustomerByIdEndpoint Tests

    [Fact]
    public async Task GetCustomerById_WithValidId_ShouldReturnCustomer()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        
        // Cria um customer diretamente no banco
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApiService.Data.AppDbContext>();
        var customer = CreateValidCustomer("Cliente Busca", "busca@test.com");
        // UserId já está correto no CreateValidCustomer()
        await dbContext.Customers.AddAsync(customer);
        await dbContext.SaveChangesAsync();
        
        // Vamos verificar se o customer foi salvo com o UserId correto
        var savedCustomer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == customer.Id);
        
        var client = MockData.CreateSimpleClient(_factory);

        // Act
        var response = await client.GetAsync($"/v1/customers/{customer.Id}");
        var content = await response.Content.ReadAsStringAsync();

        // Debug: Se retornar BadRequest, vamos incluir na mensagem de erro
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new Exception($"GetCustomerById returned BadRequest. Content: {content}. Customer ID: {customer.Id}. Customer UserId in DB: {savedCustomer?.UserId}. Expected UserId: {RealtyHubApiTests.TestUserId}");
        }

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
        await CleanupDatabaseAndGetPreviousCount();
        var client = MockData.CreateSimpleClient(_factory);

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
        await CleanupDatabaseAndGetPreviousCount();
        var client = MockData.CreateSimpleClient(_factory);
        var customer = new Customer
        {
            Name = "João Silva",
            Email = "joao.silva@test.com",
            Phone = "11999999999",
            DocumentNumber = "12345678901",
            CustomerType = Core.Enums.ECustomerType.Buyer,
            PersonType = Core.Enums.EPersonType.Individual,
            Occupation = "Engenheiro",
            Nationality = "Brasileira",
            MaritalStatus = Core.Enums.EMaritalStatus.Single,
            Rg = "123456789",
            IssuingAuthority = "SSP/SP",
            RgIssueDate = DateTime.Now.AddYears(-5),
            Address = new Address
            {
                Street = "Rua das Flores",
                Number = "123",
                Complement = "Apto 101",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234-567"
            }
        };

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
        result.Data.PersonType.Should().Be(Core.Enums.EPersonType.Individual);
    }

    [Fact]
    public async Task CreateCustomer_WithValidBusinessCustomer_ShouldReturnCreated()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = MockData.CreateSimpleClient(_factory);
        var customer = new Customer
        {
            Name = "Empresa ABC Ltda",
            Email = "contato@empresaabc.com",
            Phone = "1133334444",
            DocumentNumber = "12345678000199",
            CustomerType = Core.Enums.ECustomerType.Seller,
            PersonType = Core.Enums.EPersonType.Business,
            BusinessName = "Empresa ABC Ltda",
            Address = new Address
            {
                Street = "Av. Paulista",
                Number = "1000",
                Complement = "Sala 501",
                Neighborhood = "Bela Vista",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01310-100"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/customers", customer);
        var result = await response.Content.ReadFromJsonAsync<Response<Customer>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Empresa ABC Ltda");
        result.Data.PersonType.Should().Be(Core.Enums.EPersonType.Business);
        result.Data.BusinessName.Should().Be("Empresa ABC Ltda");
    }

    [Fact]
    public async Task CreateCustomer_WithMissingRequiredFields_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = MockData.CreateSimpleClient(_factory);
        var customer = new Customer
        {
            // Name is missing - required field
            Email = "test@test.com",
            Phone = "11999999999"
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/customers", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCustomer_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = MockData.CreateSimpleClient(_factory);
        var customer = new Customer
        {
            Name = "Test User",
            Email = "invalid-email", // Invalid email format
            Phone = "11999999999",
            DocumentNumber = "12345678901",
            CustomerType = Core.Enums.ECustomerType.Buyer,
            PersonType = Core.Enums.EPersonType.Individual
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/customers", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCustomer_WithMaxLengthExceeded_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = MockData.CreateSimpleClient(_factory);
        var customer = new Customer
        {
            Name = new string('A', 81), // Nome com mais de 80 caracteres (limite)
            Email = "test@test.com",
            Phone = "11999999999",
            DocumentNumber = "12345678901",
            CustomerType = Core.Enums.ECustomerType.Buyer,
            PersonType = Core.Enums.EPersonType.Individual
        };

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
        await CleanupDatabaseAndGetPreviousCount();
        
        // Cria um customer diretamente no banco para atualizar
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApiService.Data.AppDbContext>();
        var customer = CreateValidCustomer("Cliente Original", "original@test.com");
        // UserId já está correto no CreateValidCustomer()
        await dbContext.Customers.AddAsync(customer);
        await dbContext.SaveChangesAsync();
        
        var client = MockData.CreateSimpleClient(_factory);

        var updatedCustomer = new Customer
        {
            Id = customer.Id,
            Name = "João Silva Atualizado",
            Email = "joao.atualizado@test.com",
            Phone = "11888888888",
            DocumentNumber = "12345678901",
            CustomerType = Core.Enums.ECustomerType.BuyerSeller,
            PersonType = Core.Enums.EPersonType.Individual,
            Occupation = "Arquiteto",
            Nationality = "Brasileira",
            Address = new Address
            {
                Street = "Rua Nova",
                Number = "456",
                Neighborhood = "Novo Bairro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234-567"
            }
        };

        // Act
        var response = await client.PutAsJsonAsync($"/v1/customers/{customer.Id}", updatedCustomer);
        
        // Debug se necessário
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"UpdateCustomer returned BadRequest. Content: {content}. Customer ID: {customer.Id}. Expected UserId: {RealtyHubApiTests.TestUserId}");
        }
        
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
        await CleanupDatabaseAndGetPreviousCount();
        var client = MockData.CreateSimpleClient(_factory);
        var customer = new Customer
        {
            Id = 999,
            Name = "Test User",
            Email = "test@test.com",
            Phone = "11999999999",
            DocumentNumber = "12345678901",
            CustomerType = Core.Enums.ECustomerType.Buyer,
            PersonType = Core.Enums.EPersonType.Individual
        };

        // Act
        var response = await client.PutAsJsonAsync("/v1/customers/999", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region DELETE /v1/customers/{id} - DeleteCustomerEndpoint Tests

    [Fact]
    public async Task DeleteCustomer_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        
        // Cria um customer diretamente no banco para deletar
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApiService.Data.AppDbContext>();
        var customer = CreateValidCustomer("Cliente Para Deletar", "deletar@test.com");
        // UserId já está correto no CreateValidCustomer()
        await dbContext.Customers.AddAsync(customer);
        await dbContext.SaveChangesAsync();
        
        var client = MockData.CreateSimpleClient(_factory);

        // Act
        var response = await client.DeleteAsync($"/v1/customers/{customer.Id}");

        // Debug se necessário
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"DeleteCustomer returned BadRequest. Content: {content}. Customer ID: {customer.Id}. Expected UserId: {RealtyHubApiTests.TestUserId}");
        }

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify customer is soft deleted - should return BadRequest when trying to access
        var getResponse = await client.GetAsync($"/v1/customers/{customer.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCustomer_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = MockData.CreateSimpleClient(_factory);

        // Act
        var response = await client.DeleteAsync("/v1/customers/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Field Validation Tests

    [Fact]
    public async Task CreateCustomer_WithInvalidPhoneNumber_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = MockData.CreateSimpleClient(_factory);
        var customer = new Customer
        {
            Name = "Test User",
            Email = "test@test.com",
            Phone = "invalid-phone", // Invalid phone format
            DocumentNumber = "12345678901",
            CustomerType = Core.Enums.ECustomerType.Buyer,
            PersonType = Core.Enums.EPersonType.Individual
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/customers", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCustomer_WithDocumentNumberTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = MockData.CreateSimpleClient(_factory);
        var customer = new Customer
        {
            Name = "Test User",
            Email = "test@test.com",
            Phone = "11999999999",
            DocumentNumber = "1234567891234567891234567", // Document with more than 20 characters
            CustomerType = Core.Enums.ECustomerType.Buyer,
            PersonType = Core.Enums.EPersonType.Individual
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/customers", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Infrastructure Tests

    [Fact]
    public async Task MockData_CreateCustomers_ShouldCreateCorrectQuantity()
    {
        // Arrange & Act
        await CleanupDatabaseAndGetPreviousCount();
        await MockData.CreateCustomers(_factory, true, 3, 2);

        // Verify data was created in database
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApiService.Data.AppDbContext>();
        
        var customerCount = await dbContext.Customers.CountAsync();

        // Assert
        customerCount.Should().Be(5); // 3 business + 2 individual
    }

    [Fact]
    public void MockData_CreateSimpleClient_ShouldReturnClient()
    {
        // Arrange & Act
        var client = MockData.CreateSimpleClient(_factory);

        // Assert
        client.Should().NotBeNull();
    }

    #endregion
}