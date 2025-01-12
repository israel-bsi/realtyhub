using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

public class ViewingMapping : IEntityTypeConfiguration<Viewing>
{
    public void Configure(EntityTypeBuilder<Viewing> builder)
    {
        builder.ToTable("Viewing");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Date)
            .IsRequired();

        builder.Property(v => v.Status)
            .IsRequired();

        builder.Property(v => v.CustomerId)
            .IsRequired();

        builder.Property(v => v.PropertyId)
            .IsRequired();

        builder.HasOne(v => v.Customer)
            .WithMany()
            .HasForeignKey(v => v.CustomerId);

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