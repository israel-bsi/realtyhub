using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RealtyHub.ApiService.Data.Mappings.Identity;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="IdentityUserLogin{TKey}"/></c> para o modelo de dados.
/// </summary>
public class IdentityUserLoginMapping : IEntityTypeConfiguration<IdentityUserLogin<long>>
{
    /// <summary>
    /// Configura as propriedades da entidade <c><see cref="IdentityUserLogin{TKey}"/></c>.
    /// </summary>
    /// <param name="builder">
    /// O construtor utilizado para configurar a entidade <c><see cref="IdentityUserLogin{TKey}"/></c>.
    /// </param>
    public void Configure(EntityTypeBuilder<IdentityUserLogin<long>> builder)
    {
        builder.ToTable("IdentityUserLogin");

        builder.HasKey(u => new { u.LoginProvider, u.ProviderKey });
        builder.Property(u => u.LoginProvider).HasMaxLength(128);
        builder.Property(u => u.ProviderKey).HasMaxLength(128);
        builder.Property(u => u.ProviderDisplayName).HasMaxLength(255);
    }
}