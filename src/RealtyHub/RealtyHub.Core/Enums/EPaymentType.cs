using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Enums;

public enum EPaymentType
{
    [Display(Name = "Boleto")]
    BankSlip = 1,
    [Display(Name = "Transferência Bancária")]
    BankTranfer = 2,
    [Display(Name = "Cheque")]
    Check = 3,
    [Display(Name = "Dinheiro")]
    Cash = 4,
    [Display(Name = "Pix")]
    Pix = 5,
    [Display(Name = "Financiamento")]
    Financing = 6,
    [Display(Name = "Cartão de Crédito")]
    CreditCard = 7,
    [Display(Name = "FGTS")]
    Fgts = 8,
    [Display(Name = "Outros")]
    Others = 9
}