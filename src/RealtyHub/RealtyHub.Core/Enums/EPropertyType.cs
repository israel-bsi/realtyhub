using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Enums;

public enum EPropertyType
{
    [Display(Name = "Casa")]
    House = 1,
    [Display(Name = "Apartamento")]
    Apartment = 2,
    [Display(Name = "Ponto Comercial")]
    Commercial = 3,
    [Display(Name = "Terreno")]
    Land = 4,
    [Display(Name = "Kitnet")]
    Kitnet = 5,
    [Display(Name = "Fazenda")]
    Farm = 6,
    [Display(Name = "Outros")]
    Others = 7
}