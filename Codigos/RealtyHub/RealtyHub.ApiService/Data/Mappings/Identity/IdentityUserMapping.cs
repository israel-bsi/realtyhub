using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.ApiService.Models;

namespace RealtyHub.ApiService.Data.Mappings.Identity;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="User"/></c> para o modelo de dados.
/// </summary>
public class IdentityUserMapping : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// Configura as propriedades da entidade <c><see cref="User"/></c>.
    /// </summary>
    /// <param name="builder">
    /// O construtor utilizado para configurar a entidade <c><see cref="User"/></c>.
    /// </param>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("IdentityUser");

        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.NormalizedUserName).IsUnique();
        builder.HasIndex(u => u.NormalizedEmail).IsUnique();

        builder.Property(u => u.Creci);
        builder.Property(u => u.GivenName).HasMaxLength(100);
        builder.Property(u => u.Email).HasMaxLength(180);
        builder.Property(u => u.NormalizedEmail).HasMaxLength(180);
        builder.Property(u => u.UserName).HasMaxLength(160);
        builder.Property(u => u.NormalizedUserName).HasMaxLength(180);
        builder.Property(u => u.PhoneNumber).HasMaxLength(20);
        builder.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

        builder.HasMany<IdentityUserClaim<long>>()
            .WithOne()
            .HasForeignKey(u => u.UserId)
            .IsRequired();

        builder.HasMany<IdentityUserLogin<long>>()
            .WithOne()
            .HasForeignKey(u => u.UserId)
            .IsRequired();

        builder.HasMany<IdentityUserToken<long>>()
            .WithOne()
            .HasForeignKey(u => u.UserId)
            .IsRequired();

        builder.HasMany<IdentityUserRole<long>>()
            .WithOne()
            .HasForeignKey(u => u.UserId)
            .IsRequired();
    }
}