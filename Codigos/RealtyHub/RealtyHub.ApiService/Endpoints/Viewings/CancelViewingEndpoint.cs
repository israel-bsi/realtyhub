using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Viewings;

/// <summary>
/// Endpoint responsável por cancelar uma visita.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de cancelamento de visitas.
/// </remarks>
public class CancelViewingEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para cancelar uma visita.
    /// </summary>
    /// <remarks>
    /// Registra a rota PUT que recebe o ID da visita a ser cancelada.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id:long}/cancel", HandlerAsync)
            .WithName("Viewings: Cancel")
            .WithSummary("Cancela uma visita")
            .WithDescription("Cancela uma visita")
            .WithOrder(3)
            .Produces<Response<Viewing?>>()
            .Produces<Response<Viewing?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que cancela uma visita.
    /// </summary>
    /// <remarks>
    /// Este método cria uma requisição para cancelar uma visita com base no ID fornecido e no usuário autenticado,
    /// e chama o handler para processar o cancelamento.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IViewingHandler"/> responsável pelas operações relacionadas a visitas.</param>
    /// <param name="id">ID da visita a ser cancelada.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com os detalhes da visita cancelada, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IViewingHandler handler,
        long id)
    {
        var request = new CancelViewingRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.CancelAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}