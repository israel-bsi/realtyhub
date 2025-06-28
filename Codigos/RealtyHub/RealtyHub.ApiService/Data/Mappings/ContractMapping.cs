using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="Contrato"/></c> para o modelo de dados.
/// </summary>
public class ContractMapping : IEntityTypeConfiguration<Contrato>
{
    /// <summary>
    /// Configura as propriedades e relacionamentos da entidade <c><see cref="Contrato"/></c>.
    /// </summary>
    /// <param name="builder">O construtor utilizado para configurar a entidade <c><see cref="Contrato"/></c>.</param>
    public void Configure(EntityTypeBuilder<Contrato> builder)
    {
        builder.ToTable("Contrato");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.DataEmissao);

        builder.Property(c => c.DataAssinatura);

        builder.Property(c => c.DataVigencia);

        builder.Property(c => c.DataTermino);

        builder.Property(c => c.ArquivoId)
            .IsRequired();

        builder.Property(c => c.UsuarioId)
            .IsRequired();

        builder.Property(c => c.Ativo)
            .IsRequired();

        builder.Property(c => c.CriadoEm)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(c => c.AtualizadoEm)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.HasOne(c => c.Proposta)
            .WithOne()
            .HasForeignKey<Contrato>(c => c.PropostaId);

        builder.HasOne(c => c.Vendedor)
            .WithMany()
            .HasForeignKey(c => c.VendedorId);

        builder.HasOne(c => c.Comprador)
            .WithMany()
            .HasForeignKey(c => c.CompradorId);

        builder.Ignore(c => c.CaminhoArquivo);
    }
}