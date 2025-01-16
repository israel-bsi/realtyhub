using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesImages;
using RealtyHub.Core.Responses;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RealtyHub.Web.Handlers;

public class PropertyImageHandler(IHttpClientFactory httpClientFactory) : IPropertyImageHandler
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(Configuration.HttpClientName);
    public async Task<Response<PropertyImage?>> CreateAsync(CreatePropertyImageRequest request)
    {
        if (request.FileBytes is null || request.FileBytes.Count == 0)
            return new Response<PropertyImage?>(null, 400, "Nenhum arquivo encontrado");

        using var content = new MultipartFormDataContent();
        foreach (var fileData in request.FileBytes)
        {
            var fileContent = new ByteArrayContent(fileData.Content);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(fileData.ContentType);

            content.Add(fileContent, "images", fileData.Name);
        }

        var url = $"/v1/properties/{request.PropertyId}/images";
        var response = await _httpClient.PostAsync(url, content);

        var data = await response.Content.ReadFromJsonAsync<Response<PropertyImage?>>();

        if (!response.IsSuccessStatusCode)
            return new Response<PropertyImage?>(null, (int)response.StatusCode, data?.Message);

        return data ?? new Response<PropertyImage?>(null, 400, "Falha ao criar as imagens");
    }

    public async Task<Response<PropertyImage?>> DeleteAsync(DeletePropertyImageRequest request)
    {
        var url = $"/v1/properties/{request.PropertyId}/images/{request.ImageId}";
        var response = await _httpClient.DeleteAsync(url);
        var data = await response.Content.ReadFromJsonAsync<Response<PropertyImage?>>();
        if (!response.IsSuccessStatusCode)
            return new Response<PropertyImage?>(null, (int)response.StatusCode, data?.Message);
        return data ?? new Response<PropertyImage?>(null, 400, "Falha ao deletar a imagem");
    }

    public async Task<Response<List<PropertyImage>?>> GetAllByPropertyAsync(GetAllPropertyImagesByPropertyRequest request)
    {
        var url = $"/v1/properties/{request.PropertyId}/images";
        var response = await _httpClient.GetAsync(url);
        var data = await response.Content.ReadFromJsonAsync<Response<List<PropertyImage>?>>();
        if (!response.IsSuccessStatusCode)
            return new Response<List<PropertyImage>?>(null, (int)response.StatusCode, data?.Message);
        return data ?? new Response<List<PropertyImage>?>(null, 400, "Falha ao buscar as imagens");
    }
}