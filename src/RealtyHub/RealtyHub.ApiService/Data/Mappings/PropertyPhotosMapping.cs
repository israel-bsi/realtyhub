using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

public class PropertyPhotosMapping : IEntityTypeConfiguration<PropertyPhoto>
{
    public void Configure(EntityTypeBuilder<PropertyPhoto> builder)
    {
        builder.ToTable("PropertyPhotos");

        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.Extension)
            .IsRequired();

        builder.Property(pi => pi.IsThumbnail)
            .IsRequired();

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