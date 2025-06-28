using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Models;
using RealtyHub.Core.Models;
using System.Reflection;

namespace RealtyHub.ApiService.Data;

/// <summary>
/// Representa o contexto de dados da aplicação, integrando o ASP.NET Identity com as entidades do sistema.
/// </summary>
public class AppDbContext :
    IdentityDbContext
    <
        User,
        IdentityRole<long>,
        long,
        IdentityUserClaim<long>,
        IdentityUserRole<long>,
        IdentityUserLogin<long>,
        IdentityRoleClaim<long>,
        IdentityUserToken<long>
    >
{
    /// <summary>
    /// Inicializa uma nova instância de <c><see cref="AppDbContext"/></c> utilizando as opções configuradas para o contexto.
    /// </summary>
    /// <param name="options">As opções configuradas para o contexto.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Obtém ou define o conjunto de dados para a entidade <c><see cref="Cliente"/></c>.
    /// </summary>
    /// <value>Uma instância de <c><see cref="DbSet{Customer}"/></c> representando os clientes.</value>
    public DbSet<Cliente> Customers { get; set; } = null!;

    /// <summary>
    /// Obtém ou define o conjunto de dados para a entidade <c><see cref="Imovel"/></c>.
    /// </summary>
    /// <value>Uma instância de <c><see cref="DbSet{Property}"/></c> representando os imóveis.</value>
    public DbSet<Imovel> Properties { get; set; } = null!;

    /// <summary>
    /// Obtém ou define o conjunto de dados para a entidade <c><see cref="Viewing"/></c>.
    /// </summary>
    /// <value>Uma instância de <c><see cref="DbSet{Viewing}"/></c> representando as visitas.</value>
    public DbSet<Visita> Viewing { get; set; } = null!;

    /// <summary>
    /// Obtém ou define o conjunto de dados para a entidade <c><see cref="Proposta"/></c>.
    /// </summary>
    /// <value>Uma instância de <c><see cref="DbSet{Offer}"/></c> representando as propostas.</value>
    public DbSet<Proposta> Offers { get; set; } = null!;

    /// <summary>
    /// Obtém ou define o conjunto de dados para a entidade <c><see cref="Pagamento"/></c>.
    /// </summary>
    /// <value>Uma instância de <c><see cref="DbSet{Payment}"/></c> representando os pagamentos.</value>
    public DbSet<Pagamento> Payments { get; set; } = null!;

    /// <summary>
    /// Obtém ou define o conjunto de dados para a entidade <c><see cref="Contrato"/></c>.
    /// </summary>
    /// <value>Uma instância de <c><see cref="DbSet{Contract}"/></c> representando os contratos.</value>
    public DbSet<Contrato> Contracts { get; set; } = null!;

    /// <summary>
    /// Obtém ou define o conjunto de dados para a entidade <c><see cref="FotoImovel"/></c>.
    /// </summary>
    /// <value>Uma instância de <c><see cref="DbSet{PropertyPhoto}"/></c> representando as fotos de propriedades.</value>
    public DbSet<FotoImovel> PropertyPhotos { get; set; } = null!;

    /// <summary>
    /// Obtém ou define o conjunto de dados para a entidade <c><see cref="ModeloContrato"/></c>.
    /// </summary>
    /// <value>Uma instância de <c><see cref="DbSet{ContractTemplate}"/></c> representando os templates de contrato.</value>
    public DbSet<ModeloContrato> ContractTemplates { get; set; } = null!;

    /// <summary>
    /// Obtém ou define o conjunto de dados para a entidade <c><see cref="Condominio"/></c>.
    /// </summary>
    /// <value>Uma instância de <c><see cref="DbSet{Condominium}"/></c> representando os condomínios.</value>
    public DbSet<Condominio> Condominiums { get; set; } = null!;

    /// <summary>
    /// Configura o modelo para o contexto, aplicando as configurações de todas as entidades contidas no assembly.
    /// </summary>
    /// <param name="builder">O construtor do modelo utilizado para configurar as entidades.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}