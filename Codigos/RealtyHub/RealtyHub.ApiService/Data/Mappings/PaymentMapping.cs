using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

public class PaymentMapping : IEntityTypeConfiguration<Payment>
{
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