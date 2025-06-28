using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Contracts;

/// <summary>
/// Endpoint responsável por deletar um contrato.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de exclusão lógica de contratos.
/// </remarks>
public class DeleteContractEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para deletar um contrato.
    /// </summary>
    /// <remarks>
    /// Registra a rota DELETE que espera um parâmetro numérico (ID) na URL e chama o manipulador para executar a operação.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id:long}", HandlerAsync)
            .WithName("Contracts: Delete")
            .WithSummary("Deleta um contrato")
            .WithDescription("Deleta um contrato")
            .WithOrder(3)
            .Produces<Response<Contrato?>>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que recebe a requisição para deletar um contrato.
    /// </summary>
    /// <remarks>
    /// Este método extrai o ID do contrato da rota, associa o ID do usuário autenticado à requisição,
    /// e chama o handler para realizar a exclusão lógica do contrato. Retorna NoContent se a operação for bem-sucedida
    /// ou BadRequest em caso de falha.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IContractHandler"/> responsável pelas operações relacionadas a contratos.</param>
    /// <param name="id">ID do contrato a ser deletado.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 204 No Content, se a exclusão for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IContractHandler handler,
        long id)
    {
        var request = new DeleteContractRequest
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