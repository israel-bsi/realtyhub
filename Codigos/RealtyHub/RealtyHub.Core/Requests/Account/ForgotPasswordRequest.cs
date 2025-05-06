using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Requests.Account;

/// <summary>
/// Classe que representa uma requisição de recuperação de senha.
/// </summary>
/// <remarks>
/// <para>Esta classe é usada para enviar uma requisição de recuperação de senha para o sistema.</para>
/// <para>Ela contém as informações necessárias para identificar o usuário que está solicitando a recuperação de senha.</para>
/// </remarks>
public class ForgotPasswordRequest : Request
{
    /// <summary>
    /// Email do usuário que está solicitando a recuperação de senha.
    /// </summary>
    /// <remarks>
    /// <para>Este campo é obrigatório e deve conter um endereço de email válido.</para>
    /// <para>O email será usado para enviar as instruções de recuperação de senha.</para>
    /// </remarks>
    /// <value>O endereço de email do usuário.</value>
    [Required(ErrorMessage = "Email é obrigatório")]
    public string Email { get; set; } = string.Empty;
}