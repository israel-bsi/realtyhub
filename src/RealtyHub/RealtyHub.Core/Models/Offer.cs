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
    public long BuyerId { get; set; }
    public List<Payment> Payments { get; set; } = [];
    public DateTime? SubmissionDate { get; set; } = DateTime.UtcNow;
    public EOfferStatus OfferStatus { get; set; } = EOfferStatus.Analysis;
    public string UserId { get; set; } = string.Empty;
    public Customer Buyer { get; set; } = null!;
    public Property Property { get; set; } = null!;
    public Contract? Contract { get; set; }
}