namespace RealtyHub.Core.Requests.Emails;

/// <summary>
/// Classe que representa uma mensagem de e-mail para redefinição de senha
/// </summary>
/// <remarks>
/// <para>Esta classe é usada para encapsular os dados necessários para enviar uma mensagem de e-mail
/// que contém um link de redefinição de senha, incluindo o endereço de e-mail do destinatário e o link de redefinição.</para>
/// <para>A classe herda de <see cref="EmailMesage"/>, que contém propriedades comuns para mensagens de e-mail.</para>
/// </remarks>
public class ResetPasswordMessage : EmailMesage
{
    /// <summary>
    /// Link de redefinição de senha
    /// </summary>
    /// <remarks>
    /// Este campo deve conter um link válido para que o destinatário possa redefinir sua senha.
    /// </remarks>
    /// <value>O link de redefinição de senha.</value>
    public string ResetPasswordLink { get; set; } = string.Empty;
}