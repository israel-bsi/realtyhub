using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Models.Account;

/// <summary>
/// Representa o modelo de redefinição de senha do usuário.
/// </summary>
public class PasswordResetModel
{
    /// <summary>
    /// Senha do usuário.
    /// </summary>
    /// <value>A senha do usuário, mínimo de 6 caracteres</value>
    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Mínimo de 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Confirmação da senha do usuário.
    /// </summary>
    /// <value>A confirmação da senha do usuário, mínimo de 6 caracteres</value>
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Verifica se a senha e a confirmação da senha são iguais.
    /// </summary>
    /// <value>Retorna true se as senhas forem iguais, caso contrário, false.</value>
    public bool IsEqual => Password == ConfirmPassword;

    /// <summary>
    /// Mensagem de erro caso as senhas não sejam iguais.
    /// </summary>
    /// <value>Mensagem de erro se as senhas não coincidirem.</value>
    public string Message => IsEqual ? string.Empty : "As senhas não coincidem";
}