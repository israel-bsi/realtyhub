namespace RealtyHub.Core.Responses;

public class UserResponse
{
    public string GivenName { get; set; } = string.Empty;
    public string Creci { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string VerificationCode { get; set; } = string.Empty;
    public Dictionary<string, string> Claims { get; set; } = new();
}