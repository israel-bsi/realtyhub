using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Condominiums;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Condominiums;

/// <summary>
/// Endpoint responsável por obter todos os condomínios.
/// </summary>
/// <remarks>
/// Implementa <c><see cref="IEndpoint"/></c> para mapear a rota de listagem de condomínios.
/// </remarks>
public class GetAllCondominiumsEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para obter todos os condomínios.
    /// </summary>
    /// <remarks>
    /// Registra a rota responsável por retornar a lista de condomínios com paginação e opção de filtro.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo.</param>
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/", HandlerAsync)
            .WithName("Condominiums: Get All")
            .WithSummary("Obtém todos os condomínios")
            .WithDescription("Obtém todos os condomínios")
            .WithOrder(5)
            .Produces<PagedResponse<List<Condominio>>>()
            .Produces<PagedResponse<List<Condominio>>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que recebe a requisição para obter todos os condomínios.
    /// </summary>
    /// <remarks>
    /// Aplica paginação, possibilita filtrar pelo campo desejado e retorna apenas condomínios ativos.
    /// </remarks>
    /// <param name="user">Objeto <c><see cref="ClaimsPrincipal"/></c> que contém os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <c><see cref="ICondominiumHandler"/></c> que executa a busca de condomínios.</param>
    /// <param name="filterBy">Campo utilizado para filtrar os condomínios.</param>
    /// <param name="pageNumber">Número da página solicitada.</param>
    /// <param name="pageSize">Quantidade de itens por página.</param>
    /// <param name="searchTerm">Termo utilizado na busca de condomínios.</param>
    /// <returns>
    /// Um objeto <c><see cref="IResult"/></c> representando o resultado da operação:
    /// <para>- HTTP 200 OK com a lista paginada, se a busca for bem-sucedida;</para>
    /// <para>- HTTP 400 BadRequest com os detalhes, em caso de erro.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ICondominiumHandler handler,
        [FromQuery] string filterBy = "",
        [FromQuery] int pageNumber = Core.Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Core.Configuration.DefaultPageSize,
        [FromQuery] string searchTerm = "")
    {
        var request = new GetAllCondominiumsRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            FilterBy = filterBy
        };

        var result = await handler.GetAllAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}