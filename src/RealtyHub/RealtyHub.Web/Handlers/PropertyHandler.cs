using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Core.Responses;
using System.Net.Http.Json;

namespace RealtyHub.Web.Handlers;

public class PropertyHandler(IHttpClientFactory httpClientFactory) : IPropertyHandler
{
    private readonly HttpClient _httpClient = httpClientFactory
        .CreateClient(Configuration.HttpClientName);
    public async Task<Response<Property?>> CreateAsync(Property request)
    {
        var result = await _httpClient
            .PostAsJsonAsync("v1/properties", request);

        var data = await result.Content.ReadFromJsonAsync<Response<Property?>>();

        if (!result.IsSuccessStatusCode)
            return new Response<Property?>(null, 400, data?.Message);

        return data ?? new Response<Property?>(null, 400,
            "Falha ao criar o imóvel");
    }

    public async Task<Response<Property?>> UpdateAsync(Property request)
    {
        var result = await _httpClient
            .PutAsJsonAsync($"v1/properties/{request.Id}", request);

        return await result.Content.ReadFromJsonAsync<Response<Property?>>()
            ?? new Response<Property?>(null, 400,
                "Falha ao atualizar o imóvel");
    }

    public async Task<Response<Property?>> DeleteAsync(DeletePropertyRequest request)
    {
        var result = await _httpClient
            .DeleteAsync($"v1/properties/{request.Id}");

        return await result.Content.ReadFromJsonAsync<Response<Property?>>()
               ?? new Response<Property?>(null, 400,
                   "Falha ao excluir o imóvel");
    }

    public async Task<Response<Property?>> GetByIdAsync(GetPropertyByIdRequest request)
    {
        var response = await _httpClient.GetAsync($"v1/properties/{request.Id}");

        if (!response.IsSuccessStatusCode)
            return new Response<Property?>(null, 400,
                "Não foi possível obter o imóvel");

        var property = await response.Content.ReadFromJsonAsync<Response<Property?>>();

        return property is null
            ? new Response<Property?>(null, 400, "Não foi possível obter o imóvel")
            : new Response<Property?>(property.Data);
    }

    public async Task<PagedResponse<List<Property>?>> GetAllAsync(GetAllPropertiesRequest request)
    {
        var url = $"v1/properties?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        if (!string.IsNullOrEmpty(request.SearchTerm))
            url = $"{url}&searchTerm={request.SearchTerm}";

        return await _httpClient.GetFromJsonAsync<PagedResponse<List<Property>?>>(url)
               ?? new PagedResponse<List<Property>?>(null, 400,
                   "Não foi possível obter os imóveis");
    }

    public async Task<Response<List<Viewing>?>> GetAllViewingsAsync(GetAllViewingsByPropertyRequest request)
    {
        var response = await _httpClient
            .GetAsync($"v1/properties/{request.PropertyId}/viewings");

        if (!response.IsSuccessStatusCode)
            return new Response<List<Viewing>?>(null, 400,
                "Não foi possível obter as visitas");

        var viewings = await response.Content.ReadFromJsonAsync<Response<List<Viewing>?>>();
        return viewings is null
            ? new Response<List<Viewing>?>(null, 400,
                "Não foi possível obter as visitas")
            : new Response<List<Viewing>?>(viewings.Data);
    }
}