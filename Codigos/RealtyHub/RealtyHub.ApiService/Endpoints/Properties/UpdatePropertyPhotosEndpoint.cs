using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesPhotos;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Properties;

/// <summary>
/// Endpoint responsável por atualizar as fotos de um imóvel.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de atualização de fotos de imóveis.
/// </remarks>
public class UpdatePropertyPhotosEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para atualizar as fotos de um imóvel.
    /// </summary>
    /// <remarks>
    /// Registra a rota PUT que recebe os dados atualizados das fotos e o ID do imóvel ao qual elas pertencem.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:long}/photos", HandlerAsync)
            .WithName("Properties: Update Photos")
            .WithSummary("Atualiza as fotos de um imóvel")
            .WithDescription("Atualiza as fotos de um imóvel")
            .WithOrder(7)
            .Produces<Response<List<PropertyPhoto>?>>()
            .Produces<Response<List<PropertyPhoto>?>>(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Manipulador da rota que atualiza as fotos de um imóvel.
    /// </summary>
    /// <remarks>
    /// Este método recebe os dados atualizados das fotos, associa o ID do imóvel e o ID do usuário autenticado,
    /// e chama o handler para realizar a atualização.
    /// </remarks>
    /// <param name="id">ID do imóvel ao qual as fotos pertencem.</param>
    /// <param name="handler">Instância de <see cref="IPropertyPhotosHandler"/> responsável pelas operações relacionadas a fotos de imóveis.</param>
    /// <param name="request">Objeto <see cref="UpdatePropertyPhotosRequest"/> contendo os dados atualizados das fotos.</param>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com os detalhes das fotos atualizadas, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        long id,
        IPropertyPhotosHandler handler,
        UpdatePropertyPhotosRequest request,
        ClaimsPrincipal user)
    {
        request.PropertyId = id;
        request.UserId = user.Identity?.Name ?? string.Empty;

        var result = await handler.UpdateAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}