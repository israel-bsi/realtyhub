using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Responses;
using System.Net.Http.Json;

namespace RealtyHub.Web.Handlers;

public class OfferHandler(IHttpClientFactory httpClientFactory) : IOfferHandler
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(Configuration.HttpClientName);
    public async Task<Response<Offer?>> CreateAsync(Offer request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/offers", request);

        var data = await result.Content.ReadFromJsonAsync<Response<Offer?>>();

        if (!result.IsSuccessStatusCode)
            return new Response<Offer?>(null, 400, data?.Message);

        return data ?? new Response<Offer?>(null, 400, "Falha ao criar a proposta");
    }

    public async Task<Response<Offer?>> UpdateAsync(Offer request)
    {
        var url = $"v1/offers/{request.Id}";
        var result = await _httpClient.PutAsJsonAsync(url, request);

        return await result.Content.ReadFromJsonAsync<Response<Offer?>>()
               ?? new Response<Offer?>(null, 400, "Falha ao atualizar a proposta");
    }

    public async Task<Response<Offer?>> RejectAsync(RejectOfferRequest request)
    {
        var url = $"v1/offers/{request.Id}/reject";
        var result = await _httpClient.PutAsJsonAsync(url, request);

        return await result.Content.ReadFromJsonAsync<Response<Offer?>>()
               ?? new Response<Offer?>(null, 400, "Falha ao rejeitar a proposta");
    }

    public async Task<Response<Offer?>> AcceptAsync(AcceptOfferRequest request)
    {
        var url = $"v1/offers/{request.Id}/accept";
        var result = await _httpClient.PutAsJsonAsync(url, request);

        return await result.Content.ReadFromJsonAsync<Response<Offer?>>()
               ?? new Response<Offer?>(null, 400, "Falha ao aceitar a proposta");
    }

    public async Task<Response<Offer?>> GetByIdAsync(GetOfferByIdRequest request)
    {
        var url = $"v1/offers/{request.Id}";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return new Response<Offer?>(null, 400, "Não foi possível obter a proposta");

        var result = await response.Content.ReadFromJsonAsync<Response<Offer?>>();

        return result is null
            ? new Response<Offer?>(null, 400, "Não foi possível obter a proposta")
            : new Response<Offer?>(result.Data);
    }

    public async Task<Response<Offer?>> GetAcceptedByProperty(GetOfferAcceptedByProperty request)
    {
        var url = $"v1/offers/property/{request.PropertyId}/accepted";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return new Response<Offer?>(null, 400, "Não foi possível obter a proposta aceita");

        var result = await response.Content.ReadFromJsonAsync<Response<Offer?>>();

        return result is null
            ? new Response<Offer?>(null, 400, "Não foi possível obter a proposta aceita")
            : new Response<Offer?>(result.Data);
    }

    public async Task<PagedResponse<List<Offer>?>> GetAllOffersByPropertyAsync(
        GetAllOffersByPropertyRequest request)
    {
        var url = $"v1/offers/property/{request.PropertyId}" +
                  $"?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        if (request.StartDate is not null & request.EndDate is not null)
            url = $"{url}&startDate={request.StartDate}&endDate={request.EndDate}";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return new PagedResponse<List<Offer>?>(null, 400, "Não foi possível obter as propostas");

        return await response.Content.ReadFromJsonAsync<PagedResponse<List<Offer>?>>()
               ?? new PagedResponse<List<Offer>?>(null, 400, "Não foi possível obter as propostas");
    }

    public async Task<PagedResponse<List<Offer>?>> GetAllOffersByCustomerAsync(
        GetAllOffersByCustomerRequest request)
    {
        var url = $"v1/offers/customer/{request.CustomerId}?" +
                  $"pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        if (request.StartDate is not null & request.EndDate is not null)
            url = $"{url}&startDate={request.StartDate}&endDate={request.EndDate}";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return new PagedResponse<List<Offer>?>(null, 400, "Não foi possível obter as propostas");

        return await response.Content.ReadFromJsonAsync<PagedResponse<List<Offer>?>>()
               ?? new PagedResponse<List<Offer>?>(null, 400, "Não foi possível obter as propostas");
    }

    public async Task<PagedResponse<List<Offer>?>> GetAllAsync(GetAllOffersRequest request)
    {
        var url = $"v1/offers?pageNumber={request.PageNumber}&pageSize={request.PageSize}";
        if (request.StartDate is not null & request.EndDate is not null)
            url = $"{url}&startDate={request.StartDate}&endDate={request.EndDate}";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return new PagedResponse<List<Offer>?>(null, 400, "Não foi possível obter as propostas");

        return await response.Content.ReadFromJsonAsync<PagedResponse<List<Offer>?>>()
               ?? new PagedResponse<List<Offer>?>(null, 400, "Não foi possível obter as propostas");
    }
}