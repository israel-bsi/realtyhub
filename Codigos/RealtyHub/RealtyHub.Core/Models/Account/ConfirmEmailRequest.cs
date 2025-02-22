namespace RealtyHub.Core.Models.Account;

public class ConfirmEmailRequest
{
    public long UserId { get; set; }
    public string Token { get; set; } = string.Empty;
}