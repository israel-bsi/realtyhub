using System.Net.Http.Json;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.Web.Services;

/// <summary>
/// Serviço responsável por obter o relatório de ofertas do backend.
/// </summary>
public class OfferReport
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="OfferReport"/> utilizando a fábrica de <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="httpClientFactory">Fábrica para criar instâncias do HttpClient configuradas para a comunicação com o backend.</param>
    public OfferReport(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory
            .CreateClient(Configuration.HttpClientName);
    }

    /// <summary>
    /// Obtém o relatório de ofertas do backend.
    /// </summary>
    /// <returns>
    /// Task contendo um <see cref="Response{Report}"/> que indica se o relatório foi obtido com sucesso.
    /// Em caso de falha, retorna uma resposta com código 400 e mensagem de erro.
    /// </returns>
    public async Task<Response<Report>> GetOfferAsync()
    {
        var result = await _httpClient.GetAsync("v1/reports/offer");

        return await result.Content.ReadFromJsonAsync<Response<Report>>()
               ?? new Response<Report>(null, 400, "Não foi possível obter o relatório de ofertas");
    }
}