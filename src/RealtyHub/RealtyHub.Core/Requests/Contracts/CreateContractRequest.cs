using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Requests.Contracts;

public class CreateContractRequest : Request
{
    [Required(ErrorMessage = "A data de emissão é obrigatoria")]
    public DateTime IssueDate { get; set; }
    [Required(ErrorMessage = "A data de assinatura é obrigatoria")]
    public DateTime SignatureDate { get; set; }
    [Required(ErrorMessage = "A data de vigência é obrigatoria")]
    public DateTime EffectiveDate { get; set; }
    [Required(ErrorMessage = "A data de término é obrigatoria")]
    public DateTime TermEndDate { get; set; }
    [Required(ErrorMessage = "O contrato é obrigatório")]
    public long OfferId { get; set; }
    public bool IsActive { get; set; }
}