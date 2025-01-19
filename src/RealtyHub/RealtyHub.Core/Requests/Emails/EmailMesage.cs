namespace RealtyHub.Core.Requests.Emails;

public abstract class EmailMesage
{
    public string EmailTo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}