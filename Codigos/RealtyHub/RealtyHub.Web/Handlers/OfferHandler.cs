using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Responses;
using System.Net.Http.Json;

namespace RealtyHub.Web.Handlers;

/// <summary>
/// Handler responsável pelas operações relacionadas a propostas de compra de imóvel na aplicação web.
/// </summary>
public class OfferHandler : IOfferHandler
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="OfferHandler"/> utilizando a fábrica de HttpClient.
    /// </summary>
    /// <param name="httpClientFactory">Fábrica de <see cref="IHttpClientFactory"/> para criar o cliente HTTP.</param>
    public OfferHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory
            .CreateClient(Configuration.HttpClientName);
    }

    /// <summary>
    /// Cria uma nova proposta.
    /// </summary>
    /// <remarks>
    /// Envia os dados da proposta para a API e retorna o resultado da operação.
    /// </remarks>
    public async Task<Response<Proposta?>> CreateAsync(Proposta request)
    {
        var result = await _httpClient
            .PostAsJsonAsync("v1/offers", request);

        return await result.Content.ReadFromJsonAsync<Response<Proposta?>>()
               ?? new Response<Proposta?>(null, 400, "Falha ao criar a proposta");
    }

    /// <summary>
    /// Atualiza os dados de uma proposta existente.
    /// </summary>
    /// <remarks>
    /// Envia os dados atualizados da proposta para a API e retorna o resultado da operação.
    /// </remarks>
    public async Task<Response<Proposta?>> UpdateAsync(Proposta request)
    {
        var result = await _httpClient
            .PutAsJsonAsync($"v1/offers/{request.Id}", request);

        return await result.Content.ReadFromJsonAsync<Response<Proposta?>>()
               ?? new Response<Proposta?>(null, 400, "Falha ao atualizar a proposta");
    }

    /// <summary>
    /// Rejeita uma proposta.
    /// </summary>
    /// <remarks>
    /// Envia a requisição para rejeitar a proposta à API e retorna o resultado da operação.
    /// </remarks>
    public async Task<Response<Proposta?>> RejectAsync(RejectOfferRequest request)
    {
        var result = await _httpClient
            .PutAsJsonAsync($"v1/offers/{request.Id}/reject", request);

        return await result.Content.ReadFromJsonAsync<Response<Proposta?>>()
               ?? new Response<Proposta?>(null, 400, "Falha ao rejeitar a proposta");
    }

    /// <summary>
    /// Aceita uma proposta.
    /// </summary>
    /// <remarks>
    /// Envia a requisição para aceitar a proposta à API e retorna o resultado da operação.
    /// </remarks>
    public async Task<Response<Proposta?>> AcceptAsync(AcceptOfferRequest request)
    {
        var result = await _httpClient
            .PutAsJsonAsync($"v1/offers/{request.Id}/accept", request);

        return await result.Content.ReadFromJsonAsync<Response<Proposta?>>()
               ?? new Response<Proposta?>(null, 400, "Falha ao aceitar a proposta");
    }

    /// <summary>
    /// Obtém os dados de uma proposta pelo ID.
    /// </summary>
    /// <remarks>
    /// Realiza uma requisição GET à API para buscar os dados da proposta especificada.
    /// </remarks>
    public async Task<Response<Proposta?>> GetByIdAsync(GetOfferByIdRequest request)
    {
        var response = await _httpClient.GetAsync($"v1/offers/{request.Id}");

        return await response.Content.ReadFromJsonAsync<Response<Proposta?>>()
               ?? new Response<Proposta?>(null, 400, "Falha ao obter a proposta");
    }

    /// <summary>
    /// Obtém a proposta aceita para um determinado imóvel.
    /// </summary>
    /// <remarks>
    /// Realiza uma requisição GET à API para buscar a proposta aceita associada a um imóvel.
    /// </remarks>
    public async Task<Response<Proposta?>> GetAcceptedByProperty(GetOfferAcceptedByProperty request)
    {
        var url = $"v1/offers/property/{request.PropertyId}/accepted";
        var response = await _httpClient.GetAsync(url);

        return await response.Content.ReadFromJsonAsync<Response<Proposta?>>()
               ?? new Response<Proposta?>(null, 400, "Falha ao obter a proposta aceita");
    }

    /// <summary>
    /// Obtém uma lista paginada de propostas para um imóvel específico.
    /// </summary>
    /// <remarks>
    /// Envia uma requisição GET à API com parâmetros de paginação e, opcionalmente, intervalo de datas para filtrar as propostas.
    /// </remarks>
    public async Task<PagedResponse<List<Proposta>?>> GetAllOffersByPropertyAsync(
        GetAllOffersByPropertyRequest request)
    {
        var url = $"v1/offers/property/{request.PropertyId}" +
                  $"?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        if (request.StartDate is not null & request.EndDate is not null)
            url = $"{url}&startDate={request.StartDate}&endDate={request.EndDate}";

        var response = await _httpClient.GetAsync(url);

        return await response.Content.ReadFromJsonAsync<PagedResponse<List<Proposta>?>>()
               ?? new PagedResponse<List<Proposta>?>(null, 400, "Falha ao obter as propostas");
    }

    /// <summary>
    /// Obtém uma lista paginada de propostas associadas a um cliente.
    /// </summary>
    /// <remarks>
    /// Envia uma requisição GET à API com parâmetros de paginação e, opcionalmente, intervalo de datas para filtrar as propostas do cliente.
    /// </remarks>
    public async Task<PagedResponse<List<Proposta>?>> GetAllOffersByCustomerAsync(
        GetAllOffersByCustomerRequest request)
    {
        var url = $"v1/offers/customer/{request.CustomerId}?" +
                  $"pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        if (request.StartDate is not null & request.EndDate is not null)
            url = $"{url}&startDate={request.StartDate}&endDate={request.EndDate}";

        var response = await _httpClient.GetAsync(url);

        return await response.Content.ReadFromJsonAsync<PagedResponse<List<Proposta>?>>()
               ?? new PagedResponse<List<Proposta>?>(null, 400, "Falha ao obter as propostas");
    }

    /// <summary>
    /// Obtém uma lista paginada de todas as propostas.
    /// </summary>
    /// <remarks>
    /// Envia uma requisição GET à API para buscar todas as propostas com suporte a paginação e, opcionalmente, filtro por intervalo de datas.
    /// </remarks>
    public async Task<PagedResponse<List<Proposta>?>> GetAllAsync(GetAllOffersRequest request)
    {
        var url = $"v1/offers?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        if (request.StartDate is not null & request.EndDate is not null)
            url = $"{url}&startDate={request.StartDate}&endDate={request.EndDate}";

        var response = await _httpClient.GetAsync(url);

        return await response.Content.ReadFromJsonAsync<PagedResponse<List<Proposta>?>>()
               ?? new PagedResponse<List<Proposta>?>(null, 400, "Falha ao obter as propostas");
    }
}