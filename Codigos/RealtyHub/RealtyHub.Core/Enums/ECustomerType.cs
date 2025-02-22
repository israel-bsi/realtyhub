using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Enums;

public enum ECustomerType
{
    [Display(Name = "Vendedor")]
    Seller = 1,
    [Display(Name = "Comprador")]
    Buyer = 2,
    [Display(Name = "Ambos")]
    BuyerSeller = 3
}