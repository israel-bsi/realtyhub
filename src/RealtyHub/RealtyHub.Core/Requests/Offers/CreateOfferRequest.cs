using System.ComponentModel.DataAnnotations;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Requests.Payments;

namespace RealtyHub.Core.Requests.Offers;

public class CreateOfferRequest : Request
{
    [Required(ErrorMessage = "A data de submissão é obrigatória")]
    public DateTime Submission { get; set; }
    [Required(ErrorMessage = "O valor da proposta é obrigatório")]
    public decimal Amount { get; set; }
    [Required(ErrorMessage = "O imóvel é obrigatório")]
    public long PropertyId { get; set; }
    [Required(ErrorMessage = "O cliente é obrigatório")]
    public long CustomerId { get; set; }
    [Required(ErrorMessage = "O status da proposta é obrigatório")]
    public EOfferStatus OfferStatus { get; set; }
    public List<CreatePaymentRequest> CreatePaymentRequests { get; set; } = [];
}