using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RealtyHub.ApiService.Data.Mappings.Identity;

public class IdentityUserLoginMapping : IEntityTypeConfiguration<IdentityUserLogin<long>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<long>> builder)
    {
        builder.ToTable("IdentityUserLogin");

        builder.HasKey(u => new { u.LoginProvider, u.ProviderKey });
        builder.Property(u => u.LoginProvider).HasMaxLength(128);
        builder.Property(u => u.ProviderKey).HasMaxLength(128);
        builder.Property(u => u.ProviderDisplayName).HasMaxLength(255);
    }
}