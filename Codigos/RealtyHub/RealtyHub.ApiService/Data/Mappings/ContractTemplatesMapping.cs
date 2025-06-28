using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

/// <summary>
/// Configura o mapeamento da entidade <c><see cref="ModeloContrato"/></c> para o modelo de dados.
/// </summary>
public class ContractTemplatesMapping : IEntityTypeConfiguration<ModeloContrato>
{
    /// <summary>
    /// Configura as propriedades e relacionamentos da entidade <c><see cref="ModeloContrato"/></c>.
    /// </summary>
    /// <param name="builder">O construtor utilizado para configurar a entidade <c><see cref="ModeloContrato"/></c>.</param>
    public void Configure(EntityTypeBuilder<ModeloContrato> builder)
    {
        builder.ToTable("ModeloContrato");

        builder.HasKey(cc => cc.Id);

        builder.Property(cc => cc.Extensao)
            .IsRequired();

        builder.Property(cc => cc.Nome)
            .IsRequired();

        builder.Property(cc => cc.Tipo)
            .IsRequired();

        builder.Property(cc => cc.MostrarNaHome)
            .IsRequired();

        builder.Ignore(cc => cc.Caminho);

        builder.HasData(new ModeloContrato
        {
            Id = "a2c16556-5098-4496-ae7a-1f9b6d0e8fcf",
            Extensao = ".docx",
            Tipo = ETipoContrato.PJPJ,
            Nome = "Modelo de Contrato - PJPJ",
            MostrarNaHome = false
        });

        builder.HasData(new ModeloContrato
        {
            Id = "f7581a63-f4f0-4881-b6ed-6a4100b4182e",
            Extensao = ".docx",
            Tipo = ETipoContrato.PFPF,
            Nome = "Modelo de Contrato - PFPF",
            MostrarNaHome = false
        });

        builder.HasData(new ModeloContrato
        {
            Id = "2f4c556d-6850-4b3d-afe9-d7c2bd282718",
            Extensao = ".docx",
            Tipo = ETipoContrato.PFPJ,
            Nome = "Modelo de Contrato - PFPJ",
            MostrarNaHome = false
        });

        builder.HasData(new ModeloContrato
        {
            Id = "fd7ed50d-8f42-4288-8877-3cb8095370e7",
            Extensao = ".docx",
            Tipo = ETipoContrato.PJPF,
            Nome = "Modelo de Contrato - PJPF",
            MostrarNaHome = false
        });

        builder.HasData(new ModeloContrato
        {
            Id = "2824aec3-3219-4d81-a97a-c3b80ca72844",
            Extensao = ".pdf",
            Tipo = ETipoContrato.Nenhum,
            Nome = "Modelo Padrão",
            MostrarNaHome = true
        });
    }
}