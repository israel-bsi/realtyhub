using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RealtyHub.ApiService.Data.Mappings.Identity;

public class IdentityUserTokenMapping : IEntityTypeConfiguration<IdentityUserToken<long>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<long>> builder)
    {
        builder.ToTable("IdentityUserToken");

        builder.HasKey(u => new { u.UserId, u.LoginProvider, u.Name });
        builder.Property(u => u.LoginProvider).HasMaxLength(120);
        builder.Property(u => u.Name).HasMaxLength(180);
    }
}