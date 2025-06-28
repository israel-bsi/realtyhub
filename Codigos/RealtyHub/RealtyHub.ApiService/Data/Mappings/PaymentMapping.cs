using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="Pagamento"/></c> para o modelo de dados.
/// </summary>
public class PaymentMapping : IEntityTypeConfiguration<Pagamento>
{
    /// <summary>
    /// Configura as propriedades e relacionamentos da entidade <c><see cref="Pagamento"/></c>.
    /// </summary>
    /// <param name="builder">O construtor utilizado para configurar a entidade <c><see cref="Pagamento"/></c>.</param>
    public void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        builder.ToTable("Pagamento");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Valor)
            .IsRequired();

        builder.Property(x => x.TipoPagamento)
            .IsRequired();

        builder.Property(c => c.UsuarioId)
            .IsRequired();

        builder.Property(x => x.Ativo)
            .HasDefaultValue(true);

        builder.Property(x => x.CriadoEm)
            .HasDefaultValueSql("NOW()");

        builder.Property(x => x.AtualizadoEm)
            .HasDefaultValueSql("NOW()");

        builder.HasOne(p => p.Proposta)
            .WithMany(o => o.Pagamentos)
            .HasForeignKey(p => p.PropostaId);
    }
}