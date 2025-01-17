namespace RealtyHub.Core.Models.Account;

public class UserConfirmEmailRequest
{
    public long UserId { get; set; }
    public string Token { get; set; } = string.Empty;
}