using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Models.Account;

public class PasswordResetModel
{
    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Minimo de 6 caracteres")]
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public bool IsEqual => Password == ConfirmPassword;
    public string Message => IsEqual ? string.Empty : "As senhas não coincidem";
}