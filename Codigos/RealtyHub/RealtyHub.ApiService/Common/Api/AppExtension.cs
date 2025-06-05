using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Utilities.FakeEntities;

namespace RealtyHub.ApiService.Common.Api;

/// <summary>
/// Classe estática que agrupa métodos de extensão para configurar
/// e inicializar o ambiente da aplicação.
/// </summary>
public static class AppExtension
{
    /// <summary>
    /// Configura rotas de criação de dados de teste em ambiente
    /// de desenvolvimento (clientes e propriedades).
    /// </summary>
    /// <remarks>
    /// Este método é utilizado para popular o banco de dados
    /// com dados de teste durante o desenvolvimento.
    /// </remarks>
    /// <param name="app">Instância do aplicativo.</param>
    public static void ConfigureDevEnvironment(this WebApplication app)
    {
        app.MapPost("v1/customers/create-many", async (
                AppDbContext context,
                [FromQuery] int individualQuantity = 0,
                [FromQuery] int businessQuantity = 0) =>
        {
            if (individualQuantity > 0)
            {
                var individualCustomers = CustomerFake
                    .GetFakeIndividualCustomers(individualQuantity);
                await context.Customers.AddRangeAsync(individualCustomers);
            }

            if (businessQuantity > 0)
            {
                var businessCustomers = CustomerFake
                    .GetFakeBusinessCustomers(businessQuantity);
                await context.Customers.AddRangeAsync(businessCustomers);

            }

            await context.SaveChangesAsync();

            return Results.Created();
        });

        app.MapPost("v1/properties/createmany", async (
            AppDbContext context,
            [FromQuery] int quantity = 0,
            [FromQuery] int customerId = 0,
            [FromQuery] int condominiumId = 0) =>
        {
            if (quantity > 0)
            {
                var properties = PropertyFake
                    .GetFakeProperties(quantity, customerId, condominiumId);
                await context.Properties.AddRangeAsync(properties);
            }

            await context.SaveChangesAsync();

            return Results.Created();
        });
    }

    /// <summary>
    /// Habilita a autenticação e autorização na aplicação.
    /// </summary>
    /// <remarks>
    /// Este método habilita a autenticação e autorização
    /// para a aplicação, permitindo o uso de cookies
    /// e tokens JWT.
    /// </remarks>
    /// <param name="app">Instância do aplicativo.</param>
    public static void UseSecurity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }

    /// <summary>
    /// Configura a exibição de arquivos estáticos, definindo
    /// os caminhos físicos e lógicos para cada pasta.
    /// </summary>
    /// <remarks>
    /// Este método configura os diretórios para armazenar
    /// fotos, contratos, templates de contratos e relatórios.
    /// </remarks>
    /// <param name="app">Instância do aplicativo.</param>
    public static void UseStaticFiles(this WebApplication app)
    {
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Configuration.PhotosPath),
            RequestPath = "/photos"
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Configuration.ContractsPath),
            RequestPath = "/contracts"
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Configuration.ContractTemplatesPath),
            RequestPath = "/contracts-templates"
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Configuration.ReportsPath),
            RequestPath = "/reports"
        });
    }

    /// <summary>
    /// Aplica automaticamente as migrações do banco de dados
    /// ao iniciar a aplicação.
    /// </summary>
    /// <remarks>
    /// Este método garante que o banco de dados esteja atualizado
    /// com as últimas migrações antes de iniciar a aplicação.
    /// </remarks>
    /// <param name="app">Instância do aplicativo.</param>
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}