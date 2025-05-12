using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RealtyHub.ApiService.Data.Mappings.Identity;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="IdentityRoleClaim{TKey}"/></c> para o modelo de dados.
/// </summary>
public class IdentityRoleClaimMapping : IEntityTypeConfiguration<IdentityRoleClaim<long>>
{
    /// <summary>
    /// Configura as propriedades da entidade <c><see cref="IdentityRoleClaim{TKey}"/></c>.
    /// </summary>
    /// <param name="builder">
    /// O construtor utilizado para configurar a entidade <c><see cref="IdentityRoleClaim{TKey}"/></c>.
    /// </param>
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<long>> builder)
    {
        builder.ToTable("IdentityRoleClaim");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.ClaimType).HasMaxLength(255);
        builder.Property(u => u.ClaimValue).HasMaxLength(255);
    }
}