using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

public class ContractMapping : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("Contract");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.IssueDate);

        builder.Property(c => c.SignatureDate);

        builder.Property(c => c.EffectiveDate);

        builder.Property(c => c.TermEndDate);

        builder.Property(c => c.Content)
            .IsRequired();

        builder.Property(c => c.OfferId)
            .IsRequired();

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(c => c.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.HasOne(c => c.Offer)
            .WithOne()
            .HasForeignKey<Contract>(c => c.OfferId);
    }
}