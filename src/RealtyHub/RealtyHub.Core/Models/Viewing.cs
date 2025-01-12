using RealtyHub.Core.Enums;

namespace RealtyHub.Core.Models;

public class Viewing : Entity
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public EViewingStatus Status { get; set; }
    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = new();
    public long PropertyId { get; set; }
    public Property Property { get; set; } = new();
    public string UserId { get; set; } = string.Empty;
}