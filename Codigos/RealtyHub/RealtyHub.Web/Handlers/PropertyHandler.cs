using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Core.Responses;
using System.Net.Http.Json;

namespace RealtyHub.Web.Handlers;

/// <summary>
/// Handler responsável por gerenciar as operações de imóveis na aplicação web.
/// </summary>
public class PropertyHandler : IPropertyHandler
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="PropertyHandler"/> utilizando a fábrica de HttpClient.
    /// </summary>
    /// <param name="httpClientFactory">Fábrica de <see cref="IHttpClientFactory"/> para criar o cliente HTTP.</param>
    public PropertyHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory
            .CreateClient(Configuration.HttpClientName);
    }

    /// <summary>
    /// Cria um novo imóvel.
    /// </summary>
    /// <remarks>
    /// Envia uma requisição POST para a API com os dados do imóvel e retorna o resultado da operação.
    /// Caso a resposta não possa ser convertida, retorna um objeto de resposta com erro.
    /// </remarks>
    public async Task<Response<Imovel?>> CreateAsync(Imovel request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/properties", request);

        return await result.Content.ReadFromJsonAsync<Response<Imovel?>>()
               ?? new Response<Imovel?>(null, 400, "Falha ao criar o imóvel");
    }

    /// <summary>
    /// Atualiza os dados de um imóvel existente.
    /// </summary>
    /// <remarks>
    /// Envia uma requisição PUT para a API com os dados atualizados do imóvel identificado por seu ID.
    /// Retorna o resultado da operação ou uma resposta de erro se a atualização falhar.
    /// </remarks>
    public async Task<Response<Imovel?>> UpdateAsync(Imovel request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/properties/{request.Id}", request);

        return await result.Content.ReadFromJsonAsync<Response<Imovel?>>()
            ?? new Response<Imovel?>(null, 400, "Falha ao atualizar o imóvel");
    }

    /// <summary>
    /// Exclui um imóvel pelo ID.
    /// </summary>
    /// <remarks>
    /// Envia uma requisição DELETE para a API utilizando o ID do imóvel a ser excluído
    /// e retorna o resultado da operação.
    /// </remarks>
    public async Task<Response<Imovel?>> DeleteAsync(DeletePropertyRequest request)
    {
        var result = await _httpClient.DeleteAsync($"v1/properties/{request.Id}");

        return result.IsSuccessStatusCode
            ? new Response<Imovel?>(null, 200, "Imóvel excluído com sucesso")
            : new Response<Imovel?>(null, 400, "Falha ao excluir o imóvel");
    }

    /// <summary>
    /// Obtém os dados de um imóvel pelo ID.
    /// </summary>
    /// <remarks>
    /// Realiza uma requisição GET para a API com o ID do imóvel e retorna os detalhes do imóvel.
    /// Se a operação falhar, retorna uma resposta de erro.
    /// </remarks>
    public async Task<Response<Imovel?>> GetByIdAsync(GetPropertyByIdRequest request)
    {
        var response = await _httpClient.GetAsync($"v1/properties/{request.Id}");

        return await response.Content.ReadFromJsonAsync<Response<Imovel?>>()
            ?? new Response<Imovel?>(null, 400, "Falha ao obter o imóvel");
    }

    /// <summary>
    /// Obtém uma lista paginada de imóveis.
    /// </summary>
    /// <remarks>
    /// Constrói a URL com parâmetros de paginação, filtro e termo de busca, e realiza uma requisição GET à API.
    /// Retorna um <see cref="PagedResponse{List{Property}?}"/> contendo os imóveis ou uma mensagem de erro em caso de falha.
    /// </remarks>
    public async Task<PagedResponse<List<Imovel>?>> GetAllAsync(GetAllPropertiesRequest request)
    {
        var url = $"v1/properties?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        if (!string.IsNullOrEmpty(request.FilterBy))
            url = $"{url}&filterBy={request.FilterBy}";

        if (!string.IsNullOrEmpty(request.SearchTerm))
            url = $"{url}&searchTerm={request.SearchTerm}";

        return await _httpClient.GetFromJsonAsync<PagedResponse<List<Imovel>?>>(url)
               ?? new PagedResponse<List<Imovel>?>(null, 400, "Falha ao obter os imóveis");
    }

    /// <summary>
    /// Obtém uma lista paginada de visitas associadas a um imóvel.
    /// </summary>
    /// <remarks>
    /// Constrói a URL com parâmetros de paginação, termo de busca, filtro e intervalo de datas,
    /// e realiza uma requisição GET à API para obter as visitas do imóvel especificado.
    /// Retorna um <see cref="PagedResponse{List{Viewing}?}"/> com as visitas ou uma mensagem de erro se a operação falhar.
    /// </remarks>
    public async Task<PagedResponse<List<Visita>?>> GetAllViewingsAsync(GetAllViewingsByPropertyRequest request)
    {
        var url = $"v1/properties/{request.PropertyId}/viewings?" +
                  $"pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        if (!string.IsNullOrEmpty(request.SearchTerm))
            url = $"{url}&searchTerm={request.SearchTerm}";

        if (!string.IsNullOrEmpty(request.FilterBy))
            url = $"{url}&filterBy={request.FilterBy}";

        if (request.StartDate is not null & request.EndDate is not null)
            url = $"{url}&startDate={request.StartDate}&endDate={request.EndDate}";

        var response = await _httpClient.GetAsync(url);

        return await response.Content.ReadFromJsonAsync<PagedResponse<List<Visita>?>>()
               ?? new PagedResponse<List<Visita>?>(null, 400, "Falha ao obter as visitas");
    }
}