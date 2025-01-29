namespace RealtyHub.Core.Requests.Emails;

public abstract class EmailMesage
{
    public string EmailTo { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string AttachmentPath { get; set; } = string.Empty;
}