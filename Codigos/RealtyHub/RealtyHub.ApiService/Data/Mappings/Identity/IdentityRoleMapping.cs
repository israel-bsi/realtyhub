using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RealtyHub.ApiService.Data.Mappings.Identity;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="IdentityRole{TKey}"/></c> para o modelo de dados.
/// </summary>
public class IdentityRoleMapping : IEntityTypeConfiguration<IdentityRole<long>>
{
    /// <summary>
    /// Configura as propriedades da entidade <c><see cref="IdentityRole{TKey}"/></c>.
    /// </summary>
    /// <param name="builder">
    /// O construtor utilizado para configurar a entidade <c><see cref="IdentityRole{TKey}"/></c>.
    /// </param>
    public void Configure(EntityTypeBuilder<IdentityRole<long>> builder)
    {
        builder.ToTable("IdentityRole");

        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.NormalizedName).IsUnique();

        builder.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();
        builder.Property(u => u.Name).HasMaxLength(256);
        builder.Property(u => u.NormalizedName).HasMaxLength(256);
    }
}