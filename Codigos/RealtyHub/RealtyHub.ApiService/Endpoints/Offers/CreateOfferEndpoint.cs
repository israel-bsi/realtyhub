using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Offers;

/// <summary>
/// Endpoint responsável por criar uma nova proposta.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de criação de propostas.
/// </remarks>
public class CreateOfferEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para criar uma nova proposta.
    /// </summary>
    /// <remarks>
    /// Registra a rota POST que recebe os dados da proposta e cria uma nova proposta.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandlerAsync)
            .WithName("Offers: Create")
            .WithSummary("Cria uma proposta")
            .WithDescription("Cria uma proposta")
            .WithOrder(1)
            .Produces<Response<Proposta?>>(StatusCodes.Status201Created)
            .Produces<Response<Proposta?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que cria uma nova proposta.
    /// </summary>
    /// <remarks>
    /// Este método recebe os dados da proposta, associa o ID do usuário autenticado à proposta,
    /// e chama o handler para criar a proposta.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IOfferHandler"/> responsável pelas operações relacionadas a propostas.</param>
    /// <param name="request">Objeto <see cref="Proposta"/> contendo os dados da proposta a ser criada.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 201 Created com os detalhes da proposta criada, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IOfferHandler handler,
        Proposta request)
    {
        request.UsuarioId = user.Identity?.Name ?? string.Empty;

        var result = await handler.CreateAsync(request);

        return result.IsSuccess
            ? Results.Created($"/{result.Data?.Id}", result)
            : Results.BadRequest(result);
    }
}