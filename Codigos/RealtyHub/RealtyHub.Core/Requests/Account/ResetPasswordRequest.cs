using System.ComponentModel.DataAnnotations;
using RealtyHub.Core.Models.Account;

namespace RealtyHub.Core.Requests.Account;

/// <summary>
/// Classe que representa uma requisição de redefinição de senha.
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class ResetPasswordRequest : Request
{
    /// <summary>
    /// Token de redefinição de senha.
    /// </summary>
    /// <remarks>
    /// O token é usado para verificar a autenticidade da solicitação de redefinição de senha.
    /// </remarks>
    /// <value>O token de redefinição de senha.</value>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// E-mail do usuário.
    /// </summary>
    /// <value>O endereço de e-mail do usuário.</value>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Modelo de redefinição de senha.
    /// </summary>    
    /// <value>O modelo de redefinição de senha.</value>    
    [ValidateComplexType]
    public PasswordResetModel PasswordResetModel { get; set; } = new();
}