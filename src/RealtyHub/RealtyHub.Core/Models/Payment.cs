using System.Text.Json.Serialization;
using RealtyHub.Core.Enums;

namespace RealtyHub.Core.Models;

public class Payment : Entity
{
    public long Id { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public EPaymentType PaymentType { get; set; }
    public EPaymentStatus PaymentStatus { get; set; }
    public long OfferId { get; set; }
    [JsonIgnore]
    public Offer Offer { get; set; } = new();
    public bool IsActive { get; set; }
}