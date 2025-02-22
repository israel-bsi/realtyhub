using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

public class ContractTemplatesMapping : IEntityTypeConfiguration<ContractTemplate>
{
    public void Configure(EntityTypeBuilder<ContractTemplate> builder)
    {
        builder.ToTable("ContractTemplate");

        builder.HasKey(cc => cc.Id);

        builder.Property(cc => cc.Extension)
            .IsRequired();

        builder.Property(cc => cc.Name)
            .IsRequired();

        builder.Property(cc=>cc.Type)
            .IsRequired();

        builder.Property(cc => cc.ShowInPage)
            .IsRequired();

        builder.Ignore(cc => cc.Path);

        builder.HasData(new ContractTemplate
        {
            Id = "a2c16556-5098-4496-ae7a-1f9b6d0e8fcf",
            Extension = ".docx",
            Type = EContractModelType.PJPJ,
            Name = "Modelo de Contrato - PJPJ",
            ShowInPage = false
        });

        builder.HasData(new ContractTemplate
        {
            Id = "f7581a63-f4f0-4881-b6ed-6a4100b4182e",
            Extension = ".docx",
            Type = EContractModelType.PFPF,
            Name = "Modelo de Contrato - PFPF",
            ShowInPage = false
        });

        builder.HasData(new ContractTemplate
        {
            Id = "2f4c556d-6850-4b3d-afe9-d7c2bd282718",
            Extension = ".docx",
            Type = EContractModelType.PFPJ,
            Name = "Modelo de Contrato - PFPJ",
            ShowInPage = false
        });

        builder.HasData(new ContractTemplate
        {
            Id = "fd7ed50d-8f42-4288-8877-3cb8095370e7",
            Extension = ".docx",
            Type = EContractModelType.PJPF,
            Name = "Modelo de Contrato - PJPF",
            ShowInPage = false
        });

        builder.HasData(new ContractTemplate
        {
            Id = "2824aec3-3219-4d81-a97a-c3b80ca72844",
            Extension = ".pdf",
            Type = EContractModelType.None,
            Name = "Modelo Padrão",
            ShowInPage = true
        });
    }
}