using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesPhotos;
using RealtyHub.Core.Responses;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RealtyHub.Web.Handlers;

public class PropertyPhotosHandler(IHttpClientFactory httpClientFactory) : IPropertyPhotosHandler
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(Configuration.HttpClientName);
    public async Task<Response<PropertyPhoto?>> CreateAsync(CreatePropertyPhotosRequest request)
    {
        if (request.FileBytes is null || request.FileBytes.Count == 0)
            return new Response<PropertyPhoto?>(null, 400, "Nenhum arquivo encontrado");

        using var content = new MultipartFormDataContent();
        foreach (var fileData in request.FileBytes)
        {
            var fileContent = new ByteArrayContent(fileData.Content);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(fileData.ContentType);

            fileContent.Headers.Add("IsThumbnail", fileData.IsThumbnail.ToString());
            fileContent.Headers.Add("Id", $"{fileData.Id}");

            content.Add(fileContent, "photos", fileData.Name);
        }

        var url = $"/v1/properties/{request.PropertyId}/photos";
        var response = await _httpClient.PostAsync(url, content);

        var data = await response.Content.ReadFromJsonAsync<Response<PropertyPhoto?>>();

        if (!response.IsSuccessStatusCode)
            return new Response<PropertyPhoto?>(null, (int)response.StatusCode, data?.Message);

        return data ?? new Response<PropertyPhoto?>(null, 400, "Falha ao adicionar as fotos");
    }

    public async Task<Response<List<PropertyPhoto>?>> UpdateAsync(UpdatePorpertyPhotosRequest request)
    {
        var url = $"/v1/properties/{request.PropertyId}/photos";
        var result = await _httpClient.PutAsJsonAsync(url, request);

        if (!result.IsSuccessStatusCode)
            return new Response<List<PropertyPhoto>?>(null, 400, "Falha ao atualizar as fotos");

        var data = await result.Content.ReadFromJsonAsync<Response<List<PropertyPhoto>?>>();

        return data ?? new Response<List<PropertyPhoto>?>(data?.Data, 400, data?.Message);
    }

    public async Task<Response<PropertyPhoto?>> DeleteAsync(DeletePropertyPhotoRequest request)
    {
        var url = $"/v1/properties/{request.PropertyId}/photos/{request.Id}";

        var response = await _httpClient.DeleteAsync(url);

        var data = await response.Content.ReadFromJsonAsync<Response<PropertyPhoto?>>();
        if (!response.IsSuccessStatusCode)

            return new Response<PropertyPhoto?>(null, (int)response.StatusCode, data?.Message);
        return data ?? new Response<PropertyPhoto?>(null, 400, "Falha ao deletar a foto");
    }

    public async Task<Response<List<PropertyPhoto>?>> GetAllByPropertyAsync(
        GetAllPropertyPhotosByPropertyRequest request)
    {
        var url = $"/v1/properties/{request.PropertyId}/photos";

        var response = await _httpClient.GetAsync(url);

        var data = await response.Content.ReadFromJsonAsync<Response<List<PropertyPhoto>?>>();
        if (!response.IsSuccessStatusCode)

            return new Response<List<PropertyPhoto>?>(null, (int)response.StatusCode, data?.Message);
        return data ?? new Response<List<PropertyPhoto>?>(null, 400, "Falha ao buscar as fotos");
    }
}