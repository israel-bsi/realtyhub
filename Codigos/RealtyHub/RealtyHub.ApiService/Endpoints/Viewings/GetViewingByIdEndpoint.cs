using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Viewings;

/// <summary>
/// Endpoint responsável por recuperar uma visita específica pelo seu ID.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de obtenção de uma visita pelo ID.
/// </remarks>
public class GetViewingByIdEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para recuperar uma visita pelo seu ID.
    /// </summary>
    /// <remarks>
    /// Registra a rota GET que retorna os detalhes de uma visita específica com base no ID fornecido.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:long}", HandlerAsync)
            .WithName("Viewings: Get by Id")
            .WithSummary("Recupera uma visita")
            .WithDescription("Recupera uma visita")
            .WithOrder(4)
            .Produces<Response<Visita?>>()
            .Produces<Response<Visita?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que retorna os detalhes de uma visita pelo seu ID.
    /// </summary>
    /// <remarks>
    /// Este método cria uma requisição para buscar uma visita específica com base no ID fornecido
    /// e chama o handler para processar a requisição.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IViewingHandler"/> responsável pelas operações relacionadas a visitas.</param>
    /// <param name="id">ID da visita a ser recuperada.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com os detalhes da visita, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IViewingHandler handler,
        long id)
    {
        var request = new GetViewingByIdRequest
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