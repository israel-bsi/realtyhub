using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Requests.Account;

/// <summary>
/// Classe que representa uma requisição de login de usuário.
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class LoginRequest : Request
{
    /// <summary>
    /// E-mail do usuário que está tentando fazer login.
    /// </summary>    
    /// <value>O endereço de e-mail do usuário.</value>    
    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário que está tentando fazer login.
    /// </summary>    
    /// <value> A senha do usuário.</value>    
    [Required(ErrorMessage = "Senha inválida")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
    public string Password { get; set; } = string.Empty;
}