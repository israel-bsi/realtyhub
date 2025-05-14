using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Properties;

/// <summary>
/// Endpoint responsável por deletar um imóvel.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de exclusão de imóveis.
/// </remarks>
public class DeletePropertyEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para deletar um imóvel.
    /// </summary>
    /// <remarks>
    /// Registra a rota DELETE que recebe o ID do imóvel a ser excluído.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id:long}", HandlerAsync)
            .WithName("Properties: Delete")
            .WithSummary("Deleta um imóvel")
            .WithDescription("Deleta um imóvel")
            .WithOrder(3)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<Response<Property?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que deleta um imóvel.
    /// </summary>
    /// <remarks>
    /// Este método cria uma requisição para excluir um imóvel com base no ID fornecido e no usuário autenticado,
    /// e chama o handler para processar a exclusão.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IPropertyHandler"/> responsável pelas operações relacionadas a imóveis.</param>
    /// <param name="id">ID do imóvel a ser excluído.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 204 No Content, se a exclusão for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IPropertyHandler handler,
        long id)
    {
        var request = new DeletePropertyRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty
        };
        var result = await handler.DeleteAsync(request);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(result);
    }
}