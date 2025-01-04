using RealtyHub.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Requests.Payments;

public class UpdatePaymentRequest : Request
{
    public long Id { get; set; }
    [Required(ErrorMessage = "A data de pagamento é obrigatória")]
    public DateTime PaymentDate { get; set; }
    [Required(ErrorMessage = "O valor do pagamento é obrigatório")]
    public decimal Amount { get; set; }
    [Required(ErrorMessage = "O tipo de pagamento é obrigatório")]
    public EPaymentType PaymentType { get; set; }
    [Required(ErrorMessage = "O status do pagamento é obrigatório")]
    public EPaymentStatus PaymentStatus { get; set; }
}