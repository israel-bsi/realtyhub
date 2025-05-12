using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="Payment"/></c> para o modelo de dados.
/// </summary>
public class PaymentMapping : IEntityTypeConfiguration<Payment>
{
    /// <summary>
    /// Configura as propriedades e relacionamentos da entidade <c><see cref="Payment"/></c>.
    /// </summary>
    /// <param name="builder">O construtor utilizado para configurar a entidade <c><see cref="Payment"/></c>.</param>
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payment");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount)
            .IsRequired();

        builder.Property(x => x.PaymentType)
            .IsRequired();

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("NOW()");

        builder.Property(x => x.UpdatedAt)
            .HasDefaultValueSql("NOW()");

        builder.HasOne(p => p.Offer)
            .WithMany(o => o.Payments)
            .HasForeignKey(p => p.OfferId);
    }
}