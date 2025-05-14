using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Contracts;

/// <summary>
/// Endpoint responsável por atualizar os dados de um contrato existente.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de atualização de contratos.
/// </remarks>
public class UpdateContractEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para atualizar um contrato.
    /// </summary>
    /// <remarks>
    /// Registra a rota PUT que espera um parâmetro numérico (ID) e os dados atualizados do contrato,
    /// chamando o manipulador para executar a operação.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id:long}", HandlerAsync)
            .WithName("Contracts: Update")
            .WithSummary("Atualiza um contrato")
            .WithDescription("Atualiza um contrato")
            .WithOrder(2)
            .Produces<Response<Contract?>>()
            .Produces(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que recebe a requisição para atualizar um contrato.
    /// </summary>
    /// <remarks>
    /// Este método extrai o ID do contrato e os dados atualizados da requisição,
    /// associa o ID do usuário autenticado à requisição e chama o handler para realizar a atualização.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IContractHandler"/> responsável pelas operações relacionadas a contratos.</param>
    /// <param name="request">Objeto <see cref="Contract"/> contendo os dados atualizados do contrato.</param>
    /// <param name="id">ID do contrato a ser atualizado.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com os dados atualizados do contrato, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IContractHandler handler,
        Contract request,
        long id)
    {
        request.Id = id;
        request.UserId = user.Identity?.Name ?? string.Empty;
        var result = await handler.UpdateAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}