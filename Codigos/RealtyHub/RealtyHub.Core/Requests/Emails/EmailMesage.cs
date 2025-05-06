namespace RealtyHub.Core.Requests.Emails;

/// <summary>
/// Classe base que representa uma mensagem de e-mail genérica.
/// </summary>
/// <remarks>
/// <para>Esta classe é usada para encapsular os dados necessários para enviar uma mensagem de e-mail,
/// incluindo o endereço de e-mail do destinatário, o assunto e o corpo da mensagem.</para>
/// <para>A classe pode ser estendida para criar mensagens de e-mail específicas, como mensagens de confirmação
/// ou mensagens com anexos.</para>
/// </remarks>
public abstract class EmailMesage
{
    /// <summary>
    /// Endereço de e-mail do destinatário.
    /// </summary>
    /// <remarks>
    /// Este campo é obrigatório e deve conter o endereço de e-mail do destinatário.
    /// </remarks>
    /// <value>O endereço de e-mail do destinatário.</value>
    public string EmailTo { get; set; } = string.Empty;

    /// <summary>
    /// Assunto da mensagem de e-mail.
    /// </summary>
    /// <remarks>
    /// Este campo é obrigatório e deve conter o assunto da mensagem de e-mail.
    /// </remarks>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Corpo da mensagem de e-mail.
    /// </summary>
    /// <remarks>
    /// Este campo é obrigatório e deve conter o corpo da mensagem de e-mail.
    /// </remarks>
    /// <value>O corpo da mensagem de e-mail.</value>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Caminho do anexo da mensagem de e-mail.
    /// </summary>
    /// <remarks>
    /// Este campo é opcional e deve conter o caminho do arquivo a ser anexado à mensagem de e-mail.
    /// </remarks>
    /// <value>O caminho do anexo da mensagem de e-mail.</value>
    public string AttachmentPath { get; set; } = string.Empty;
}