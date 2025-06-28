using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Viewings;

/// <summary>
/// Endpoint responsável por agendar uma nova visita.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de agendamento de visitas.
/// </remarks>
public class ScheduleViewingEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para agendar uma nova visita.
    /// </summary>
    /// <remarks>
    /// Registra a rota POST que recebe os dados da visita e cria um novo agendamento.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandlerAsync)
            .WithName("Viewings: Schedule")
            .WithSummary("Agenda uma nova visita")
            .WithDescription("Agenda uma nova visita")
            .WithOrder(1)
            .Produces<Response<Visita?>>(StatusCodes.Status201Created)
            .Produces<Response<Visita?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que agenda uma nova visita.
    /// </summary>
    /// <remarks>
    /// Este método recebe os dados da visita, associa o ID do usuário autenticado à visita,
    /// e chama o handler para criar o agendamento.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IViewingHandler"/> responsável pelas operações relacionadas a visitas.</param>
    /// <param name="request">Objeto <see cref="Visita"/> contendo os dados da visita a ser agendada.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 201 Created com os detalhes da visita agendada, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IViewingHandler handler,
        Visita request)
    {
        request.UsuarioId = user.Identity?.Name ?? string.Empty;
        var result = await handler.ScheduleAsync(request);

        return result.IsSuccess
            ? Results.Created($"/{result.Data?.Id}", result)
            : Results.BadRequest(result);
    }
}