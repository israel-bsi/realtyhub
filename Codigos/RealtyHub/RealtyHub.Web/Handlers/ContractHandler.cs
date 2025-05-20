using System.Net.Http.Json;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Responses;

namespace RealtyHub.Web.Handlers;

/// <summary>
/// Handler responsável pelas operações de contratos na aplicação web.
/// </summary>
public class ContractHandler : IContractHandler
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ContractHandler"/> utilizando a fábrica de HttpClient.
    /// </summary>
    /// <param name="httpClientFactory">Fábrica de <see cref="IHttpClientFactory"/> para criar o cliente HTTP.</param>
    public ContractHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory
            .CreateClient(Configuration.HttpClientName);
    }

    /// <summary>
    /// Cria um novo contrato.
    /// </summary>
    /// <remarks>
    /// Ajusta as datas do contrato para UTC, envia os dados para a API e retorna o resultado da operação.
    /// </remarks>
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

    /// <summary>
    /// Atualiza os dados de um contrato existente.
    /// </summary>
    /// <remarks>
    /// Envia os dados atualizados do contrato para a API e retorna o resultado da operação.
    /// </remarks>
    public async Task<Response<Contract?>> UpdateAsync(Contract request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/contracts/{request.Id}", request);

        return await result.Content.ReadFromJsonAsync<Response<Contract?>>()
            ?? new Response<Contract?>(null, 400, "Falha ao atualizar o contrato");
    }

    /// <summary>
    /// Exclui um contrato pelo ID.
    /// </summary>
    /// <remarks>
    /// Envia uma requisição para a API para remover o contrato especificado.
    /// </remarks>
    public async Task<Response<Contract?>> DeleteAsync(DeleteContractRequest request)
    {
        var result = await _httpClient.DeleteAsync($"v1/contracts/{request.Id}");

        return result.IsSuccessStatusCode
            ? new Response<Contract?>(null, 200, "Contrato excluído com sucesso")
            : new Response<Contract?>(null, 400, "Falha ao excluir o contrato");
    }

    /// <summary>
    /// Obtém os dados de um contrato pelo ID.
    /// </summary>
    /// <remarks>
    /// Realiza uma requisição para a API para buscar os dados do contrato especificado.
    /// </remarks>
    public async Task<Response<Contract?>> GetByIdAsync(GetContractByIdRequest request)
    {
        var result = await _httpClient.GetAsync($"v1/contracts/{request.Id}");

        return await result.Content.ReadFromJsonAsync<Response<Contract?>>()
               ?? new Response<Contract?>(null, 400, "Não foi possível obter o contrato");
    }

    /// <summary>
    /// Obtém uma lista paginada de contratos.
    /// </summary>
    /// <remarks>
    /// Realiza uma requisição para a API para buscar todos os contratos, com suporte a paginação.
    /// </remarks>
    public async Task<PagedResponse<List<Contract>?>> GetAllAsync(GetAllContractsRequest request)
    {
        var url = $"v1/contracts?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        return await _httpClient.GetFromJsonAsync<PagedResponse<List<Contract>?>>(url)
               ?? new PagedResponse<List<Contract>?>(null, 400,
                   "Não foi possível obter os contratos");
    }
}