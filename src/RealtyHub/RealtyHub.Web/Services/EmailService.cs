using RealtyHub.Core.Requests.Emails;
using RealtyHub.Core.Responses;
using RealtyHub.Core.Services;
using System.Net.Http.Json;

namespace RealtyHub.Web.Services;

public class EmailService(IHttpClientFactory httpClientFactory) : IEmailService
{
    private readonly HttpClient _httpClient = httpClientFactory
        .CreateClient(Configuration.HttpClientName);
    public Task<Response<bool>> SendConfirmationLinkAsync(ConfirmEmailMessage message)
    {
        throw new NotImplementedException();
    }

    public Task<Response<bool>> SendResetPasswordLinkAsync(ResetPasswordMessage message)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<bool>> SendContractAsync(AttachmentMessage message)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/emails/contract", message);

        return result.IsSuccessStatusCode
            ? new Response<bool>(true)
            : new Response<bool>(false, (int)result.StatusCode, "Não foi possível enviar o contrato");
    }
}