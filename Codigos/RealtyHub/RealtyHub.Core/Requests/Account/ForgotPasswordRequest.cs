using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Requests.Account;

/// <summary>
/// Classe que representa uma requisição de recuperação de senha.
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class ForgotPasswordRequest : Request
{
    /// <summary>
    /// E-mail do usuário que está solicitando a recuperação de senha.
    /// </summary>    
    /// <value>O endereço de e-mail do usuário.</value>
    [Required(ErrorMessage = "Email é obrigatório")]
    public string Email { get; set; } = string.Empty;
}