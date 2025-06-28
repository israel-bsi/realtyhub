using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="Imovel"/></c> para o modelo de dados.
/// </summary>
public class PropertyMapping : IEntityTypeConfiguration<Imovel>
{
    /// <summary>
    /// Configura as propriedades e relacionamentos da entidade <c><see cref="Imovel"/></c>.
    /// </summary>
    /// <param name="builder">O construtor utilizado para configurar a entidade <c><see cref="Imovel"/></c>.</param>
    public void Configure(EntityTypeBuilder<Imovel> builder)
    {
        builder.ToTable("Imovel");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Titulo)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(p => p.Descricao)
            .IsRequired();

        builder.Property(p => p.Preco)
            .IsRequired();

        builder.Property(p => p.TipoImovel)
            .IsRequired();

        builder.Property(p => p.Quarto)
            .IsRequired();

        builder.Property(p => p.Banheiro)
            .IsRequired();

        builder.Property(p => p.Garagem)
            .IsRequired();

        builder.Property(p => p.Area)
            .IsRequired();

        builder.Property(p => p.DetalhesTransacao);

        builder.Property(p => p.Novo)
            .IsRequired();

        builder.Property(p => p.NumeroRegistro)
            .IsRequired();

        builder.Property(p => p.RegistroCartorio)
            .IsRequired();

        builder.Property(c => c.UsuarioId)
            .IsRequired();

        builder.Property(p => p.Ativo)
            .IsRequired();

        builder.Property(p => p.ExibirNaHome)
            .IsRequired();

        builder.HasOne(p => p.Vendedor)
            .WithMany(c => c.Imoveis)
            .HasForeignKey(p => p.VendedorId);

        builder.HasOne(p => p.Condominio)
            .WithMany(c => c.Imoveis)
            .HasForeignKey(p => p.CondominioId);

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

            address.Property(a => a.Numero)
                .IsRequired()
                .HasColumnName("Numero");

            address.Property(a => a.Cidade)
                .IsRequired()
                .HasColumnName("Cidade")
                .HasMaxLength(80);

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

        builder.HasMany(p => p.FotosImovel)
            .WithOne(pi => pi.Imovel)
            .HasForeignKey(pi => pi.ImovelId);
    }
}