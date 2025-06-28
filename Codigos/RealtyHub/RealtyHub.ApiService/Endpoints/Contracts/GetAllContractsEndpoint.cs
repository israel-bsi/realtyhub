using Microsoft.AspNetCore.Mvc;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Contracts;

/// <summary>
/// Endpoint responsável por recuperar todos os contratos.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> e mapeia a rota de listagem de contratos.
/// </remarks>
public class GetAllContractsEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para recuperar todos os contratos.
    /// </summary>
    /// <remarks>
    /// Registra a rota GET que retorna uma lista paginada de contratos associados ao usuário autenticado.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandlerAsync)
            .WithName("Contracts: Get All")
            .WithSummary("Recupera todos os contratos")
            .WithDescription("Recupera todos os contratos")
            .WithOrder(5)
            .Produces<PagedResponse<List<Contrato>?>>()
            .Produces(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que recebe a requisição para recuperar todos os contratos.
    /// </summary>
    /// <remarks>
    /// Este método aplica paginação e retorna apenas os contratos associados ao usuário autenticado.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IContractHandler"/> responsável pelas operações relacionadas a contratos.</param>
    /// <param name="pageNumber">Número da página solicitada.</param>
    /// <param name="pageSize">Quantidade de itens por página.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando o resultado da operação:
    /// <para>- HTTP 200 OK com a lista paginada de contratos, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IContractHandler handler,
        [FromQuery] int pageNumber = Core.Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Core.Configuration.DefaultPageSize)
    {
        var request = new GetAllContractsRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await handler.GetAllAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}