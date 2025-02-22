using System.Net.Http.Json;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.Web.Services;

public class OfferReport(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory
        .CreateClient(Configuration.HttpClientName);

    public async Task<Response<Report>> GetOfferAsync()
    {
        var result = await _httpClient.GetAsync("v1/reports/offer");

        return await result.Content.ReadFromJsonAsync<Response<Report>>()
               ?? new Response<Report>(null, 400, "Não foi possível obter o relatório de ofertas");
    }
}