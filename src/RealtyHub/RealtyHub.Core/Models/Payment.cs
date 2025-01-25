using RealtyHub.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealtyHub.Core.Models;

public class Payment : Entity
{
    public long Id { get; set; }
    [Required(ErrorMessage = "O valor do pagamento é obrigatório")]
    public decimal Amount { get; set; }
    [Required(ErrorMessage = "O tipo de pagamento é obrigatório")]
    public EPaymentType PaymentType { get; set; } = EPaymentType.Cash;
    public bool Installments { get; set; }
    [Range(1, 24)] 
    public int InstallmentsCount { get; set; } = 1;
    public long OfferId { get; set; }
    [JsonIgnore]
    public Offer Offer { get; set; } = new();
    public bool IsActive { get; set; }
    public string UserId { get; set; } = string.Empty;
}