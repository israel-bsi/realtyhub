using Microsoft.AspNetCore.Mvc;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Offers;

/// <summary>
/// Endpoint responsável por recuperar todas as propostas.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de listagem de todas as propostas.
/// </remarks>
public class GetAllOffersEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para recuperar todas as propostas.
    /// </summary>
    /// <remarks>
    /// Registra a rota GET que retorna uma lista paginada de todas as propostas.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandlerAsync)
            .WithName("Offers: Get All")
            .WithSummary("Recupera todas as propostas")
            .WithDescription("Recupera todas as propostas")
            .WithOrder(5)
            .Produces<PagedResponse<Offer?>>()
            .Produces<PagedResponse<Offer?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que retorna todas as propostas.
    /// </summary>
    /// <remarks>
    /// Este método cria uma requisição para buscar todas as propostas, aplicando filtros de data e paginação,
    /// e chama o handler para processar a requisição.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IOfferHandler"/> responsável pelas operações relacionadas a propostas.</param>
    /// <param name="startDate">Data inicial para filtrar as propostas <see cref="string"/> (opcional).</param>
    /// <param name="endDate">Data final para filtrar as propostas <see cref="string"/> (opcional).</param>
    /// <param name="pageNumber">Número da página solicitada <see cref="int"/> (padrão: <see cref="Core.Configuration.DefaultPageNumber"/>).</param>
    /// <param name="pageSize">Quantidade de itens por página <see cref="int"/> (padrão: <see cref="Core.Configuration.DefaultPageSize"/>).</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com a lista paginada de propostas, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IOfferHandler handler,
        [FromQuery] string? startDate,
        [FromQuery] string? endDate,
        [FromQuery] int pageNumber = Core.Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Core.Configuration.DefaultPageSize)
    {
        var request = new GetAllOffersRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
            PageNumber = pageNumber,
            PageSize = pageSize,
            StartDate = startDate,
            EndDate = endDate
        };
        var result = await handler.GetAllAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}