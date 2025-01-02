using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Requests.Offers;

public class UpdateOfferRequest : Request
{
    public long Id { get; set; }
    [Required(ErrorMessage = "A data de submissão é obrigatória")]
    public DateTime Submission { get; set; }
    [Required(ErrorMessage = "O valor da proposta é obrigatório")]
    public decimal Amount { get; set; }
}