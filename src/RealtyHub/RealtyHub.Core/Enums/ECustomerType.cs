using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Enums;

public enum ECustomerType
{
    [Display(Name = "Pessoa Física")]
    Individual = 1,
    [Display(Name = "Pessoa Jurídica")]
    Business = 2
}