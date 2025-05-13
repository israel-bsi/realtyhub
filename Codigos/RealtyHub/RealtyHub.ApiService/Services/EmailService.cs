using RealtyHub.Core.Requests.Emails;
using RealtyHub.Core.Responses;
using RealtyHub.Core.Services;
using System.Net;
using System.Net.Mail;

namespace RealtyHub.ApiService.Services;

/// <summary>
/// Classe responsável pelo envio de e-mails utilizando o protocolo SMTP.
/// </summary>
/// <remarks>
/// A classe <c><see cref="EmailService"/></c> implementa a interface
/// <c><see cref="IEmailService"/></c> e fornece métodos para enviar e-mails
/// </remarks>
public class EmailService : IEmailService
{
    /// <summary>
    /// Envia um link de confirmação para o e-mail do usuário.
    /// </summary>
    /// <remarks>
    /// Este método configura um cliente SMTP e envia um e-mail 
    /// formatado em HTML contendo um link para confirmação de e-mail.
    /// </remarks>
    /// <param name="message">
    /// O objeto do tipo <c><see cref="ConfirmEmailMessage"/></c> 
    /// que contém o e-mail de destino e o link de confirmação.
    /// </param>
    /// <returns>
    /// Um objeto <c><see cref="Response{TData}"/></c> indicando o resultado do envio do e-mail.
    /// </returns>
    public Task<Response<bool>> SendConfirmationLinkAsync(ConfirmEmailMessage message)
    {
        var smtpClient = ConfigureClient();
        var mailMessage = ConfigureMailMessage();

        mailMessage.To.Add(message.EmailTo);
        mailMessage.Subject = "Confirme seu email";
        mailMessage.Body = $"<h3>Por favor, confirme seu email clicando neste link:</h3>" +
                           $"<h3> <a href='{message.ConfirmationLink}'>Confirmar email</a></h3>";
        mailMessage.IsBodyHtml = true;

        try
        {
            smtpClient.Send(mailMessage);
            return Task.FromResult(new Response<bool>(true));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new Response<bool>(false, 500, ex.Message));
        }
    }

    /// <summary>
    /// Envia um link para redefinição de senha para o e-mail do usuário.
    /// </summary>
    /// <remarks>
    /// Este método configura um cliente SMTP e envia um e-mail formatado 
    /// em HTML contendo um link para redefinir a senha.
    /// </remarks>
    /// <param name="message">
    /// O objeto do tipo <c><see cref="ResetPasswordMessage"/></c> 
    /// que contém os detalhes do e-mail e o link para redefinição.
    /// </param>
    /// <returns>
    /// Um objeto <c><see cref="Response{TData}"/></c> indicando se o envio do e-mail foi realizado com sucesso.
    /// </returns>
    public Task<Response<bool>> SendResetPasswordLinkAsync(ResetPasswordMessage message)
    {
        var smtpClient = ConfigureClient();
        var mailMessage = ConfigureMailMessage();

        mailMessage.To.Add(message.EmailTo);
        mailMessage.Subject = "Redefinir senha";
        mailMessage.Body = $"<h3>Para redefinir sua senha, clique no link abaixo:</h3>" +
                           $"<h3> <a href='{message.ResetPasswordLink}'>Redefinir senha</a></h3>";
        mailMessage.IsBodyHtml = true;

        try
        {
            smtpClient.Send(mailMessage);
            return Task.FromResult(new Response<bool>(true));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new Response<bool>(false, 500, ex.Message));
        }
    }

    /// <summary>
    /// Envia um contrato como anexo para o e-mail do usuário.
    /// </summary>
    /// <remarks>
    /// Este método configura um cliente SMTP e envia um e-mail com um anexo, 
    /// utilizando os detalhes fornecidos na mensagem.
    /// </remarks>
    /// <param name="message">
    /// O objeto do tipo <c><see cref="AttachmentMessage"/></c> que contém 
    /// o caminho do anexo, assunto e corpo do e-mail.
    /// </param>
    /// <returns>
    /// Um objeto <c><see cref="Response{TData}"/></c> indicando se o envio 
    /// do e-mail com anexo foi realizado com sucesso.
    /// </returns>
    public Task<Response<bool>> SendContractAsync(AttachmentMessage message)
    {
        var smtpClient = ConfigureClient();
        var mailMessage = ConfigureMailMessage();

        var attachment = new Attachment(message.AttachmentPath);
        mailMessage.Attachments.Add(attachment);

        mailMessage.To.Add(message.EmailTo);
        mailMessage.Subject = message.Subject;
        mailMessage.Body = message.Body;
        mailMessage.IsBodyHtml = true;

        try
        {
            smtpClient.Send(mailMessage);
            return Task.FromResult(new Response<bool>(true));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new Response<bool>(false, 500, ex.Message));
        }
    }

    /// <summary>
    /// Configura e retorna um cliente SMTP.
    /// </summary>
    /// <remarks>
    /// Este método configura o cliente SMTP com as credenciais 
    /// e as configurações definidas em <c><see cref="Configuration.EmailSettings"/></c>.
    /// </remarks>
    /// <returns>
    /// Um objeto <c>SmtpClient</c> configurado.
    /// </returns>
    private static SmtpClient ConfigureClient()
    {
        return new SmtpClient
        {
            Host = "smtp.gmail.com",
            EnableSsl = true,
            Port = 587,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(
                Configuration.EmailSettings.EmailFrom,
                Configuration.EmailSettings.EmailPassword)
        };
    }

    /// <summary>
    /// Configura e retorna uma mensagem de e-mail.
    /// </summary>
    /// <remarks>
    /// Este método cria um objeto <c><see cref="MailMessage"/></c> definindo o remetente 
    /// com base nas configurações em <c><see cref="Configuration.EmailSettings"/></c>.
    /// </remarks>
    /// <returns>
    /// Um objeto <c><see cref="MailMessage"/></c> configurado.
    /// </returns>
    private static MailMessage ConfigureMailMessage()
    {
        return new MailMessage
        {
            From = new MailAddress(Configuration.EmailSettings.EmailFrom, "Realty Hub")
        };
    }
}