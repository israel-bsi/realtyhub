using System.Net.Http.Json;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.Web.Services;

public class PropertyReport
{
    private readonly HttpClient _httpClient;

    public PropertyReport(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory
            .CreateClient(Configuration.HttpClientName);
    }

    public async Task<Response<Report>> GetPropertyAsync()
    {
        var result = await _httpClient.GetAsync("v1/reports/property");

        return await result.Content.ReadFromJsonAsync<Response<Report>>()
               ?? new Response<Report>(null, 400, "Não foi possível obter o relatório de imóveis");
    }
}