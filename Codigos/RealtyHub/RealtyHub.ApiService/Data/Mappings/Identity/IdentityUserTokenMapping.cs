using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RealtyHub.ApiService.Data.Mappings.Identity;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="IdentityUserToken{TKey}"/></c> para o modelo de dados.
/// </summary>
public class IdentityUserTokenMapping : IEntityTypeConfiguration<IdentityUserToken<long>>
{
    /// <summary>
    /// Configura as propriedades da entidade <c><see cref="IdentityUserToken{TKey}"/></c>.
    /// </summary>
    /// <param name="builder">
    /// O construtor utilizado para configurar a entidade <c><see cref="IdentityUserToken{TKey}"/></c>.
    /// </param>
    public void Configure(EntityTypeBuilder<IdentityUserToken<long>> builder)
    {
        builder.ToTable("IdentityUserToken");

        builder.HasKey(u => new { u.UserId, u.LoginProvider, u.Name });
        builder.Property(u => u.LoginProvider).HasMaxLength(120);
        builder.Property(u => u.Name).HasMaxLength(180);
    }
}