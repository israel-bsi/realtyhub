using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Condominiums;

/// <summary>
/// Endpoint responsável por atualizar os dados de um condomínio existente.
/// </summary>
/// <remarks>
/// Implementa <c><see cref="IEndpoint"/></c> para mapear a rota de atualização de condomínios.
/// </remarks>
public class UpdateCondominiumEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para atualizar um condomínio.
    /// </summary>
    /// <remarks>
    /// Este método registra a rota HTTP PUT que aceita um parâmetro de identificação do condomínio
    /// e os dados atualizados, configurando os códigos de resposta HTTP esperados.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo.</param>
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPut("/{id:long}", HandlerAsync)
            .WithName("Condominiums: Update")
            .WithSummary("Atualiza um condomínio")
            .WithDescription("Atualiza os dados de um condomínio existente")
            .WithOrder(2)
            .Produces<Response<Condominium?>>()
            .Produces<Response<Condominium?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota para a atualização de um condomínio.
    /// </summary>
    /// <remarks>
    /// Este método extrai o ID do condomínio e os dados atualizados da requisição,
    /// atribui o ID do usuário autenticado à requisição e invoca o handler para realizar a atualização.
    /// </remarks>
    /// <param name="user">Objeto <c><see cref="ClaimsPrincipal"/></c> que contém os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <c><see cref="ICondominiumHandler"/></c> que executa a atualização do condomínio.</param>
    /// <param name="request">Objeto <c><see cref="Condominium"/></c> contendo os dados atualizados do condomínio.</param>
    /// <param name="id">ID do condomínio a ser atualizado.</param>
    /// <returns>
    /// Um objeto <c><see cref="IResult"/></c> representando o resultado da operação:
    /// <para>- HTTP 200 OK com os dados atualizados do condomínio, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ICondominiumHandler handler,
        Condominium request,
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