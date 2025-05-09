namespace RealtyHub.Core.Requests.Emails;

/// <summary>
/// Classe que representa uma mensagem de e-mail para confirmação de conta
/// </summary>
/// <remarks>
/// A classe herda de <see cref="EmailMessage"/>, que contém propriedades comuns para mensagens de e-mail.
/// </remarks>
public class ConfirmEmailMessage : EmailMessage
{
    /// <summary>
    /// Link de confirmação para o e-mail
    /// </summary>
    /// <value>O link de confirmação para o e-mail.</value>
    public string ConfirmationLink { get; set; } = string.Empty;
}