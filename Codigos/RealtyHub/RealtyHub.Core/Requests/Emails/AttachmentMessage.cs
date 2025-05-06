namespace RealtyHub.Core.Requests.Emails;

/// <summary>
/// Classe que representa uma mensagem de e-mail com um anexo específico, associada a um contrato.
/// </summary>
/// <remarks>
/// <para>Esta classe é usada para encapsular os dados necessários para enviar uma mensagem de e-mail
/// que contém um anexo, incluindo o Id do contrato associado à mensagem.</para>
/// <para>A classe herda de <see cref="EmailMesage"/>, que contém propriedades comuns para mensagens de e-mail.</para>
/// </remarks>
public class AttachmentMessage : EmailMesage
{
    /// <summary>
    /// Identificador do contrato associado à mensagem de e-mail.
    /// </summary>
    /// <remarks>
    /// Este campo deve conter o Id de um contrato existente
    /// </remarks>
    /// <value>O Id do contrato associado à mensagem de e-mail.</value>
    public long ContractId { get; set; }
}