using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RealtyHub.ApiService.Data.Mappings.Identity;

public class IdentityRoleMapping : IEntityTypeConfiguration<IdentityRole<long>>
{
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