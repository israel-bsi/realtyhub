using RealtyHub.Core.Requests.Emails;
using RealtyHub.Core.Responses;
using RealtyHub.Core.Services;
using System.Net.Http.Json;

namespace RealtyHub.Web.Services;

/// <summary>
/// Serviço responsável por enviar e-mails relacionados à confirmação de conta, 
/// recuperação de senha e envio de contratos.
/// </summary>
public class EmailService : IEmailService
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="EmailService"/> utilizando a fábrica de <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="httpClientFactory">Fábrica para criar instâncias do HttpClient configuradas para a comunicação com o backend.</param>
    public EmailService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory
            .CreateClient(Configuration.HttpClientName);
    }

    /// <summary>
    /// Envia um link de confirmação de e-mail para o usuário.
    /// </summary>
    /// <param name="message">Objeto contendo as informações necessárias para o envio do e-mail de confirmação.</param>
    /// <returns>Task contendo um <see cref="Response{T}"/> indicando se o envio foi bem-sucedido.</returns>
    public Task<Response<bool>> SendConfirmationLinkAsync(ConfirmEmailMessage message)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Envia um link para resetar a senha do usuário.
    /// </summary>
    /// <param name="message">Objeto contendo as informações necessárias para o envio do e-mail de recuperação de senha.</param>
    /// <returns>Task contendo um <see cref="Response{T}"/> indicando se o envio foi bem-sucedido.</returns>
    public Task<Response<bool>> SendResetPasswordLinkAsync(ResetPasswordMessage message)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Envia um contrato por e-mail como anexo.
    /// </summary>
    /// <param name="message">Objeto contendo as informações do contrato e do destinatário.</param>
    /// <returns>
    /// Task contendo um <see cref="Response{T}"/> que indica se o envio do contrato foi realizado com sucesso.
    /// </returns>
    public async Task<Response<bool>> SendContractAsync(AttachmentMessage message)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/emails/contract", message);

        return result.IsSuccessStatusCode
            ? new Response<bool>(true)
            : new Response<bool>(false, (int)result.StatusCode, "Não foi possível enviar o contrato");
    }
}