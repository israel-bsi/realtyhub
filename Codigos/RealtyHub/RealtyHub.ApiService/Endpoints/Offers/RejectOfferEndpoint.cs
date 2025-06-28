using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Offers;

/// <summary>
/// Endpoint responsável por rejeitar uma proposta.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de rejeição de propostas.
/// </remarks>
public class RejectOfferEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para rejeitar uma proposta.
    /// </summary>
    /// <remarks>
    /// Registra a rota PUT que rejeita uma proposta com base no ID fornecido.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id:long}/reject", HandlerAsync)
            .WithName("Offers: Reject")
            .WithSummary("Rejeita uma proposta")
            .WithDescription("Rejeita uma proposta")
            .WithOrder(3)
            .Produces<Response<Proposta?>>()
            .Produces<Response<Proposta?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que rejeita uma proposta.
    /// </summary>
    /// <remarks>
    /// Este método cria uma requisição para rejeitar uma proposta com base no ID fornecido e no usuário autenticado,
    /// e chama o handler para processar a rejeição da proposta.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IOfferHandler"/> responsável pelas operações relacionadas a propostas.</param>
    /// <param name="id">ID da proposta a ser rejeitada.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com os detalhes da proposta rejeitada, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IOfferHandler handler,
        long id)
    {
        var request = new RejectOfferRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty
        };
        var result = await handler.RejectAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}