using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Enums;

public enum EPropertyType
{
    [Display(Name = "Casa")]
    Casa = 1,
    [Display(Name = "Apartamento")]
    Apartamento = 2,
    [Display(Name = "Ponto Comercial")]
    Comercial = 3,
    [Display(Name = "Terreno")]
    Terreno = 4,
    [Display(Name = "Kitnet")]
    Kitnet = 5,
    [Display(Name = "Chácara")]
    Chácara = 6,
    [Display(Name = "Sítio")]
    Sítio = 7,
    [Display(Name = "Fazenda")]
    Fazenda = 8,
    [Display(Name = "Cobertura")]
    Cobertura = 9,
    [Display(Name = "Galpão")]
    Galpão = 10,
    [Display(Name = "Outros")]
    Outros = 11
}