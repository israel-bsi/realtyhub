using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Core.Responses;
using System.Net.Http.Json;

namespace RealtyHub.Web.Handlers;

/// <summary>
/// Handler responsável pelas operações de clientes na aplicação web.
/// </summary>
public class CustomerHandler : ICustomerHandler
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="CustomerHandler"/> utilizando a fábrica de HttpClient.
    /// </summary>
    /// <param name="httpClientFactory">Fábrica de <see cref="IHttpClientFactory"/> para criar o cliente HTTP.</param>
    public CustomerHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory
            .CreateClient(Configuration.HttpClientName);
    }

    /// <summary>
    /// Cria um novo cliente.
    /// </summary>
    /// <remarks>
    /// Envia os dados do cliente para a API e retorna o resultado da operação.
    /// </remarks>
    public async Task<Response<Customer?>> CreateAsync(Customer request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/customers", request);

        return await result.Content.ReadFromJsonAsync<Response<Customer?>>()
               ?? new Response<Customer?>(null, 400, "Falha ao criar o cliente");
    }

    /// <summary>
    /// Atualiza os dados de um cliente existente.
    /// </summary>
    /// <remarks>
    /// Envia os dados atualizados do cliente para a API e retorna o resultado da operação.
    /// </remarks>
    public async Task<Response<Customer?>> UpdateAsync(Customer request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/customers/{request.Id}", request);

        return await result.Content.ReadFromJsonAsync<Response<Customer?>>()
            ?? new Response<Customer?>(null, 400, "Falha ao atualizar o cliente");
    }

    /// <summary>
    /// Exclui um cliente pelo ID.
    /// </summary>
    /// <remarks>
    /// Envia uma requisição para a API para remover o cliente especificado.
    /// </remarks>
    public async Task<Response<Customer?>> DeleteAsync(DeleteCustomerRequest request)
    {
        var result = await _httpClient.DeleteAsync($"v1/customers/{request.Id}");

        return result.IsSuccessStatusCode
            ? new Response<Customer?>(null, 200, "Cliente excluído com sucesso")
            : new Response<Customer?>(null, 400, "Falha ao excluir o cliente");
    }

    /// <summary>
    /// Obtém os dados de um cliente pelo ID.
    /// </summary>
    /// <remarks>
    /// Realiza uma requisição para a API para buscar os dados do cliente especificado.
    /// </remarks>
    public async Task<Response<Customer?>> GetByIdAsync(GetCustomerByIdRequest request)
    {
        var response = await _httpClient.GetAsync($"v1/customers/{request.Id}");

        return await response.Content.ReadFromJsonAsync<Response<Customer?>>()
               ?? new Response<Customer?>(null, 400, "Falha ao obter o cliente");
    }

    /// <summary>
    /// Obtém uma lista paginada de clientes, com suporte a busca por termo.
    /// </summary>
    /// <remarks>
    /// Realiza uma requisição para a API para buscar todos os clientes, podendo filtrar por termo de busca.
    /// </remarks>
    public async Task<PagedResponse<List<Customer>?>> GetAllAsync(GetAllCustomersRequest request)
    {
        var url = $"v1/customers?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        if (!string.IsNullOrEmpty(request.SearchTerm))
            url = $"{url}&searchTerm={request.SearchTerm}";

        return await _httpClient.GetFromJsonAsync<PagedResponse<List<Customer>?>>(url)
               ?? new PagedResponse<List<Customer>?>(null, 400, "Falha ao obter os clientes");
    }
}