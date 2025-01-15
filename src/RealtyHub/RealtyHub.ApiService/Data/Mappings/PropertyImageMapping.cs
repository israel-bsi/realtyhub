using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

public class PropertyImageMapping : IEntityTypeConfiguration<PropertyImage>
{
    public void Configure(EntityTypeBuilder<PropertyImage> builder)
    {
        builder.ToTable("PropertyImage");

        builder.HasKey(pi => pi.Id);

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(pi => pi.IsActive)
            .IsRequired();

        builder.Property(pi => pi.PropertyId)
            .IsRequired();

        builder.Property(pi => pi.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(pi => pi.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");
    }
}