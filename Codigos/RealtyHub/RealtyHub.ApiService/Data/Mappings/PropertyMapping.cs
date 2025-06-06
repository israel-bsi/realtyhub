﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="Property"/></c> para o modelo de dados.
/// </summary>
public class PropertyMapping : IEntityTypeConfiguration<Property>
{
    /// <summary>
    /// Configura as propriedades e relacionamentos da entidade <c><see cref="Property"/></c>.
    /// </summary>
    /// <param name="builder">O construtor utilizado para configurar a entidade <c><see cref="Property"/></c>.</param>
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Property");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(p => p.Description)
            .IsRequired();

        builder.Property(p => p.Price)
            .IsRequired();

        builder.Property(p => p.PropertyType)
            .IsRequired();

        builder.Property(p => p.Bedroom)
            .IsRequired();

        builder.Property(p => p.Bathroom)
            .IsRequired();

        builder.Property(p => p.Garage)
            .IsRequired();

        builder.Property(p => p.Area)
            .IsRequired();

        builder.Property(p => p.TransactionsDetails);

        builder.Property(p => p.IsNew)
            .IsRequired();

        builder.Property(p => p.RegistryNumber)
            .IsRequired();

        builder.Property(p => p.RegistryRecord)
            .IsRequired();

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.Property(p => p.ShowInHome)
            .IsRequired();

        builder.HasOne(p => p.Seller)
            .WithMany(c => c.Properties)
            .HasForeignKey(p => p.SellerId);

        builder.HasOne(p => p.Condominium)
            .WithMany(c => c.Properties)
            .HasForeignKey(p => p.CondominiumId);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasDefaultValueSql("NOW()")
            .IsRequired();

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

            address.Property(a => a.Number)
                .IsRequired()
                .HasColumnName("Number");

            address.Property(a => a.City)
                .IsRequired()
                .HasColumnName("City")
                .HasMaxLength(80);

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

        builder.HasMany(p => p.PropertyPhotos)
            .WithOne(pi => pi.Property)
            .HasForeignKey(pi => pi.PropertyId);
    }
}