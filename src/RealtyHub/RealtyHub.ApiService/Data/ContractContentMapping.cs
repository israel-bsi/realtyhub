using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data;

public class ContractContentMapping : IEntityTypeConfiguration<ContractContent>
{
    public void Configure(EntityTypeBuilder<ContractContent> builder)
    {
        builder.ToTable("ContractContent");

        builder.HasKey(cc => cc.Id);

        builder.Property(cc => cc.Extension)
            .IsRequired();

        builder.Property(cc => cc.Name)
            .IsRequired();

        builder.Property(cc => cc.UserId)
            .IsRequired();

        builder.Property(cc => cc.IsActive)
            .IsRequired();

        builder.Property(cc => cc.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(cc => cc.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Ignore(cc => cc.Path);
    }
}