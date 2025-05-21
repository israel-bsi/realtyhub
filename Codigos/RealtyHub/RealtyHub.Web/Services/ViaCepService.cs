using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using RealtyHub.Core.Services;
using System.Text.Json;

namespace RealtyHub.Web.Services;

/// <summary>
/// Serviço responsável por buscar informações de endereço a partir do serviço ViaCep.
/// </summary>
public class ViaCepService : IViaCepService
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ViaCepService"/> utilizando a fábrica de <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="httpClientFactory">Fábrica para criar instâncias do HttpClient.</param>
    public ViaCepService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    /// <summary>
    /// Busca as informações de endereço utilizando o CEP informado através da API ViaCep.
    /// </summary>
    /// <param name="cep">CEP a ser pesquisado.</param>
    /// <returns>
    /// Task contendo um <see cref="Response{ViaCepResponse?}"/> com as informações retornadas pela API ou mensagem de erro.
    /// </returns>
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

    /// <summary>
    /// Retorna um objeto <see cref="Address"/> com base no CEP informado, realizando a busca através do serviço ViaCep.
    /// </summary>
    /// <param name="cep">CEP a ser pesquisado.</param>
    /// <returns>
    /// Task contendo um <see cref="Response{Address?}"/> com as informações de endereço ou mensagem de erro.
    /// </returns>
    public async Task<Response<Address?>> GetAddressAsync(string cep)
    {
        try
        {
            var searchAddressAsync = await SearchAddressAsync(cep);
            if (searchAddressAsync is { IsSuccess: false, Data: null })
                return new Response<Address?>(null, 500, searchAddressAsync.Message);

            var address = new Address
            {
                ZipCode = cep,
                Street = searchAddressAsync.Data!.Street,
                Neighborhood = searchAddressAsync.Data.Neighborhood,
                City = searchAddressAsync.Data.City,
                State = searchAddressAsync.Data.State,
                Complement = searchAddressAsync.Data.Complement
            };

            return new Response<Address?>(address);
        }
        catch (Exception e)
        {
            return new Response<Address?>(null, 500, e.Message);
        }
    }
}