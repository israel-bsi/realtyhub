namespace RealtyHub.Core.Requests.Emails;

/// <summary>
/// Classe que representa uma mensagem de e-mail para redefinição de senha
/// </summary>
/// <remarks>
/// A classe herda de <see cref="EmailMessage"/>, que contém propriedades comuns para mensagens de e-mail.
/// </remarks>
public class ResetPasswordMessage : EmailMessage
{
    /// <summary>
    /// Link de redefinição de senha
    /// </summary>    
    /// <value>O link de redefinição de senha.</value>
    public string ResetPasswordLink { get; set; } = string.Empty;
}