using System.Net.Http.Json;
using System.Text;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models.Account;
using RealtyHub.Core.Requests.Account;
using RealtyHub.Core.Responses;

namespace RealtyHub.Web.Handlers;

public class AccountHandler(IHttpClientFactory httpClientFactory) : IAccountHandler
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(Configuration.HttpClientName);
    public async Task<Response<string>> LoginAsync(LoginRequest request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/identity/login?useCookies=true", request);

        return result.IsSuccessStatusCode
            ? new Response<string>("Login realizado com sucesso!", 200, "Login realizado com sucesso!")
            : new Response<string>(null, (int)result.StatusCode, "Não foi possível realizar login");
    }
    public async Task<Response<string>> RegisterAsync(RegisterRequest request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/identity/registeruser", request);

        var content = await result.Content.ReadAsStringAsync();

        if (content.Contains("DuplicateUserName"))
            return new Response<string>(null, 400, "E-mail já cadastrado");

        if (!result.IsSuccessStatusCode)
            return new Response<string>(null, (int)result.StatusCode, "Não foi possível realizar o cadastro");

        var data = await result.Content.ReadFromJsonAsync<Response<string>>();

        return new Response<string>(null, 201, data?.Message);
    }

    public async Task<Response<string>> ConfirmEmail(UserConfirmEmailRequest user)
    {
        var url = $"v1/identity/confirm-email?userId={user.UserId}&token={user.Token}";
        var result = await _httpClient.GetAsync(url);

        var content = await result.Content.ReadAsStringAsync();

        if (content.Contains("InvalidToken"))
            return new Response<string>(null, 400, "Token inválido");

        var data = await result.Content.ReadFromJsonAsync<Response<string>>();

        return result.IsSuccessStatusCode
            ? new Response<string>(data?.Data, 200, data?.Message)
            : new Response<string>(data?.Data, (int)result.StatusCode, data?.Message);
    }

    public async Task LogoutAsync()
    {
        var emptyContent = new StringContent("{}", Encoding.UTF8, "application/json");
        await _httpClient.PostAsync("v1/identity/logout", emptyContent);
    }
}