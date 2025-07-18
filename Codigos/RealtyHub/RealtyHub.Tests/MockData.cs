using RealtyHub.ApiService.Data;
using RealtyHub.Core.Utilities.FakeEntities;
using RealtyHub.Core.Requests.Account;
using System.Net.Http.Json;

namespace RealtyHub.Tests;

public class MockData
{
    public static async Task CreateCustomers(RealtyHubApiTests application,
        bool create, int quantityBusiness, int quantityIndividual)
    {
        using var scope = application.Services.CreateScope();
        var provider = scope.ServiceProvider;
        await using var dbContext = provider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        if (create)
        {
            var customersBusinessToCreate = CustomerFake.GetFakeBusinessCustomers(quantityBusiness);
            var customersIndividualToCreate = CustomerFake.GetFakeIndividualCustomers(quantityIndividual);
            
            // CORREÇÃO: Atribui o UserId correto para os customers fake
            foreach (var customer in customersBusinessToCreate)
            {
                customer.UserId = RealtyHubApiTests.TestUserId;
            }
            
            foreach (var customer in customersIndividualToCreate)
            {
                customer.UserId = RealtyHubApiTests.TestUserId;
            }
            
            await dbContext.Customers.AddRangeAsync(customersBusinessToCreate);
            await dbContext.Customers.AddRangeAsync(customersIndividualToCreate);
            await dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Cria um cliente HTTP simples sem autenticação.
    /// Use quando a autorização estiver desabilitada nos testes.
    /// </summary>
    public static HttpClient CreateSimpleClient(RealtyHubApiTests application)
    {
        return application.CreateClient();
    }

    /// <summary>
    /// Cria um cliente HTTP autenticado para testes que requerem autenticação real.
    /// Use apenas com RealtyHubApiTestsWithAuth.
    /// </summary>
    public static async Task<HttpClient> CreateAuthenticatedClient(RealtyHubApiTestsWithAuth application)
    {
        var client = application.CreateClient();

        // URLs corretas baseadas no mapeamento de endpoints
        const string registerUrl = "/v1/identity/register-user";
        const string loginUrl = "/v1/identity/login?useCookies=true";

        // Dados de registro completos conforme RegisterRequest
        var registerRequest = new RegisterRequest
        {
            Creci = "123456",
            GivenName = "Israel Test User",
            Email = "israel@gmail.com",
            Password = "!W92X+!Q@rOwC48+v.V3",
            ConfirmPassword = "!W92X+!Q@rOwC48+v.V3"
        };

        // Dados de login conforme LoginRequest
        var loginRequest = new LoginRequest
        {
            Email = "israel@gmail.com",
            Password = "!W92X+!Q@rOwC48+v.V3"
        };

        try
        {
            // Registrar o usuário primeiro
            var registerResponse = await client.PostAsJsonAsync(registerUrl, registerRequest);
            
            // Se o registro falhou com "usuário já existe", continua com o login
            if (!registerResponse.IsSuccessStatusCode)
            {
                var registerContent = await registerResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Registro falhou: {registerContent}");
            }

            // Fazer login
            var loginResponse = await client.PostAsJsonAsync(loginUrl, loginRequest);
            if (!loginResponse.IsSuccessStatusCode)
            {
                var loginContent = await loginResponse.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Login falhou: {loginContent}");
            }

            return client;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao criar cliente autenticado: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Método obsoleto - use CreateSimpleClient para testes sem auth ou CreateAuthenticatedClient para testes com auth
    /// </summary>
    [Obsolete("Use CreateSimpleClient para testes sem auth ou CreateAuthenticatedClient para testes com auth")]
    public static HttpClient CreateClient(RealtyHubApiTests application)
    {
        return CreateSimpleClient(application);
    }

    /// <summary>
    /// Método obsoleto - use CreateSimpleClient para testes sem auth ou CreateAuthenticatedClient para testes com auth
    /// </summary>
    [Obsolete("Use CreateSimpleClient para testes sem auth ou CreateAuthenticatedClient para testes com auth")]
    public static async Task<HttpClient> CreateClient(RealtyHubApiTestsWithAuth application)
    {
        return await CreateAuthenticatedClient(application);
    }
}