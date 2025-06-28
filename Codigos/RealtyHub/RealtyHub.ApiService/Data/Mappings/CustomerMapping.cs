using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="Cliente"/></c> para o modelo de dados.
/// </summary>
public class CustomerMapping : IEntityTypeConfiguration<Cliente>
{
    /// <summary>
    /// Configura as propriedades e relacionamentos da entidade <c><see cref="Cliente"/></c>.
    /// </summary>
    /// <param name="builder">O construtor utilizado para configurar a entidade <c><see cref="Cliente"/></c>.</param>
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Cliente");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Telefone)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(c => c.TipoPessoa)
            .IsRequired();

        builder.Property(c => c.NumeroDocumento)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Rg)
            .HasMaxLength(20);

        builder.Property(c => c.DataEmissaoRg);

        builder.Property(c => c.AutoridadeEmissora)
            .HasMaxLength(80);

        builder.Property(c => c.Nacionalidade)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(c => c.TipoStatusCivil)
            .IsRequired();

        builder.Property(c => c.Ocupacao)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(c => c.NomeFantasia)
            .HasMaxLength(80);

        builder.Property(c => c.Ativo)
            .HasDefaultValue(true);

        builder.Property(c => c.UsuarioId)
            .IsRequired();

        builder.Property(c => c.Ativo)
            .HasDefaultValue(true);

        builder.Property(c => c.CriadoEm)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(c => c.AtualizadoEm)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

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