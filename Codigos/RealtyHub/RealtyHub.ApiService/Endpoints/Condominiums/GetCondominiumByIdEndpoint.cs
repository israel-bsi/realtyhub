using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Condominiums;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Condominiums;

/// <summary>
/// Endpoint responsável por recuperar um condomínio específico pelo seu ID.
/// </summary>
/// <remarks>
/// Implementa <c><see cref="IEndpoint"/></c> para mapear a rota de obtenção de um condomínio.
/// </remarks>
public class GetCondominiumByIdEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para recuperar um condomínio pelo seu ID.
    /// </summary>
    /// <remarks>
    /// Este método registra a rota HTTP GET que aceita um parâmetro de identificação do condomínio,
    /// configurando os códigos de resposta HTTP esperados.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo.</param>
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/{id:long}", HandlerAsync)
            .WithName("Condominiums: Get by id")
            .WithSummary("Recupera um condomínio")
            .WithDescription("Recupera um condomínio baseado no seu ID")
            .WithOrder(3)
            .Produces<Response<Condominio?>>()
            .Produces<Response<Condominio?>>(StatusCodes.Status404NotFound);

    /// <summary>
    /// Manipulador da rota para a obtenção de um condomínio.
    /// </summary>
    /// <remarks>
    /// O método atribui o ID do usuário autenticado à requisição,
    /// e invoca o handler para buscar o condomínio.
    /// </remarks>
    /// <param name="user">Objeto <c><see cref="ClaimsPrincipal"/></c> que contém os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <c><see cref="ICondominiumHandler"/></c> que executa a busca do condomínio.</param>
    /// <param name="id">ID do condomínio a ser recuperado.</param>
    /// <returns>
    /// Um objeto <c><see cref="IResult"/></c> representando o resultado da operação:
    /// <para>- HTTP 200 OK com os detalhes do condomínio, se encontrado;</para>
    /// <para>- HTTP 404 Not Found, se o condomínio não for encontrado.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ICondominiumHandler handler,
        long id)
    {
        var request = new GetCondominiumByIdRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.GetByIdAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.NotFound(result);
    }
}