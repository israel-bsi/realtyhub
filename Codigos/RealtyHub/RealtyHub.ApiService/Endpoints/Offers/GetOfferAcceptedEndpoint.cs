using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Offers;

/// <summary>
/// Endpoint responsável por recuperar a proposta aceita de um imóvel.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de obtenção da proposta aceita de um imóvel.
/// </remarks>
public class GetOfferAcceptedEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para recuperar a proposta aceita de um imóvel.
    /// </summary>
    /// <remarks>
    /// Registra a rota GET que retorna a proposta aceita associada a um imóvel específico.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/property/{id:long}/accepted", HandlerAsync)
            .WithName("Offers: Get Accepted")
            .WithSummary("Recupera a proposta aceita")
            .WithDescription("Recupera a proposta aceita")
            .WithOrder(4)
            .Produces<Response<Proposta?>>()
            .Produces<Response<Proposta?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que retorna a proposta aceita de um imóvel.
    /// </summary>
    /// <remarks>
    /// Este método cria uma requisição para buscar a proposta aceita associada a um imóvel específico
    /// e chama o handler para processar a requisição.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IOfferHandler"/> responsável pelas operações relacionadas a propostas.</param>
    /// <param name="id">ID do imóvel cuja proposta aceita será recuperada.</param>
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
        var request = new GetOfferAcceptedByProperty
        {
            PropertyId = id,
            UserId = user.Identity?.Name ?? string.Empty
        };
        var result = await handler.GetAcceptedByProperty(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}