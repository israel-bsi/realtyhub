using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Offers;

/// <summary>
/// Endpoint responsável por atualizar uma proposta existente.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de atualização de propostas.
/// </remarks>
public class UpdateOfferEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para atualizar uma proposta.
    /// </summary>
    /// <remarks>
    /// Registra a rota PUT que recebe os dados atualizados da proposta e o ID da proposta a ser modificada.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id:long}", HandlerAsync)
            .WithName("Offers: Update")
            .WithSummary("Atualiza uma proposta")
            .WithDescription("Atualiza uma proposta")
            .WithOrder(2)
            .Produces<Response<Offer?>>()
            .Produces<Response<Offer?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que atualiza uma proposta existente.
    /// </summary>
    /// <remarks>
    /// Este método recebe os dados atualizados da proposta, associa o ID do usuário autenticado à proposta,
    /// e chama o handler para realizar a atualização.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IOfferHandler"/> responsável pelas operações relacionadas a propostas.</param>
    /// <param name="request">Objeto <see cref="Offer"/> contendo os dados atualizados da proposta.</param>
    /// <param name="id">ID da proposta a ser atualizada.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com os detalhes da proposta atualizada, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IOfferHandler handler,
        Offer request,
        long id)
    {
        request.Id = id;
        request.UserId = user.Identity?.Name ?? string.Empty;

        var result = await handler.UpdateAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}