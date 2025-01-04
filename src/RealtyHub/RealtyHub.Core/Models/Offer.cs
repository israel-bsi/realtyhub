using RealtyHub.Core.Enums;

namespace RealtyHub.Core.Models;

public class Offer : Entity
{
    public long Id { get; set; }
    public DateTime Submission { get; set; }
    public decimal Amount { get; set; }
    public EOfferStatus OfferStatus { get; set; }
    public List<Payment> Payments { get; set; } = [];
    public long PropertyId { get; set; }
    public Property Property { get; set; } = new();
    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = new();
}