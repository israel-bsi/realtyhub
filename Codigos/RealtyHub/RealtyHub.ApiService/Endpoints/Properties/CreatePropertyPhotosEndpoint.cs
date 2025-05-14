using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesPhotos;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Properties;

/// <summary>
/// Endpoint responsável por adicionar fotos a um imóvel.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de adição de fotos a imóveis.
/// </remarks>
public class CreatePropertyPhotosEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para adicionar fotos a um imóvel.
    /// </summary>
    /// <remarks>
    /// Registra a rota POST que recebe as fotos e o ID do imóvel para associá-las.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:long}/photos", HandlerAsync)
            .WithName("Properties: Add Photos")
            .WithSummary("Adiciona fotos a um imóvel")
            .WithDescription("Adiciona fotos a um imóvel")
            .WithOrder(6)
            .Produces<Response<PropertyPhoto?>>(StatusCodes.Status201Created)
            .Produces<Response<Property?>>(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Manipulador da rota que adiciona fotos a um imóvel.
    /// </summary>
    /// <remarks>
    /// Este método recebe as fotos enviadas na requisição, associa o ID do imóvel e o ID do usuário autenticado,
    /// e chama o handler para processar a adição das fotos.
    /// </remarks>
    /// <param name="httpRequest">Objeto <see cref="HttpRequest"/> contendo os dados da requisição HTTP.</param>
    /// <param name="id">ID do imóvel ao qual as fotos serão associadas.</param>
    /// <param name="handler">Instância de <see cref="IPropertyPhotosHandler"/> responsável pelas operações relacionadas a fotos de imóveis.</param>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 201 Created com os detalhes da foto adicionada, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        HttpRequest httpRequest,
        long id,
        IPropertyPhotosHandler handler,
        ClaimsPrincipal user)
    {
        var request = new CreatePropertyPhotosRequest
        {
            HttpRequest = httpRequest,
            PropertyId = id,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.CreateAsync(request);

        return result.IsSuccess
            ? Results.Created($"/{result.Data?.Id}", result)
            : Results.BadRequest(result);
    }
}