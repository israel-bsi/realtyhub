using System.ComponentModel.DataAnnotations;
using RealtyHub.Core.Models.Account;

namespace RealtyHub.Core.Requests.Account;

/// <summary>
/// Classe que representa uma requisição de redefinição de senha.
/// </summary>
/// <remarks>
/// <para>Esta classe contém as propriedades necessárias para a redefinição de senha de um usuário,
/// incluindo o token de redefinição, o e-mail do usuário e o novo modelo de senha.</para>
/// <para>As propriedades são validadas para garantir que os dados fornecidos estejam corretos e atendam aos requisitos mínimos.</para>
/// </remarks>
public class ResetPasswordRequest
{
    /// <summary>
    /// Token de redefinição de senha.
    /// </summary>
    /// <remarks>
    /// <para>Esta propriedade é obrigatória e deve ser preenchida com um token válido.</para>
    /// <para>O token é usado para verificar a autenticidade da solicitação de redefinição de senha.</para>
    /// </remarks>
    /// <value>
    /// O token de redefinição de senha.
    /// </value>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// E-mail do usuário.
    /// </summary>
    /// <remarks>
    /// <para>Esta propriedade é obrigatória e deve ser preenchida com um endereço de e-mail válido.</para>
    /// <para>O e-mail é utilizado para identificar o usuário que está solicitando a redefinição de senha.</para>
    /// </remarks>
    /// <value>
    /// O endereço de e-mail do usuário.
    /// </value>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// ID do usuário.
    /// </summary>
    /// <remarks>
    /// <para>Esta propriedade é obrigatória e deve ser preenchida com um ID válido.</para>
    /// <para>O ID é usado para identificar o usuário no sistema.</para>
    /// </remarks>
    /// <value>
    /// O ID do usuário.
    /// </value>
    public long UserId { get; set; }

    /// <summary>
    /// Modelo de redefinição de senha.
    /// </summary>
    /// <remarks>
    /// <para>Esta propriedade é obrigatória e deve ser preenchida com um modelo de senha válido.</para>
    /// <para>O modelo de senha contém as informações necessárias para redefinir a senha do usuário.</para>
    /// </remarks>
    /// <value>O modelo de redefinição de senha.</value>    
    [ValidateComplexType]
    public PasswordResetModel PasswordResetModel { get; set; } = new();
}