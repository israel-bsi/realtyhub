using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="Proposta"/></c> para o modelo de dados.
/// </summary>
public class OfferMapping : IEntityTypeConfiguration<Proposta>
{
    /// <summary>
    /// Configura as propriedades e relacionamentos da entidade <c><see cref="Proposta"/></c>.
    /// </summary>
    /// <param name="builder">O construtor utilizado para configurar a entidade <c><see cref="Proposta"/></c>.</param>
    public void Configure(EntityTypeBuilder<Proposta> builder)
    {
        builder.ToTable("Proposta");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DataDeEnvio)
            .IsRequired();

        builder.Property(x => x.Valor)
            .IsRequired();

        builder.Property(x => x.StatusProposta)
            .IsRequired();

        builder.Property(c => c.UsuarioId)
            .IsRequired();

        builder.Property(x => x.CriadoEm)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(x => x.AtualizadoEm)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.HasOne(x => x.Imovel)
            .WithMany()
            .HasForeignKey(x => x.ImovelId);

        builder.HasOne(x => x.Comprador)
            .WithMany()
            .HasForeignKey(x => x.CompradorId);

        builder.HasOne(x => x.Contrato)
            .WithOne(x => x.Proposta)
            .HasForeignKey<Contrato>(x => x.PropostaId);
    }
}