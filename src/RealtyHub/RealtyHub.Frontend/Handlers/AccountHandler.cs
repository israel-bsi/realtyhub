﻿using System.Text;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Requests.Account;
using RealtyHub.Core.Responses;

namespace RealtyHub.Frontend.Handlers;

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
        var result = await _httpClient.PostAsJsonAsync("v1/identity/register", request);
        return result.IsSuccessStatusCode 
            ? new Response<string>("Cadastro realizado com sucesso!", 201, "Cadastro realizado com sucesso!")
            : new Response<string>(null, (int)result.StatusCode,"Não foi possível realizar o cadastro");
        
    }
    public async Task LogoutAsync()
    {
        var emptyContent = new StringContent("{}", Encoding.UTF8, "application/json");
        await _httpClient.PostAsync("v1/identity/logout", emptyContent);
    }
}