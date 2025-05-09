namespace RealtyHub.Core.Requests.Emails;

/// <summary>
/// Classe que representa uma mensagem de e-mail com um anexo específico, associada a um contrato.
/// </summary>
/// <remarks>
/// A classe herda de <see cref="EmailMessage"/>, que contém propriedades comuns para mensagens de e-mail.
/// </remarks>
public class AttachmentMessage : EmailMessage
{
    /// <summary>
    /// Identificador do contrato associado à mensagem de e-mail.
    /// </summary>
    /// <value>O Id do contrato associado à mensagem de e-mail.</value>
    public long ContractId { get; set; }
}