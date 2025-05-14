using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesPhotos;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Properties;

/// <summary>
/// Endpoint responsável por excluir uma foto de um imóvel.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de exclusão de fotos de imóveis.
/// </remarks>
public class DeletePropertyPhotoEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para excluir uma foto de um imóvel.
    /// </summary>
    /// <remarks>
    /// Registra a rota DELETE que recebe o ID do imóvel e o ID da foto a ser excluída.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:long}/photos/{photoId}", HandlerAsync)
            .WithName("Properties: Delete Photos")
            .WithSummary("Exclui uma foto de um imóvel")
            .WithDescription("Exclui uma foto de um imóvel")
            .WithOrder(7)
            .Produces<Response<PropertyPhoto?>>(StatusCodes.Status204NoContent)
            .Produces<Response<Property?>>(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Manipulador da rota que exclui uma foto de um imóvel.
    /// </summary>
    /// <remarks>
    /// Este método cria uma requisição para excluir uma foto de um imóvel com base no ID do imóvel,
    /// no ID da foto e no usuário autenticado, e chama o handler para processar a exclusão.
    /// </remarks>
    /// <param name="id">ID do imóvel ao qual a foto está associada.</param>
    /// <param name="handler">Instância de <see cref="IPropertyPhotosHandler"/> responsável pelas operações relacionadas a fotos de imóveis.</param>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="photoId">ID da foto a ser excluída.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 204 No Content, se a exclusão for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        long id,
        IPropertyPhotosHandler handler,
        ClaimsPrincipal user,
        string photoId)
    {
        var request = new DeletePropertyPhotoRequest
        {
            Id = photoId,
            PropertyId = id,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.DeleteAsync(request);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(result);
    }
}