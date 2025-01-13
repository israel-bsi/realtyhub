using RealtyHub.Core.Requests.Emails;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Services;

public interface IEmailService
{
    Task<Response<bool>> SendVerificationCodeAsync(EmailMessageRequest request);
}