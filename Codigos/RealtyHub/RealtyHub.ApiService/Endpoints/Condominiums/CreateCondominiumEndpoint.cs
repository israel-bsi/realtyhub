using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Condominiums;

/// <summary>
/// Endpoint responsável por criar novos condomínios.
/// </summary>
/// <remarks>
/// Implementa <see cref="IEndpoint"/> para mapear a rota de criação de condomínios.
/// </remarks>
public class CreateCondominiumEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para criar um condomínio.
    /// </summary>
    /// <param name="app">O construtor de rotas do aplicativo.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandlerAsync)
            .WithName("Condominiums: Create")
            .WithSummary("Cria um novo condomínio")
            .WithDescription("Cria um novo condomínio")
            .WithOrder(1)
            .Produces<Response<Condominio?>>(StatusCodes.Status201Created)
            .Produces<Response<Condominio?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que recebe a requisição para criar um condomínio.
    /// </summary>
    /// <remarks>
    /// O método atribui o ID do usuário autenticado a requisição,
    /// e invoca o handler para criar o condomínio.
    /// </remarks>
    /// <param name="user">Instância de <c><see cref="ClaimsPrincipal"/></c> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <c><see cref="ICondominiumHandler"/></c> que executa a criação de condomínios.</param>
    /// <param name="request">Objeto <c><see cref="Condominio"/></c> contendo os dados do novo condomínio.</param>
    /// <returns>
    /// Um <c><see cref="IResult"/></c> representando o resultado da operação:
    /// <para>- HTTP 201 Created com o recurso criado, se a criação for bem-sucedida;</para>
    /// <para>- HTTP 400 BadRequest com os detalhes, em caso de erro.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ICondominiumHandler handler,
        Condominio request)
    {
        request.UsuarioId = user.Identity?.Name ?? string.Empty;
        Response<Condominio?> result = await handler.CreateAsync(request);

        return result.IsSuccess
            ? Results.Created($"/{result.Data?.Id}", result)
            : Results.BadRequest(result);
    }
}