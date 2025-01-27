using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Data.Mappings;

public class ContractContentMapping : IEntityTypeConfiguration<ContractContent>
{
    public void Configure(EntityTypeBuilder<ContractContent> builder)
    {
        builder.ToTable("ContractContent");

        builder.HasKey(cc => cc.Id);

        builder.Property(cc => cc.Extension)
            .IsRequired();

        builder.Property(cc => cc.Name)
            .IsRequired();

        builder.Property(cc=>cc.Type)
            .IsRequired();

        builder.Property(cc => cc.ShowInPage)
            .IsRequired();

        builder.Property(cc => cc.UserId)
            .IsRequired();

        builder.Property(cc => cc.IsActive)
            .IsRequired();

        builder.Property(cc => cc.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(cc => cc.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Ignore(cc => cc.Path);

        builder.HasData(new ContractContent
        {
            Id = "a2c16556-5098-4496-ae7a-1f9b6d0e8fcf",
            Extension = ".docx",
            Type = EContractModelType.PJPJ,
            Name = "Modelo de Contrato - PJPJ",
            UserId = "",
            IsActive = true,
            ShowInPage = false
        });

        builder.HasData(new ContractContent
        {
            Id = "f7581a63-f4f0-4881-b6ed-6a4100b4182e",
            Extension = ".docx",
            Type = EContractModelType.PFPF,
            Name = "Modelo de Contrato - PFPF",
            UserId = "",
            IsActive = true,
            ShowInPage = false
        });

        builder.HasData(new ContractContent
        {
            Id = "2f4c556d-6850-4b3d-afe9-d7c2bd282718",
            Extension = ".docx",
            Type = EContractModelType.PFPJ,
            Name = "Modelo de Contrato - PFPJ",
            UserId = "",
            IsActive = true,
            ShowInPage = false
        });

        builder.HasData(new ContractContent
        {
            Id = "fd7ed50d-8f42-4288-8877-3cb8095370e7",
            Extension = ".docx",
            Type = EContractModelType.PJPF,
            Name = "Modelo de Contrato - PJPF",
            UserId = "",
            IsActive = true,
            ShowInPage = false
        });

        builder.HasData(new ContractContent
        {
            Id = "2824aec3-3219-4d81-a97a-c3b80ca72844",
            Extension = ".pdf",
            Type = EContractModelType.None,
            Name = "Modelo Padrão",
            UserId = "",
            IsActive = true,
            ShowInPage = true
        });
    }
}