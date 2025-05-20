using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Core.Responses;
using System.Net.Http.Json;

namespace RealtyHub.Web.Handlers;

/// <summary>
/// Handler responsável por gerenciar as operações relacionadas a visitas na aplicação web.
/// </summary>
public class ViewingHandler : IViewingHandler
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ViewingHandler"/> utilizando a fábrica de HttpClient.
    /// </summary>
    /// <param name="httpClientFactory">Fábrica de <see cref="IHttpClientFactory"/> para criar o cliente HTTP.</param>
    public ViewingHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory
            .CreateClient(Configuration.HttpClientName);
    }

    /// <summary>
    /// Agenda uma nova visita.
    /// </summary>
    /// <remarks>
    /// Envia uma requisição POST para a API para criar uma nova visita com os dados informados.
    /// Retorna a resposta contendo os detalhes da visita ou uma mensagem de erro em caso de falha.
    /// </remarks>
    public async Task<Response<Viewing?>> ScheduleAsync(Viewing request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/viewings", request);
        return await result.Content.ReadFromJsonAsync<Response<Viewing?>>()
               ?? new Response<Viewing?>(null, 400, "Falha ao agendar a visita");
    }

    /// <summary>
    /// Reagenda uma visita existente.
    /// </summary>
    /// <remarks>
    /// Envia uma requisição PUT para a API para atualizar os dados da visita e reagendar.
    /// Retorna os detalhes da visita reagendada ou uma mensagem de erro em caso de falha.
    /// </remarks>
    public async Task<Response<Viewing?>> RescheduleAsync(Viewing request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/viewings/{request.Id}/reschedule", request);
        return await result.Content.ReadFromJsonAsync<Response<Viewing?>>()
               ?? new Response<Viewing?>(null, 400, "Falha ao reagendar a visita");
    }

    /// <summary>
    /// Marca uma visita como finalizada.
    /// </summary>
    /// <remarks>
    /// Envia uma requisição PUT para a API para atualizar o status da visita como finalizada.
    /// Retorna os detalhes da visita finalizada ou uma mensagem de erro em caso de falha.
    /// </remarks>
    public async Task<Response<Viewing?>> DoneAsync(DoneViewingRequest request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/viewings/{request.Id}/done", request);
        return await result.Content.ReadFromJsonAsync<Response<Viewing?>>()
               ?? new Response<Viewing?>(null, 400, "Falha ao finalizar a visita");
    }

    /// <summary>
    /// Cancela uma visita.
    /// </summary>
    /// <remarks>
    /// Envia uma requisição PUT para a API para atualizar o status da visita como cancelada.
    /// Retorna os detalhes da visita cancelada ou uma mensagem de erro em caso de falha.
    /// </remarks>
    public async Task<Response<Viewing?>> CancelAsync(CancelViewingRequest request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/viewings/{request.Id}/cancel", request);
        return await result.Content.ReadFromJsonAsync<Response<Viewing?>>()
               ?? new Response<Viewing?>(null, 400, "Falha ao cancelar a visita");
    }

    /// <summary>
    /// Obtém os detalhes de uma visita pelo ID.
    /// </summary>
    /// <remarks>
    /// Realiza uma requisição GET para a API utilizando o ID da visita e retorna os detalhes correspondentes.
    /// Se a operação falhar, retorna uma resposta de erro.
    /// </remarks>
    public async Task<Response<Viewing?>> GetByIdAsync(GetViewingByIdRequest request)
    {
        var response = await _httpClient.GetAsync($"v1/viewings/{request.Id}");
        return await response.Content.ReadFromJsonAsync<Response<Viewing?>>()
            ?? new Response<Viewing?>(null, 400, "Falha ao obter a visita");
    }

    /// <summary>
    /// Obtém uma lista paginada de visitas.
    /// </summary>
    /// <remarks>
    /// Constrói a URL com parâmetros de paginação, termo de busca e, opcionalmente, intervalo de datas.
    /// Realiza uma requisição GET para a API e retorna a lista de visitas ou uma mensagem de erro em caso de falha.
    /// </remarks>
    public async Task<PagedResponse<List<Viewing>?>> GetAllAsync(GetAllViewingsRequest request)
    {
        var url = $"v1/viewings?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        if (!string.IsNullOrEmpty(request.SearchTerm))
            url = $"{url}&searchTerm={request.SearchTerm}";

        if (request.StartDate is not null & request.EndDate is not null)
            url = $"{url}&startDate={request.StartDate}&endDate={request.EndDate}";

        var response = await _httpClient.GetAsync(url);
        return await response.Content.ReadFromJsonAsync<PagedResponse<List<Viewing>?>>()
               ?? new PagedResponse<List<Viewing>?>(null, 400, "Não foi possível obter as visitas");
    }
}