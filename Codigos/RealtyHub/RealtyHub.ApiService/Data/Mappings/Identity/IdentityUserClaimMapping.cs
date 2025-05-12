using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RealtyHub.ApiService.Data.Mappings.Identity;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="IdentityUserClaim{TKey}"/></c> para o modelo de dados.
/// </summary>
public class IdentityUserClaimMapping : IEntityTypeConfiguration<IdentityUserClaim<long>>
{
    /// <summary>
    /// Configura as propriedades da entidade <c><see cref="IdentityUserClaim{TKey}"/></c>.
    /// </summary>
    /// <param name="builder">
    /// O construtor utilizado para configurar a entidade <c><see cref="IdentityUserClaim{TKey}"/></c>.
    /// </param>
    public void Configure(EntityTypeBuilder<IdentityUserClaim<long>> builder)
    {
        builder.ToTable("IdentityClaim");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.ClaimType).HasMaxLength(255);
        builder.Property(u => u.ClaimValue).HasMaxLength(255);
    }
}