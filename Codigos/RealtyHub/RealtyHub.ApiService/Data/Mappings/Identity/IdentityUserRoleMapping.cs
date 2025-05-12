using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RealtyHub.ApiService.Data.Mappings.Identity;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="IdentityUserRole{TKey}"/></c> para o modelo de dados.
/// </summary>
public class IdentityUserRoleMapping : IEntityTypeConfiguration<IdentityUserRole<long>>
{
    /// <summary>
    /// Configura as propriedades da entidade <c><see cref="IdentityUserRole{TKey}"/></c>.
    /// </summary>
    /// <param name="builder">
    /// O construtor utilizado para configurar a entidade <c><see cref="IdentityUserRole{TKey}"/></c>.
    /// </param>
    public void Configure(EntityTypeBuilder<IdentityUserRole<long>> builder)
    {
        builder.ToTable("IdentityUserRole");

        builder.HasKey(u => new { u.UserId, u.RoleId });
    }
}