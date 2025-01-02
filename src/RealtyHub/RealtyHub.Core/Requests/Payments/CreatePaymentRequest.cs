using System.ComponentModel.DataAnnotations;
using RealtyHub.Core.Enums;

namespace RealtyHub.Core.Requests.Payments;

public class CreatePaymentRequest : Request
{
    [Required(ErrorMessage = "A data de pagamento é obrigatória")]
    public DateTime PaymentDate { get; set; }
    [Required(ErrorMessage = "O valor do pagamento é obrigatório")]
    public decimal Amount { get; set; }
    [Required(ErrorMessage = "O tipo de pagamento é obrigatório")]
    public EPaymentType Type { get; set; }
    [Required(ErrorMessage = "O status do pagamento é obrigatório")]
    public EPaymentStatus Status { get; set; }
    [Required(ErrorMessage = "O contrato é obrigatório")]
    public long ContractId { get; set; }
}