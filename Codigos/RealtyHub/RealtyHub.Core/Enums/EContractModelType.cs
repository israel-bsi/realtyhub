using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Enums;

public enum EContractModelType
{
    None,
    [Display(Name = "Pessoa Jurídica para Pessoa Jurídica")]
    PJPJ = 1,
    [Display(Name = "Pessoa Física para Pessoa Física")]
    PFPF = 2,
    [Display(Name = "Pessoa Física para Pessoa Jurídica")]
    PFPJ = 3,
    [Display(Name = "Pessoa Jurídica para Pessoa Física")]
    PJPF = 4
}