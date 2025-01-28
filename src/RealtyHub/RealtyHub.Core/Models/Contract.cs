using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Models;

public class Contract : Entity
{
    public long Id { get; set; }
    public long SellerId { get; set; }
    public long BuyerId { get; set; }
    public long OfferId { get; set; }
    [Required(ErrorMessage = "A data de emissão é obrigatoria")]
    public DateTime? IssueDate { get; set; }
    [Required(ErrorMessage = "A data de vigência é obrigatoria")]
    public DateTime? EffectiveDate { get; set; }
    [Required(ErrorMessage = "A data de término é obrigatoria")]
    public DateTime? TermEndDate { get; set; }
    public DateTime? SignatureDate { get; set; }
    public string FileId { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Offer? Offer { get; set; } = new();
    public Customer? Seller { get; set; }
    public Customer? Buyer { get; set; }
    public string FilePath =>
        $"{Configuration.BackendUrl}/contracts/{FileId}.pdf";
}