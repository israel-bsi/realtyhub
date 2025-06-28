using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Condominiums;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Condominiums;

/// <summary>
/// Endpoint responsável por deletar um condomínio.
/// </summary>
/// <remarks>
/// Implementa <c><see cref="IEndpoint"/></c> para mapear a rota de exclusão de condomínios.  
/// </remarks>
public class DeleteCondominiumEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para a exclusão de condomínios.
    /// </summary>
    /// <remarks>
    /// Este método registra a rota HTTP DELETE que aceita um parâmetro de identificação do condomínio,
    /// configurando os códigos de resposta HTTP esperados.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo.</param>
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapDelete("/{id:long}", HandlerAsync)
            .WithName("Condominiums: Delete")
            .WithSummary("Deleta um condomínio")
            .WithDescription("Deleta um condomínio")
            .WithOrder(4)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<Response<Condominio?>>(StatusCodes.Status404NotFound);

    /// <summary>
    /// Manipulador da rota para a deleção de um condomínio.
    /// </summary>
    /// <remarks>
    /// O método atribui o ID do usuário autenticado à requisição,
    /// e invoca o handler para deletar o condomínio.
    /// </remarks>
    /// <param name="user">Objeto <c><see cref="ClaimsPrincipal"/></c> que contém os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <c><see cref="ICondominiumHandler"/></c> que executa a exclusão do condomínio.</param>
    /// <param name="id">ID do condomínio a ser deletado.</param>
    /// <returns>
    /// Um objeto <c><see cref="IResult"/></c> representando o resultado da operação:
    /// <para>- HTTP 204 No Content, se a exclusão for bem-sucedida;</para>
    /// <para>- HTTP 404 Not Found, se o condomínio não for encontrado.</para>
    /// <para>- HTTP 400 BadRequest com os detalhes, em caso de erro.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ICondominiumHandler handler,
        long id)
    {
        var request = new DeleteCondominiumRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.DeleteAsync(request);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.NotFound(result);
    }
}
