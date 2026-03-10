using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealtyHub.Infrastructure.Data;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;

namespace RealtyHub.Tests.Common;

/// <summary>
/// Classe base para testes que precisam criar cenários completos de entidades no banco de dados.
/// Fornece métodos auxiliares para evitar repetição de código mantendo o contexto do EF Core.
/// </summary>
public abstract class BaseIntegrationTest : IClassFixture<RealtyHubApiTests>
{
    protected readonly RealtyHubApiTests Factory;

    protected BaseIntegrationTest(RealtyHubApiTests factory)
    {
        Factory = factory;
    }

    /// <summary>
    /// Limpa o banco de dados de uma entidade específica e retorna a contagem anterior.
    /// </summary>
    /// <typeparam name="TEntity">Tipo da entidade a ser limpa</typeparam>
    /// <returns>Número de entidades encontradas antes da limpeza</returns>
    protected async Task<int> CleanupDatabaseAndGetPreviousCount<TEntity>() 
        where TEntity : class
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var entities = typeof(TEntity).Name switch
        {
            nameof(Contract) => dbContext.Contracts.Cast<TEntity>(),
            nameof(Offer) => dbContext.Offers.Cast<TEntity>(),
            nameof(Property) => dbContext.Properties.Cast<TEntity>(),
            nameof(Customer) => dbContext.Customers.Cast<TEntity>(),
            nameof(Condominium) => dbContext.Condominiums.Cast<TEntity>(),
            nameof(Viewing) => dbContext.Viewing.Cast<TEntity>(),
            nameof(IdentityUser) => dbContext.Users.Cast<TEntity>(),
            _ => throw new ArgumentException($"Tipo {typeof(TEntity).Name} não suportado")
        };

        var existingCount = await entities.CountAsync();
        var existingEntities = await entities.ToListAsync();
        
        dbContext.RemoveRange(existingEntities);
        await dbContext.SaveChangesAsync();
        
        return existingCount;
    }

    /// <summary>
    /// Cria um conjunto completo de entidades relacionadas (condomínio, seller, buyer, property, offer, contract) no contexto fornecido.
    /// Mantém todas as entidades no mesmo contexto do EF Core para preservar os relacionamentos.
    /// </summary>
    /// <param name="dbContext">Contexto do banco de dados onde as entidades serão criadas</param>
    /// <returns>Tupla contendo todas as entidades criadas</returns>
    protected async Task<(Condominium condominium, Customer seller, Customer buyer, Property property, Offer offer, Contract contract)> 
        CreateCompleteContractScenarioAsync(AppDbContext dbContext)
    {
        // Cria um condomínio
        var condominium = MockData.GetValidCondominium();
        await dbContext.Condominiums.AddAsync(condominium);
        await dbContext.SaveChangesAsync();

        // Cria um comprador
        var buyer = MockData.GetValidCustomer(ECustomerType.Buyer);
        await dbContext.Customers.AddAsync(buyer);
        await dbContext.SaveChangesAsync();

        // Cria um vendedor
        var seller = MockData.GetValidCustomer(ECustomerType.Seller);
        await dbContext.Customers.AddAsync(seller);
        await dbContext.SaveChangesAsync();

        // Cria a propriedade
        var property = MockData.GetValidProperty(seller, condominium);
        await dbContext.Properties.AddAsync(property);
        await dbContext.SaveChangesAsync();

        // Cria uma oferta
        var offer = MockData.GetValidOffer(buyer, property);
        await dbContext.Offers.AddAsync(offer);
        await dbContext.SaveChangesAsync();

        // Cria o contrato
        var contract = MockData.GetValidContract(offer, seller, buyer);
        await dbContext.Contracts.AddAsync(contract);
        await dbContext.SaveChangesAsync();

        return (condominium, seller, buyer, property, offer, contract);
    }

    /// <summary>
    /// Cria apenas as entidades básicas necessárias para testes simples (sem contrato).
    /// Útil quando você precisa testar criação de contratos ou cenários que não requerem um contrato existente.
    /// </summary>
    /// <param name="dbContext">Contexto do banco de dados onde as entidades serão criadas</param>
    /// <returns>Tupla contendo as entidades básicas criadas</returns>
    protected async Task<(Condominium condominium, Customer seller, Customer buyer, Property property, Offer offer)> 
        CreateBasicScenarioAsync(AppDbContext dbContext)
    {
        // Cria um condomínio
        var condominium = MockData.GetValidCondominium();
        await dbContext.Condominiums.AddAsync(condominium);
        await dbContext.SaveChangesAsync();

        // Cria um comprador
        var buyer = MockData.GetValidCustomer(ECustomerType.Buyer);
        await dbContext.Customers.AddAsync(buyer);
        await dbContext.SaveChangesAsync();

        // Cria um vendedor
        var seller = MockData.GetValidCustomer(ECustomerType.Seller);
        await dbContext.Customers.AddAsync(seller);
        await dbContext.SaveChangesAsync();

        // Cria a propriedade
        var property = MockData.GetValidProperty(seller, condominium);
        await dbContext.Properties.AddAsync(property);
        await dbContext.SaveChangesAsync();

        // Cria uma oferta
        var offer = MockData.GetValidOffer(buyer, property);
        await dbContext.Offers.AddAsync(offer);
        await dbContext.SaveChangesAsync();

        return (condominium, seller, buyer, property, offer);
    }

    /// <summary>
    /// Cria apenas um condomínio, seller e buyer. Útil para testes mais específicos.
    /// </summary>
    /// <param name="dbContext">Contexto do banco de dados onde as entidades serão criadas</param>
    /// <returns>Tupla contendo o condomínio, seller e buyer criados</returns>
    protected async Task<(Condominium condominium, Customer seller, Customer buyer)> 
        CreateMinimalScenarioAsync(AppDbContext dbContext)
    {
        // Cria um condomínio
        var condominium = MockData.GetValidCondominium();
        await dbContext.Condominiums.AddAsync(condominium);
        await dbContext.SaveChangesAsync();

        // Cria um comprador
        var buyer = MockData.GetValidCustomer(ECustomerType.Buyer);
        await dbContext.Customers.AddAsync(buyer);
        await dbContext.SaveChangesAsync();

        // Cria um vendedor
        var seller = MockData.GetValidCustomer(ECustomerType.Seller);
        await dbContext.Customers.AddAsync(seller);
        await dbContext.SaveChangesAsync();

        return (condominium, seller, buyer);
    }

    /// <summary>
    /// Cria um DbContext usando o scope do factory.
    /// Útil quando você precisa de acesso direto ao contexto.
    /// </summary>
    /// <returns>Tupla contendo o scope e o contexto do banco</returns>
    protected (IServiceScope scope, AppDbContext dbContext) CreateDbContext()
    {
        var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return (scope, dbContext);
    }
}