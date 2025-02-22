using System.ComponentModel.DataAnnotations;
using RealtyHub.Core.Models.Account;

namespace RealtyHub.Core.Requests.Account;

public class ResetPasswordRequest
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public long UserId { get; set; }

    [ValidateComplexType] 
    public PasswordResetModel PasswordResetModel { get; set; } = new();
}