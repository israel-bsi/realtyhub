using FluentAssertions;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using System.Net.Http.Json;

namespace RealtyHub.Tests.Identity;

/// <summary>
/// Classe de testes específica para autenticação e autorização.
/// Esta classe foca exclusivamente nos fluxos de autenticação, registro, login, etc.
/// </summary>
public class AuthenticationTests : IClassFixture<RealtyHubApiTestsWithAuth>
{
    private readonly RealtyHubApiTestsWithAuth _factory;

    public AuthenticationTests(RealtyHubApiTestsWithAuth factory)
    {
        _factory = factory;
    }

    #region Authentication Flow Tests

    [Fact]
    public async Task ProtectedEndpoint_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient(); // Client without authentication

        // Act
        var response = await client.GetAsync("/v1/customers");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task AuthenticatedClient_ShouldAccessProtectedEndpoints()
    {
        // Arrange & Act
        var client = await MockData.CreateAuthenticatedClient(_factory);
        var response = await client.GetAsync("/v1/customers");

        // Assert
        client.Should().NotBeNull();
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnSuccess()
    {
        // Este teste será expandido quando implementar confirmação de email
        // Por enquanto, apenas verifica se o cliente autenticado funciona
        var client = await MockData.CreateAuthenticatedClient(_factory);
        client.Should().NotBeNull();
    }

    [Fact] 
    public async Task HealthCheck_ShouldAlwaysWork()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Contain("Ok");
    }

    #endregion

    #region Future Authentication Tests

    // TODO: Implementar quando resolver confirmação de email
    // [Fact]
    // public async Task Register_WithValidData_ShouldRequireEmailConfirmation()
    
    // [Fact] 
    // public async Task Login_WithUnconfirmedEmail_ShouldReturnError()
    
    // [Fact]
    // public async Task ConfirmEmail_WithValidToken_ShouldAllowLogin()

    #endregion
}