using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="Visita"/></c> para o modelo de dados.
/// </summary>
public class ViewingMapping : IEntityTypeConfiguration<Visita>
{
    /// <summary>
    /// Configura as propriedades e relacionamentos da entidade <c><see cref="Visita"/></c>.
    /// </summary>
    /// <param name="builder">O construtor utilizado para configurar a entidade <c><see cref="Visita"/></c>.</param>
    public void Configure(EntityTypeBuilder<Visita> builder)
    {
        builder.ToTable("Visita");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.DataVisita)
            .IsRequired();

        builder.Property(v => v.StatusVisita)
            .IsRequired();

        builder.HasOne(v => v.Comprador)
            .WithMany()
            .HasForeignKey(v => v.CompradorId);

        builder.HasOne(v => v.Imovel)
            .WithMany()
            .HasForeignKey(v => v.ImovelId);

        builder.Property(c => c.UsuarioId)
            .IsRequired();

        builder.Property(v => v.CriadoEm)
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(v => v.AtualizadoEm)
            .HasDefaultValueSql("NOW()")
            .IsRequired();
    }
}