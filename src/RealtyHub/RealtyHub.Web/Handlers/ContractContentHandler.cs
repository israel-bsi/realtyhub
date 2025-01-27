using System.Net.Http.Headers;
using System.Net.Http.Json;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.ContractsContent;
using RealtyHub.Core.Responses;

namespace RealtyHub.Web.Handlers;

public class ContractContentHandler(IHttpClientFactory httpClientFactory) : 
    IContractContentHandler
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(Configuration.HttpClientName);
    public async Task<Response<ContractContent?>> CreateAsync(
        CreateContractContentRequest request)
    {
        if (request.FileBytes is null || request.FileBytes.Count == 0)
            return new Response<ContractContent?>(null, 400, "Nenhum arquivo encontrado");

        using var content = new MultipartFormDataContent();
        foreach (var fileData in request.FileBytes)
        {
            var fileContent = new ByteArrayContent(fileData.Content);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(fileData.ContentType);

            fileContent.Headers.Add("Id", $"{fileData.Id}");

            content.Add(fileContent, "contracts", fileData.Name);
        }

        const string url = "/v1/contracts-content";
        var response = await _httpClient.PostAsync(url, content);

        return await response.Content.ReadFromJsonAsync<Response<ContractContent?>>()
            ?? new Response<ContractContent?>(null, 400, "Falha ao adicionar os contratos");
    }

    public async Task<Response<ContractContent?>> UpdateAsync(
        UpdateContractContentRequest request)
    {
        if (request.FileBytes is null || request.FileBytes.Count == 0)
            return new Response<ContractContent?>(null, 400, "Nenhum arquivo encontrado");

        using var content = new MultipartFormDataContent();
        foreach (var fileData in request.FileBytes)
        {
            var fileContent = new ByteArrayContent(fileData.Content);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(fileData.ContentType);

            fileContent.Headers.Add("Id", $"{fileData.Id}");

            content.Add(fileContent, "contracts", fileData.Name);
        }

        var url = $"/v1/contracts-content/{request.Id}";
        var response = await _httpClient.PutAsync(url, content);

        return await response.Content.ReadFromJsonAsync<Response<ContractContent?>>()
            ?? new Response<ContractContent?>(null, 400, "Falha ao atualizar os contratos");
    }

    public async Task<Response<ContractContent?>> DeleteAsync(
        DeleteContractContentRequest request)
    {
        var url = $"/v1/contracts-content/{request.Id}";

        var response = await _httpClient.DeleteAsync(url);

        return await response.Content.ReadFromJsonAsync<Response<ContractContent?>>()
            ?? new Response<ContractContent?>(null, 400, "Falha ao deletar o contrato");
    }

    public async Task<Response<List<ContractContent>?>> GetAllByUserAsync(
        GetAllContractContentByUserRequest request)
    {
        var url = "/v1/contracts-content";

        var response = await _httpClient.GetAsync(url);

        return await response.Content.ReadFromJsonAsync<Response<List<ContractContent>?>>()
            ?? new Response<List<ContractContent>?>(null, 400, "Falha ao buscar os contratos");
    }
}