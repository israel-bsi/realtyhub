using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Requests.Account;

/// <summary>
/// Classe que representa uma solicitação de login de usuário.
/// </summary>
/// <remarks>
/// <para>Esta classe é usada para enviar as credenciais de login do usuário para o sistema.</para>
/// <para>Ela contém as informações necessárias para autenticar o usuário e iniciar uma sessão.</para>
/// </remarks>
public class LoginRequest
{
    /// <summary>
    /// Email do usuário que está tentando fazer login.
    /// </summary>
    /// <remarks>
    /// <para>Este campo é obrigatório e deve conter um endereço de email válido.</para>
    /// <para>O email será usado para identificar o usuário no sistema.</para>
    /// </remarks>
    /// <value>
    /// O endereço de email do usuário.
    /// </value>
    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário que está tentando fazer login.
    /// </summary>
    /// <remarks>
    /// <para>Este campo é obrigatório e deve conter uma senha válida.</para>
    /// <para>A senha deve ter pelo menos 6 caracteres.</para>
    /// <para>A senha será usada para autenticar o usuário no sistema.</para>
    /// </remarks>
    /// <value>
    /// A senha do usuário.
    /// </value>
    [Required(ErrorMessage = "Senha inválida")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
    public string Password { get; set; } = string.Empty;
}