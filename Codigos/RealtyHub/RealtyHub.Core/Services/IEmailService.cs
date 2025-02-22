using RealtyHub.Core.Requests.Emails;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Services;

public interface IEmailService
{
    Task<Response<bool>> SendConfirmationLinkAsync(ConfirmEmailMessage message);
    Task<Response<bool>> SendResetPasswordLinkAsync(ResetPasswordMessage message);
    Task<Response<bool>> SendContractAsync(AttachmentMessage message);
}