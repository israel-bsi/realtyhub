using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="Offer"/></c> para o modelo de dados.
/// </summary>
public class OfferMapping : IEntityTypeConfiguration<Offer>
{
    /// <summary>
    /// Configura as propriedades e relacionamentos da entidade <c><see cref="Offer"/></c>.
    /// </summary>
    /// <param name="builder">O construtor utilizado para configurar a entidade <c><see cref="Offer"/></c>.</param>
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.ToTable("Offer");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SubmissionDate)
            .IsRequired();

        builder.Property(x => x.Amount)
            .IsRequired();

        builder.Property(x => x.OfferStatus)
            .IsRequired();

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.HasOne(x => x.Property)
            .WithMany()
            .HasForeignKey(x => x.PropertyId);

        builder.HasOne(x => x.Buyer)
            .WithMany()
            .HasForeignKey(x => x.BuyerId);

        builder.HasOne(x => x.Contract)
            .WithOne(x => x.Offer)
            .HasForeignKey<Contract>(x => x.OfferId);
    }
}