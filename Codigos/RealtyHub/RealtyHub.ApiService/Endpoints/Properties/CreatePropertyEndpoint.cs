using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Properties;

/// <summary>
/// Endpoint responsável por criar um novo imóvel.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de criação de imóveis.
/// </remarks>
public class CreatePropertyEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para criar um novo imóvel.
    /// </summary>
    /// <remarks>
    /// Registra a rota POST que recebe os dados do imóvel e cria um novo registro.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandlerAsync)
            .WithName("Properties: Create")
            .WithSummary("Cria um novo imóvel")
            .WithDescription("Cria um novo imóvel")
            .WithOrder(1)
            .Produces<Response<Property?>>(StatusCodes.Status201Created)
            .Produces<Response<Property?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que cria um novo imóvel.
    /// </summary>
    /// <remarks>
    /// Este método recebe os dados do imóvel, associa o ID do usuário autenticado ao imóvel,
    /// e chama o handler para criar o registro do imóvel.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IPropertyHandler"/> responsável pelas operações relacionadas a imóveis.</param>
    /// <param name="request">Objeto <see cref="Property"/> contendo os dados do imóvel a ser criado.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 201 Created com os detalhes do imóvel criado, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IPropertyHandler handler,
        Property request)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;
        var result = await handler.CreateAsync(request);

        return result.IsSuccess
            ? Results.Created($"/{result.Data?.Id}", result)
            : Results.BadRequest(result);
    }
}