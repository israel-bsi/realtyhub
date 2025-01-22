using RealtyHub.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Models;

public class Offer : Entity
{
    public long Id { get; set; }
    [Required(ErrorMessage = "O valor da proposta é obrigatório")]
    public decimal Amount { get; set; }
    [Required(ErrorMessage = "O imóvel é obrigatório")]
    public long PropertyId { get; set; }
    [Required(ErrorMessage = "O cliente é obrigatório")]
    public long CustomerId { get; set; }
    [Required(ErrorMessage = "Detalhes do pagamento é um campo obrigatório")]
    public string PaymentDetails { get; set; } = string.Empty;
    public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;
    public EOfferStatus OfferStatus { get; set; } = EOfferStatus.Analysis;
    public Customer Customer { get; set; } = new();
    public Property Property { get; set; } = new();
    public string UserId { get; set; } = string.Empty;
}