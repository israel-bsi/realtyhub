using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Enums;

[Flags]
public enum ECustomerType
{
    [Display(Name = "Vendedor")]
    Seller = 1,
    [Display(Name = "Comprador")]
    Buyer = 2
}