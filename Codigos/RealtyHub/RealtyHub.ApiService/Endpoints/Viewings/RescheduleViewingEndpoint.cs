using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Viewings;

/// <summary>
/// Endpoint responsável por reagendar uma visita.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de reagendamento de visitas.
/// </remarks>
public class RescheduleViewingEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para reagendar uma visita.
    /// </summary>
    /// <remarks>
    /// Registra a rota PUT que recebe os dados atualizados da visita e o ID da visita a ser reagendada.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id:long}/reschedule", HandlerAsync)
            .WithName("Viewings: Reschedule")
            .WithSummary("Reagenda uma visita")
            .WithDescription("Reagenda uma visita")
            .WithOrder(2)
            .Produces<Response<Visita?>>()
            .Produces<Response<Visita?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que reagenda uma visita.
    /// </summary>
    /// <remarks>
    /// Este método recebe os dados atualizados da visita, associa o ID do usuário autenticado e o ID da visita,
    /// e chama o handler para realizar o reagendamento.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IViewingHandler"/> responsável pelas operações relacionadas a visitas.</param>
    /// <param name="request">Objeto <see cref="Visita"/> contendo os dados atualizados da visita.</param>
    /// <param name="id">ID da visita a ser reagendada.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com os detalhes da visita reagendada, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IViewingHandler handler,
        Visita request,
        long id)
    {
        request.Id = id;
        request.UsuarioId = user.Identity?.Name ?? string.Empty;
        var result = await handler.RescheduleAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}