﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="Condominium"/></c> para o modelo de dados.
/// </summary>
public class CondominiumMapping : IEntityTypeConfiguration<Condominium>
{
    /// <summary>
    /// Configura as propriedades e os relacionamentos da entidade <c><see cref="Condominium"/></c>.
    /// </summary>
    /// <param name="builder">O construtor para configurar a entidade <c><see cref="Condominium"/></c>.</param>
    public void Configure(EntityTypeBuilder<Condominium> builder)
    {
        builder.ToTable("Condominium");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(x => x.Units)
            .IsRequired();

        builder.Property(x => x.Floors)
            .IsRequired();

        builder.Property(x => x.HasElevator)
            .IsRequired();

        builder.Property(x => x.HasSwimmingPool)
            .IsRequired();

        builder.Property(x => x.HasPartyRoom)
            .IsRequired();

        builder.Property(x => x.HasPlayground)
            .IsRequired();

        builder.Property(x => x.HasFitnessRoom)
            .IsRequired();

        builder.Property(x => x.CondominiumValue)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

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