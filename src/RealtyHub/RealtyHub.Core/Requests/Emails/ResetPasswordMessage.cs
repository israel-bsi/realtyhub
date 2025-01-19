namespace RealtyHub.Core.Requests.Emails;

public class ResetPasswordMessage : EmailMesage
{
    public string ResetPasswordLink { get; set; } = string.Empty;
}