namespace RealtyHub.Core.Requests.Emails;

public class EmailMessageRequest
{
    public string EmailTo { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ConfirmationLink { get; set; } = string.Empty;
}