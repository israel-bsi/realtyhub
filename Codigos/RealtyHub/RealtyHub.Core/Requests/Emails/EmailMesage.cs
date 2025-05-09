namespace RealtyHub.Core.Requests.Emails;

/// <summary>
/// Classe abstrata que representa uma mensagem de e-mail genérica.
/// </summary>
public abstract class EmailMessage
{
    /// <summary>
    /// Endereço de e-mail do destinatário.
    /// </summary>    
    /// <value>O endereço de e-mail do destinatário.</value>
    public string EmailTo { get; set; } = string.Empty;

    /// <summary>
    /// Assunto da mensagem de e-mail.
    /// </summary>
    /// <value>O assunto da mensagem de e-mail.</value>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Corpo da mensagem de e-mail.
    /// </summary>    
    /// <value>O corpo da mensagem de e-mail.</value>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Caminho do anexo da mensagem de e-mail.
    /// </summary>
    /// <value>O caminho do anexo da mensagem de e-mail.</value>
    public string AttachmentPath { get; set; } = string.Empty;
}