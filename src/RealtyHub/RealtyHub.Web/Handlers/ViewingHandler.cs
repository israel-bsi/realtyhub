using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Core.Responses;
using System.Net.Http.Json;

namespace RealtyHub.Web.Handlers;

public class ViewingHandler(IHttpClientFactory httpClientFactory) : IViewingHandler
{
    private readonly HttpClient _httpClient = httpClientFactory
       .CreateClient(Configuration.HttpClientName);
    public async Task<Response<Viewing?>> ScheduleAsync(Viewing request)
    {
        var result = await _httpClient
            .PostAsJsonAsync("v1/viewings", request);

        var data = await result.Content.ReadFromJsonAsync<Response<Viewing?>>();

        if (!result.IsSuccessStatusCode)
            return new Response<Viewing?>(null, 400, data?.Message);

        return data ?? new Response<Viewing?>(null, 400,
            "Falha ao agendar a visita");
    }

    public async Task<Response<Viewing?>> RescheduleAsync(Viewing request)
    {
        var result = await _httpClient
            .PutAsJsonAsync($"v1/viewings/{request.Id}/reschedule", request);

        return await result.Content.ReadFromJsonAsync<Response<Viewing?>>()
            ?? new Response<Viewing?>(null, 400,
                "Falha ao reagendar a visita");
    }

    public async Task<Response<Viewing?>> DoneAsync(DoneViewingRequest request)
    {
        var result = await _httpClient
            .PutAsJsonAsync($"v1/viewings/{request.Id}/done", request);

        return await result.Content.ReadFromJsonAsync<Response<Viewing?>>()
            ?? new Response<Viewing?>(null, 400,
                "Falha ao concluir a visita");
    }

    public async Task<Response<Viewing?>> CancelAsync(CancelViewingRequest request)
    {
        var result = await _httpClient
            .PutAsJsonAsync($"v1/viewings/{request.Id}/cancel", request);

        return await result.Content.ReadFromJsonAsync<Response<Viewing?>>()
               ?? new Response<Viewing?>(null, 400,
                   "Falha ao cancelar a visita");
    }

    public async Task<Response<Viewing?>> GetByIdAsync(GetViewingByIdRequest request)
    {
        var response = await _httpClient.GetAsync($"v1/viewings/{request.Id}");

        if (!response.IsSuccessStatusCode)
            return new Response<Viewing?>(null, 400,
               "Não foi possível obter a visita");

        var viewing = await response.Content.ReadFromJsonAsync<Response<Viewing?>>();

        return viewing is null
            ? new Response<Viewing?>(null, 400, "Não foi possível obter a visita")
            : new Response<Viewing?>(viewing.Data);
    }

    public async Task<PagedResponse<List<Viewing>?>> GetAllAsync(GetAllViewingsRequest request)
    {
        var url = $"v1/viewings?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        if (!string.IsNullOrEmpty(request.SearchTerm))
            url = $"{url}&searchTerm={request.SearchTerm}";

        return await _httpClient.GetFromJsonAsync<PagedResponse<List<Viewing>?>>(url)
               ?? new PagedResponse<List<Viewing>?>(null, 400,
                   "Não foi possível obter as visitas");
    }
}