using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesPhotos;
using RealtyHub.Core.Responses;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RealtyHub.Web.Handlers;

/// <summary>
/// Handler responsável por gerenciar as operações relacionadas às fotos de propriedades na aplicação web.
/// </summary>
public class PropertyPhotosHandler : IPropertyPhotosHandler
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="PropertyPhotosHandler"/> utilizando a fábrica de HttpClient.
    /// </summary>
    /// <param name="httpClientFactory">Fábrica de <see cref="IHttpClientFactory"/> para criar o cliente HTTP.</param>
    public PropertyPhotosHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory
            .CreateClient(Configuration.HttpClientName);
    }

    /// <summary>
    /// Cria novas fotos para uma propriedade.
    /// </summary>
    /// <remarks>
    /// Valida se há arquivos para upload. Em seguida, cria um conteúdo multipart/form-data com os arquivos,
    /// incluindo cabeçalhos indicando se a foto é thumbnail e o seu identificador.
    /// Envia os dados para a API e retorna o resultado da operação.
    /// </remarks>
    public async Task<Response<FotoImovel?>> CreateAsync(CreatePropertyPhotosRequest request)
    {
        if (request.FileBytes is null || request.FileBytes.Count == 0)
            return new Response<FotoImovel?>(null, 400, "Nenhum arquivo encontrado");

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

        return await response.Content.ReadFromJsonAsync<Response<FotoImovel?>>()
               ?? new Response<FotoImovel?>(null, 400, "Falha ao adicionar as fotos");
    }

    /// <summary>
    /// Atualiza as fotos de uma propriedade.
    /// </summary>
    /// <remarks>
    /// Envia uma requisição PUT para a API com as informações das fotos a serem atualizadas.
    /// Retorna a lista atualizada de fotos ou uma resposta de erro em caso de falha.
    /// </remarks>
    public async Task<Response<List<FotoImovel>?>> UpdateAsync(UpdatePropertyPhotosRequest request)
    {
        var url = $"/v1/properties/{request.PropertyId}/photos";
        var result = await _httpClient.PutAsJsonAsync(url, request);

        return await result.Content.ReadFromJsonAsync<Response<List<FotoImovel>?>>()
               ?? new Response<List<FotoImovel>?>(null, 400, "Falha ao atualizar as fotos");
    }

    /// <summary>
    /// Exclui uma foto de uma propriedade.
    /// </summary>
    /// <remarks>
    /// Envia uma requisição DELETE para a API utilizando o ID da propriedade e o ID da foto.
    /// Retorna uma resposta indicando sucesso ou falha na exclusão.
    /// </remarks>
    public async Task<Response<FotoImovel?>> DeleteAsync(DeletePropertyPhotoRequest request)
    {
        var url = $"/v1/properties/{request.PropertyId}/photos/{request.Id}";
        var response = await _httpClient.DeleteAsync(url);

        return response.IsSuccessStatusCode
            ? new Response<FotoImovel?>(null, 204)
            : new Response<FotoImovel?>(null, 400, "Falha ao deletar a foto");
    }

    /// <summary>
    /// Obtém todas as fotos associadas a uma propriedade.
    /// </summary>
    /// <remarks>
    /// Realiza uma requisição GET para a API utilizando o ID da propriedade e retorna a lista de fotos.
    /// Em caso de falha, retorna uma resposta de erro.
    /// </remarks>
    public async Task<Response<List<FotoImovel>?>> GetAllByPropertyAsync(
        GetAllPropertyPhotosByPropertyRequest request)
    {
        var url = $"/v1/properties/{request.PropertyId}/photos";
        var response = await _httpClient.GetAsync(url);

        return await response.Content.ReadFromJsonAsync<Response<List<FotoImovel>?>>()
               ?? new Response<List<FotoImovel>?>(null, 400, "Falha ao buscar as fotos");
    }
}