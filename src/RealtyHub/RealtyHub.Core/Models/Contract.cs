namespace RealtyHub.Core.Models;

public class Contract : Entity
{
    public long Id { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime SignatureDate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime TermEndDate { get; set; }
    public string Content { get; set; } = string.Empty;
    public long OfferId { get; set; }
    public Offer Offer { get; set; } = new();
    public bool IsActive { get; set; }
    public string UserId { get; set; } = string.Empty;
}