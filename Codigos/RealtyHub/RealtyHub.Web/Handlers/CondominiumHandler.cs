using System.Net.Http.Json;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Condominiums;
using RealtyHub.Core.Responses;

namespace RealtyHub.Web.Handlers;

public class CondominiumHandler(IHttpClientFactory httpClientFactory) : ICondominiumHandler
{
    private readonly HttpClient _httpClient = httpClientFactory
        .CreateClient(Configuration.HttpClientName);
    public async Task<Response<Condominium?>> CreateAsync(Condominium request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/condominiums", request);

        return await result.Content.ReadFromJsonAsync<Response<Condominium?>>()
               ?? new Response<Condominium?>(null, 400, "Falha ao criar o condomínio");
    }

    public async Task<Response<Condominium?>> UpdateAsync(Condominium request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/condominiums/{request.Id}", request);

        return await result.Content.ReadFromJsonAsync<Response<Condominium?>>()
            ?? new Response<Condominium?>(null, 400, "Falha ao atualizar o condomínio");
    }

    public async Task<Response<Condominium?>> DeleteAsync(DeleteCondominiumRequest request)
    {
        var result = await _httpClient.DeleteAsync($"v1/condominiums/{request.Id}");

        return result.IsSuccessStatusCode
            ? new Response<Condominium?>(null, 200, "Condomínio excluído com sucesso")
            : new Response<Condominium?>(null, 400, "Falha ao excluir o condomínio");
    }

    public async Task<Response<Condominium?>> GetByIdAsync(GetCondominiumByIdRequest request)
    {
        var response = await _httpClient.GetAsync($"v1/condominiums/{request.Id}");

        return await response.Content.ReadFromJsonAsync<Response<Condominium?>>()
            ?? new Response<Condominium?>(null, 400, "Falha ao obter o condomínio");
    }

    public async Task<PagedResponse<List<Condominium>?>> GetAllAsync(GetAllCondominiumsRequest request)
    {
        var url = $"v1/condominiums?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        if (!string.IsNullOrEmpty(request.FilterBy))
            url = $"{url}&filterBy={request.FilterBy}";

        if (!string.IsNullOrEmpty(request.SearchTerm))
            url = $"{url}&searchTerm={request.SearchTerm}";

        return await _httpClient.GetFromJsonAsync<PagedResponse<List<Condominium>?>>(url)
               ?? new PagedResponse<List<Condominium>?>(null, 400, "Falha ao obter os condomínios");
    }
}