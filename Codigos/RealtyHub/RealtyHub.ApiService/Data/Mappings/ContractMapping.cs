﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="Contract"/></c> para o modelo de dados.
/// </summary>
public class ContractMapping : IEntityTypeConfiguration<Contract>
{
    /// <summary>
    /// Configura as propriedades e relacionamentos da entidade <c><see cref="Contract"/></c>.
    /// </summary>
    /// <param name="builder">O construtor utilizado para configurar a entidade <c><see cref="Contract"/></c>.</param>
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("Contract");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.IssueDate);

        builder.Property(c => c.SignatureDate);

        builder.Property(c => c.EffectiveDate);

        builder.Property(c => c.TermEndDate);

        builder.Property(c => c.FileId)
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

        builder.HasOne(c => c.Seller)
            .WithMany()
            .HasForeignKey(c => c.SellerId);

        builder.HasOne(c => c.Buyer)
            .WithMany()
            .HasForeignKey(c => c.BuyerId);

        builder.Ignore(c => c.FilePath);
    }
}