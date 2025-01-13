using System.Net;
using System.Net.Mail;
using RealtyHub.Core.Requests.Emails;
using RealtyHub.Core.Responses;
using RealtyHub.Core.Services;

namespace RealtyHub.ApiService.Services;

public class EmailService : IEmailService
{
    public Task<Response<bool>> SendVerificationCodeAsync(EmailMessageRequest request)
    {
        var smtpClient = ConfigureClient();
        var mailMessage = ConfigureMailMessage();

        mailMessage.To.Add(request.EmailTo);
        mailMessage.Subject = "Código de confirmação";
        mailMessage.Body = $"<h3>Por favor, confirme seu email clicando neste link:</h3>" +
                           $"<h3> <a href='{request.ConfirmationLink}'>Confirmar email</a></h3>";
        mailMessage.IsBodyHtml = true;

        try
        {
            smtpClient.Send(mailMessage);
            return Task.FromResult(new Response<bool>(true));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new Response<bool>(
                false, 
                (int)HttpStatusCode.InternalServerError, 
                ex.Message));
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
            From = new MailAddress(Configuration.EmailSettings.EmailFrom,
                "Realty Hub")
        };
    }
}