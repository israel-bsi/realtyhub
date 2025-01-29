using RealtyHub.Core.Requests.Emails;
using RealtyHub.Core.Responses;
using RealtyHub.Core.Services;
using System.Net;
using System.Net.Mail;

namespace RealtyHub.ApiService.Services;

public class EmailService : IEmailService
{
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
            return Task.FromResult(new Response<bool>(
                false, 500, ex.Message));
        }
    }

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

    private static MailMessage ConfigureMailMessage()
    {
        return new MailMessage
        {
            From = new MailAddress(Configuration.EmailSettings.EmailFrom, "Realty Hub")
        };
    }
}