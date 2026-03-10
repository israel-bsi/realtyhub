using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealtyHub.Infrastructure.Data;
using RealtyHub.Core.Requests.Emails;
using RealtyHub.Core.Responses;
using RealtyHub.Core.Services;

namespace RealtyHub.Tests;

/// <summary>
/// Factory para testes de integração com autenticação desabilitada.
/// Use esta classe quando quiser focar apenas na validação de campos e lógica de negócio.
/// Cada instância usa um banco de dados isolado para evitar interferência entre testes.
/// </summary>
public class RealtyHubApiTests : WebApplicationFactory<ApiService.Program>
{
    private readonly string _databaseName;
    public const string TestUserId = "1";
    public const string TestUserEmail = "test@test.com";

    public RealtyHubApiTests()
    {
        // Cria um nome único de banco para cada instância de teste
        _databaseName = $"InMemoryDbForTesting_{Guid.NewGuid()}";
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var root = new InMemoryDatabaseRoot();

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName, root);
            });
        });

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the registration for AppExtension's ApplyMigrations call
            // by ensuring the context uses in-memory database
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            // MOCK DO SERVIÇO DE EMAIL
            // Remove o serviço de email real
            services.RemoveAll<IEmailService>();
            
            // Adiciona o mock do serviço de email
            services.AddSingleton<IEmailService, MockEmailService>();

            // CONFIGURAR AUTENTICAÇÃO FAKE ANTES DA AUTORIZAÇÃO
            // Remove existing authentication services
            services.RemoveAll<IAuthenticationSchemeProvider>();

            // Add fake authentication for testing
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });

            // BYPASS AUTHORIZATION FOR TESTING
            // Remove existing authorization services
            services.RemoveAll<IAuthorizationService>();
            services.RemoveAll<IAuthorizationPolicyProvider>();
            services.RemoveAll<IAuthorizationHandlerProvider>();

            // Add a policy that allows everything
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes("Test")
                    .RequireAuthenticatedUser()
                    .Build();
            });

            // Register a custom authorization service that always allows
            services.AddSingleton<IAuthorizationService, AllowAllAuthorizationService>();
        });

        // Override environment to avoid calling ApplyMigrations
        builder.UseEnvironment("Testing");
    }

    /// <summary>
    /// Limpa o banco de dados, removendo todos os dados.
    /// Útil para garantir isolamento entre testes.
    /// </summary>
    public async Task CleanupDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Remove todos os dados das tabelas principais
        var customers = await dbContext.Customers.ToListAsync();
        dbContext.Customers.RemoveRange(customers);
        
        await dbContext.SaveChangesAsync();
    }
}

/// <summary>
/// Mock do serviço de email para testes.
/// Simula o envio de emails sem realmente enviá-los.
/// </summary>
public class MockEmailService : IEmailService
{
    public Task<Response<bool>> SendConfirmationLinkAsync(ConfirmEmailMessage message)
    {
        // Simula sucesso no envio do email de confirmação
        return Task.FromResult(new Response<bool>(true, 200, "Email de confirmação enviado com sucesso!"));
    }

    public Task<Response<bool>> SendResetPasswordLinkAsync(ResetPasswordMessage message)
    {
        // Simula sucesso no envio do email de reset de senha
        return Task.FromResult(new Response<bool>(true, 200, "Email de redefinição de senha enviado com sucesso!"));
    }

    public Task<Response<bool>> SendContractAsync(AttachmentMessage message)
    {
        // Simula sucesso no envio do contrato
        return Task.FromResult(new Response<bool>(true, 200, "Contrato enviado com sucesso!"));
    }
}

/// <summary>
/// Serviço de autorização personalizado que sempre permite acesso.
/// Usado apenas em testes para bypassar autenticação.
/// </summary>
public class AllowAllAuthorizationService : IAuthorizationService
{
    public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, IEnumerable<IAuthorizationRequirement> requirements)
    {
        return Task.FromResult(AuthorizationResult.Success());
    }

    public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName)
    {
        return Task.FromResult(AuthorizationResult.Success());
    }
}

/// <summary>
/// Handler de autenticação personalizado para testes que simula um usuário autenticado.
/// </summary>
public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, RealtyHubApiTests.TestUserId),
            new Claim(ClaimTypes.Email, RealtyHubApiTests.TestUserEmail),
            new Claim(ClaimTypes.NameIdentifier, RealtyHubApiTests.TestUserId)
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}