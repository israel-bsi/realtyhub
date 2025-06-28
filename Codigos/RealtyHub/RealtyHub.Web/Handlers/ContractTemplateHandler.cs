using System.Net.Http.Json;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.Web.Handlers;

/// <summary>
/// Handler responsável pelas operações de modelos de contratos na aplicação web.
/// </summary>
public class ContractTemplateHandler : IContractTemplateHandler
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ContractTemplateHandler"/> utilizando a fábrica de HttpClient.
    /// </summary>
    /// <param name="httpClientFactory">Fábrica de <see cref="IHttpClientFactory"/> para criar o cliente HTTP.</param>
    public ContractTemplateHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory
            .CreateClient(Configuration.HttpClientName);
    }

    /// <summary>
    /// Obtém todos os modelos de contratos disponíveis.
    /// </summary>
    /// <remarks>
    /// Realiza uma requisição para a API para buscar a lista de modelos de contratos.
    /// Retorna a lista ou uma mensagem de erro em caso de falha.
    /// </remarks>
    public async Task<Response<List<ModeloContrato>?>> GetAllAsync()
    {
        var response = await _httpClient.GetAsync("v1/contracts-templates");

        return await response.Content.ReadFromJsonAsync<Response<List<ModeloContrato>?>>()
            ?? new Response<List<ModeloContrato>?>(null, 400, "Falha ao buscar os contratos");
    }
}