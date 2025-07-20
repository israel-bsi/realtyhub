using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using RealtyHub.Core.Models.Account;
using RealtyHub.Core.Requests.Account;
using RealtyHub.Core.Responses;
using User = RealtyHub.ApiService.Models.User;

namespace RealtyHub.Tests.Identity;

/// <summary>
/// Classe de testes de integração para os endpoints de Identity/Autenticação.
/// Estes testes focam na validação de campos, lógica de negócio e fluxos de autenticação,
/// sem se preocupar com autenticação (que é bypassada).
/// Cada teste é completamente isolado e limpa o banco antes da execução.
/// </summary>
public class AuthenticationTests : IClassFixture<RealtyHubApiTests>
{
    private readonly RealtyHubApiTests _factory;

    public AuthenticationTests(RealtyHubApiTests factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Limpa o banco de dados
    /// </summary>
    private async Task CleanupDatabaseAndGetPreviousCount()
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        
        // Remove todos os usuários existentes
        var existingUsers = await userManager.Users.ToListAsync();
        foreach (var user in existingUsers)
        {
            await userManager.DeleteAsync(user);
        }
    }

    /// <summary>
    /// Cria um usuário válido para uso nos testes.
    /// </summary>
    private static RegisterRequest CreateValidRegisterRequest(string email = "teste@exemplo.com")
    {
        return new RegisterRequest
        {
            Email = email,
            Password = "Senha123!",
            GivenName = "Usuário Teste",
            Creci = "123456F"
        };
    }

    /// <summary>
    /// Cria um request de login válido.
    /// </summary>
    private static LoginRequest CreateValidLoginRequest(string email = "teste@exemplo.com")
    {
        return new LoginRequest
        {
            Email = email,
            Password = "Senha123!"
        };
    }

    #region POST /v1/identity/register-user - RegisterUserEndpoint Tests

    [Fact]
    public async Task RegisterUser_WithValidData_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var request = CreateValidRegisterRequest();

        // Act
        var response = await client.PostAsJsonAsync("/v1/identity/register-user", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegisterUser_WithMissingRequiredFields_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var request = new RegisterRequest
        {
            // Email is missing - required field
            Password = "Senha123!",
            GivenName = "Usuário Teste",
            Creci = "123456F"
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/identity/register-user", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var request = CreateValidRegisterRequest();
        request.Email = "email-invalido";

        // Act
        var response = await client.PostAsJsonAsync("/v1/identity/register-user", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_WithShortPassword_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var request = CreateValidRegisterRequest();
        request.Password = "123"; // Senha muito curta

        // Act
        var response = await client.PostAsJsonAsync("/v1/identity/register-user", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_WithDuplicateEmail_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var request = CreateValidRegisterRequest("duplicado@exemplo.com");

        // Registra o primeiro usuário
        await client.PostAsJsonAsync("/v1/identity/register-user", request);

        // Act - Tenta registrar o mesmo email novamente
        var response = await client.PostAsJsonAsync("/v1/identity/register-user", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_WithMissingCreci_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var request = CreateValidRegisterRequest();
        request.Creci = string.Empty;

        // Act
        var response = await client.PostAsJsonAsync("/v1/identity/register-user", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_WithMissingGivenName_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var request = CreateValidRegisterRequest();
        request.GivenName = string.Empty;

        // Act
        var response = await client.PostAsJsonAsync("/v1/identity/register-user", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region POST /v1/identity/login - LoginEndpoint Tests

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        
        // Registra um usuário primeiro
        var registerRequest = CreateValidRegisterRequest();
        await client.PostAsJsonAsync("/v1/identity/register-user", registerRequest);

        await SetUserEmailAsConfirmed(registerRequest.Email);
        
        var loginRequest = CreateValidLoginRequest();

        // Act
        var response = await client.PostAsJsonAsync("/v1/identity/login?useCookies=true", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_WithInvalidEmail_ShouldReturnUnauthorized()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var request = CreateValidLoginRequest();
        request.Email = "inexistente@exemplo.com";

        // Act
        var response = await client.PostAsJsonAsync("/v1/identity/login?useCookies=true", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturnUnauthorized()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        
        // Registra um usuário primeiro
        var registerRequest = CreateValidRegisterRequest();
        await client.PostAsJsonAsync("/v1/identity/register-user", registerRequest);

        await SetUserEmailAsConfirmed(registerRequest.Email);

        var loginRequest = CreateValidLoginRequest();
        loginRequest.Password = "SenhaErrada123!";

        // Act
        var response = await client.PostAsJsonAsync("/v1/identity/login?useCookies=true", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithMissingEmail_ShouldReturnUnauthorized()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var request = CreateValidLoginRequest();
        request.Email = string.Empty;

        // Act
        var response = await client.PostAsJsonAsync("/v1/identity/login?useCookies=true", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Login_WithMissingPassword_ShouldReturnUnauthorized()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var request = CreateValidLoginRequest();
        request.Password = string.Empty;

        // Act
        var response = await client.PostAsJsonAsync("/v1/identity/login?useCookies=true", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region GET /v1/identity/confirm-email - ConfirmEmailEndpoint Tests

    [Fact]
    public async Task ConfirmEmail_WithValidToken_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        
        // Registra um usuário
        var registerRequest = CreateValidRegisterRequest();
        await client.PostAsJsonAsync("/v1/identity/register-user", registerRequest);
        
        // Obtém o usuário criado
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = await userManager.FindByEmailAsync(registerRequest.Email);
        user.Should().NotBeNull();
        
        // Gera um token de confirmação
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user!);
        var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token))
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

        // Act
        var response = await client.GetAsync($"/v1/identity/confirm-email?userId={user!.Id}&token={encodedToken}");
        var result = await response.Content.ReadFromJsonAsync<Response<string>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Message.Should().Contain("confirmado com sucesso");
    }

    [Fact]
    public async Task ConfirmEmail_WithInvalidUserId_ShouldReturnNotFound()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/identity/confirm-email?userId=999&token=invalid-token");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ConfirmEmail_WithInvalidToken_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        
        // Registra um usuário
        var registerRequest = CreateValidRegisterRequest();
        await client.PostAsJsonAsync("/v1/identity/register-user", registerRequest);
        
        // Obtém o usuário criado
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = await userManager.FindByEmailAsync(registerRequest.Email);
        user.Should().NotBeNull();

        // Act
        var response = await client.GetAsync($"/v1/identity/confirm-email?userId={user!.Id}&token=token-invalido");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ConfirmEmail_WithAlreadyConfirmedEmail_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        
        // Registra um usuário
        var registerRequest = CreateValidRegisterRequest();
        await client.PostAsJsonAsync("/v1/identity/register-user", registerRequest);
        
        // Obtém o usuário criado
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = await userManager.FindByEmailAsync(registerRequest.Email);
        user.Should().NotBeNull();
        
        // Confirma o email manualmente
        await userManager.ConfirmEmailAsync(user!, await userManager.GenerateEmailConfirmationTokenAsync(user!));
        
        // Gera um novo token
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user!);
        var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token))
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

        // Act
        var response = await client.GetAsync($"/v1/identity/confirm-email?userId={user!.Id}&token={encodedToken}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region POST /v1/identity/logout - LogoutEndpoint Tests

    [Fact]
    public async Task Logout_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsync("/v1/identity/logout", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region POST /v1/identity/forgot-password - ForgotPasswordEndpoint Tests

    [Fact]
    public async Task ForgotPassword_WithValidEmail_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        
        // Registra um usuário primeiro
        var registerRequest = CreateValidRegisterRequest();
        await client.PostAsJsonAsync("/v1/identity/register-user", registerRequest);
        
        var request = new { registerRequest.Email };

        // Act
        var response = await client.PostAsJsonAsync("/v1/identity/forgot-password", request);
        var result = await response.Content.ReadFromJsonAsync<Response<string>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        if (result != null)
        {
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Contain("email");
        }
    }

    [Fact]
    public async Task ForgotPassword_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var request = new { Email = "inexistente@exemplo.com" };

        // Act
        var response = await client.PostAsJsonAsync("/v1/identity/forgot-password", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ForgotPassword_WithEmptyEmail_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var request = new { Email = string.Empty };

        // Act
        var response = await client.PostAsJsonAsync("/v1/identity/forgot-password", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region POST /v1/identity/reset-password - ResetPasswordEndpoint Tests

    [Fact]
    public async Task ResetPassword_WithValidData_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        
        // Registra um usuário
        var registerRequest = CreateValidRegisterRequest();
        await client.PostAsJsonAsync("/v1/identity/register-user", registerRequest);
        
        // Obtém o usuário criado
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = await userManager.FindByEmailAsync(registerRequest.Email);
        user.Should().NotBeNull();

        await SetUserEmailAsConfirmed(registerRequest.Email);

        // Gera um token de reset
        var token = await userManager.GeneratePasswordResetTokenAsync(user!);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var request = new ResetPasswordRequest
        {
            UserId = user!.Id.ToString(),
            Email = registerRequest.Email,
            Token = encodedToken,
            PasswordResetModel = new PasswordResetModel
            {
                Password = "NovaSenha123!",
                ConfirmPassword = "NovaSenha123!"
            }
        };

        // Act
        var url = $"v1/identity/reset-password?userId={request.UserId}&token={request.Token}";
        var response = await client.PostAsJsonAsync(url, request);
        
        // Verifica se a resposta tem conteúdo antes de tentar deserializar
        var content = await response.Content.ReadAsStringAsync();
        Response<string>? result = null;
        
        if (!string.IsNullOrEmpty(content))
        {
            result = await response.Content.ReadFromJsonAsync<Response<string>>();
        }

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        if (response.StatusCode == HttpStatusCode.OK && result != null)
        {
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Contain("redefinida");
        }
    }

    [Fact]
    public async Task ResetPassword_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        var request = new ResetPasswordRequest
        {
            UserId = "0",
            Email = "inexistente@exemplo.com",
            Token = "token-invalido",
            PasswordResetModel = new PasswordResetModel
            {
                Password = "NovaSenha123!",
                ConfirmPassword = "NovaSenha123!"
            }
        };

        // Act
        var url = $"v1/identity/reset-password?userId={request.UserId}&token={request.Token}";
        var response = await client.PostAsJsonAsync(url, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ResetPassword_WithInvalidToken_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        
        // Registra um usuário
        var registerRequest = CreateValidRegisterRequest();
        await client.PostAsJsonAsync("/v1/identity/register-user", registerRequest);

        // Obtém o usuário criado
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = await userManager.FindByEmailAsync(registerRequest.Email);
        user.Should().NotBeNull();

        var request = new ResetPasswordRequest
        {
            UserId = user!.Id.ToString(),
            Email = registerRequest.Email,
            Token = "token-invalido",
            PasswordResetModel = new PasswordResetModel
            {
                Password = "NovaSenha123!",
                ConfirmPassword = "NovaSenha123!"
            }
        };

        // Act
        var url = $"v1/identity/reset-password?userId={request.UserId}&token={request.Token}";
        var response = await client.PostAsJsonAsync(url, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ResetPassword_WithShortPassword_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        
        // Registra um usuário
        var registerRequest = CreateValidRegisterRequest();
        await client.PostAsJsonAsync("/v1/identity/register-user", registerRequest);
        
        // Obtém o usuário criado
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = await userManager.FindByEmailAsync(registerRequest.Email);
        user.Should().NotBeNull();
        
        // Gera um token de reset
        var token = await userManager.GeneratePasswordResetTokenAsync(user!);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        
        var request = new ResetPasswordRequest
        {
            Email = registerRequest.Email,
            Token = encodedToken,
            UserId = user!.Id.ToString(),
            PasswordResetModel = new PasswordResetModel
            {
                Password = "123", // Senha muito curta
                ConfirmPassword = "123"
            }
        };

        // Act
        var url = $"v1/identity/reset-password?userId={request.UserId}&token={request.Token}";
        var response = await client.PostAsJsonAsync(url, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region GET /v1/identity/manage/info - ManageInfoEndpoint Tests

    [Fact]
    public async Task ManageInfo_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/v1/identity/manage-info");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Business Logic Tests

    [Fact]
    public async Task Login_WithUnconfirmedEmail_ShouldReturnUnauthorized()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        
        // Registra um usuário (email não confirmado por padrão)
        var registerRequest = CreateValidRegisterRequest();
        await client.PostAsJsonAsync("/v1/identity/register-user", registerRequest);
        
        var loginRequest = CreateValidLoginRequest();

        // Act
        var response = await client.PostAsJsonAsync("/v1/identity/login?useCookies=true", loginRequest);

        // Assert
        // O login deve falhar porque o email não está confirmado
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithConfirmedEmail_ShouldReturnOk()
    {
        // Arrange
        await CleanupDatabaseAndGetPreviousCount();
        var client = _factory.CreateClient();
        
        // Registra um usuário
        var registerRequest = CreateValidRegisterRequest();
        await client.PostAsJsonAsync("/v1/identity/register-user", registerRequest);
        
        // Confirma o email manualmente
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = await userManager.FindByEmailAsync(registerRequest.Email);
        user.Should().NotBeNull();
        
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user!);
        await userManager.ConfirmEmailAsync(user!, token);
        
        var loginRequest = CreateValidLoginRequest();

        // Act
        var response = await client.PostAsJsonAsync("/v1/identity/login?useCookies=true", loginRequest);

        // Assert
        // Agora o login deve funcionar porque o email está confirmado
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Infrastructure Tests

    [Fact]
    public void MockData_CreateSimpleClient_ShouldReturnClient()
    {
        // Arrange & Act
        var client = _factory.CreateClient();

        // Assert
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

    /// <summary>
    /// Confirma o e-mail do usuário utilizando o token gerado pelo UserManager.
    /// </summary>
    private async Task SetUserEmailAsConfirmed(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = await userManager.FindByEmailAsync(email);
        user.Should().NotBeNull();

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user!);
        await userManager.ConfirmEmailAsync(user!, token);
    }
}
