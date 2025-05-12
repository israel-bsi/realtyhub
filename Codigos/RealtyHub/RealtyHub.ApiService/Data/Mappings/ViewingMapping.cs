using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="Viewing"/></c> para o modelo de dados.
/// </summary>
public class ViewingMapping : IEntityTypeConfiguration<Viewing>
{
    /// <summary>
    /// Configura as propriedades e relacionamentos da entidade <c><see cref="Viewing"/></c>.
    /// </summary>
    /// <param name="builder">O construtor utilizado para configurar a entidade <c><see cref="Viewing"/></c>.</param>
    public void Configure(EntityTypeBuilder<Viewing> builder)
    {
        builder.ToTable("Viewing");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.ViewingDate)
            .IsRequired();

        builder.Property(v => v.ViewingStatus)
            .IsRequired();

        builder.HasOne(v => v.Buyer)
            .WithMany()
            .HasForeignKey(v => v.BuyerId);

        builder.HasOne(v => v.Property)
            .WithMany()
            .HasForeignKey(v => v.PropertyId);

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(v => v.CreatedAt)
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(v => v.UpdatedAt)
            .HasDefaultValueSql("NOW()")
            .IsRequired();
    }
}