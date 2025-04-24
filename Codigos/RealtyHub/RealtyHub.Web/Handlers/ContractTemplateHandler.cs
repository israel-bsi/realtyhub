using System.Net.Http.Json;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.Web.Handlers;

public class ContractTemplateHandler : 
    IContractTemplateHandler
{
    private readonly HttpClient _httpClient;

    public ContractTemplateHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory
            .CreateClient(Configuration.HttpClientName);
    }

    public async Task<Response<List<ContractTemplate>?>> GetAllAsync()
    {
        var response = await _httpClient.GetAsync("v1/contracts-templates");

        return await response.Content.ReadFromJsonAsync<Response<List<ContractTemplate>?>>()
            ?? new Response<List<ContractTemplate>?>(null, 400, "Falha ao buscar os contratos");
    }
}