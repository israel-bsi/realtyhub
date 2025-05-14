using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Offers;

/// <summary>
/// Endpoint responsável por recuperar uma proposta específica pelo seu ID.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de obtenção de uma proposta pelo ID.
/// </remarks>
public class GetOfferByIdEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para recuperar uma proposta pelo seu ID.
    /// </summary>
    /// <remarks>
    /// Registra a rota GET que retorna os detalhes de uma proposta específica com base no ID fornecido.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:long}", HandlerAsync)
            .WithName("Offers: Get By Id")
            .WithSummary("Recupera uma proposta")
            .WithDescription("Recupera uma proposta")
            .WithOrder(4)
            .Produces<Response<Offer?>>()
            .Produces<Response<Offer?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que retorna os detalhes de uma proposta pelo seu ID.
    /// </summary>
    /// <remarks>
    /// Este método cria uma requisição para buscar uma proposta específica com base no ID fornecido
    /// e chama o handler para processar a requisição.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IOfferHandler"/> responsável pelas operações relacionadas a propostas.</param>
    /// <param name="id">ID da proposta a ser recuperada.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com os detalhes da proposta, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IOfferHandler handler,
        long id)
    {
        var request = new GetOfferByIdRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty
        };
        var result = await handler.GetByIdAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}