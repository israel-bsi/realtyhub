using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesPhotos;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Properties;

/// <summary>
/// Endpoint responsável por recuperar todas as fotos de um imóvel.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de listagem de fotos de um imóvel.
/// </remarks>
public class GetAllPropertyPhotosByPropertyEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para recuperar todas as fotos de um imóvel.
    /// </summary>
    /// <remarks>
    /// Registra a rota GET que retorna todas as fotos associadas a um imóvel específico.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:long}/photos", HandlerAsync)
            .WithName("Properties: Get Photos By Property")
            .WithSummary("Recupera todas as fotos de um imóvel")
            .WithDescription("Recupera todas as fotos de um imóvel")
            .WithOrder(8)
            .Produces<Response<List<PropertyPhoto>?>>()
            .Produces<Response<Property?>>(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Manipulador da rota que retorna todas as fotos de um imóvel.
    /// </summary>
    /// <remarks>
    /// Este método cria uma requisição para buscar todas as fotos associadas a um imóvel específico
    /// e chama o handler para processar a requisição.
    /// </remarks>
    /// <param name="id">ID do imóvel cujas fotos serão recuperadas.</param>
    /// <param name="handler">Instância de <see cref="IPropertyPhotosHandler"/> responsável pelas operações relacionadas a fotos de imóveis.</param>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com a lista de fotos do imóvel, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        long id,
        IPropertyPhotosHandler handler,
        ClaimsPrincipal user)
    {
        var request = new GetAllPropertyPhotosByPropertyRequest
        {
            PropertyId = id,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.GetAllByPropertyAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}