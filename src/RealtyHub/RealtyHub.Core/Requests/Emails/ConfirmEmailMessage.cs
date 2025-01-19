namespace RealtyHub.Core.Requests.Emails;

public class ConfirmEmailMessage : EmailMesage
{
    public string ConfirmationLink { get; set; } = string.Empty;
}