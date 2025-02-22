namespace RealtyHub.Core.Models.Account;

public class User
{
    public string Email { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string Creci { get; set; } = string.Empty;
    public Dictionary<string, string> Claims { get; set; } = new();
}