using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Viewings;

/// <summary>
/// Endpoint responsável por finalizar uma visita.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de finalização de visitas.
/// </remarks>
public class DoneViewingEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para finalizar uma visita.
    /// </summary>
    /// <remarks>
    /// Registra a rota PUT que recebe o ID da visita a ser finalizada.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id:long}/done", HandlerAsync)
            .WithName("Viewings: Done")
            .WithSummary("Finaliza uma visita")
            .WithDescription("Finaliza uma visita")
            .WithOrder(6)
            .Produces<Response<Visita?>>()
            .Produces<Response<Visita?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que finaliza uma visita.
    /// </summary>
    /// <remarks>
    /// Este método cria uma requisição para finalizar uma visita com base no ID fornecido e no usuário autenticado,
    /// e chama o handler para processar a finalização.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IViewingHandler"/> responsável pelas operações relacionadas a visitas.</param>
    /// <param name="request">Objeto <see cref="DoneViewingRequest"/> contendo os dados da visita a ser finalizada.</param>
    /// <param name="id">ID da visita a ser finalizada.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com os detalhes da visita finalizada, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IViewingHandler handler,
        DoneViewingRequest request,
        long id)
    {
        request.Id = id;
        request.UserId = user.Identity?.Name ?? string.Empty;

        var result = await handler.DoneAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}