using RealtyHub.Core.Models.Account;
using RealtyHub.Core.Requests.Account;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

public interface IAccountHandler
{
    Task<Response<string>> LoginAsync(LoginRequest request);
    Task<Response<string>> RegisterAsync(RegisterRequest request);
    Task<Response<string>> ConfirmEmailAsync(ConfirmEmailRequest request);
    Task<Response<string>> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<Response<string>> ResetPasswordAsync(ResetPasswordRequest request);
    Task LogoutAsync();
}