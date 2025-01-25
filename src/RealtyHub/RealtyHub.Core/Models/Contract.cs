using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Models;

public class Contract : Entity
{
    public long Id { get; set; }
    [Required(ErrorMessage = "A data de emissão é obrigatoria")]
    [DataType(DataType.Date)]
    public DateTime? IssueDate { get; set; }
    [Required(ErrorMessage = "A data de vigência é obrigatoria")]
    public DateTime? EffectiveDate { get; set; }
    [Required(ErrorMessage = "A data de término é obrigatoria")]
    public DateTime? TermEndDate { get; set; }
    public DateTime? SignatureDate { get; set; }
    public string Content { get; set; } = string.Empty;
    public long OfferId { get; set; }
    public Offer Offer { get; set; } = new();
    public bool IsActive { get; set; }
    public string UserId { get; set; } = string.Empty;
}