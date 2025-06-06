﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="Customer"/></c> para o modelo de dados.
/// </summary>
public class CustomerMapping : IEntityTypeConfiguration<Customer>
{
    /// <summary>
    /// Configura as propriedades e relacionamentos da entidade <c><see cref="Customer"/></c>.
    /// </summary>
    /// <param name="builder">O construtor utilizado para configurar a entidade <c><see cref="Customer"/></c>.</param>
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customer");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Phone)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(c => c.PersonType)
            .IsRequired();

        builder.Property(c => c.DocumentNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Rg)
            .HasMaxLength(20);

        builder.Property(c => c.RgIssueDate);

        builder.Property(c => c.IssuingAuthority)
            .HasMaxLength(80);

        builder.Property(c => c.Nationality)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(c => c.MaritalStatus)
            .IsRequired();

        builder.Property(c => c.Occupation)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(c => c.BusinessName)
            .HasMaxLength(80);

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(c => c.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.OwnsOne(a => a.Address, address =>
        {
            address.Property(a => a.Street)
                .IsRequired()
                .HasColumnName("Street")
                .HasMaxLength(80);

            address.Property(a => a.Neighborhood)
                .IsRequired()
                .HasColumnName("Neighborhood")
                .HasMaxLength(80);

            address.Property(a => a.City)
                .IsRequired()
                .HasColumnName("City")
                .HasMaxLength(80);

            address.Property(a => a.Number)
                .IsRequired()
                .HasColumnName("Number");

            address.Property(a => a.State)
                .IsRequired()
                .HasColumnName("State")
                .HasMaxLength(80);

            address.Property(a => a.Country)
                .IsRequired()
                .HasColumnName("Country")
                .HasMaxLength(80);

            address.Property(a => a.ZipCode)
                .IsRequired()
                .HasColumnName("ZipCode")
                .HasMaxLength(20);

            address.Property(a => a.Complement)
                .HasColumnName("Complement")
                .HasMaxLength(80);
        });
    }
}