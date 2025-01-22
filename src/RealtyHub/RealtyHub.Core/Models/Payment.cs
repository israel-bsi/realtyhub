using RealtyHub.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealtyHub.Core.Models;

public class Payment : Entity
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
    public long OfferId { get; set; }
    [JsonIgnore]
    public Offer Offer { get; set; } = new();
    public bool IsActive { get; set; }
    public string UserId { get; set; } = string.Empty;
}