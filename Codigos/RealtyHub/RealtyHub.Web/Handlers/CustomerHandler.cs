using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Core.Responses;
using System.Net.Http.Json;

namespace RealtyHub.Web.Handlers;

public class CustomerHandler : ICustomerHandler
{
    private readonly HttpClient _httpClient;

    public CustomerHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory
            .CreateClient(Configuration.HttpClientName);
    }

    public async Task<Response<Customer?>> CreateAsync(Customer request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/customers", request);

        return await result.Content.ReadFromJsonAsync<Response<Customer?>>() 
               ?? new Response<Customer?>(null, 400, "Falha ao criar o cliente");
    }

    public async Task<Response<Customer?>> UpdateAsync(Customer request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/customers/{request.Id}", request);

        return await result.Content.ReadFromJsonAsync<Response<Customer?>>()
            ?? new Response<Customer?>(null, 400, "Falha ao atualizar o cliente");
    }

    public async Task<Response<Customer?>> DeleteAsync(DeleteCustomerRequest request)
    {
        var result = await _httpClient.DeleteAsync($"v1/customers/{request.Id}");

        return result.IsSuccessStatusCode
            ? new Response<Customer?>(null, 200, "Cliente excluído com sucesso")
            : new Response<Customer?>(null, 400, "Falha ao excluir o cliente");
    }

    public async Task<Response<Customer?>> GetByIdAsync(GetCustomerByIdRequest request)
    {
        var response = await _httpClient.GetAsync($"v1/customers/{request.Id}");

        return await response.Content.ReadFromJsonAsync<Response<Customer?>>() 
               ?? new Response<Customer?>(null, 400, "Falha ao obter o cliente");
    }

    public async Task<PagedResponse<List<Customer>?>> GetAllAsync(GetAllCustomersRequest request)
    {
        var url = $"v1/customers?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        if (!string.IsNullOrEmpty(request.SearchTerm))
            url = $"{url}&searchTerm={request.SearchTerm}";

        return await _httpClient.GetFromJsonAsync<PagedResponse<List<Customer>?>>(url)
               ?? new PagedResponse<List<Customer>?>(null, 400, "Falha ao obter os clientes");
    }
}