using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Enums;

public enum EOfferStatus
{
    [Display(Name = "Em Análise")]
    Analysis = 1,
    [Display(Name = "Aceita")]
    Accepted = 2,
    [Display(Name = "Rejeitada")]
    Rejected = 3
}