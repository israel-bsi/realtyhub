using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Enums;

public enum EMaritalStatus
{
    [Display(Name = "Solteiro(a)")]
    Single = 1,
    [Display(Name = "Casado(a)")]
    Married = 2,
    [Display(Name = "Divorciado(a)")]
    Divorced = 3,
    [Display(Name = "Viúvo(a)")]
    Widowed = 4,
    [Display(Name = "Noivo(a)")]
    Engaged = 5
}