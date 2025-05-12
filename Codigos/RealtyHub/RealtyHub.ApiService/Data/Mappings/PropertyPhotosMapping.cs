using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="PropertyPhoto"/></c> para o modelo de dados.
/// </summary>
public class PropertyPhotosMapping : IEntityTypeConfiguration<PropertyPhoto>
{
    /// <summary>
    /// Configura as propriedades e relacionamentos da entidade <c><see cref="PropertyPhoto"/></c>.
    /// </summary>
    /// <param name="builder">O construtor utilizado para configurar a entidade <c><see cref="PropertyPhoto"/></c>.</param>
    public void Configure(EntityTypeBuilder<PropertyPhoto> builder)
    {
        builder.ToTable("PropertyPhotos");

        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.Extension)
            .IsRequired();

        builder.Property(pi => pi.IsThumbnail)
            .IsRequired();

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(pi => pi.IsActive)
            .IsRequired();

        builder.Property(pi => pi.PropertyId)
            .IsRequired();

        builder.Property(pi => pi.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(pi => pi.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");
    }
}