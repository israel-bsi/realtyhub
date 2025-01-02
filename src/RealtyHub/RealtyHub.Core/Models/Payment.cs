using RealtyHub.Core.Enums;

namespace RealtyHub.Core.Models;

public class Payment : Entity
{
    public long Id { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public EPaymentType Type { get; set; }
    public EPaymentStatus Status { get; set; }
    public long ContractId { get; set; }
    public Contract Contract { get; set; } = new();
    public bool IsActive { get; set; }
}