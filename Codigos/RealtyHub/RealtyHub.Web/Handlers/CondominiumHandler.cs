using System.Net.Http.Json;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Condominiums;
using RealtyHub.Core.Responses;

namespace RealtyHub.Web.Handlers;

/// <summary>
/// Handler responsável pelas operações de condomínio na aplicação web.
/// </summary>
public class CondominiumHandler : ICondominiumHandler
{
    /// <summary>
    /// Cliente HTTP utilizado para realizar requisições à API.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="CondominiumHandler"/> utilizando a fábrica de HttpClient.
    /// </summary>
    /// <param name="httpClientFactory">Fábrica de <see cref="IHttpClientFactory"/> para criar o cliente HTTP.</param>
    public CondominiumHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory
            .CreateClient(Configuration.HttpClientName);
    }

    /// <summary>
    /// Cria um novo condomínio.
    /// </summary>
    /// <remarks>
    /// Envia os dados do condomínio para a API e retorna o resultado da operação.
    /// </remarks>
    public async Task<Response<Condominium?>> CreateAsync(Condominium request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/condominiums", request);

        return await result.Content.ReadFromJsonAsync<Response<Condominium?>>()
               ?? new Response<Condominium?>(null, 400, "Falha ao criar o condomínio");
    }

    /// <summary>
    /// Atualiza os dados de um condomínio existente.
    /// </summary>
    /// <remarks>
    /// Envia os dados atualizados do condomínio para a API e retorna o resultado da operação.
    /// </remarks>
    public async Task<Response<Condominium?>> UpdateAsync(Condominium request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/condominiums/{request.Id}", request);

        return await result.Content.ReadFromJsonAsync<Response<Condominium?>>()
            ?? new Response<Condominium?>(null, 400, "Falha ao atualizar o condomínio");
    }

    /// <summary>
    /// Exclui um condomínio pelo ID.
    /// </summary>
    /// <remarks>
    /// Envia uma requisição para a API para remover o condomínio especificado.
    /// </remarks>
    public async Task<Response<Condominium?>> DeleteAsync(DeleteCondominiumRequest request)
    {
        var result = await _httpClient.DeleteAsync($"v1/condominiums/{request.Id}");

        return result.IsSuccessStatusCode
            ? new Response<Condominium?>(null, 200, "Condomínio excluído com sucesso")
            : new Response<Condominium?>(null, 400, "Falha ao excluir o condomínio");
    }

    /// <summary>
    /// Obtém os dados de um condomínio pelo ID.
    /// </summary>
    /// <remarks>
    /// Realiza uma requisição para a API para buscar os dados do condomínio especificado.
    /// </remarks>
    public async Task<Response<Condominium?>> GetByIdAsync(GetCondominiumByIdRequest request)
    {
        var response = await _httpClient.GetAsync($"v1/condominiums/{request.Id}");

        return await response.Content.ReadFromJsonAsync<Response<Condominium?>>()
            ?? new Response<Condominium?>(null, 400, "Falha ao obter o condomínio");
    }

    /// <summary>
    /// Obtém uma lista paginada de condomínios, com suporte a filtros e busca.
    /// </summary>
    /// <remarks>
    /// Realiza uma requisição para a API para buscar todos os condomínios, podendo filtrar por termo de busca e outros critérios.
    /// </remarks>
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