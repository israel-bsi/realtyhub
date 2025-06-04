using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models.Account;
using RealtyHub.Core.Requests.Account;
using RealtyHub.Core.Responses;
using System.Net.Http.Json;
using System.Text;

namespace RealtyHub.Web.Handlers;

/// <summary>
/// Handler responsável pelas operações de autenticação e gerenciamento de conta de usuário na aplicação web.
/// </summary>
public class AccountHandler : IAccountHandler
{
    /// <summary>
    /// Cliente HTTP utilizado para realizar requisições à API.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="AccountHandler"/> utilizando a fábrica de HttpClient.
    /// </summary>
    /// <param name="httpClientFactory">Fábrica de <see cref="IHttpClientFactory"/> para criar o cliente HTTP.</param>
    public AccountHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory
            .CreateClient(Configuration.HttpClientName);
    }

    /// <summary>
    /// Realiza o login do usuário.
    /// </summary>
    /// <remarks>
    /// Envia a requisição de login para a API, utilizando cookies para autenticação.
    /// Retorna sucesso se o status HTTP for positivo, caso contrário retorna mensagem de erro.
    /// </remarks>
    public async Task<Response<string>> LoginAsync(LoginRequest request)
    {
        var result = await _httpClient
            .PostAsJsonAsync("v1/identity/login?useCookies=true", request);

        return result.IsSuccessStatusCode
            ? new Response<string>()
            : new Response<string>(null, (int)result.StatusCode, "Não foi possível realizar login");
    }

    /// <summary>
    /// Realiza o cadastro de um novo usuário.
    /// </summary>
    /// <remarks>
    /// Envia os dados de cadastro para a API. Retorna erro se o e-mail já estiver cadastrado ou se a operação falhar.
    /// </remarks>
    public async Task<Response<string>> RegisterAsync(RegisterRequest request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/identity/register-user", request);

        var content = await result.Content.ReadAsStringAsync();

        if (content.Contains("DuplicateUserName"))
            return new Response<string>(null, 400, "E-mail já cadastrado");

        if (!result.IsSuccessStatusCode)
            return new Response<string>(null, (int)result.StatusCode,
                "Não foi possível realizar o cadastro");

        var data = await result.Content.ReadFromJsonAsync<Response<string>>();

        return new Response<string>(null, 201, data?.Message);
    }

    /// <summary>
    /// Confirma o e-mail do usuário utilizando o token enviado por e-mail.
    /// </summary>
    /// <remarks>
    /// Valida o token de confirmação de e-mail recebido pela API. Retorna erro se o token for inválido.
    /// </remarks>
    public async Task<Response<string>> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        var url = $"v1/identity/confirm-email?userId={request.UserId}&token={request.Token}";
        var result = await _httpClient.GetAsync(url);

        var content = await result.Content.ReadAsStringAsync();

        if (content.Contains("InvalidToken"))
            return new Response<string>(null, 400, "Token inválido");

        var data = await result.Content.ReadFromJsonAsync<Response<string>>();

        return result.IsSuccessStatusCode
            ? new Response<string>(data?.Data, 200, data?.Message)
            : new Response<string>(data?.Data, (int)result.StatusCode, data?.Message);
    }

    /// <summary>
    /// Solicita redefinição de senha para o usuário.
    /// </summary>
    /// <remarks>
    /// Envia a requisição para a API para iniciar o processo de recuperação de senha. Retorna erro se o usuário não for encontrado.
    /// </remarks>
    public async Task<Response<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/identity/forgot-password", request);
        var content = await result.Content.ReadAsStringAsync();
        if (content.Contains("Usuário não encontrado"))
            return new Response<string>(null, 404, "Usuário não encontrado");

        if (!result.IsSuccessStatusCode)
            return new Response<string>(null, (int)result.StatusCode,
                "Não foi possível redefinir a senha");

        var data = await result.Content.ReadFromJsonAsync<Response<string>>();
        return new Response<string>(null, 200, data?.Message);
    }

    /// <summary>
    /// Redefine a senha do usuário utilizando o token recebido por e-mail.
    /// </summary>
    /// <remarks>
    /// Envia a nova senha e o token para a API. Retorna erro se o token for inválido ou se a operação falhar.
    /// </remarks>
    public async Task<Response<string>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var url = $"v1/identity/reset-password?userId={request.UserId}&token={request.Token}";
        var result = await _httpClient.PostAsJsonAsync(url, request);
        var content = await result.Content.ReadAsStringAsync();

        if (content.Contains("InvalidToken"))
            return new Response<string>(null, 400, "Token inválido");

        var data = await result.Content.ReadFromJsonAsync<Response<string>>();

        return result.IsSuccessStatusCode
            ? new Response<string>(data?.Data, 200, data?.Message)
            : new Response<string>(data?.Data, (int)result.StatusCode, data?.Message);
    }

    /// <summary>
    /// Realiza o logout do usuário.
    /// </summary>
    /// <remarks>
    /// Envia uma requisição para a API para encerrar a sessão do usuário autenticado.
    /// </remarks>
    public async Task LogoutAsync()
    {
        var emptyContent = new StringContent("{}", Encoding.UTF8, "application/json");
        await _httpClient.PostAsync("v1/identity/logout", emptyContent);
    }
}