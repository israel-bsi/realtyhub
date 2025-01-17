using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Enums;

public enum EPaymentStatus
{
    [Display(Name = "Pendente")]
    Pending = 1,
    [Display(Name = "Pago")]
    Paid = 2,
    [Display(Name = "Cancelado")]
    Canceled = 3
}