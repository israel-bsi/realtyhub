using System.Text.Json;
using RealtyHub.Core.Responses;
using RealtyHub.Core.Services;

namespace RealtyHub.ApiService.Services;

public class ViaCepService(IHttpClientFactory httpClientFactory) : IViaCepService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
    public async Task<Response<ViaCepResponse?>> SearchAddressAsync(string cep)
    {
        try
        {
            var url = $"https://viacep.com.br/ws/{cep}/json/";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var statusMessage = await response.Content.ReadAsStringAsync();
                return new Response<ViaCepResponse?>(null, (int)response.StatusCode, statusMessage);
            }

            var content = await response.Content.ReadAsStringAsync();
            var viaCepResponse = JsonSerializer.Deserialize<ViaCepResponse>(content);

            return new Response<ViaCepResponse?>(viaCepResponse);
        }
        catch (Exception ex)
        {
            return new Response<ViaCepResponse?>(null, 500, ex.Message);
        }
    }
}