using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealtyHub.Core.Requests.Account;

/// <summary>
/// Classe que representa uma requisição de registro de usuário.
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class RegisterRequest : Request
{
    /// <summary>
    /// Número do CRECI do usuário.
    /// </summary>
    /// <remarks>
    /// O número do CRECI é um registro profissional necessário para corretores de imóveis.
    /// </remarks>
    /// <value>O número do CRECI do usuário.</value>
    [Required(ErrorMessage = "Creci é obrigatório")]
    public string Creci { get; set; } = string.Empty;

    /// <summary>
    /// Nome do usuário.
    /// </summary>
    /// <value>O nome completo do usuário.</value>
    [Required(ErrorMessage = "Nome é obrigatório")]
    public string GivenName { get; set; } = string.Empty;

    /// <summary>
    /// E-mail do usuário.
    /// </summary>
    /// <value>O endereço de e-mail do usuário.</value>
    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário.
    /// </summary>    
    /// <value>A senha do usuário.</value>
    [Required(ErrorMessage = "Senha inválida")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Confirmação da senha do usuário.
    /// </summary>    
    /// <value>A confirmação da senha do usuário.</value>
    [Required(ErrorMessage = "Senha inválida")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
    [JsonIgnore]
    public string ConfirmPassword { get; set; } = string.Empty;
}