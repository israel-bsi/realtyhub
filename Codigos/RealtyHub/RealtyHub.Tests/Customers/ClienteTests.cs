using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using System.Net.Http.Json;

namespace RealtyHub.Tests.Customers;

public class ClienteTests
{
    [Fact]
    public async Task GetCustomersReturnsOkStatusCode()
    {
        // Arrange
        await using var application = new RealtyHubApiTests();

        // Act
        await MockData.CreateCustomers(application, true, 5, 5);
        var client = await MockData.CreateClient(application);
        var result = await client.GetAsync("/v1/customers");
        var customers = await result.Content.ReadFromJsonAsync<Response<List<Cliente>>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(customers?.Data);
        Assert.Equal(10, customers.Data.Count);
    }

    [Fact]
    public async Task GetCustomerByIdReturnsOkStatusCode()
    {
        // Arrange
        await using var application = new RealtyHubApiTests();
        // Act
        await MockData.CreateCustomers(application, true, 1, 0);
        var client = await MockData.CreateClient(application);
        var result = await client.GetAsync("/v1/customers/1");
        var customer = await result.Content.ReadFromJsonAsync<Response<Cliente>>();
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(customer?.Data);
        Assert.Equal(1, customer.Data.Id);
    }
}