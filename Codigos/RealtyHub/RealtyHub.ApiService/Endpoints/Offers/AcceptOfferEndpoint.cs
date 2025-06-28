using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Offers;

/// <summary>
/// Endpoint responsável por aceitar uma proposta.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de aceitação de propostas.
/// </remarks>
public class AcceptOfferEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para aceitar uma proposta.
    /// </summary>
    /// <remarks>
    /// Registra a rota PUT que aceita uma proposta com base no ID fornecido.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id:long}/accept", HandlerAsync)
            .WithName("Offers: Accept")
            .WithSummary("Aceita uma proposta")
            .WithDescription("Aceita uma proposta")
            .WithOrder(7)
            .Produces<Response<Proposta?>>()
            .Produces<Response<Proposta?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que aceita uma proposta.
    /// </summary>
    /// <remarks>
    /// Este método cria uma requisição para aceitar uma proposta com base no ID fornecido e no usuário autenticado,
    /// e chama o handler para processar a aceitação da proposta.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IOfferHandler"/> responsável pelas operações relacionadas a propostas.</param>
    /// <param name="id">ID da proposta a ser aceita.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com os detalhes da proposta aceita, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IOfferHandler handler,
        long id)
    {
        var request = new AcceptOfferRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty
        };
        var result = await handler.AcceptAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}