using System.Net.Http.Json;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Responses;

namespace RealtyHub.Web.Handlers;

public class ContractHandler(IHttpClientFactory httpClientFactory) : IContractHandler
{
    private readonly HttpClient _httpClient = httpClientFactory
        .CreateClient(Configuration.HttpClientName);
    public async Task<Response<Contract?>> CreateAsync(Contract request)
    {
        request.IssueDate = request.IssueDate?.Date.ToUniversalTime();
        request.EffectiveDate = request.EffectiveDate?.Date.ToUniversalTime();
        request.TermEndDate = request.TermEndDate?.Date.ToUniversalTime();
        request.SignatureDate = request.SignatureDate?.Date.ToUniversalTime();

        var result = await _httpClient.PostAsJsonAsync("v1/contracts", request);

        return await result.Content.ReadFromJsonAsync<Response<Contract?>>() 
               ?? new Response<Contract?>(null, 400, "Falha ao criar o contrato");
    }

    public async Task<Response<Contract?>> UpdateAsync(Contract request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/contracts/{request.Id}", request);

        return await result.Content.ReadFromJsonAsync<Response<Contract?>>()
            ?? new Response<Contract?>(null, 400, "Falha ao atualizar o contrato");
    }

    public async Task<Response<Contract?>> DeleteAsync(DeleteContractRequest request)
    {
        var result = await _httpClient.DeleteAsync($"v1/contracts/{request.Id}");

        return await result.Content.ReadFromJsonAsync<Response<Contract?>>()
               ?? new Response<Contract?>(null, 400, "Falha ao excluir o contrato");
    }

    public async Task<Response<Contract?>> GetByIdAsync(GetContractByIdRequest request)
    {
        var result = await _httpClient.GetAsync($"v1/contracts/{request.Id}");

        return await result.Content.ReadFromJsonAsync<Response<Contract?>>() 
               ?? new Response<Contract?>(null, 400, "Não foi possível obter o contrato");
    }

    public async Task<PagedResponse<List<Contract>?>> GetAllAsync(GetAllContractsRequest request)
    {
        var url = $"v1/contracts?pageNumber={request.PageNumber}&pageSize={request.PageSize}";
        
        return await _httpClient.GetFromJsonAsync<PagedResponse<List<Contract>?>>(url)
               ?? new PagedResponse<List<Contract>?>(null, 400, 
                   "Não foi possível obter os contratos");
    }
}