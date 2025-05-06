namespace RealtyHub.Core.Requests.Emails;

/// <summary>
/// Classe que representa uma mensagem de e-mail para confirmação de conta
/// </summary>
/// <remarks>
/// <para>Esta classe é usada para encapsular os dados necessários para enviar uma mensagem de e-mail
/// que contém um link de confirmação, incluindo o endereço de e-mail do destinatário e o link de confirmação.</para>
/// <para>A classe herda de <see cref="EmailMesage"/>, que contém propriedades comuns para mensagens de e-mail.</para>
/// </remarks>
public class ConfirmEmailMessage : EmailMesage
{
    /// <summary>
    /// Link de confirmação para o e-mail
    /// </summary>
    /// <remarks>
    /// Este campo deve conter um link válido para que o destinatário possa confirmar seu e-mail.
    /// </remarks>
    /// <value>O link de confirmação para o e-mail.</value>
    public string ConfirmationLink { get; set; } = string.Empty;
}