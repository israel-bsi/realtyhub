using RealtyHub.Core.Requests.Emails;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Services;

/// <summary>
/// Interface que define os métodos para o serviço de envio de e-mails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envia um link de confirmação para o e-mail do usuário.
    /// </summary>
    /// <param name="message">A mensagem contendo os detalhes do link de confirmação.</param>
    /// <returns>Um objeto Response indicando se o envio foi realizado com sucesso.</returns>
    Task<Response<bool>> SendConfirmationLinkAsync(ConfirmEmailMessage message);

    /// <summary>
    /// Envia um link para redefinição de senha para o e-mail do usuário.
    /// </summary>
    /// <param name="message">A mensagem contendo os detalhes do link de redefinição de senha.</param>
    /// <returns>Um objeto Response indicando se o envio foi realizado com sucesso.</returns>
    Task<Response<bool>> SendResetPasswordLinkAsync(ResetPasswordMessage message);

    /// <summary>
    /// Envia um contrato como anexo para o e-mail do usuário.
    /// </summary>
    /// <param name="message">A mensagem contendo os detalhes do contrato a ser enviado.</param>
    /// <returns>Um objeto Response indicando se o envio foi realizado com sucesso.</returns>
    Task<Response<bool>> SendContractAsync(AttachmentMessage message);
}