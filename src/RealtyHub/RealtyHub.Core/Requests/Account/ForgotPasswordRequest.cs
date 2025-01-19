using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Requests.Account;

public class ForgotPasswordRequest : Request
{
    [Required(ErrorMessage = "Email é obrigatório")]
    public string Email { get; set; } = string.Empty;
}