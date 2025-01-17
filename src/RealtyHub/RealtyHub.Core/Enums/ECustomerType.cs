using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Enums;

public enum ECustomerType
{
    [Display(Name = "Física")]
    Individual = 1,
    [Display(Name = "Jurídica")]
    Business = 2
}