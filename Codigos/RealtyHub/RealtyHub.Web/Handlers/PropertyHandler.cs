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
        var result = await _httpClient.PostAsJsonAsync("v1/properties", request);

        return await result.Content.ReadFromJsonAsync<Response<Property?>>() 
               ?? new Response<Property?>(null, 400, "Falha ao criar o imóvel");
    }

    public async Task<Response<Property?>> UpdateAsync(Property request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/properties/{request.Id}", request);

        return await result.Content.ReadFromJsonAsync<Response<Property?>>()
            ?? new Response<Property?>(null, 400, "Falha ao atualizar o imóvel");
    }

    public async Task<Response<Property?>> DeleteAsync(DeletePropertyRequest request)
    {
        var result = await _httpClient.DeleteAsync($"v1/properties/{request.Id}");

        return result.IsSuccessStatusCode
            ? new Response<Property?>(null, 200, "Imóvel excluído com sucesso")
            : new Response<Property?>(null, 400, "Falha ao excluir o imóvel");
    }

    public async Task<Response<Property?>> GetByIdAsync(GetPropertyByIdRequest request)
    {
        var response = await _httpClient.GetAsync($"v1/properties/{request.Id}");

        return await response.Content.ReadFromJsonAsync<Response<Property?>>()
            ?? new Response<Property?>(null, 400, "Falha ao obter o imóvel");
    }

    public async Task<PagedResponse<List<Property>?>> GetAllAsync(GetAllPropertiesRequest request)
    {
        var url = $"v1/properties?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        if (!string.IsNullOrEmpty(request.FilterBy))
            url = $"{url}&filterBy={request.FilterBy}";

        if (!string.IsNullOrEmpty(request.SearchTerm))
            url = $"{url}&searchTerm={request.SearchTerm}";

        return await _httpClient.GetFromJsonAsync<PagedResponse<List<Property>?>>(url)
               ?? new PagedResponse<List<Property>?>(null, 400, "Falha ao obter os imóveis");
    }

    public async Task<PagedResponse<List<Viewing>?>> GetAllViewingsAsync(
        GetAllViewingsByPropertyRequest request)
    {
        var url = $"v1/properties/{request.PropertyId}/viewings?" +
                  $"pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        if (!string.IsNullOrEmpty(request.SearchTerm))
            url = $"{url}&searchTerm={request.SearchTerm}";

        if (!string.IsNullOrEmpty(request.FilterBy))
            url = $"{url}&filterBy={request.FilterBy}";

        if (request.StartDate is not null & request.EndDate is not null)
            url = $"{url}&startDate={request.StartDate}&endDate={request.EndDate}";

        var response = await _httpClient.GetAsync(url);

        return await response.Content.ReadFromJsonAsync<PagedResponse<List<Viewing>?>>()
               ?? new PagedResponse<List<Viewing>?>(null, 400, "Falha ao obter as visitas");
    }
}