using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="Condominio"/></c> para o modelo de dados.
/// </summary>
public class CondominiumMapping : IEntityTypeConfiguration<Condominio>
{
    /// <summary>
    /// Configura as propriedades e os relacionamentos da entidade <c><see cref="Condominio"/></c>.
    /// </summary>
    /// <param name="builder">O construtor para configurar a entidade <c><see cref="Condominio"/></c>.</param>
    public void Configure(EntityTypeBuilder<Condominio> builder)
    {
        builder.ToTable("Condominio");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(x => x.Unidades)
            .IsRequired();

        builder.Property(x => x.Andares)
            .IsRequired();

        builder.Property(x => x.PossuiElevador)
            .IsRequired();

        builder.Property(x => x.PossuiPiscina)
            .IsRequired();

        builder.Property(x => x.PossuiSalaoFesta)
            .IsRequired();

        builder.Property(x => x.PossuiPlayground)
            .IsRequired();

        builder.Property(x => x.PossuiAcademia)
            .IsRequired();

        builder.Property(x => x.TaxaCondominial)
            .IsRequired();

        builder.Property(x => x.Ativo)
            .IsRequired();

        builder.Property(p => p.CriadoEm)
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(p => p.AtualizadoEm)
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.OwnsOne(a => a.Endereco, address =>
        {
            address.Property(a => a.Logradouro)
                .IsRequired()
                .HasColumnName("Logradouro")
                .HasMaxLength(80);

            address.Property(a => a.Bairro)
                .IsRequired()
                .HasColumnName("Bairro")
                .HasMaxLength(80);

            address.Property(a => a.Cidade)
                .IsRequired()
                .HasColumnName("Cidade")
                .HasMaxLength(80);

            address.Property(a => a.Numero)
                .IsRequired()
                .HasColumnName("Numero");

            address.Property(a => a.Estado)
                .IsRequired()
                .HasColumnName("Estado")
                .HasMaxLength(80);

            address.Property(a => a.Pais)
                .IsRequired()
                .HasColumnName("Pais")
                .HasMaxLength(80);

            address.Property(a => a.Cep)
                .IsRequired()
                .HasColumnName("Cep")
                .HasMaxLength(20);

            address.Property(a => a.Complemento)
                .HasColumnName("Complemento")
                .HasMaxLength(80);
        });
    }
}