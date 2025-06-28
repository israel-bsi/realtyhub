using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="FotoImovel"/></c> para o modelo de dados.
/// </summary>
public class PropertyPhotosMapping : IEntityTypeConfiguration<FotoImovel>
{
    /// <summary>
    /// Configura as propriedades e relacionamentos da entidade <c><see cref="FotoImovel"/></c>.
    /// </summary>
    /// <param name="builder">O construtor utilizado para configurar a entidade <c><see cref="FotoImovel"/></c>.</param>
    public void Configure(EntityTypeBuilder<FotoImovel> builder)
    {
        builder.ToTable("FotoImovel");

        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.Extensao)
            .IsRequired();

        builder.Property(pi => pi.Miniatura)
            .IsRequired();

        builder.Property(c => c.UsuarioId)
            .IsRequired();

        builder.Property(pi => pi.Ativo)
            .IsRequired();

        builder.Property(pi => pi.ImovelId)
            .IsRequired();

        builder.Property(pi => pi.CriadoEm)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(pi => pi.AtualizadoEm)
            .IsRequired()
            .HasDefaultValueSql("NOW()");
    }
}